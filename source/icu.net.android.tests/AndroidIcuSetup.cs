// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Runtime.InteropServices;
using Icu;

namespace icu.net.android.tests;

internal static class AndroidIcuSetup
{
	private const int RTLD_NOW = 2;
	private const int RTLD_GLOBAL = 0x00100;
	private static readonly object DataSetupLock = new();
	private static readonly IntPtr JavaLoadedLibrary = (IntPtr)1;
	private static string? _writableNativeLibDir;
	private static readonly string SystemLibcPath = File.Exists("/system/lib64/libc.so")
		? "/system/lib64/libc.so"
		: "/system/lib/libc.so";

	[DllImport("/system/lib64/libc.so", EntryPoint = "dlsym", BestFitMapping = false)]
	private static extern IntPtr SystemLib64Dlsym(IntPtr handle, string symbol);

	[DllImport("/system/lib/libc.so", EntryPoint = "dlsym", BestFitMapping = false)]
	private static extern IntPtr SystemLibDlsym(IntPtr handle, string symbol);

	[DllImport("/system/lib64/libc.so", EntryPoint = "dlopen", BestFitMapping = false)]
	private static extern IntPtr SystemLib64Dlopen(string file, int mode);

	[DllImport("/system/lib/libc.so", EntryPoint = "dlopen", BestFitMapping = false)]
	private static extern IntPtr SystemLibDlopen(string file, int mode);

	internal static void Prepare()
	{
#if __ANDROID__
		var context = Android.App.Application.Context;
		var nativeDir = context?.ApplicationInfo?.NativeLibraryDir;
		if (!string.IsNullOrEmpty(nativeDir) && context != null)
		{
			var loadDir = EnsureWritableNativeLibs(nativeDir, context);
			Wrapper.SetPreferredIcu4cDirectory(loadDir);
			Wrapper.SetAndroidBundledIcuMajorVersion(72);
			Wrapper.SetAndroidNativeLibraryLoader(LoadBundledNativeLibrary);
			Wrapper.SetAndroidSymbolResolver(ResolveSymbol);
			EnsureIcuDataFile(context);
		}
#endif
	}

	internal static void Configure()
	{
		Prepare();
		Wrapper.Verbose = true;
		ApplyDataDirectory();
		Wrapper.Init();
	}

	internal static void ApplyDataDirectory()
	{
#if __ANDROID__
		var context = Android.App.Application.Context;
		if (context?.FilesDir != null)
			Wrapper.DataDirectory = GetIcuDataDirectory(context);
#endif
	}

#if __ANDROID__
	private static IntPtr Dlsym(IntPtr handle, string symbol) =>
		SystemLibcPath.Contains("lib64", StringComparison.Ordinal)
			? SystemLib64Dlsym(handle, symbol)
			: SystemLibDlsym(handle, symbol);

	private static IntPtr Dlopen(string path, int mode) =>
		SystemLibcPath.Contains("lib64", StringComparison.Ordinal)
			? SystemLib64Dlopen(path, mode)
			: SystemLibDlopen(path, mode);

	private static IntPtr ResolveSymbol(string symbol)
	{
		var ptr = Dlsym(IntPtr.Zero, symbol);
		if (ptr != IntPtr.Zero)
			return ptr;

		foreach (var lib in new[] { "icuuc", "icui18n" })
		{
			try
			{
				var handle = NativeLibrary.Load(lib);
				if (NativeLibrary.TryGetExport(handle, symbol, out ptr) && ptr != IntPtr.Zero)
					return ptr;
			}
			catch (DllNotFoundException)
			{
			}
		}

		return IntPtr.Zero;
	}

	private static string GetIcuDataDirectory(Android.Content.Context context) =>
		Path.Combine(context.FilesDir!.AbsolutePath, "icu");

	private static void EnsureIcuDataFile(Android.Content.Context context)
	{
		const int icuMajor = 72;
		var datName = $"icudt{icuMajor}l.dat";
		var dataDir = GetIcuDataDirectory(context);

		lock (DataSetupLock)
		{
			Directory.CreateDirectory(dataDir);
			var datPath = Path.Combine(dataDir, datName);
			if (!File.Exists(datPath))
			{
				var tempPath = datPath + ".tmp";
				using (var input = context.Assets!.Open(datName))
				using (var output = File.Create(tempPath))
					input.CopyTo(output);
				File.Move(tempPath, datPath, overwrite: true);
			}
		}
	}

	private static IntPtr LoadBundledNativeLibrary(string libraryFileName)
	{
		var nativeDir = _writableNativeLibDir
		                ?? Android.App.Application.Context?.ApplicationInfo?.NativeLibraryDir;
		if (!string.IsNullOrEmpty(nativeDir))
		{
			foreach (var candidate in GetNativeLibraryCandidates(nativeDir, libraryFileName))
			{
				var dlopenHandle = Dlopen(candidate, RTLD_NOW | RTLD_GLOBAL);
				if (dlopenHandle != IntPtr.Zero)
					return dlopenHandle;

				try
				{
					var handle = NativeLibrary.Load(candidate);
					if (handle != IntPtr.Zero)
						return handle;
				}
				catch (DllNotFoundException)
				{
				}
			}
		}

		var apkPath = GetNativeLibraryApkPath();
		var abi = GetAbiFolder();
		if (apkPath != null && abi != null)
		{
			foreach (var fileName in GetLibraryFileNameVariants(libraryFileName))
			{
				var zipPath = $"{apkPath}!/lib/{abi}/{fileName}";
				try
				{
					var handle = NativeLibrary.Load(zipPath);
					if (handle != IntPtr.Zero)
						return handle;
				}
				catch (DllNotFoundException)
				{
				}

				var dlopenHandle = Dlopen(zipPath, RTLD_NOW | RTLD_GLOBAL);
				if (dlopenHandle != IntPtr.Zero)
					return dlopenHandle;
			}
		}

		if (TryLoadWithJavaLibrary(libraryFileName))
			return JavaLoadedLibrary;

		return IntPtr.Zero;
	}

	private static IEnumerable<string> GetNativeLibraryCandidates(string nativeDir, string libraryFileName)
	{
		foreach (var fileName in GetLibraryFileNameVariants(libraryFileName))
		{
			var path = Path.Combine(nativeDir, fileName);
			if (File.Exists(path))
				yield return path;
		}
	}

	private static IEnumerable<string> GetLibraryFileNameVariants(string libraryFileName)
	{
		yield return libraryFileName;

		// APK libs are renamed to libicu*.so but ELF NEEDED entries use libicu*.so.72.
		const int icuMajor = 72;
		if (libraryFileName.EndsWith($".so.{icuMajor}", StringComparison.Ordinal))
		{
			var unversioned = libraryFileName[..^($".{icuMajor}".Length)];
			if (!string.Equals(unversioned, libraryFileName, StringComparison.Ordinal))
				yield return unversioned;
		}
		else if (libraryFileName.EndsWith(".so", StringComparison.Ordinal) &&
		         libraryFileName.StartsWith("libicu", StringComparison.Ordinal))
		{
			yield return $"{libraryFileName}.{icuMajor}";
		}
	}

	private static string EnsureWritableNativeLibs(string readOnlyNativeDir, Android.Content.Context context)
	{
		if (_writableNativeLibDir != null)
			return _writableNativeLibDir;

		lock (DataSetupLock)
		{
			if (_writableNativeLibDir != null)
				return _writableNativeLibDir;

			var destDir = Path.Combine(context.FilesDir!.AbsolutePath, "native-libs");
			Directory.CreateDirectory(destDir);

			const int icuMajor = 72;
			var libs = new List<string> { "libc++_shared.so" };
			foreach (var lib in new[] { "icudata", "icuuc", "icui18n" })
			{
				libs.Add($"lib{lib}.so");
				libs.Add($"lib{lib}.so.{icuMajor}");
			}

			foreach (var lib in libs.Distinct())
			{
				var src = Path.Combine(readOnlyNativeDir, lib);
				if (!File.Exists(src))
					continue;

				var dst = Path.Combine(destDir, lib);
				if (!File.Exists(dst))
					File.Copy(src, dst);
			}

			foreach (var lib in new[] { "icudata", "icuuc", "icui18n" })
			{
				var unversioned = Path.Combine(destDir, $"lib{lib}.so");
				var versioned = Path.Combine(destDir, $"lib{lib}.so.{icuMajor}");
				if (!File.Exists(unversioned) || File.Exists(versioned))
					continue;

				try
				{
					File.CreateSymbolicLink(versioned, unversioned);
				}
				catch (IOException)
				{
					File.Copy(unversioned, versioned, overwrite: false);
				}
			}

			_writableNativeLibDir = destDir;
			return destDir;
		}
	}

	private static bool TryLoadWithJavaLibrary(string libraryFileName)
	{
		if (!libraryFileName.StartsWith("lib", StringComparison.Ordinal) ||
		    !libraryFileName.EndsWith(".so", StringComparison.Ordinal))
			return false;

		var shortName = libraryFileName.Substring(3, libraryFileName.Length - 6);
		try
		{
			Java.Lang.JavaSystem.LoadLibrary(shortName);
			return true;
		}
		catch (Java.Lang.UnsatisfiedLinkError)
		{
			return false;
		}
	}

	private static string? GetNativeLibraryApkPath()
	{
		var appInfo = Android.App.Application.Context?.ApplicationInfo;
		if (appInfo == null)
			return null;

		var abi = GetAbiFolder();
		if (!string.IsNullOrEmpty(abi) && appInfo.SplitSourceDirs != null)
		{
			foreach (var split in appInfo.SplitSourceDirs)
			{
				if (split.Contains(abi, StringComparison.Ordinal))
					return split;
			}
		}

		return appInfo.SourceDir;
	}

	private static string? GetAbiFolder()
	{
		var nativeDir = Android.App.Application.Context?.ApplicationInfo?.NativeLibraryDir;
		if (!string.IsNullOrEmpty(nativeDir))
			return Path.GetFileName(nativeDir.TrimEnd('/'));

		return Android.OS.Build.SupportedAbis?.FirstOrDefault() switch
		{
			"x86_64" => "x86_64",
			"arm64-v8a" => "arm64-v8a",
			"armeabi-v7a" => "armeabi-v7a",
			"x86" => "x86",
			_ => null
		};
	}
#endif
}
