// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Icu.Collation;

namespace Icu
{
	internal static class NativeMethods
	{
		private const int MinIcuVersionDefault = 44;
		private const int MaxIcuVersionDefault = 60;
		private static int minIcuVersion = MinIcuVersionDefault;
		private static int maxIcuVersion = MaxIcuVersionDefault;

		public static void SetMinMaxIcuVersions(int minVersion = MinIcuVersionDefault,
			int maxVersion = MaxIcuVersionDefault)
		{
			if (minVersion < MinIcuVersionDefault || minVersion > MaxIcuVersionDefault)
			{
				throw new ArgumentOutOfRangeException("minVersion",
					string.Format("supported ICU versions are between {0} and {1}",
					MinIcuVersionDefault, MaxIcuVersionDefault));
			}
			if (maxVersion < MinIcuVersionDefault || maxVersion > MaxIcuVersionDefault)
			{
				throw new ArgumentOutOfRangeException("maxVersion",
					string.Format("supported ICU versions are between {0} and {1}",
					MinIcuVersionDefault, MaxIcuVersionDefault));
			}
			minIcuVersion = Math.Min(minVersion, maxVersion);
			maxIcuVersion = Math.Max(minVersion, maxVersion);
		}

		private static MethodsContainer Methods;

		static NativeMethods()
		{
			Methods = new MethodsContainer();
		}

		#region Dynamic method loading

		#region Native methods for Linux

		private const int RTLD_NOW = 2;

		[DllImport("libdl.so", SetLastError = true)]
		private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string file, int mode);

		[DllImport("libdl.so", SetLastError = true)]
		private static extern int dlclose(IntPtr handle);

		[DllImport("libdl.so", SetLastError = true)]
		private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string name);

		#endregion

		#region Native methods for Windows

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LoadLibraryEx(string dllToLoad, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[Flags]
		internal enum LoadLibraryFlags : uint
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

		private static int IcuVersion;
		private static string _IcuPath;
		private static IntPtr _IcuCommonLibHandle;
		private static IntPtr _IcuI18NLibHandle;

		private static bool IsWindows
		{
			get { return Platform.OperatingSystem == OperatingSystemType.Windows; }
		}

		private static IntPtr IcuCommonLibHandle
		{
			get
			{
				if (_IcuCommonLibHandle == IntPtr.Zero)
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
				Assembly currentAssembly = typeof(NativeMethods).Assembly;
#else
				Assembly currentAssembly = typeof(NativeMethods).GetTypeInfo().Assembly;
#endif
				var managedPath = currentAssembly.CodeBase ?? currentAssembly.Location;
				var uri = new Uri(managedPath);

				return Path.GetDirectoryName(uri.LocalPath);
			}
		}

		private static void AddDirectoryToSearchPath(string directory)
		{
			// Only perform this for Linux because we are using LoadLibraryEx
			// to ensure that a library's dependencies is loaded starting from
			// where that library is located.
			if (!IsWindows)
			{
				var ldLibPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
				Environment.SetEnvironmentVariable("LD_LIBRARY_PATH",
					string.Format("{0}:{1}", directory, ldLibPath));
			}
		}

		private static bool CheckDirectoryForIcuBinaries(string directory, string libraryName)
		{
			if (!Directory.Exists(directory))
				return false;

			var filePattern = IsWindows ? libraryName + "*.dll" : "lib" + libraryName + ".so.*";
			var files = Directory.EnumerateFiles(directory, filePattern).ToList();
			if (files.Count > 0)
			{
				// Do a reverse sort so that we use the highest version
				files.Sort((x, y) => string.CompareOrdinal(y, x));
				var filePath = files[0];
				var version = IsWindows
					? Path.GetFileNameWithoutExtension(filePath).Substring(5) // strip icuuc
					: Path.GetFileName(filePath).Substring(12); // strip libicuuc.so.
				int icuVersion;
				if (int.TryParse(version, out icuVersion))
				{
					Trace.TraceInformation("Setting IcuVersion to {0} (found in {1})",
						icuVersion, directory);
					IcuVersion = icuVersion;
					_IcuPath = directory;

					AddDirectoryToSearchPath(directory);
					return true;
				}
			}
			return false;
		}

		private static IntPtr LoadIcuLibrary(string libraryName)
		{
			if (IcuVersion <= 0)
			{
				// Look for ICU binaries in x86/x64 subdirectory first
				if (!CheckDirectoryForIcuBinaries(
					Path.Combine(DirectoryOfThisAssembly, Platform.ProcessArchitecture),
					libraryName))
				{
					// otherwise check the current directory
					CheckDirectoryForIcuBinaries(DirectoryOfThisAssembly, libraryName);
					// If we don't find it here we rely on it being in the PATH somewhere...
				}
			}
			var handle = GetIcuLibHandle(libraryName, IcuVersion > 0 ? IcuVersion : maxIcuVersion);
			if (handle == IntPtr.Zero)
			{
				throw new FileLoadException(string.Format("Can't load ICU library (version {0})", IcuVersion),
					libraryName);
			}
			return handle;
		}

		private static IntPtr GetIcuLibHandle(string basename, int icuVersion)
		{
			if (icuVersion < minIcuVersion)
				return IntPtr.Zero;
			IntPtr handle;
			string libPath;
			if (IsWindows)
			{
				var libName = string.Format("{0}{1}.dll", basename, icuVersion);
				var isIcuPathSpecified = !string.IsNullOrEmpty(_IcuPath);
				libPath = isIcuPathSpecified ? Path.Combine(_IcuPath, libName) : libName;

				var loadLibraryFlags = LoadLibraryFlags.NONE;

				if (isIcuPathSpecified)
					loadLibraryFlags |= LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS;

				handle = LoadLibraryEx(libPath, IntPtr.Zero, loadLibraryFlags);
				var lastError = Marshal.GetLastWin32Error();

				if (handle == IntPtr.Zero && lastError != 0)
				{
					string errorMessage = new Win32Exception(lastError).Message;
					Trace.WriteLine(string.Format("Unable to load [{0}]. Error: {1}", libPath, errorMessage));
				}
			}
			else
			{
				var libName = string.Format("lib{0}.so.{1}", basename, icuVersion);
				libPath = string.IsNullOrEmpty(_IcuPath) ? libName : Path.Combine(_IcuPath, libName);
				handle = dlopen(libPath, RTLD_NOW);
			}
			if (handle == IntPtr.Zero)
			{
				Trace.TraceWarning("{0} of {1} failed with error {2}",
					IsWindows ? "LoadLibrary" : "dlopen",
					libPath, Marshal.GetLastWin32Error());
				return GetIcuLibHandle(basename, icuVersion - 1);
			}

			IcuVersion = icuVersion;
			return handle;
		}

		public static void Cleanup()
		{
			u_cleanup();
			if (IsWindows)
			{
				if (_IcuCommonLibHandle != IntPtr.Zero)
					FreeLibrary(_IcuCommonLibHandle);
				if (_IcuI18NLibHandle != IntPtr.Zero)
					FreeLibrary(_IcuI18NLibHandle);
			}
			else
			{
				if (_IcuCommonLibHandle != IntPtr.Zero)
					dlclose(_IcuCommonLibHandle);
				if (_IcuI18NLibHandle != IntPtr.Zero)
					dlclose(_IcuI18NLibHandle);
			}
			_IcuCommonLibHandle = IntPtr.Zero;
			_IcuI18NLibHandle = IntPtr.Zero;
			IcuVersion = 0;
			_IcuPath = null;
			Methods = new MethodsContainer();
		}

		private static T GetMethod<T>(IntPtr handle, string methodName, bool missingInMinimal = false) where T: class
		{
			var versionedMethodName = string.Format("{0}_{1}", methodName, IcuVersion);
			var methodPointer = IsWindows ?
				GetProcAddress(handle, versionedMethodName) :
				dlsym(handle, versionedMethodName);
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
					string.Format("Do you have the full version of ICU installed? " +
					"The method '{0}' is not included in the minimal version of ICU.", methodName));
			}
			return default(T);
		}

		#endregion

		/// <summary>
		/// This function does cleanup of the enumerator object
		/// </summary>
		/// <param name="en">Enumeration to be closed</param>
		public static void uenum_close(IntPtr en)
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
		public static IntPtr uenum_unext(
			RuleBasedCollator.SafeEnumeratorHandle en,
			out int resultLength,
			out ErrorCode status)
		{
			if (Methods.uenum_unext == null)
				Methods.uenum_unext = GetMethod<MethodsContainer.uenum_unextDelegate>(IcuCommonLibHandle, "uenum_unext");
			return Methods.uenum_unext(en, out resultLength, out status);
		}

		#region Unicode collator

		/// <summary>
		/// Open a Collator for comparing strings.
		/// Collator pointer is used in all the calls to the Collation
		/// service. After finished, collator must be disposed of by calling ucol_close
		/// </summary>
		/// <param name="loc">The locale containing the required collation rules.
		///Special values for locales can be passed in -
		///if NULL is passed for the locale, the default locale
		///collation rules will be used. If empty string ("") or
		///"root" are passed, UCA rules will be used.</param>
		/// <param name="status">A pointer to an ErrorCode to receive any errors
		///</param>
		/// <returns>pointer to a Collator or 0 if an error occurred</returns>
		public static RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_open(
			[MarshalAs(UnmanagedType.LPStr)] string loc,
			out ErrorCode status)
		{
			if (Methods.ucol_open == null)
				Methods.ucol_open = GetMethod<MethodsContainer.ucol_openDelegate>(IcuI18NLibHandle, "ucol_open");
			return Methods.ucol_open(loc, out status);
		}

		///<summary>
		/// Produce an Collator instance according to the rules supplied.
		/// The rules are used to change the default ordering, defined in the
		/// UCA in a process called tailoring. The resulting Collator pointer
		/// can be used in the same way as the one obtained by ucol_strcoll.
		/// </summary>
		/// <param name="rules">A string describing the collation rules. For the syntax
		///    of the rules please see users guide.</param>
		/// <param name="rulesLength">The length of rules, or -1 if null-terminated.</param>
		/// <param name="normalizationMode">The normalization mode</param>
		/// <param name="strength">The default collation strength; can be also set in the rules</param>
		/// <param name="parseError">A pointer to ParseError to recieve information about errors
		/// occurred during parsing. This argument can currently be set
		/// to NULL, but at users own risk. Please provide a real structure.</param>
		/// <param name="status">A pointer to an ErrorCode to receive any errors</param>
		/// <returns>A pointer to a UCollator. It is not guaranteed that NULL be returned in case
		///         of error - please use status argument to check for errors.</returns>
		public static RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openRules(
			[MarshalAs(UnmanagedType.LPWStr)] string rules,
			int rulesLength,
			NormalizationMode normalizationMode,
			CollationStrength strength,
			ref ParseError parseError,
			out ErrorCode status)
		{
			if (Methods.ucol_openRules == null)
				Methods.ucol_openRules = GetMethod<MethodsContainer.ucol_openRulesDelegate>(IcuI18NLibHandle, "ucol_openRules");
			return Methods.ucol_openRules(rules, rulesLength, normalizationMode, strength, ref parseError,
				out status);
		}

		/// <summary>
		/// Close a UCollator.
		/// Once closed, a UCollator should not be used. Every open collator should
		/// be closed. Otherwise, a memory leak will result.
		/// </summary>
		/// <param name="coll">The UCollator to close.</param>
		public static void ucol_close(IntPtr coll)
		{
			if (Methods.ucol_close == null)
				Methods.ucol_close = GetMethod<MethodsContainer.ucol_closeDelegate>(IcuI18NLibHandle, "ucol_close");
			Methods.ucol_close(coll);
		}

		/**
 * Compare two strings.
 * The strings will be compared using the options already specified.
 * @param coll The UCollator containing the comparison rules.
 * @param source The source string.
 * @param sourceLength The length of source, or -1 if null-terminated.
 * @param target The target string.
 * @param targetLength The length of target, or -1 if null-terminated.
 * @return The result of comparing the strings; one of UCOL_EQUAL,
 * UCOL_GREATER, UCOL_LESS
 * @see ucol_greater
 * @see ucol_greaterOrEqual
 * @see ucol_equal
 * @stable ICU 2.0
 */

		public static CollationResult ucol_strcoll(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[MarshalAs(UnmanagedType.LPWStr)] string target,
			Int32 targetLength)
		{
			if (Methods.ucol_strcoll == null)
				Methods.ucol_strcoll = GetMethod<MethodsContainer.ucol_strcollDelegate>(IcuI18NLibHandle, "ucol_strcoll");
			return Methods.ucol_strcoll(collator, source, sourceLength, target, targetLength);
		}

		/**
 * Determine how many locales have collation rules available.
 * This function is most useful as determining the loop ending condition for
 * calls to {@link #ucol_getAvailable }.
 * @return The number of locales for which collation rules are available.
 * @see ucol_getAvailable
 * @stable ICU 2.0
 */

		public static Int32 ucol_countAvailable()
		{
			if (Methods.ucol_countAvailable == null)
				Methods.ucol_countAvailable = GetMethod<MethodsContainer.ucol_countAvailableDelegate>(IcuI18NLibHandle, "ucol_countAvailable");
			return Methods.ucol_countAvailable();
		}

		/**
 * Create a string enumerator of all locales for which a valid
 * collator may be opened.
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */

		public static RuleBasedCollator.SafeEnumeratorHandle ucol_openAvailableLocales(out ErrorCode status)
		{
			if (Methods.ucol_openAvailableLocales == null)
				Methods.ucol_openAvailableLocales = GetMethod<MethodsContainer.ucol_openAvailableLocalesDelegate>(IcuI18NLibHandle, "ucol_openAvailableLocales");
			return Methods.ucol_openAvailableLocales(out status);
		}


		/**
 * Get a sort key for a string from a UCollator.
 * Sort keys may be compared using <TT>strcmp</TT>.
 * @param coll The UCollator containing the collation rules.
 * @param source The string to transform.
 * @param sourceLength The length of source, or -1 if null-terminated.
 * @param result A pointer to a buffer to receive the attribute.
 * @param resultLength The maximum size of result.
 * @return The size needed to fully store the sort key..
 * @see ucol_keyHashCode
 * @stable ICU 2.0
 */

		public static Int32 ucol_getSortKey(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result,
			Int32 resultLength)
		{
			if (Methods.ucol_getSortKey == null)
				Methods.ucol_getSortKey = GetMethod<MethodsContainer.ucol_getSortKeyDelegate>(IcuI18NLibHandle, "ucol_getSortKey");
			return Methods.ucol_getSortKey(collator, source, sourceLength, result, resultLength);
		}

		/**
 * Universal attribute setter
 * @param coll collator which attributes are to be changed
 * @param attr attribute type
 * @param value attribute value
 * @param status to indicate whether the operation went on smoothly or there were errors
 * @see UColAttribute
 * @see UColAttributeValue
 * @see ucol_getAttribute
 * @stable ICU 2.0
 */

		public static void ucol_setAttribute(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			CollationAttributeValue value,
			out ErrorCode status)
		{
			if (Methods.ucol_setAttribute == null)
				Methods.ucol_setAttribute = GetMethod<MethodsContainer.ucol_setAttributeDelegate>(IcuI18NLibHandle, "ucol_setAttribute");
			Methods.ucol_setAttribute(collator, attr, value, out status);
		}

		/**
 * Universal attribute getter
 * @param coll collator which attributes are to be changed
 * @param attr attribute type
 * @return attribute value
 * @param status to indicate whether the operation went on smoothly or there were errors
 * @see UColAttribute
 * @see UColAttributeValue
 * @see ucol_setAttribute
 * @stable ICU 2.0
 */

		public static CollationAttributeValue ucol_getAttribute(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			out ErrorCode status)
		{
			if (Methods.ucol_getAttribute == null)
				Methods.ucol_getAttribute = GetMethod<MethodsContainer.ucol_getAttributeDelegate>(IcuI18NLibHandle, "ucol_getAttribute");
			return Methods.ucol_getAttribute(collator, attr, out status);
		}

		/**
 * Thread safe cloning operation. The result is a clone of a given collator.
 * @param coll collator to be cloned
 * @param stackBuffer user allocated space for the new clone.
 * If NULL new memory will be allocated.
 *  If buffer is not large enough, new memory will be allocated.
 *  Clients can use the U_COL_SAFECLONE_BUFFERSIZE.
 *  This will probably be enough to avoid memory allocations.
 * @param pBufferSize pointer to size of allocated space.
 *  If *pBufferSize == 0, a sufficient size for use in cloning will
 *  be returned ('pre-flighting')
 *  If *pBufferSize is not enough for a stack-based safe clone,
 *  new memory will be allocated.
 * @param status to indicate whether the operation went on smoothly or there were errors
 *    An informational status value, U_SAFECLONE_ALLOCATED_ERROR, is used if any
 * allocations were necessary.
 * @return pointer to the new clone
 * @see ucol_open
 * @see ucol_openRules
 * @see ucol_close
 * @stable ICU 2.0
 */

		public static RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeClone(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			IntPtr stackBuffer,
			ref Int32 pBufferSize,
			out ErrorCode status)
		{
			if (Methods.ucol_safeClone == null)
				Methods.ucol_safeClone = GetMethod<MethodsContainer.ucol_safeCloneDelegate>(IcuI18NLibHandle, "ucol_safeClone");
			return Methods.ucol_safeClone(collator, stackBuffer, ref pBufferSize, out status);
		}

		/**
 * gets the locale name of the collator. If the collator
 * is instantiated from the rules, then this function returns
 * NULL.
 * @param coll The UCollator for which the locale is needed
 * @param type You can choose between requested, valid and actual
 *             locale. For description see the definition of
 *             ULocDataLocaleType in uloc.h
 * @param status error code of the operation
 * @return real locale name from which the collation data comes.
 *         If the collator was instantiated from rules, returns
 *         NULL.
 * @stable ICU 2.8
 *
 */

		// Return IntPtr instead of marshalling string as unmanaged LPStr. By default, marshalling
		// creates a copy of the string and tries to de-allocate the C memory used by the
		// char*. Using IntPtr will not create a copy of any object and therefore will not
		// try to de-allocate memory. De-allocating memory from a string literal is not a
		// good Idea. To call the function use Marshal.PtrToString*(ucol_getLocaleByType(...);

		public static IntPtr ucol_getLocaleByType(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			LocaleType type,
			out ErrorCode status)
		{
			if (Methods.ucol_getLocaleByType == null)
				Methods.ucol_getLocaleByType = GetMethod<MethodsContainer.ucol_getLocaleByTypeDelegate>(IcuI18NLibHandle, "ucol_getLocaleByType");
			return Methods.ucol_getLocaleByType(collator, type, out status);
		}


		public static int ucol_getRulesEx(
			RuleBasedCollator.SafeRuleBasedCollatorHandle coll,
			UColRuleOption delta,
			IntPtr buffer,
			int bufferLen)
		{
			if (Methods.ucol_getRulesEx == null)
				Methods.ucol_getRulesEx = GetMethod<MethodsContainer.ucol_getRulesExDelegate>(IcuI18NLibHandle, "ucol_getRulesEx");
			return Methods.ucol_getRulesEx(coll, delta, buffer, bufferLen);
		}

		public static int ucol_getBound(byte[] source, int sourceLength,
			UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
			out ErrorCode status)
		{
			if (Methods.ucol_getBound == null)
				Methods.ucol_getBound = GetMethod<MethodsContainer.ucol_getBoundDelegate>(IcuI18NLibHandle, "ucol_getBound");
			return Methods.ucol_getBound(source, sourceLength, boundType, noOfLevels, result, resultLength, out status);
		}

		#endregion // Unicode collator

		public enum  LocaleType
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

		public enum CollationAttributeValue
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

		public enum CollationAttribute
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

		public enum CollationResult
		{
			Equal = 0,
			Greater = 1,
			Less = -1
		}

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
				[MarshalAs(UnmanagedType.LPStr)]string directory);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int u_charNameDelegate(
				int code,
				Character.UCharNameChoice nameChoice,
				IntPtr buffer,
				int bufferLength,
				out ErrorCode errorCode);

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
			internal delegate int u_charTypeDelegate(int characterCode);

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
			internal delegate void uenum_closeDelegate(IntPtr en);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uenum_unextDelegate(
				RuleBasedCollator.SafeEnumeratorHandle en,
				out int resultLength,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uloc_getLCIDDelegate([MarshalAs(UnmanagedType.LPStr)]string localeID);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uloc_getLocaleForLCIDDelegate(int lcid, IntPtr locale, int localeCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr uloc_getISO3CountryDelegate(
				[MarshalAs(UnmanagedType.LPStr)]string locale);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr uloc_getISO3LanguageDelegate(
				[MarshalAs(UnmanagedType.LPStr)]string locale);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openDelegate(
				[MarshalAs(UnmanagedType.LPStr)] string loc,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openRulesDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string rules,
				int rulesLength,
				NormalizationMode normalizationMode,
				CollationStrength strength,
				ref ParseError parseError,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucol_closeDelegate(IntPtr coll);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate CollationResult ucol_strcollDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				[MarshalAs(UnmanagedType.LPWStr)] string source,
				Int32 sourceLength,
				[MarshalAs(UnmanagedType.LPWStr)] string target,
				Int32 targetLength);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Int32 ucol_countAvailableDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeEnumeratorHandle ucol_openAvailableLocalesDelegate(out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Int32 ucol_getSortKeyDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				[MarshalAs(UnmanagedType.LPWStr)] string source,
				Int32 sourceLength,
				[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result,
				Int32 resultLength);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucol_setAttributeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				CollationAttribute attr,
				CollationAttributeValue value,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate CollationAttributeValue ucol_getAttributeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				CollationAttribute attr,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeCloneDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				IntPtr stackBuffer,
				ref int pBufferSize,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ucol_getLocaleByTypeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				LocaleType type,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_getRulesExDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle coll,
				UColRuleOption delta,
				IntPtr buffer,
				int bufferLen);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_getBoundDelegate(byte[] source, int sourceLength,
				UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_countAvailableDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr uloc_getAvailableDelegate(int n);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getLanguageDelegate(string localeID, IntPtr language,
				int languageCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getScriptDelegate(string localeID, IntPtr script,
				int scriptCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getCountryDelegate(string localeID, IntPtr country,
				int countryCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getVariantDelegate(string localeID, IntPtr variant,
				int variantCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getDisplayNameDelegate(string localeID, string inLocaleID,
				IntPtr result, int maxResultSize, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getDisplayLanguageDelegate(string localeID, string displayLocaleID,
				IntPtr result, int maxResultSize, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getDisplayScriptDelegate(string localeID, string displayLocaleID,
				IntPtr result, int maxResultSize, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getDisplayCountryDelegate(string localeID, string displayLocaleID,
				IntPtr result, int maxResultSize, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getDisplayVariantDelegate(string localeID, string displayLocaleID,
				IntPtr result, int maxResultSize, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getNameDelegate(string localeID, IntPtr name,
				int nameCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_getBaseNameDelegate(string localeID, IntPtr name,
				int nameCapacity, out ErrorCode err);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int uloc_canonicalizeDelegate(string localeID, IntPtr name,
				int nameCapacity, out ErrorCode err);

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

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm_normalizeDelegate(string source, int sourceLength,
				Normalizer.UNormalizationMode mode, int options,
				IntPtr result, int resultLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate byte unorm_isNormalizedDelegate(string source, int sourceLength,
				Normalizer.UNormalizationMode mode, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ubrk_openDelegate(BreakIterator.UBreakIteratorType type,
				string locale, string text, int textLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ubrk_openRulesDelegate(string rules, int rulesLength,
				string text, int textLength, out ParseError parseError, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ubrk_closeDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubrk_firstDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubrk_nextDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubrk_getRuleStatusDelegate(IntPtr bi);

			/// <summary>
			/// Get the statuses from the break rules that determined the most
			/// recently returned break position.  The values appear in the rule
			/// source within brackets, {123}, for example.The default status value
			/// for rules that do not explicitly provide one is zero.
			///
			/// For word break iterators, the possible values are defined in
			/// <see cref="Icu.BreakIterator.UWordBreak"/>
			/// </summary>
			/// <returns>The number of rule status values that determined the most recent
			/// boundary returned from the break iterator.</returns>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubrk_getRuleStatusVecDelegate(IntPtr bi,
				[Out, MarshalAs(UnmanagedType.LPArray)]Int32[] fillInVector,
				Int32 capacity,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ubrk_setTextDelegate(IntPtr bi, string text, int textLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uset_closeDelegate(IntPtr set);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uset_openDelegate(char start, char end);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uset_openPatternDelegate(string pattern, int patternLength, ref ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uset_addDelegate(IntPtr set, char c);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uset_toPatternDelegate(IntPtr set, IntPtr result, int resultCapacity,
				bool escapeUnprintable, ref ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uset_addStringDelegate(IntPtr set, string str, int strLen);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uset_getItemDelegate(IntPtr set, int itemIndex, out int start,
				out int end, IntPtr str, int strCapacity, ref ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uset_getItemCountDelegate(IntPtr set);


			internal u_initDelegate u_init;
			internal u_cleanupDelegate u_cleanup;
			internal u_getDataDirectoryDelegate u_getDataDirectory;
			internal u_setDataDirectoryDelegate u_setDataDirectory;
			internal u_charNameDelegate u_charName;
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
			internal uenum_closeDelegate uenum_close;
			internal uenum_unextDelegate uenum_unext;
			internal ucol_openDelegate ucol_open;
			internal ucol_openRulesDelegate ucol_openRules;
			internal ucol_closeDelegate ucol_close;
			internal ucol_strcollDelegate ucol_strcoll;
			internal ucol_countAvailableDelegate ucol_countAvailable;
			internal ucol_openAvailableLocalesDelegate ucol_openAvailableLocales;
			internal ucol_getSortKeyDelegate ucol_getSortKey;
			internal ucol_setAttributeDelegate ucol_setAttribute;
			internal ucol_getAttributeDelegate ucol_getAttribute;
			internal ucol_safeCloneDelegate ucol_safeClone;
			internal ucol_getLocaleByTypeDelegate ucol_getLocaleByType;
			internal ucol_getRulesExDelegate ucol_getRulesEx;
			internal ucol_getBoundDelegate ucol_getBound;
			internal uloc_countAvailableDelegate uloc_countAvailable;
			internal uloc_getLCIDDelegate uloc_getLCID;
			internal uloc_getLocaleForLCIDDelegate uloc_getLocaleForLCID;
			internal uloc_getISO3CountryDelegate uloc_getISO3Country;
			internal uloc_getISO3LanguageDelegate uloc_getISO3Language;
			internal uloc_getAvailableDelegate uloc_getAvailable;
			internal uloc_getLanguageDelegate uloc_getLanguage;
			internal uloc_getScriptDelegate uloc_getScript;
			internal uloc_getCountryDelegate uloc_getCountry;
			internal uloc_getVariantDelegate uloc_getVariant;
			internal uloc_getDisplayNameDelegate uloc_getDisplayName;
			internal uloc_getDisplayLanguageDelegate uloc_getDisplayLanguage;
			internal uloc_getDisplayScriptDelegate uloc_getDisplayScript;
			internal uloc_getDisplayCountryDelegate uloc_getDisplayCountry;
			internal uloc_getDisplayVariantDelegate uloc_getDisplayVariant;
			internal uloc_getNameDelegate uloc_getName;
			internal uloc_getBaseNameDelegate uloc_getBaseName;
			internal uloc_canonicalizeDelegate uloc_canonicalize;
			internal u_strToLowerDelegate u_strToLower;
			internal u_strToTitleDelegate u_strToTitle;
			internal u_strToUpperDelegate u_strToUpper;
			internal unorm_normalizeDelegate _unorm_normalize;
			internal unorm_isNormalizedDelegate _unorm_isNormalized;
			internal ubrk_openDelegate ubrk_open;
			internal ubrk_openRulesDelegate ubrk_openRules;
			internal ubrk_closeDelegate ubrk_close;
			internal ubrk_firstDelegate ubrk_first;
			internal ubrk_nextDelegate ubrk_next;
			internal ubrk_getRuleStatusDelegate ubrk_getRuleStatus;
			internal ubrk_getRuleStatusVecDelegate ubrk_getRuleStatusVec;
			internal ubrk_setTextDelegate ubrk_setText;
			internal uset_closeDelegate uset_close;
			internal uset_openDelegate uset_open;
			internal uset_openPatternDelegate uset_openPattern;
			internal uset_addDelegate uset_add;
			internal uset_toPatternDelegate uset_toPattern;
			internal uset_addStringDelegate uset_addString;
			internal uset_getItemDelegate uset_getItem;
			internal uset_getItemCountDelegate uset_getItemCount;
		}

		/// <summary>get the name of an ICU code point</summary>
		public static void u_init(out ErrorCode errorCode)
		{
			if (Methods.u_init == null)
				Methods.u_init = GetMethod<MethodsContainer.u_initDelegate>(IcuCommonLibHandle, "u_init");
			Methods.u_init(out errorCode);
		}

		/// <summary>Clean up the ICU files that could be locked</summary>
		public static void u_cleanup()
		{
			if (Methods.u_cleanup == null)
				Methods.u_cleanup = GetMethod<MethodsContainer.u_cleanupDelegate>(IcuCommonLibHandle, "u_cleanup");
			Methods.u_cleanup();
		}

		/// <summary>Return the ICU data directory</summary>
		public static IntPtr u_getDataDirectory()
		{
			if (Methods.u_getDataDirectory == null)
				Methods.u_getDataDirectory = GetMethod<MethodsContainer.u_getDataDirectoryDelegate>(IcuCommonLibHandle, "u_getDataDirectory");
			return Methods.u_getDataDirectory();
		}

		/// <summary>Set the ICU data directory</summary>
		public static void u_setDataDirectory(
			[MarshalAs(UnmanagedType.LPStr)]string directory)
		{
			if (Methods.u_setDataDirectory == null)
				Methods.u_setDataDirectory = GetMethod<MethodsContainer.u_setDataDirectoryDelegate>(IcuCommonLibHandle, "u_setDataDirectory");
			Methods.u_setDataDirectory(directory);
		}

		/// <summary>get the name of an ICU code point</summary>
		public static int u_charName(
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

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// get the numeric value for the Unicode digit
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static int u_digit(
			int characterCode,
			byte radix)
		{
			if (Methods.u_digit == null)
				Methods.u_digit = GetMethod<MethodsContainer.u_digitDelegate>(IcuCommonLibHandle, "u_digit");
			return Methods.u_digit(characterCode, radix);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// gets any of a variety of integer property values for the Unicode digit
		/// </summary>
		/// <param name="characterCode">The codepoint to look up</param>
		/// <param name="choice">The property value to look up</param>
		/// <remarks>DO NOT expose this method directly. Instead, make a specific implementation
		/// for each property needed. This not only makes it easier to use, but more importantly
		/// it prevents accidental use of the UCHAR_GENERAL_CATEGORY, which returns an
		/// enumeration that doesn't match the enumeration in FwKernel: LgGeneralCharCategory
		/// </remarks>
		/// ------------------------------------------------------------------------------------
		public static int u_getIntPropertyValue(
			int characterCode,
			Character.UProperty choice)
		{
			if (Methods.u_getIntPropertyValue == null)
				Methods.u_getIntPropertyValue = GetMethod<MethodsContainer.u_getIntPropertyValueDelegate>(IcuCommonLibHandle, "u_getIntPropertyValue");
			return Methods.u_getIntPropertyValue(characterCode, choice);
		}

		public static void u_getUnicodeVersion(out VersionInfo versionArray)
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
		public static void u_getVersion(out VersionInfo versionArray)
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
		public static int u_charType(int characterCode)
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
		public static double u_getNumericValue(
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
		public static bool u_ispunct(
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
		public static bool u_isMirrored(
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
		public static bool u_iscntrl(
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
		public static bool u_isspace(
			int characterCode)
		{
			if (Methods.u_isspace == null)
				Methods.u_isspace = GetMethod<MethodsContainer.u_isspaceDelegate>(IcuCommonLibHandle, "u_isspace");
			return Methods.u_isspace(characterCode);
		}

		#region LCID

		/// ------------------------------------------------------------------------------------
		/// <summary>Get the ICU LCID for a locale</summary>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getLCID([MarshalAs(UnmanagedType.LPStr)]string localeID)
		{
			if (Methods.uloc_getLCID == null)
				Methods.uloc_getLCID = GetMethod<MethodsContainer.uloc_getLCIDDelegate>(IcuCommonLibHandle, "uloc_getLCID");
			return Methods.uloc_getLCID(localeID);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>Gets the ICU locale ID for the specified Win32 LCID value. </summary>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getLocaleForLCID(int lcid, IntPtr locale, int localeCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getLocaleForLCID == null)
				Methods.uloc_getLocaleForLCID = GetMethod<MethodsContainer.uloc_getLocaleForLCIDDelegate>(IcuCommonLibHandle, "uloc_getLocaleForLCID");
			return Methods.uloc_getLocaleForLCID(lcid, locale, localeCapacity, out err);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>Return the ISO 3 char value, if it exists</summary>
		/// ------------------------------------------------------------------------------------
		public static IntPtr uloc_getISO3Country(
			[MarshalAs(UnmanagedType.LPStr)]string locale)
		{
			if (Methods.uloc_getISO3Country == null)
				Methods.uloc_getISO3Country = GetMethod<MethodsContainer.uloc_getISO3CountryDelegate>(IcuCommonLibHandle, "uloc_getISO3Country");
			return Methods.uloc_getISO3Country(locale);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>Return the ISO 3 char value, if it exists</summary>
		/// ------------------------------------------------------------------------------------
		public static IntPtr uloc_getISO3Language(
			[MarshalAs(UnmanagedType.LPStr)]string locale)
		{
			if (Methods.uloc_getISO3Language == null)
				Methods.uloc_getISO3Language = GetMethod<MethodsContainer.uloc_getISO3LanguageDelegate>(IcuCommonLibHandle, "uloc_getISO3Language");
			return Methods.uloc_getISO3Language(locale);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the size of the all available locale list.
		/// </summary>
		/// <returns>the size of the locale list </returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_countAvailable()
		{
			if (Methods.uloc_countAvailable == null)
				Methods.uloc_countAvailable = GetMethod<MethodsContainer.uloc_countAvailableDelegate>(IcuCommonLibHandle, "uloc_countAvailable");
			return Methods.uloc_countAvailable();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the specified locale from a list of all available locales.
		/// The return value is a pointer to an item of a locale name array. Both this array
		/// and the pointers it contains are owned by ICU and should not be deleted or written
		/// through by the caller. The locale name is terminated by a null pointer.
		/// </summary>
		/// <param name="n">n  the specific locale name index of the available locale list</param>
		/// <returns>a specified locale name of all available locales</returns>
		/// ------------------------------------------------------------------------------------
		public static IntPtr uloc_getAvailable(int n)
		{
			if (Methods.uloc_getAvailable == null)
				Methods.uloc_getAvailable = GetMethod<MethodsContainer.uloc_getAvailableDelegate>(IcuCommonLibHandle, "uloc_getAvailable");
			return Methods.uloc_getAvailable(n);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the language code for the specified locale.
		/// </summary>
		/// <param name="localeID">the locale to get the language code with </param>
		/// <param name="language">the language code for localeID </param>
		/// <param name="languageCapacity">the size of the language buffer to store the language
		/// code with </param>
		/// <param name="err">error information if retrieving the language code failed</param>
		/// <returns>the actual buffer size needed for the language code. If it's greater
		/// than languageCapacity, the returned language code will be truncated</returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getLanguage(string localeID, IntPtr language,
			int languageCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getLanguage == null)
				Methods.uloc_getLanguage = GetMethod<MethodsContainer.uloc_getLanguageDelegate>(IcuCommonLibHandle, "uloc_getLanguage");
			return Methods.uloc_getLanguage(localeID, language, languageCapacity, out err);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the script code for the specified locale.
		/// </summary>
		/// <param name="localeID">the locale to get the script code with </param>
		/// <param name="script">the script code for localeID </param>
		/// <param name="scriptCapacity">the size of the script buffer to store the script
		/// code with </param>
		/// <param name="err">error information if retrieving the script code failed</param>
		/// <returns>the actual buffer size needed for the script code. If it's greater
		/// than scriptCapacity, the returned script code will be truncated</returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getScript(string localeID, IntPtr script,
			int scriptCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getScript == null)
				Methods.uloc_getScript = GetMethod<MethodsContainer.uloc_getScriptDelegate>(IcuCommonLibHandle, "uloc_getScript");
			return Methods.uloc_getScript(localeID, script, scriptCapacity, out err);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the country code for the specified locale.
		/// </summary>
		/// <param name="localeID">the locale to get the country code with </param>
		/// <param name="country">the country code for localeID </param>
		/// <param name="countryCapacity">the size of the country buffer to store the country
		/// code with </param>
		/// <param name="err">error information if retrieving the country code failed</param>
		/// <returns>the actual buffer size needed for the country code. If it's greater
		/// than countryCapacity, the returned country code will be truncated</returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getCountry(string localeID, IntPtr country,
			int countryCapacity,out ErrorCode err)
		{
			if (Methods.uloc_getCountry == null)
				Methods.uloc_getCountry = GetMethod<MethodsContainer.uloc_getCountryDelegate>(IcuCommonLibHandle, "uloc_getCountry");
			return Methods.uloc_getCountry(localeID, country, countryCapacity, out err);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the variant code for the specified locale.
		/// </summary>
		/// <param name="localeID">the locale to get the variant code with </param>
		/// <param name="variant">the variant code for localeID </param>
		/// <param name="variantCapacity">the size of the variant buffer to store the variant
		/// code with </param>
		/// <param name="err">error information if retrieving the variant code failed</param>
		/// <returns>the actual buffer size needed for the variant code. If it's greater
		/// than variantCapacity, the returned variant code will be truncated</returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getVariant(string localeID, IntPtr variant,
			int variantCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getVariant == null)
				Methods.uloc_getVariant = GetMethod<MethodsContainer.uloc_getVariantDelegate>(IcuCommonLibHandle, "uloc_getVariant");
			return Methods.uloc_getVariant(localeID, variant, variantCapacity, out err);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full name suitable for display for the specified locale.
		/// </summary>
		/// <param name="localeID">the locale to get the displayable name with</param>
		/// <param name="inLocaleID">Specifies the locale to be used to display the name. In
		/// other words, if the locale's language code is "en", passing Locale::getFrench()
		/// for inLocale would result in "Anglais", while passing Locale::getGerman() for
		/// inLocale would result in "Englisch".  </param>
		/// <param name="result">the displayable name for localeID</param>
		/// <param name="maxResultSize">the size of the name buffer to store the displayable
		/// full name with</param>
		/// <param name="err">error information if retrieving the displayable name failed</param>
		/// <returns>the actual buffer size needed for the displayable name. If it's greater
		/// than variantCapacity, the returned displayable name will be truncated.</returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getDisplayName(string localeID, string inLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (Methods.uloc_getDisplayName == null)
				Methods.uloc_getDisplayName = GetMethod<MethodsContainer.uloc_getDisplayNameDelegate>(IcuCommonLibHandle, "uloc_getDisplayName");
			return Methods.uloc_getDisplayName(localeID, inLocaleID, result, maxResultSize, out err);
		}

		public static int uloc_getDisplayLanguage(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (Methods.uloc_getDisplayLanguage == null)
				Methods.uloc_getDisplayLanguage = GetMethod<MethodsContainer.uloc_getDisplayLanguageDelegate>(IcuCommonLibHandle, "uloc_getDisplayLanguage");
			return Methods.uloc_getDisplayLanguage(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		public static int uloc_getDisplayScript(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (Methods.uloc_getDisplayScript == null)
				Methods.uloc_getDisplayScript = GetMethod<MethodsContainer.uloc_getDisplayScriptDelegate>(IcuCommonLibHandle, "uloc_getDisplayScript");
			return Methods.uloc_getDisplayScript(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		public static int uloc_getDisplayCountry(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (Methods.uloc_getDisplayCountry == null)
				Methods.uloc_getDisplayCountry = GetMethod<MethodsContainer.uloc_getDisplayCountryDelegate>(IcuCommonLibHandle, "uloc_getDisplayCountry");
			return Methods.uloc_getDisplayCountry(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		public static int uloc_getDisplayVariant(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (Methods.uloc_getDisplayVariant == null)
				Methods.uloc_getDisplayVariant = GetMethod<MethodsContainer.uloc_getDisplayVariantDelegate>(IcuCommonLibHandle, "uloc_getDisplayVariant");
			return Methods.uloc_getDisplayVariant(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		public static int uloc_getName(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getName == null)
				Methods.uloc_getName = GetMethod<MethodsContainer.uloc_getNameDelegate>(IcuCommonLibHandle, "uloc_getName");
			return Methods.uloc_getName(localeID, name, nameCapacity, out err);
		}

		public static int uloc_getBaseName(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (Methods.uloc_getBaseName == null)
				Methods.uloc_getBaseName = GetMethod<MethodsContainer.uloc_getBaseNameDelegate>(IcuCommonLibHandle, "uloc_getBaseName");
			return Methods.uloc_getBaseName(localeID, name, nameCapacity, out err);
		}

		public static int uloc_canonicalize(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (Methods.uloc_canonicalize == null)
				Methods.uloc_canonicalize = GetMethod<MethodsContainer.uloc_canonicalizeDelegate>(IcuCommonLibHandle, "uloc_canonicalize");
			var res = Methods.uloc_canonicalize(localeID, name, nameCapacity, out err);
			return res;
		}

		#endregion

		/// <summary>Return the lower case equivalent of the string.</summary>
		public static int u_strToLower(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (Methods.u_strToLower == null)
				Methods.u_strToLower = GetMethod<MethodsContainer.u_strToLowerDelegate>(IcuCommonLibHandle, "u_strToLower");
			return Methods.u_strToLower(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

		/// <summary>Return the title case equivalent of the string.</summary>
		public static int u_strToTitle(IntPtr dest, int destCapacity, string src,
			int srcLength, IntPtr titleIter, [MarshalAs(UnmanagedType.LPStr)] string locale,
			out ErrorCode errorCode)
		{
			if (Methods.u_strToTitle == null)
				Methods.u_strToTitle = GetMethod<MethodsContainer.u_strToTitleDelegate>(IcuCommonLibHandle, "u_strToTitle", true);
			return Methods.u_strToTitle(dest, destCapacity, src, srcLength, titleIter,
				locale, out errorCode);
		}

		/// <summary>Return the upper case equivalent of the string.</summary>
		public static int u_strToUpper(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (Methods.u_strToUpper == null)
				Methods.u_strToUpper = GetMethod<MethodsContainer.u_strToUpperDelegate>(IcuCommonLibHandle, "u_strToUpper");
			return Methods.u_strToUpper(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

		#region normalize

		/// <summary>
		/// Normalize a string according to the given mode and options.
		/// </summary>
		public static int unorm_normalize(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, int options,
			IntPtr result, int resultLength, out ErrorCode errorCode)
		{
			if (Methods._unorm_normalize == null)
				Methods._unorm_normalize = GetMethod<MethodsContainer.unorm_normalizeDelegate>(IcuCommonLibHandle, "unorm_normalize");
			return Methods._unorm_normalize(source, sourceLength, mode, options, result,
				resultLength, out errorCode);
		}

		// Note that ICU's UBool type is typedef to an 8-bit integer.

		/// <summary>
		/// Check whether a string is normalized according to the given mode and options.
		/// </summary>
		public static byte unorm_isNormalized(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, out ErrorCode errorCode)
		{
			if (Methods._unorm_isNormalized == null)
				Methods._unorm_isNormalized = GetMethod<MethodsContainer.unorm_isNormalizedDelegate>(IcuCommonLibHandle, "unorm_isNormalized");
			return Methods._unorm_isNormalized(source, sourceLength, mode, out errorCode);
		}

		#endregion normalize

		#region Break iterator

		/// <summary>
		/// Open a new UBreakIterator for locating text boundaries for a specified locale.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The text.</param>
		/// <param name="textLength">Length of the text.</param>
		/// <param name="errorCode">The error code.</param>
		/// <returns></returns>
		public static IntPtr ubrk_open(BreakIterator.UBreakIteratorType type,
			string locale, string text, int textLength, out ErrorCode errorCode)
		{
			if (Methods.ubrk_open == null)
				Methods.ubrk_open = GetMethod<MethodsContainer.ubrk_openDelegate>(IcuCommonLibHandle, "ubrk_open", true);
			return Methods.ubrk_open(type, locale, text, textLength, out errorCode);
		}

		/// <summary>
		/// Open a new BreakIterator that uses the given rules to break text.
		/// </summary>
		/// <param name="rules">The rules to use for break iterator</param>
		/// <param name="rulesLength">The length of the rules.</param>
		/// <param name="text">The text.</param>
		/// <param name="textLength">Length of the text.</param>
		/// <param name="parseError">Receives position and context information for any syntax errors detected while parsing the rules. </param>
		/// <param name="errorCode">The error code.</param>
		/// <returns></returns>
		public static IntPtr ubrk_openRules(
			string rules, int rulesLength,
			string text, int textLength,
			out ParseError parseError, out ErrorCode errorCode)
		{
			if (Methods.ubrk_openRules == null)
				Methods.ubrk_openRules = GetMethod<MethodsContainer.ubrk_openRulesDelegate>(IcuCommonLibHandle, "ubrk_openRules", true);
			return Methods.ubrk_openRules(rules, rulesLength, text, textLength, out parseError, out errorCode);
		}

		/// <summary>
		/// Sets the iterator to a new text
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <param name="text">Text to examine</param>
		/// <param name="textLength">The length of the text</param>
		/// <param name="errorCode">The error code</param>
		public static void ubrk_setText(IntPtr bi, string text, int textLength, out ErrorCode errorCode)
		{
			if (Methods.ubrk_setText == null)
				Methods.ubrk_setText = GetMethod<MethodsContainer.ubrk_setTextDelegate>(IcuCommonLibHandle, "ubrk_setText", true);
			Methods.ubrk_setText(bi, text, textLength, out errorCode);
		}

		/// <summary>
		/// Close a UBreakIterator.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		public static void ubrk_close(IntPtr bi)
		{
			if (Methods.ubrk_close == null)
				Methods.ubrk_close = GetMethod<MethodsContainer.ubrk_closeDelegate>(IcuCommonLibHandle, "ubrk_close", true);
			Methods.ubrk_close(bi);
		}

		/// <summary>
		/// Determine the index of the first character in the text being scanned.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_first(IntPtr bi)
		{
			if (Methods.ubrk_first == null)
				Methods.ubrk_first = GetMethod<MethodsContainer.ubrk_firstDelegate>(IcuCommonLibHandle, "ubrk_first", true);
			return Methods.ubrk_first(bi);
		}

		/// <summary>
		/// Determine the text boundary following the current text boundary.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_next(IntPtr bi)
		{
			if (Methods.ubrk_next == null)
				Methods.ubrk_next = GetMethod<MethodsContainer.ubrk_nextDelegate>(IcuCommonLibHandle, "ubrk_next", true);
			return Methods.ubrk_next(bi);
		}

		/// <summary>
		/// Return the status from the break rule that determined the most recently returned break position.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_getRuleStatus(IntPtr bi)
		{
			if (Methods.ubrk_getRuleStatus == null)
				Methods.ubrk_getRuleStatus = GetMethod<MethodsContainer.ubrk_getRuleStatusDelegate>(IcuCommonLibHandle, "ubrk_getRuleStatus", true);
			return Methods.ubrk_getRuleStatus(bi);
		}

		/// <summary>
		/// Return the status from the break rule that determined the most recently returned break position.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <param name="fillInVector">An array to be filled in with the status values.</param>
		/// <param name="capacity">The length of the supplied vector. A length of zero causes the function to return the number of status values, in the normal way, without attemtping to store any values.</param>
		/// <param name="status">Receives error codes.</param>
		/// <returns>The number of rule status values from rules that determined the most recent boundary returned by the break iterator.</returns>
		public static int ubrk_getRuleStatusVec(IntPtr bi,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] fillInVector,
			int capacity,
			out ErrorCode status)
		{
			if (Methods.ubrk_getRuleStatusVec == null)
				Methods.ubrk_getRuleStatusVec = GetMethod<MethodsContainer.ubrk_getRuleStatusVecDelegate>(IcuCommonLibHandle, "ubrk_getRuleStatusVec", true);
			return Methods.ubrk_getRuleStatusVec(bi, fillInVector, capacity, out status);
		}

		#endregion Break iterator

		#region Unicode set

		/// <summary>
		/// Disposes of the storage used by Unicode set.  This function should be called exactly once for objects returned by uset_open()
		/// </summary>
		/// <param name="set">Unicode set to dispose of </param>
		public static void uset_close(IntPtr set)
		{
			if (Methods.uset_close == null)
				Methods.uset_close = GetMethod<MethodsContainer.uset_closeDelegate>(IcuCommonLibHandle, "uset_close");
			Methods.uset_close(set);
		}

		/// <summary>
		/// Creates a Unicode set that contains the range of characters start..end, inclusive.
		/// If start > end then an empty set is created (same as using uset_openEmpty().
		/// </summary>
		/// <param name="start">First character of the range, inclusive</param>
		/// <param name="end">Last character of the range, inclusive</param>
		/// <returns>Unicode set of characters.  The caller must call uset_close() on it when done</returns>
		public static IntPtr uset_open(char start, char end)
		{
			if (Methods.uset_open == null)
				Methods.uset_open = GetMethod<MethodsContainer.uset_openDelegate>(IcuCommonLibHandle, "uset_open");
			return Methods.uset_open(start, end);
		}

		/// <summary>
		/// Creates a set from the given pattern.
		/// </summary>
		/// <param name="pattern">A string specifying what characters are in the set</param>
		/// <param name="patternLength">Length of the pattern, or -1 if null terminated</param>
		/// <param name="status">The error code</param>
		/// <returns>Unicode set</returns>
		public static IntPtr uset_openPattern(string pattern, int patternLength, ref ErrorCode status)
		{
			if (Methods.uset_openPattern == null)
				Methods.uset_openPattern = GetMethod<MethodsContainer.uset_openPatternDelegate>(IcuCommonLibHandle, "uset_openPattern");
			return Methods.uset_openPattern(pattern, patternLength, ref status);
		}

		/// <summary>
		/// Adds the given character to the given Unicode set.  After this call, uset_contains(set, c) will return TRUE.  A frozen set will not be modified.
		/// </summary>
		/// <param name="set">The object to which to add the character</param>
		/// <param name="c">The character to add</param>
		public static void uset_add(IntPtr set, char c)
		{
			if (Methods.uset_add == null)
				Methods.uset_add = GetMethod<MethodsContainer.uset_addDelegate>(IcuCommonLibHandle, "uset_add");
			Methods.uset_add(set, c);
		}

		/// <summary>
		/// Returns a string representation of this set.  If the result of calling this function is
		/// passed to a uset_openPattern(), it will produce another set that is equal to this one.
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <param name="result">The string to receive the rules, may be NULL</param>
		/// <param name="resultCapacity">The capacity of result, may be 0 if result is NULL</param>
		/// <param name="escapeUnprintable">if TRUE then convert unprintable characters to their hex escape representations,
		/// \uxxxx or \Uxxxxxxxx. Unprintable characters are those other than U+000A, U+0020..U+007E.</param>
		/// <param name="status">Error code</param>
		/// <returns>Length of string, possibly larger than resultCapacity</returns>
		public static int uset_toPattern(IntPtr set, IntPtr result, int resultCapacity,
			bool escapeUnprintable, ref ErrorCode status)
		{
			if (Methods.uset_toPattern == null)
				Methods.uset_toPattern = GetMethod<MethodsContainer.uset_toPatternDelegate>(IcuCommonLibHandle, "uset_toPattern");
			return Methods.uset_toPattern(set, result, resultCapacity, escapeUnprintable, ref status);
		}

		/// <summary>
		/// Adds the given string to the given Unicode set
		/// </summary>
		/// <param name="set">The Unicode set to which to add the string</param>
		/// <param name="str">The string to add</param>
		/// <param name="strLen">The length of the string or -1 if null</param>
		public static void uset_addString(IntPtr set, string str, int strLen)
		{
			if (Methods.uset_addString == null)
				Methods.uset_addString = GetMethod<MethodsContainer.uset_addStringDelegate>(IcuCommonLibHandle, "uset_addString");
			Methods.uset_addString(set, str, strLen);
		}

		/// <summary>
		/// Returns an item of this Unicode set.  An item is either a range of characters or a single multicharacter string.
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <param name="itemIndex">A non-negative integer in the range 0..uset_getItemCount(set)-1</param>
		/// <param name="start">Pointer to variable to receive first character in range, inclusive</param>
		/// <param name="end">POinter to variable to receive the last character in range, inclusive</param>
		/// <param name="str">Buffer to receive the string, may be NULL</param>
		/// <param name="strCapacity">Capcacity of str, or 0 if str is NULL</param>
		/// <param name="ec">Error Code</param>
		/// <returns>The length of the string (>=2), or 0 if the item is a range, in which case it
		///  is the range *start..*end, or -1 if itemIndex is out of range</returns>
		public static int uset_getItem(IntPtr set, int itemIndex, out int start,
			out int end, IntPtr str, int strCapacity, ref ErrorCode ec)
		{
			if (Methods.uset_getItem == null)
				Methods.uset_getItem = GetMethod<MethodsContainer.uset_getItemDelegate>(IcuCommonLibHandle, "uset_getItem");
			return Methods.uset_getItem(set, itemIndex, out start, out end, str, strCapacity, ref ec);
		}

		/// <summary>
		/// Returns the number of items in this set.  An item is either a range of characters or a single multicharacter string
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <returns>A non-negative integer counting the character ranges and/or strings contained in the set</returns>
		public static int uset_getItemCount(IntPtr set)
		{
			if (Methods.uset_getItemCount == null)
				Methods.uset_getItemCount = GetMethod<MethodsContainer.uset_getItemCountDelegate>(IcuCommonLibHandle, "uset_getItemCount");
			return Methods.uset_getItemCount(set);
		}

		#endregion // Unicode set
	}
}
