// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using Android.Content;

namespace Icu
{
	internal static class AndroidIcuBootstrap
	{
		private const int IcuMajorVersion = 70;
		private const int RTLD_NOW = 2;
		private const int RTLD_GLOBAL = 0x00100;
		private const int RTLD_NOLOAD = 0x4;
		private static readonly object DataSetupLock = new();
		// JavaSystem.LoadLibrary succeeds without a real dlopen handle; NativeMethods.AndroidDlsym recognizes this sentinel.
		private static readonly IntPtr JavaLoadedLibrary = (IntPtr)1;
		private static string? _writableNativeLibDir;
		private static bool _configured;

		internal static void EnsureConfigured()
		{
			if (_configured)
				return;

			var context = Android.App.Application.Context;
			var nativeDir = context?.ApplicationInfo?.NativeLibraryDir;
			if (string.IsNullOrEmpty(nativeDir) || context == null)
				return;

			var loadDir = EnsureWritableNativeLibs(nativeDir, context);
			NativeMethods.PreferredDirectory = loadDir;
			NativeMethods.AndroidBundledIcuMajorVersion = IcuMajorVersion;
			NativeMethods.AndroidLoadNativeLibrary = LoadBundledNativeLibrary;
			EnsureIcuDataFile(context);
			Wrapper.DataDirectory = GetIcuDataDirectory(context);
			_configured = true;
		}

		private static string GetIcuDataDirectory(Context context) =>
			Path.Combine(context.FilesDir!.AbsolutePath, "icu");

		private static void EnsureIcuDataFile(Context context)
		{
			var datName = $"icudt{IcuMajorVersion}l.dat";
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
					// Prefer NativeLibrary.Load so NativeLibrary.TryGetExport can resolve symbols.
					try
					{
						var handle = NativeLibrary.Load(candidate);
						if (handle != IntPtr.Zero)
							return handle;
					}
					catch (DllNotFoundException)
					{
					}

					var dlopenHandle = Dlopen(candidate, RTLD_NOW | RTLD_GLOBAL);
					if (dlopenHandle != IntPtr.Zero)
						return dlopenHandle;
				}
			}

			// Fallback when libs are not materialized on disk (APK zip path).
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

			// Last resort when dlopen/NativeLibrary cannot open the .so directly.
			var javaHandle = TryLoadWithJavaLibrary(libraryFileName);
			if (javaHandle != IntPtr.Zero)
				return javaHandle;

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

			// APK libs are renamed to libicu*.so but ELF NEEDED entries use libicu*.so.70.
			if (libraryFileName.EndsWith($".so.{IcuMajorVersion}", StringComparison.Ordinal))
			{
				var unversioned = libraryFileName[..^($".{IcuMajorVersion}".Length)];
				if (!string.Equals(unversioned, libraryFileName, StringComparison.Ordinal))
					yield return unversioned;
			}
			else if (libraryFileName.EndsWith(".so", StringComparison.Ordinal) &&
			         libraryFileName.StartsWith("libicu", StringComparison.Ordinal))
			{
				yield return $"{libraryFileName}.{IcuMajorVersion}";
			}
		}

		private static string EnsureWritableNativeLibs(string readOnlyNativeDir, Context context)
		{
			if (_writableNativeLibDir != null)
				return _writableNativeLibDir;

			lock (DataSetupLock)
			{
				if (_writableNativeLibDir != null)
					return _writableNativeLibDir;

				var destDir = Path.Combine(context.FilesDir!.AbsolutePath, "native-libs");
				Directory.CreateDirectory(destDir);

				var libs = new List<string> { "libc++_shared.so" };
				foreach (var lib in new[] { "icudata", "icuuc", "icui18n" })
				{
					libs.Add($"lib{lib}.so");
					libs.Add($"lib{lib}.so.{IcuMajorVersion}");
				}

				foreach (var lib in libs.Distinct())
				{
					var dst = Path.Combine(destDir, lib);
					if (File.Exists(dst))
						continue;

					var src = Path.Combine(readOnlyNativeDir, lib);
					if (File.Exists(src))
					{
						File.Copy(src, dst);
						continue;
					}

					// NativeLibraryDir often has no regular files to copy (split APK / linker-loaded
					// libs). Fall back to extracting the .so entries from the installed APK(s).
					TryExtractNativeLibraryFromApk(lib, dst);
				}

				foreach (var lib in new[] { "icudata", "icuuc", "icui18n" })
				{
					var unversioned = Path.Combine(destDir, $"lib{lib}.so");
					var versioned = Path.Combine(destDir, $"lib{lib}.so.{IcuMajorVersion}");
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

		private static void TryExtractNativeLibraryFromApk(string libraryFileName, string destinationPath)
		{
			var abi = GetAbiFolder();
			if (abi == null)
				return;

			var entryName = $"lib/{abi}/{libraryFileName}";
			foreach (var apkPath in GetApkPaths())
			{
				if (string.IsNullOrEmpty(apkPath) || !File.Exists(apkPath))
					continue;

				try
				{
					using var zip = ZipFile.OpenRead(apkPath);
					var entry = zip.GetEntry(entryName);
					if (entry == null)
						continue;

					var tempPath = destinationPath + ".tmp";
					using (var input = entry.Open())
					using (var output = File.Create(tempPath))
						input.CopyTo(output);
					File.Move(tempPath, destinationPath, overwrite: true);
					return;
				}
				catch (InvalidDataException)
				{
				}
				catch (IOException)
				{
				}
			}
		}

		private static IEnumerable<string> GetApkPaths()
		{
			var appInfo = Android.App.Application.Context?.ApplicationInfo;
			if (appInfo == null)
				yield break;

			if (appInfo.SplitSourceDirs != null)
			{
				foreach (var split in appInfo.SplitSourceDirs)
					yield return split;
			}

			if (!string.IsNullOrEmpty(appInfo.SourceDir))
				yield return appInfo.SourceDir;
		}

		private static IntPtr TryLoadWithJavaLibrary(string libraryFileName)
		{
			if (!libraryFileName.StartsWith("lib", StringComparison.Ordinal) ||
			    !libraryFileName.EndsWith(".so", StringComparison.Ordinal))
				return IntPtr.Zero;

			var shortName = libraryFileName.Substring(3, libraryFileName.Length - 6);
			try
			{
				Java.Lang.JavaSystem.LoadLibrary(shortName);
			}
			catch (Java.Lang.UnsatisfiedLinkError)
			{
				return IntPtr.Zero;
			}

			// Prefer a real linker handle so dlsym/NativeLibrary.TryGetExport can resolve ICU symbols.
			// System.loadLibrary alone is not visible to RTLD_DEFAULT under Android linker namespaces.
			foreach (var fileName in GetLibraryFileNameVariants(libraryFileName))
			{
				var handle = Dlopen(fileName, RTLD_NOW | RTLD_NOLOAD | RTLD_GLOBAL);
				if (handle != IntPtr.Zero)
					return handle;
			}

			return JavaLoadedLibrary;
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

		// dlopen lives in libdl on modern Android; keep libc paths as a fallback.
		private static readonly string SystemLibdlPath = File.Exists("/system/lib64/libdl.so")
			? "/system/lib64/libdl.so"
			: File.Exists("/system/lib/libdl.so")
				? "/system/lib/libdl.so"
				: File.Exists("/system/lib64/libc.so")
					? "/system/lib64/libc.so"
					: "/system/lib/libc.so";

		[DllImport("/system/lib64/libdl.so", EntryPoint = "dlopen", BestFitMapping = false)]
		private static extern IntPtr SystemLib64LibdlDlopen(
			[MarshalAs(UnmanagedType.LPUTF8Str)] string file, int mode);

		[DllImport("/system/lib/libdl.so", EntryPoint = "dlopen", BestFitMapping = false)]
		private static extern IntPtr SystemLibLibdlDlopen(
			[MarshalAs(UnmanagedType.LPUTF8Str)] string file, int mode);

		[DllImport("/system/lib64/libc.so", EntryPoint = "dlopen", BestFitMapping = false)]
		private static extern IntPtr SystemLib64Dlopen(
			[MarshalAs(UnmanagedType.LPUTF8Str)] string file, int mode);

		[DllImport("/system/lib/libc.so", EntryPoint = "dlopen", BestFitMapping = false)]
		private static extern IntPtr SystemLibDlopen(
			[MarshalAs(UnmanagedType.LPUTF8Str)] string file, int mode);

		private static IntPtr Dlopen(string path, int mode)
		{
			try
			{
				if (SystemLibdlPath.Contains("lib64", StringComparison.Ordinal) &&
				    SystemLibdlPath.Contains("libdl", StringComparison.Ordinal))
					return SystemLib64LibdlDlopen(path, mode);
				if (SystemLibdlPath.Contains("libdl", StringComparison.Ordinal))
					return SystemLibLibdlDlopen(path, mode);
				return SystemLibdlPath.Contains("lib64", StringComparison.Ordinal)
					? SystemLib64Dlopen(path, mode)
					: SystemLibDlopen(path, mode);
			}
			catch (DllNotFoundException)
			{
				return IntPtr.Zero;
			}
			catch (EntryPointNotFoundException)
			{
				return IntPtr.Zero;
			}
		}
	}
}
