// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Icu
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	internal static partial class NativeMethods
	{
		private static readonly object _lock = new object();

#if NET
		private static readonly DllResolver _DllResolver =
			new DllResolver(Assembly.GetExecutingAssembly());
#endif

		internal static int MinIcuVersion { get; private set; } = Wrapper.MinSupportedIcuVersion;
		internal static int MaxIcuVersion { get; private set; } = Wrapper.MaxSupportedIcuVersion;

		internal static string PreferredDirectory { get; set; }

		internal static bool Verbose { get; set; }

		internal static void SetMinMaxIcuVersions(int minVersion = Wrapper.MinSupportedIcuVersion,
			int maxVersion = Wrapper.MaxSupportedIcuVersion)
		{
			Trace.WriteLineIf(Verbose, $"Setting min/max ICU versions to {minVersion} and {maxVersion}");
			if (minVersion < Wrapper.MinSupportedIcuVersion || minVersion > Wrapper.MaxSupportedIcuVersion)
			{
				throw new ArgumentOutOfRangeException(nameof(minVersion),
					$"supported ICU versions are between {Wrapper.MinSupportedIcuVersion} and {Wrapper.MaxSupportedIcuVersion}");
			}
			if (maxVersion < Wrapper.MinSupportedIcuVersion || maxVersion > Wrapper.MaxSupportedIcuVersion)
			{
				throw new ArgumentOutOfRangeException(nameof(maxVersion),
					$"supported ICU versions are between {Wrapper.MinSupportedIcuVersion} and {Wrapper.MaxSupportedIcuVersion}");
			}

			lock (_lock)
			{
				MinIcuVersion = Math.Min(minVersion, maxVersion);
				MaxIcuVersion = Math.Max(minVersion, maxVersion);
			}

			if (!IsInitialized)
			{
				// Now that we set min/max versions, reset the existing info again
				ResetIcuVersionInfo();
				return;
			}

			var rescueDataDir = Wrapper.DataDirectory;
			Cleanup();
			Wrapper.DataDirectory = rescueDataDir;
			Wrapper.Init();
		}

		private static MethodsContainer Methods;

		static NativeMethods()
		{
			Methods = new MethodsContainer();
			ResetIcuVersionInfo();
		}

		#region Dynamic method loading

#if !NET6_0_OR_GREATER
		#region Native methods for Linux

		private const int RTLD_NOW = 2;

		private const string LIBDL_NAME = "libdl.so";

		[DllImport(LIBDL_NAME, SetLastError = true)]
		private static extern IntPtr dlopen(string file, int mode);

		[DllImport(LIBDL_NAME, SetLastError = true)]
		private static extern int dlclose(IntPtr handle);

		[DllImport(LIBDL_NAME, SetLastError = true)]
		private static extern IntPtr dlsym(IntPtr handle, string name);

		[DllImport(LIBDL_NAME, EntryPoint = "dlerror")]
		private static extern IntPtr _dlerror();

		private static string dlerror()
		{
			// Don't free the string returned from _dlerror()!
			var ptr = _dlerror();
			return Marshal.PtrToStringAnsi(ptr);
		}

		#endregion

		#region Native methods for Windows

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LoadLibraryEx(string dllToLoad, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[Flags]
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private enum LoadLibraryFlags : uint
		{
			NONE = 0x00000000,
			DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
			LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
			LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
			LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
			LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
			LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
			LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
			LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
		}

		#endregion
#endif

		private static int IcuVersion;
		private static string _IcuPath;
		private static IntPtr _IcuCommonLibHandle;
		private static IntPtr _IcuI18NLibHandle;

		private static bool IsWindows => Platform.OperatingSystem == OperatingSystemType.Windows;
		private static bool IsMac => Platform.OperatingSystem == OperatingSystemType.MacOSX;

		private static IntPtr IcuCommonLibHandle
		{
			get
			{
				if (_IcuCommonLibHandle == IntPtr.Zero)
					// ReSharper disable once StringLiteralTypo
					_IcuCommonLibHandle = LoadIcuLibrary("icuuc");
				return _IcuCommonLibHandle;
			}
		}

		private static IntPtr IcuI18NLibHandle
		{
			get
			{
				if (_IcuI18NLibHandle == IntPtr.Zero)
					_IcuI18NLibHandle = LoadIcuLibrary(IsWindows ? "icuin" : "icui18n");
				return _IcuI18NLibHandle;
			}
		}

		internal static string DirectoryOfThisAssembly
		{
			get
			{
				//NOTE: .GetTypeInfo() is not supported until .NET 4.5 onwards.
#if NET40
				var currentAssembly = typeof(NativeMethods).Assembly;
#else
				var currentAssembly = typeof(NativeMethods).GetTypeInfo().Assembly;
#endif
#if NET
				var managedPath = currentAssembly.Location;
				// If the application is published as a single file, Assembly.Location will be an empty string.
				// Per the warning https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/warnings/il3000 we should use AppContext.BaseDirectory instead.
				if (string.IsNullOrEmpty(managedPath))
				{
					managedPath = AppContext.BaseDirectory;
				}
#else
				var managedPath = currentAssembly.CodeBase ?? currentAssembly.Location;
#endif
				var uri = new Uri(managedPath);

				var directoryName = Path.GetDirectoryName(uri.LocalPath);
				Trace.WriteLineIf(Verbose, $"icu.net: Directory of this assembly is {directoryName}");
				return directoryName;
			}
		}

		private static bool IsRunning64Bit => Platform.ProcessArchitecture == Platform.x64;

		private static bool IsInitialized { get; set; }

		private static void AddDirectoryToSearchPath(string directory)
		{
			// Only perform this for Linux because we are using LoadLibraryEx
			// to ensure that a library's dependencies is loaded starting from
			// where that library is located.
			if (IsWindows)
				return;

			var ldLibPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
			Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", $"{directory}:{ldLibPath}");
			Trace.WriteLineIf(Verbose, $"icu.net: adding directory '{directory}' to LD_LIBRARY_PATH '{ldLibPath}'");
		}

		private static bool CheckDirectoryForIcuBinaries(string directory, string libraryName)
		{
			Trace.WriteLineIf(Verbose, $"icu.net: checking '{directory}' for ICU binaries");
			if (!Directory.Exists(directory))
			{
				Trace.WriteLineIf(Verbose, $"icu.net: directory '{directory}' doesn't exist");
				return false;
			}

			var filePattern = IsWindows
				? libraryName + "*.dll"
				: IsMac
				? "lib" + libraryName + ".*.dylib"
				: "lib" + libraryName + ".so.*";
			var files = Directory.EnumerateFiles(directory, filePattern).ToList();
			Trace.WriteLineIf(Verbose, $"icu.net: {files.Count} files in '{directory}' match the pattern '{filePattern}'");
			if (files.Count > 0)
			{
				// Do a reverse sort so that we use the highest version
				files.Sort((x, y) => string.CompareOrdinal(y, x));
				var filePath = files[0];
				var libNameLen = libraryName.Length;
				var version = IsWindows
					? Path.GetFileNameWithoutExtension(filePath).Substring(libNameLen) // strip icuuc
					: IsMac
					? Path.GetFileNameWithoutExtension(filePath).Substring(libNameLen + 4) // strip libicuuc.
					: Path.GetFileName(filePath).Substring(libNameLen + 7); // strip libicuuc.so.
				Trace.WriteLineIf(Verbose, $"icu.net: Extracted version '{version}' from '{filePath}'");
				if (int.TryParse(version, out var icuVersion))
				{
					Trace.TraceInformation("Setting IcuVersion to {0} (found in {1})",
						icuVersion, directory);
					IcuVersion = icuVersion;
					_IcuPath = directory;

					AddDirectoryToSearchPath(directory);
					return true;
				}
				Trace.WriteLineIf(Verbose, $"icu.net: couldn't parse '{version}' as an int. Returning false.");
			}
			Trace.WriteLineIf(Verbose && files.Count <= 0, "icu.net: No files matching pattern. Returning false.");
			return false;
		}

		private static bool LocateIcuLibrary(string libraryName)
		{
			Trace.WriteLineIf(Verbose, $"icu.net: Locating ICU library '{libraryName}'");
			if (!string.IsNullOrEmpty(PreferredDirectory) &&
				CheckDirectoryForIcuBinaries(PreferredDirectory, libraryName))
			{
				return true;
			}

			var arch = IsRunning64Bit ? "x64" : "x86";
			var platform = IsWindows ? "win" : IsMac ? "osx" : "linux";

			// Look for ICU binaries in lib/{win,osx,linux}-{x86,x64} subdirectory first
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "lib", $"{platform}-{arch}"),
				libraryName))
				return true;

			// Next look in lib/{x86,x64} subdirectory
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "lib", arch),
				libraryName))
				return true;

			// Next try just {win,osx,linux}-{x86,x64} subdirectory
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, $"{platform}-{arch}"),
				libraryName))
				return true;

			// Next try just {x86,x64} subdirectory
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, arch),
				libraryName))
				return true;

			// Might be in runtimes/{win,osx,linux}/native
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "runtimes", platform, "native"),
				libraryName))
				return true;

			// Might also be in runtimes/win7-{x86,x64}/native
			if (CheckDirectoryForIcuBinaries(
				Path.Combine(DirectoryOfThisAssembly, "runtimes", $"win7-{arch}", "native"),
				libraryName))
				return true;

			// Otherwise check the current directory
			// If we don't find it here we rely on it being in the PATH somewhere...
			return CheckDirectoryForIcuBinaries(DirectoryOfThisAssembly, libraryName);
		}

		private static IntPtr LoadIcuLibrary(string libraryName)
		{
			Trace.WriteLineIf(!IsInitialized,
				"WARNING: ICU is not initialized. Please call Icu.Wrapper.Init() at the start of your application.");

			lock (_lock)
			{
				if (IcuVersion <= 0)
					LocateIcuLibrary(libraryName);

				var handle = GetIcuLibHandle(libraryName, IcuVersion > 0 ? IcuVersion : MaxIcuVersion);
				if (handle == IntPtr.Zero)
				{
					throw new FileLoadException($"Can't load ICU library (version {IcuVersion})",
						libraryName);
				}
				return handle;
			}
		}

		private static IntPtr GetIcuLibHandle(string basename, int icuVersion)
		{
			while (true)
			{
				Trace.WriteLineIf(Verbose, $"icu.net: Get ICU Lib handle for {basename}, version {icuVersion}");
				if (icuVersion < MinIcuVersion)
					return IntPtr.Zero;

				IntPtr handle;
				int lastError;
				string loadMethod;

				var libName = IsWindows
					? $"{basename}{icuVersion}.dll"
					: IsMac
					? $"lib{basename}.{icuVersion}.dylib"
					: $"lib{basename}.so.{icuVersion}";
				var libPath = string.IsNullOrEmpty(_IcuPath) ? libName : Path.Combine(_IcuPath, libName);

#if NET6_0_OR_GREATER
				string exceptionErrorMessage = null;
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
#else
				if (IsWindows)
				{
					loadMethod = nameof(LoadLibraryEx);
					var loadLibraryFlags = string.IsNullOrEmpty(_IcuPath)
						? LoadLibraryFlags.NONE
						: LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS;
					handle = LoadLibraryEx(libPath, IntPtr.Zero, loadLibraryFlags);
				}
				else if (IsMac)
				{
					loadMethod = "(no load method)";
					handle = IntPtr.Zero;
				}
				else
				{
					loadMethod = nameof(dlopen);
					handle = dlopen(libPath, RTLD_NOW);
				}
#endif

				if (handle != IntPtr.Zero)
				{
					IcuVersion = icuVersion;
					return handle;
				}

				lastError = Marshal.GetLastWin32Error();
#if NET6_0_OR_GREATER
				if (!string.IsNullOrEmpty(exceptionErrorMessage))
					exceptionErrorMessage = $" ({exceptionErrorMessage})";
				var errorMsg = IsWindows
					? $"{new Win32Exception(lastError).Message}{exceptionErrorMessage}"
					: $"{lastError}({exceptionErrorMessage})";
#else
				var errorMsg = IsWindows
					? new Win32Exception(lastError).Message
					: IsMac
					? $"{lastError} (macOS loading requires .NET 6 or greater)"
					: $"{lastError} ({dlerror()})";
#endif
				Trace.WriteLineIf(lastError != 0, $"Unable to load [{libPath}]. Error: {errorMsg}");
				Trace.TraceWarning($"{loadMethod} of {libPath} failed with error {errorMsg}");
				icuVersion -= 1;
			}
		}

		internal static void Cleanup()
		{
			Trace.WriteLineIf(Verbose, "icu.net: Cleanup");
			lock (_lock)
			{
				Methods = new MethodsContainer();
				BiDiMethods = new BiDiMethodsContainer();
				BreakIteratorMethods = new BreakIteratorMethodsContainer();
				CodepageConversionMethods = new CodepageConversionMethodsContainer();
				CollatorMethods = new CollatorMethodsContainer();
				LocalesMethods = new LocalesMethodsContainer();
				MessageFormatMethods = new MessageFormatMethodsContainer();
				NormalizeMethods = new NormalizeMethodsContainer();
				RegexMethods = new RegexMethodsContainer();
				ResourceBundleMethods = new ResourceBundleMethodsContainer();
				TransliteratorMethods = new TransliteratorMethodsContainer();
				UnicodeSetMethods = new UnicodeSetMethodsContainer();
				ResetIcuVersionInfo();

				try
				{
					u_cleanup();
				}
				catch
				{
					// ignore failures - can happen when running unit tests
				}

#if NET6_0_OR_GREATER
				if (_IcuCommonLibHandle != IntPtr.Zero)
					NativeLibrary.Free(_IcuCommonLibHandle);
				if (_IcuI18NLibHandle != IntPtr.Zero)
					NativeLibrary.Free(_IcuI18NLibHandle);
#else
				if (IsWindows)
				{
					if (_IcuCommonLibHandle != IntPtr.Zero)
						FreeLibrary(_IcuCommonLibHandle);
					if (_IcuI18NLibHandle != IntPtr.Zero)
						FreeLibrary(_IcuI18NLibHandle);
				}
				else if (!IsMac)
				{
					if (_IcuCommonLibHandle != IntPtr.Zero)
						dlclose(_IcuCommonLibHandle);
					if (_IcuI18NLibHandle != IntPtr.Zero)
						dlclose(_IcuI18NLibHandle);
				}
#endif
				_IcuCommonLibHandle = IntPtr.Zero;
				_IcuI18NLibHandle = IntPtr.Zero;
			}
		}

		private static void ResetIcuVersionInfo()
		{
			Trace.WriteLineIf(Verbose, "icu.net: Resetting ICU version info");
			IcuVersion = 0;
			_IcuPath = null;

#if !NET40
			NativeMethodsHelper.Reset();
			var icuInfo = NativeMethodsHelper.GetIcuVersionInfoForNetCoreOrWindows();

			if (icuInfo.Success)
			{
				_IcuPath = icuInfo.IcuPath.FullName;
				IcuVersion = icuInfo.IcuVersion;
			}
#endif
		}

		// This method is thread-safe and idempotent
		private static T GetMethod<T>(IntPtr handle, string methodName, bool missingInMinimal = false) where T : class
		{
			IntPtr methodPointer;

			var versionedMethodName = $"{methodName}_{IcuVersion}";
#if NET6_0_OR_GREATER
			try
			{
				NativeLibrary.TryGetExport(handle, versionedMethodName, out methodPointer);
			}
			catch (DllNotFoundException)
			{
				methodPointer = IntPtr.Zero;
			}
#else
			methodPointer = IsWindows
				? GetProcAddress(handle, versionedMethodName)
				: IsMac
				? IntPtr.Zero
				: dlsym(handle, versionedMethodName);
#endif

			// Some systems (eg. Tizen) don't use methods with IcuVersion suffix
			if (methodPointer == IntPtr.Zero)
			{
#if NET6_0_OR_GREATER
				try
				{
					NativeLibrary.TryGetExport(handle, methodName, out methodPointer);
				}
				catch (DllNotFoundException) {};
#else
				methodPointer = IsWindows
					? GetProcAddress(handle, methodName)
					: IsMac
					? IntPtr.Zero
					: dlsym(handle, methodName);
#endif
			}

			if (methodPointer != IntPtr.Zero)
			{
				// NOTE: Starting in .NET 4.5.1, Marshal.GetDelegateForFunctionPointer(IntPtr, Type) is obsolete.
#if NET40
				return Marshal.GetDelegateForFunctionPointer(
					methodPointer, typeof(T)) as T;
#else
				return Marshal.GetDelegateForFunctionPointer<T>(methodPointer);
#endif
			}
			if (missingInMinimal)
			{
				throw new MissingMemberException(
					"Do you have the full version of ICU installed? " +
					$"The method '{methodName}' is not included in the minimal version of ICU.");
			}
			return default(T);
		}

		#endregion

		internal static string GetAnsiString(Func<IntPtr, int, Tuple<ErrorCode, int>> lambda,
			int initialLength = 255)
		{
			return GetString(lambda, false, initialLength);
		}

		internal static string GetUnicodeString(Func<IntPtr, int, Tuple<ErrorCode, int>> lambda,
			int initialLength = 255)
		{
			return GetString(lambda, true, initialLength);
		}

		private static string GetString(Func<IntPtr, int, Tuple<ErrorCode, int>> lambda,
			bool isUnicodeString = false, int initialLength = 255)
		{
			var length = initialLength;
			var resPtr = Marshal.AllocCoTaskMem(length * 2);
			try
			{
				var (err, outLength) = lambda(resPtr, length);
				if (err != ErrorCode.BUFFER_OVERFLOW_ERROR && err != ErrorCode.STRING_NOT_TERMINATED_WARNING)
					ExceptionFromErrorCode.ThrowIfError(err);
				if (outLength >= length)
				{
					Marshal.FreeCoTaskMem(resPtr);
					length = outLength + 1; // allow room for the terminating NUL (FWR-505)
					resPtr = Marshal.AllocCoTaskMem(length * 2);
					(err, outLength) = lambda(resPtr, length);
				}

				ExceptionFromErrorCode.ThrowIfError(err);

				if (outLength < 0)
					return null;

				var result = isUnicodeString
					? Marshal.PtrToStringUni(resPtr)
					: Marshal.PtrToStringAnsi(resPtr);
				// Strip any garbage left over at the end of the string.
				if (err == ErrorCode.STRING_NOT_TERMINATED_WARNING && result != null)
					return result.Substring(0, outLength);
				return result;
			}
			finally
			{
				Marshal.FreeCoTaskMem(resPtr);
			}
		}

		/// <summary>
		/// This function does cleanup of the enumerator object
		/// </summary>
		/// <param name="en">Enumeration to be closed</param>
		internal static void uenum_close(IntPtr en)
		{
			if (Methods.uenum_close == null)
				Methods.uenum_close = GetMethod<MethodsContainer.uenum_closeDelegate>(IcuCommonLibHandle, "uenum_close");
			Methods.uenum_close(en);
		}

		/// <summary>
		/// This function returns the next element as a string, or <c>null</c> after all
		/// elements haven been enumerated.
		/// </summary>
		/// <returns>next element as string, or <c>null</c> after all elements haven been
		/// enumerated</returns>
		internal static IntPtr uenum_unext(
			SafeEnumeratorHandle en,
			out int resultLength,
			out ErrorCode status)
		{
			if (Methods.uenum_unext == null)
				Methods.uenum_unext = GetMethod<MethodsContainer.uenum_unextDelegate>(IcuCommonLibHandle, "uenum_unext");
			return Methods.uenum_unext(en, out resultLength, out status);
		}

		internal enum LocaleType
		{
			/// <summary>
			/// This is locale the data actually comes from
			/// </summary>
			ActualLocale = 0,
			/// <summary>
			/// This is the most specific locale supported by ICU
			/// </summary>
			ValidLocale = 1,
		}

		internal enum CollationAttributeValue
		{
			Default = -1, //accepted by most attributes
			Primary = 0, // primary collation strength
			Secondary = 1, // secondary collation strength
			Tertiary = 2, // tertiary collation strength
			Default_Strength = Tertiary,
			Quaternary = 3, //Quaternary collation strength
			Identical = 15, //Identical collation strength

			Off = 16, //Turn the feature off - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode
			On = 17, //Turn the feature on - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode

			Shifted = 20, // Valid for AlternateHandling. Alternate handling will be shifted
			NonIgnorable = 21, // Valid for AlternateHandling. Alternate handling will be non-ignorable

			LowerFirst = 24, // Valid for CaseFirst - lower case sorts before upper case
			UpperFirst = 25 // Valid for CaseFirst - upper case sorts before lower case
		}

		internal enum CollationAttribute
		{
			FrenchCollation,
			AlternateHandling,
			CaseFirst,
			CaseLevel,
			NormalizationMode,
			DecompositionMode = NormalizationMode,
			Strength,
			HiraganaQuaternaryMode,
			NumericCollation,
			AttributeCount
		}

		internal enum CollationResult
		{
			Equal = 0,
			Greater = 1,
			Less = -1
		}

		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class MethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void u_initDelegate(out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void u_cleanupDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr u_getDataDirectoryDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void u_setDataDirectoryDelegate(
				[MarshalAs(UnmanagedType.LPStr)] string directory);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_charNameDelegate(
				int code,
				Character.UCharNameChoice nameChoice,
				IntPtr buffer,
				int bufferLength,
				out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_charDirectionDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_digitDelegate(
				int characterCode,
				byte radix);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_getIntPropertyValueDelegate(
				int characterCode,
				Character.UProperty choice);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void u_getUnicodeVersionDelegate(out VersionInfo versionArray);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void u_getVersionDelegate(out VersionInfo versionArray);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate sbyte u_charTypeDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate double u_getNumericValueDelegate(
				int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
			// which are usually zero, especially in debug builds...but one day we will be sorry.
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool u_ispunctDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
			// which are usually zero, especially in debug builds...but one day we will be sorry.
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool u_isMirroredDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
			// which are usually zero, especially in debug builds...but one day we will be sorry.
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool u_iscntrlDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
			// which are usually zero, especially in debug builds...but one day we will be sorry.
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool u_isspaceDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_tolowerDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_totitleDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_toupperDelegate(int characterCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uenum_closeDelegate(IntPtr en);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uenum_unextDelegate(
				SafeEnumeratorHandle en,
				out int resultLength,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_strToLowerDelegate(IntPtr dest, int destCapacity, string src,
				int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_strToTitleDelegate(IntPtr dest, int destCapacity, string src,
				int srcLength, IntPtr titleIter, [MarshalAs(UnmanagedType.LPStr)] string locale,
				out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_strToUpperDelegate(IntPtr dest, int destCapacity, string src,
				int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);

			internal u_initDelegate u_init;
			internal u_cleanupDelegate u_cleanup;
			internal u_getDataDirectoryDelegate u_getDataDirectory;
			internal u_setDataDirectoryDelegate u_setDataDirectory;
			internal u_charNameDelegate u_charName;
			internal u_charDirectionDelegate u_charDirection;
			internal u_digitDelegate u_digit;
			internal u_getIntPropertyValueDelegate u_getIntPropertyValue;
			internal u_getUnicodeVersionDelegate u_getUnicodeVersion;
			internal u_getVersionDelegate u_getVersion;
			internal u_charTypeDelegate u_charType;
			internal u_getNumericValueDelegate u_getNumericValue;
			internal u_ispunctDelegate u_ispunct;
			internal u_isMirroredDelegate u_isMirrored;
			internal u_iscntrlDelegate u_iscntrl;
			internal u_isspaceDelegate u_isspace;
			internal u_tolowerDelegate u_tolower;
			internal u_totitleDelegate u_totitle;
			internal u_toupperDelegate u_toupper;
			internal uenum_closeDelegate uenum_close;
			internal uenum_unextDelegate uenum_unext;
			internal u_strToLowerDelegate u_strToLower;
			internal u_strToTitleDelegate u_strToTitle;
			internal u_strToUpperDelegate u_strToUpper;
		}

		/// <summary>get the name of an ICU code point</summary>
		internal static void u_init(out ErrorCode errorCode)
		{
			IsInitialized = true;
			if (Methods.u_init == null)
				Methods.u_init = GetMethod<MethodsContainer.u_initDelegate>(IcuCommonLibHandle, "u_init");
			Methods.u_init(out errorCode);
		}

		/// <summary>Clean up the ICU files that could be locked</summary>
		// ReSharper disable once MemberCanBePrivate.Global
		internal static void u_cleanup()
		{
			if (Methods.u_cleanup == null)
				Methods.u_cleanup = GetMethod<MethodsContainer.u_cleanupDelegate>(IcuCommonLibHandle, "u_cleanup");
			Methods.u_cleanup();
			IsInitialized = false;
		}

		/// <summary>Return the ICU data directory</summary>
		internal static IntPtr u_getDataDirectory()
		{
			if (Methods.u_getDataDirectory == null)
				Methods.u_getDataDirectory = GetMethod<MethodsContainer.u_getDataDirectoryDelegate>(IcuCommonLibHandle, "u_getDataDirectory");
			return Methods.u_getDataDirectory();
		}

		/// <summary>Set the ICU data directory</summary>
		internal static void u_setDataDirectory(
			[MarshalAs(UnmanagedType.LPStr)] string directory)
		{
			if (Methods.u_setDataDirectory == null)
				Methods.u_setDataDirectory = GetMethod<MethodsContainer.u_setDataDirectoryDelegate>(IcuCommonLibHandle, "u_setDataDirectory");
			Methods.u_setDataDirectory(directory);
		}

		/// <summary>get the name of an ICU code point</summary>
		internal static int u_charName(
			int code,
			Character.UCharNameChoice nameChoice,
			IntPtr buffer,
			int bufferLength,
			out ErrorCode errorCode)
		{
			if (Methods.u_charName == null)
				Methods.u_charName = GetMethod<MethodsContainer.u_charNameDelegate>(IcuCommonLibHandle, "u_charName");
			return Methods.u_charName(code, nameChoice, buffer, bufferLength, out errorCode);
		}

		/// <summary>Returns the bidirectional category value for the code point, which is used in the Unicode bidirectional algorithm</summary>
		internal static int u_charDirection(int characterCode)
		{
			if (Methods.u_charDirection == null)
				Methods.u_charDirection = GetMethod<MethodsContainer.u_charDirectionDelegate>(IcuCommonLibHandle, "u_charDirection");
			return Methods.u_charDirection(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// get the numeric value for the Unicode digit
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static int u_digit(
			int characterCode,
			byte radix)
		{
			if (Methods.u_digit == null)
				Methods.u_digit = GetMethod<MethodsContainer.u_digitDelegate>(IcuCommonLibHandle, "u_digit");
			return Methods.u_digit(characterCode, radix);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the property value for an enumerated or integer Unicode property for a code point.
		/// </summary>
		/// <param name="codePoint">The codepoint to look up</param>
		/// <param name="which">The property value to look up</param>
		/// <returns>Numeric value that is directly the property value or, for enumerated
		/// properties, corresponds to the numeric value of the enumerated constant of the
		/// respective property value enumeration type (cast to enum type if necessary). Returns
		/// 0 or 1 (for <c>false/true</c>) for binary Unicode properties. Returns a bit-mask for
		/// mask properties. Returns 0 if <paramref name="which"/> is out of bounds or if the
		/// Unicode version does not have data for the property at all, or not for this code point.
		/// </returns>
		/// <remarks>Consider adding a specific implementation for each property!</remarks>
		/// ------------------------------------------------------------------------------------
		internal static int u_getIntPropertyValue(
			int codePoint,
			Character.UProperty which)
		{
			if (Methods.u_getIntPropertyValue == null)
				Methods.u_getIntPropertyValue = GetMethod<MethodsContainer.u_getIntPropertyValueDelegate>(IcuCommonLibHandle, "u_getIntPropertyValue");
			return Methods.u_getIntPropertyValue(codePoint, which);
		}

		internal static void u_getUnicodeVersion(out VersionInfo versionArray)
		{
			if (Methods.u_getUnicodeVersion == null)
				Methods.u_getUnicodeVersion = GetMethod<MethodsContainer.u_getUnicodeVersionDelegate>(IcuCommonLibHandle, "u_getUnicodeVersion");
			Methods.u_getUnicodeVersion(out versionArray);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the ICU release version.
		/// </summary>
		/// <param name="versionArray">Stores the version information for ICU.</param>
		/// ------------------------------------------------------------------------------------
		internal static void u_getVersion(out VersionInfo versionArray)
		{
			if (Methods.u_getVersion == null)
				Methods.u_getVersion = GetMethod<MethodsContainer.u_getVersionDelegate>(IcuCommonLibHandle, "u_getVersion");
			Methods.u_getVersion(out versionArray);
		}

		/// <summary>
		/// Get the general character type.
		/// </summary>
		/// <param name="characterCode"></param>
		/// <returns></returns>
		internal static sbyte u_charType(int characterCode)
		{
			if (Methods.u_charType == null)
				Methods.u_charType = GetMethod<MethodsContainer.u_charTypeDelegate>(IcuCommonLibHandle, "u_charType");
			return Methods.u_charType(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///Get the numeric value for a Unicode code point as defined in the Unicode Character Database.
		///A "double" return type is necessary because some numeric values are fractions, negative, or too large for int32_t.
		///For characters without any numeric values in the Unicode Character Database,
		///this function will return U_NO_NUMERIC_VALUE.
		///
		///Similar to java.lang.Character.getNumericValue(), but u_getNumericValue() also supports negative values,
		///large values, and fractions, while Java's getNumericValue() returns values 10..35 for ASCII letters.
		///</summary>
		///<remarks>
		///  See also:
		///      U_NO_NUMERIC_VALUE
		///  Stable:
		///      ICU 2.2
		/// http://oss.software.ibm.com/icu/apiref/uchar_8h.html#a477
		/// </remarks>
		///<param name="characterCode">Code point to get the numeric value for</param>
		///<returns>Numeric value of c, or U_NO_NUMERIC_VALUE if none is defined.</returns>
		/// ------------------------------------------------------------------------------------
		internal static double u_getNumericValue(
			int characterCode)
		{
			if (Methods.u_getNumericValue == null)
				Methods.u_getNumericValue = GetMethod<MethodsContainer.u_getNumericValueDelegate>(IcuCommonLibHandle, "u_getNumericValue");
			return Methods.u_getNumericValue(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	Determines whether the specified code point is a punctuation character.
		/// </summary>
		/// <param name="characterCode">the code point to be tested</param>
		/// ------------------------------------------------------------------------------------
		internal static bool u_ispunct(
			int characterCode)
		{
			if (Methods.u_ispunct == null)
				Methods.u_ispunct = GetMethod<MethodsContainer.u_ispunctDelegate>(IcuCommonLibHandle, "u_ispunct");
			return Methods.u_ispunct(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	Determines whether the code point has the Bidi_Mirrored property.
		///
		///	This property is set for characters that are commonly used in Right-To-Left contexts
		///	and need to be displayed with a "mirrored" glyph.
		///
		///	Same as java.lang.Character.isMirrored(). Same as UCHAR_BIDI_MIRRORED
		/// </summary>
		///	<remarks>
		///	See also:
		///	    UCHAR_BIDI_MIRRORED
		///
		///	Stable:
		///	    ICU 2.0
		///	</remarks>
		/// <param name="characterCode">the code point to be tested</param>
		/// <returns><c>true</c> if the character has the Bidi_Mirrored property</returns>
		/// ------------------------------------------------------------------------------------
		internal static bool u_isMirrored(
			int characterCode)
		{
			if (Methods.u_isMirrored == null)
				Methods.u_isMirrored = GetMethod<MethodsContainer.u_isMirroredDelegate>(IcuCommonLibHandle, "u_isMirrored");
			return Methods.u_isMirrored(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	Determines whether the specified code point is a control character. A control
		///	character is one of the following:
		/// <list>
		///	<item>ISO 8-bit control character (U+0000..U+001f and U+007f..U+009f)</item>
		///	<item>U_CONTROL_CHAR (Cc)</item>
		///	<item>U_FORMAT_CHAR (Cf)</item>
		///	<item>U_LINE_SEPARATOR (Zl)</item>
		///	<item>U_PARAGRAPH_SEPARATOR (Zp)</item>
		///	</list>
		/// </summary>
		/// <param name="characterCode">the code point to be tested</param>
		/// ------------------------------------------------------------------------------------
		internal static bool u_iscntrl(
			int characterCode)
		{
			if (Methods.u_iscntrl == null)
				Methods.u_iscntrl = GetMethod<MethodsContainer.u_iscntrlDelegate>(IcuCommonLibHandle, "u_iscntrl");
			return Methods.u_iscntrl(characterCode);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	Determines whether the specified character is a space character.
		/// </summary>
		/// <remarks>
		///	See also:
		///	<list>
		///	<item>u_isJavaSpaceChar</item>
		///	<item>u_isWhitespace</item>
		/// <item>u_isUWhiteSpace</item>
		///	</list>
		///
		///	Stable:
		///	    ICU 2.0
		///	</remarks>
		/// <param name="characterCode">the code point to be tested</param>
		/// ------------------------------------------------------------------------------------
		internal static bool u_isspace(
			int characterCode)
		{
			if (Methods.u_isspace == null)
				Methods.u_isspace = GetMethod<MethodsContainer.u_isspaceDelegate>(IcuCommonLibHandle, "u_isspace");
			return Methods.u_isspace(characterCode);
		}

		/// <summary>Map character to its lowercase equivalent according to UnicodeData.txt</summary>
		internal static int u_tolower(int characterCode)
		{
			if (Methods.u_tolower == null)
				Methods.u_tolower = GetMethod<MethodsContainer.u_tolowerDelegate>(IcuCommonLibHandle, "u_tolower");
			return Methods.u_tolower(characterCode);
		}

		/// <summary>Map character to its title-case equivalent according to UnicodeData.txt</summary>
		internal static int u_totitle(int characterCode)
		{
			if (Methods.u_totitle == null)
				Methods.u_totitle = GetMethod<MethodsContainer.u_totitleDelegate>(IcuCommonLibHandle, "u_totitle");
			return Methods.u_totitle(characterCode);
		}

		/// <summary>Map character to its uppercase equivalent according to UnicodeData.txt</summary>
		internal static int u_toupper(int characterCode)
		{
			if (Methods.u_toupper == null)
				Methods.u_toupper = GetMethod<MethodsContainer.u_toupperDelegate>(IcuCommonLibHandle, "u_toupper");
			return Methods.u_toupper(characterCode);
		}

		/// <summary>Return the lower case equivalent of the string.</summary>
		internal static int u_strToLower(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (Methods.u_strToLower == null)
				Methods.u_strToLower = GetMethod<MethodsContainer.u_strToLowerDelegate>(IcuCommonLibHandle, "u_strToLower");
			return Methods.u_strToLower(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

		internal static int u_strToTitle(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale,
			out ErrorCode errorCode)
		{
			return u_strToTitle(dest, destCapacity, src, srcLength, IntPtr.Zero, locale,
				out errorCode);
		}

		/// <summary>Return the title case equivalent of the string.</summary>
		// ReSharper disable once MemberCanBePrivate.Global
		internal static int u_strToTitle(IntPtr dest, int destCapacity, string src,
			int srcLength, IntPtr titleIter, [MarshalAs(UnmanagedType.LPStr)] string locale,
			out ErrorCode errorCode)
		{
			if (Methods.u_strToTitle == null)
				Methods.u_strToTitle = GetMethod<MethodsContainer.u_strToTitleDelegate>(IcuCommonLibHandle, "u_strToTitle", true);
			return Methods.u_strToTitle(dest, destCapacity, src, srcLength, titleIter,
				locale, out errorCode);
		}

		/// <summary>Return the upper case equivalent of the string.</summary>
		internal static int u_strToUpper(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (Methods.u_strToUpper == null)
				Methods.u_strToUpper = GetMethod<MethodsContainer.u_strToUpperDelegate>(IcuCommonLibHandle, "u_strToUpper");
			return Methods.u_strToUpper(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

	}
}
