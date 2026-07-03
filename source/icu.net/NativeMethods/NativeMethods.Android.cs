// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Icu
{
	internal static partial class NativeMethods
	{
		private static bool _androidUsesUnversionedNativeLibs;
		private static bool _androidDependenciesLoaded;

		/// <summary>
		/// When Android APKs bundle ICU as libicuuc.so (no version suffix), set the major ICU version before Init().
		/// </summary>
		internal static int? AndroidBundledIcuMajorVersion { get; set; }

		internal static Func<string, IntPtr> AndroidLoadNativeLibrary { get; set; }

		private static IntPtr AndroidSystemLibc;

		private static bool TryCheckAndroidDirectoryForIcuBinaries(string directory, string libraryName)
		{
			// Android loads native libs from the APK without materializing them as regular files,
			// so File.Exists/EnumerateFiles on NativeLibraryDir often returns nothing.
			if (AndroidBundledIcuMajorVersion is int bundledVersion &&
			    !string.IsNullOrEmpty(PreferredDirectory) &&
			    string.Equals(directory, PreferredDirectory, StringComparison.Ordinal) &&
			    libraryName == "icuuc")
			{
				Trace.WriteLineIf(Verbose,
					$"icu.net: using Android bundled ICU {bundledVersion} from '{directory}'");
				_androidUsesUnversionedNativeLibs = true;
				IcuVersion = bundledVersion;
				_IcuPath = directory;
				AddDirectoryToSearchPath(directory);
				return true;
			}

			if (!Directory.Exists(directory))
				return false;

			var filePattern = GetLibraryFilePattern(libraryName);
			var files = Directory.EnumerateFiles(directory, filePattern).ToList();
			var unversioned = Path.Combine(directory, $"lib{libraryName}.so");
			if (File.Exists(unversioned) && !files.Contains(unversioned))
				files.Insert(0, unversioned);

			if (files.Count <= 0)
				return false;

			files.Sort((x, y) =>
			{
				var vx = TryParseIcuLibraryMajorVersion(x, libraryName, out var nx) ? nx : -1;
				var vy = TryParseIcuLibraryMajorVersion(y, libraryName, out var ny) ? ny : -1;
				var cmp = vy.CompareTo(vx);
				if (cmp != 0)
					return cmp;
				return string.Compare(Path.GetFileName(y), Path.GetFileName(x), StringComparison.Ordinal);
			});

			foreach (var filePath in files)
			{
				if (!TryParseIcuLibraryMajorVersion(filePath, libraryName, out var icuVersion))
				{
					if (Path.GetFileName(filePath) == $"lib{libraryName}.so" &&
					    AndroidBundledIcuMajorVersion is int androidVersion)
					{
						icuVersion = androidVersion;
						_androidUsesUnversionedNativeLibs = true;
					}
					else
					{
						Trace.WriteLineIf(Verbose,
							$"icu.net: could not parse ICU version from '{filePath}'. Skipping.");
						continue;
					}
				}
				Trace.WriteLineIf(Verbose, $"icu.net: Extracted version '{icuVersion}' from '{filePath}'");
				if (icuVersion < MinIcuVersion || icuVersion > MaxIcuVersion)
				{
					Trace.WriteLineIf(Verbose,
						$"icu.net: version {icuVersion} from '{filePath}' is outside [{MinIcuVersion}, {MaxIcuVersion}]. Skipping.");
					continue;
				}
				Trace.TraceInformation("Setting IcuVersion to {0} (found in {1})",
					icuVersion, directory);
				IcuVersion = icuVersion;
				_IcuPath = directory;
				AddDirectoryToSearchPath(directory);
				return true;
			}

			return false;
		}

		private static bool TryLocateAndroidIcuLibrary(string libraryName)
		{
			var arch = IsRunning64Bit ? "x64" : "x86";
			var androidArch = IsRunning64Bit ? "x86_64" : "x86";

			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "lib", $"android-{arch}"),
				libraryName))
				return true;

			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "lib", $"android-{androidArch}"),
				libraryName))
				return true;

			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "runtimes", $"android-{androidArch}", "native"),
				libraryName))
				return true;

			return CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "runtimes", "android", "native"),
				libraryName);
		}

		private static void EnsureAndroidDependenciesLoaded()
		{
			if (_androidDependenciesLoaded || string.IsNullOrEmpty(_IcuPath))
				return;

			foreach (var lib in new[] { "libc++_shared.so", "libicudata.so", "libicuuc.so", "libicui18n.so" })
			{
				if (AndroidLoadNativeLibrary != null)
				{
					var depHandle = AndroidLoadNativeLibrary(lib);
					if (depHandle != IntPtr.Zero)
						Trace.WriteLineIf(Verbose, $"icu.net: preloaded {lib}");
					else
						Trace.TraceWarning($"icu.net: failed to preload {lib}");
					continue;
				}

				var path = Path.Combine(_IcuPath, lib);
				try
				{
					NativeLibrary.Load(path);
					Trace.WriteLineIf(Verbose, $"icu.net: preloaded {lib}");
				}
				catch (DllNotFoundException ex)
				{
					Trace.TraceWarning($"icu.net: failed to preload {lib}: {ex.Message}");
				}
			}

			_androidDependenciesLoaded = true;
		}

		private static IntPtr GetAndroidIcuLibHandle(string basename, int icuVersion)
		{
			Trace.WriteLineIf(Verbose, $"icu.net: Get ICU Lib handle for {basename}, version {icuVersion}");
			if (icuVersion < MinIcuVersion)
				return IntPtr.Zero;

			var libName = _androidUsesUnversionedNativeLibs
				? $"lib{basename}.so"
				: $"lib{basename}.so.{icuVersion}";
			var libPath = string.IsNullOrEmpty(_IcuPath) ? libName : Path.Combine(_IcuPath, libName);

			string exceptionErrorMessage = null;
			IntPtr handle;
			string loadMethod;
			if (AndroidLoadNativeLibrary != null)
			{
				loadMethod = "AndroidLoadNativeLibrary";
				handle = AndroidLoadNativeLibrary(libName);
			}
			else
			{
				loadMethod = "NativeLibrary.Load";
				try
				{
					handle = NativeLibrary.Load(libPath);
				}
				catch (DllNotFoundException ex)
				{
					handle = IntPtr.Zero;
					exceptionErrorMessage = ex.Message;
				}
			}

			if (handle != IntPtr.Zero)
			{
				IcuVersion = icuVersion;
				return handle;
			}

			var lastError = Marshal.GetLastWin32Error();
			if (!string.IsNullOrEmpty(exceptionErrorMessage))
				exceptionErrorMessage = $" ({exceptionErrorMessage})";
			var errorMsg = $"{lastError}{exceptionErrorMessage}";
			Trace.WriteLineIf(lastError != 0, $"Unable to load [{libPath}]. Error: {errorMsg}");
			Trace.TraceWarning($"{loadMethod} of {libPath} failed with error {errorMsg}");
			return IntPtr.Zero;
		}

		private static IntPtr EnsureAndroidSystemLibc()
		{
			if (AndroidSystemLibc != IntPtr.Zero)
				return AndroidSystemLibc;

			try
			{
				var libcPath = File.Exists("/system/lib64/libc.so") ? "/system/lib64/libc.so" : "/system/lib/libc.so";
				AndroidSystemLibc = NativeLibrary.Load(libcPath);
			}
			catch (DllNotFoundException)
			{
				AndroidSystemLibc = IntPtr.Zero;
			}

			return AndroidSystemLibc;
		}

		private static IntPtr AndroidDlsymFromLib(IntPtr libcHandle, IntPtr libraryHandle, string symbol)
		{
			if (!NativeLibrary.TryGetExport(libcHandle, "dlsym", out var dlsymPtr))
				return IntPtr.Zero;

			var dlsym = Marshal.GetDelegateForFunctionPointer<DlsymDelegate>(dlsymPtr);
			return dlsym(libraryHandle, symbol);
		}

		// JavaSystem.LoadLibrary succeeds without a real dlopen handle; IntPtr(1) marks that case.
		private static IntPtr AndroidDlsym(IntPtr handle, string symbol)
		{
			if (handle == (IntPtr)1)
				handle = IntPtr.Zero;

			var libc = EnsureAndroidSystemLibc();
			if (libc == IntPtr.Zero)
				return IntPtr.Zero;

			var ptr = AndroidDlsymFromLib(libc, handle, symbol);
			if (ptr != IntPtr.Zero)
				return ptr;
			if (handle != IntPtr.Zero)
				return AndroidDlsymFromLib(libc, IntPtr.Zero, symbol);

			return IntPtr.Zero;
		}

		private static T GetAndroidMethod<T>(IntPtr handle, string methodName, bool missingInMinimal = false)
			where T : class
		{
			IntPtr methodPointer;

			var versionedMethodName = $"{methodName}_{IcuVersion}";
			methodPointer = AndroidDlsym(handle, versionedMethodName);
			if (methodPointer == IntPtr.Zero)
				methodPointer = AndroidDlsym(IntPtr.Zero, versionedMethodName);

			if (methodPointer == IntPtr.Zero)
			{
				methodPointer = AndroidDlsym(handle, methodName);
				if (methodPointer == IntPtr.Zero)
					methodPointer = AndroidDlsym(IntPtr.Zero, methodName);
			}

			if (methodPointer != IntPtr.Zero)
				return Marshal.GetDelegateForFunctionPointer<T>(methodPointer);

			if (missingInMinimal)
			{
				throw new MissingMemberException(
					"Do you have the full version of ICU installed? " +
					$"The method '{methodName}' is not included in the minimal version of ICU.");
			}
			return default(T);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr DlsymDelegate(IntPtr handle, string symbol);
	}
}
