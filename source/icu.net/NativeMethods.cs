// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;
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

		#region Dynamic method loading

		#region Native methods for Linux

		private const int RTLD_NOW = 2;

		[DllImport("libdl.so")]
		private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string file, int mode);

		[DllImport("libdl.so")]
		private static extern int dlclose(IntPtr handle);

		[DllImport("libdl.so")]
		private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string name);

		#endregion

		#region Native methods for Windows

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		#endregion

		private static int IcuVersion;
		private static IntPtr _IcuCommonLibHandle;
		private static IntPtr _IcuI18NLibHandle;

		private static bool IsWindows
		{
			get { return Environment.OSVersion.Platform != PlatformID.Unix; }
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

		private static IntPtr LoadIcuLibrary(string libraryName)
		{
			var handle = GetIcuLibHandle(libraryName, IcuVersion > 0 ? IcuVersion : maxIcuVersion);
			if (handle == IntPtr.Zero)
				throw new FileLoadException("Can't load ICU library", libraryName);
			return handle;
		}

		private static IntPtr GetIcuLibHandle(string basename, int icuVersion)
		{
			if (icuVersion < minIcuVersion)
				return IntPtr.Zero;
			IntPtr handle;
			if (IsWindows)
			{
				handle = LoadLibrary(string.Format("{0}{1}.dll", basename, icuVersion));
			}
			else
			{
				var libName = string.Format("lib{0}.so.{1}", basename, icuVersion);
				handle = dlopen(libName, RTLD_NOW);
			}
			if (handle == IntPtr.Zero)
				return GetIcuLibHandle(basename, icuVersion - 1);

			IcuVersion = icuVersion;
			return handle;
		}

		public static void Cleanup()
		{
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
		}

		private static T GetMethod<T>(IntPtr handle, string methodName, bool missingInMinimal = false) where T: class
		{
			var versionedMethodName = string.Format("{0}_{1}", methodName, IcuVersion);
			var methodPointer = IsWindows ?
				GetProcAddress(handle, versionedMethodName) :
				dlsym(handle, versionedMethodName);
			if (methodPointer != IntPtr.Zero)
			{
				return Marshal.GetDelegateForFunctionPointer(
					methodPointer, typeof(T)) as T;
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void uenum_closeDelegate(IntPtr en);

		private static uenum_closeDelegate _uenum_close;

		/// <summary>
		/// This function does cleanup of the enumerator object
		/// </summary>
		/// <param name="en">Enumeration to be closed</param>
		public static void uenum_close(IntPtr en)
		{
			if (_uenum_close == null)
				_uenum_close = GetMethod<uenum_closeDelegate>(IcuCommonLibHandle, "uenum_close");
			_uenum_close(en);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr uenum_unextDelegate(
			RuleBasedCollator.SafeEnumeratorHandle en,
			out int resultLength,
			out ErrorCode status);

		private static uenum_unextDelegate _uenum_unext;

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
			if (_uenum_unext == null)
				_uenum_unext = GetMethod<uenum_unextDelegate>(IcuCommonLibHandle, "uenum_unext");
			return _uenum_unext(en, out resultLength, out status);
		}


		#region Unicode collator

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openDelegate(
			[MarshalAs(UnmanagedType.LPStr)] string loc,
			out ErrorCode status);

		private static ucol_openDelegate _ucol_open;

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
			if (_ucol_open == null)
				_ucol_open = GetMethod<ucol_openDelegate>(IcuI18NLibHandle, "ucol_open");
			return _ucol_open(loc, out status);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openRulesDelegate(
			[MarshalAs(UnmanagedType.LPWStr)] string rules,
			int rulesLength,
			NormalizationMode normalizationMode,
			CollationStrength strength,
			ref ParseError parseError,
			out ErrorCode status);

		private static ucol_openRulesDelegate _ucol_openRules;

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
			if (_ucol_openRules == null)
				_ucol_openRules = GetMethod<ucol_openRulesDelegate>(IcuI18NLibHandle, "ucol_openRules");
			return _ucol_openRules(rules, rulesLength, normalizationMode, strength, ref parseError,
				out status);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void ucol_closeDelegate(IntPtr coll);

		private static ucol_closeDelegate _ucol_close;

		/// <summary>
		/// Close a UCollator.
		/// Once closed, a UCollator should not be used. Every open collator should
		/// be closed. Otherwise, a memory leak will result.
		/// </summary>
		/// <param name="coll">The UCollator to close.</param>
		public static void ucol_close(IntPtr coll)
		{
			if (_ucol_close == null)
				_ucol_close = GetMethod<ucol_closeDelegate>(IcuI18NLibHandle, "ucol_close");
			_ucol_close(coll);
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate CollationResult ucol_strcollDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[MarshalAs(UnmanagedType.LPWStr)] string target,
			Int32 targetLength);

		private static ucol_strcollDelegate _ucol_strcoll;

		public static CollationResult ucol_strcoll(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[MarshalAs(UnmanagedType.LPWStr)] string target,
			Int32 targetLength)
		{
			if (_ucol_strcoll == null)
				_ucol_strcoll = GetMethod<ucol_strcollDelegate>(IcuI18NLibHandle, "ucol_strcoll");
			return _ucol_strcoll(collator, source, sourceLength, target, targetLength);
		}

		/**
 * Determine how many locales have collation rules available.
 * This function is most useful as determining the loop ending condition for
 * calls to {@link #ucol_getAvailable }.
 * @return The number of locales for which collation rules are available.
 * @see ucol_getAvailable
 * @stable ICU 2.0
 */
		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate Int32 ucol_countAvailableDelegate();

		private static ucol_countAvailableDelegate _ucol_countAvailable;

		public static Int32 ucol_countAvailable()
		{
			if (_ucol_countAvailable == null)
				_ucol_countAvailable = GetMethod<ucol_countAvailableDelegate>(IcuI18NLibHandle, "ucol_countAvailable");
			return _ucol_countAvailable();
		}

		/**
 * Create a string enumerator of all locales for which a valid
 * collator may be opened.
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */
		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate RuleBasedCollator.SafeEnumeratorHandle ucol_openAvailableLocalesDelegate(out ErrorCode status);

		private static ucol_openAvailableLocalesDelegate _ucol_openAvailableLocales;

		public static RuleBasedCollator.SafeEnumeratorHandle ucol_openAvailableLocales(out ErrorCode status)
		{
			if (_ucol_openAvailableLocales == null)
				_ucol_openAvailableLocales = GetMethod<ucol_openAvailableLocalesDelegate>(IcuI18NLibHandle, "ucol_openAvailableLocales");
			return _ucol_openAvailableLocales(out status);
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate Int32 ucol_getSortKeyDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result,
			Int32 resultLength);

		private static ucol_getSortKeyDelegate _ucol_getSortKey;

		public static Int32 ucol_getSortKey(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source,
			Int32 sourceLength,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result,
			Int32 resultLength)
		{
			if (_ucol_getSortKey == null)
				_ucol_getSortKey = GetMethod<ucol_getSortKeyDelegate>(IcuI18NLibHandle, "ucol_getSortKey");
			return _ucol_getSortKey(collator, source, sourceLength, result, resultLength);
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void ucol_setAttributeDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			CollationAttributeValue value,
			out ErrorCode status);

		private static ucol_setAttributeDelegate _ucol_setAttribute;

		public static void ucol_setAttribute(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			CollationAttributeValue value,
			out ErrorCode status)
		{
			if (_ucol_setAttribute == null)
				_ucol_setAttribute = GetMethod<ucol_setAttributeDelegate>(IcuI18NLibHandle, "ucol_setAttribute");
			_ucol_setAttribute(collator, attr, value, out status);
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate CollationAttributeValue ucol_getAttributeDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			out ErrorCode status);

		private static ucol_getAttributeDelegate _ucol_getAttribute;

		public static CollationAttributeValue ucol_getAttribute(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			CollationAttribute attr,
			out ErrorCode status)
		{
			if (_ucol_getAttribute == null)
				_ucol_getAttribute = GetMethod<ucol_getAttributeDelegate>(IcuI18NLibHandle, "ucol_getAttribute");
			return _ucol_getAttribute(collator, attr, out status);
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeCloneDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			IntPtr stackBuffer,
			ref Int32 pBufferSize,
			out ErrorCode status);

		private static ucol_safeCloneDelegate _ucol_safeClone;

		public static RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeClone(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			IntPtr stackBuffer,
			ref Int32 pBufferSize,
			out ErrorCode status)
		{
			if (_ucol_safeClone == null)
				_ucol_safeClone = GetMethod<ucol_safeCloneDelegate>(IcuI18NLibHandle, "ucol_safeClone");
			return _ucol_safeClone(collator, stackBuffer, ref pBufferSize, out status);
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
		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr ucol_getLocaleByTypeDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			LocaleType type,
			out ErrorCode status);

		private static ucol_getLocaleByTypeDelegate _ucol_getLocaleByType;

		public static IntPtr ucol_getLocaleByType(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator, 
			LocaleType type,
			out ErrorCode status)
		{
			if (_ucol_getLocaleByType == null)
				_ucol_getLocaleByType = GetMethod<ucol_getLocaleByTypeDelegate>(IcuI18NLibHandle, "ucol_getLocaleByType");
			return _ucol_getLocaleByType(collator, type, out status);
		}


		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int ucol_getRulesExDelegate(
			RuleBasedCollator.SafeRuleBasedCollatorHandle coll,
			UColRuleOption delta,
			IntPtr buffer,
			int bufferLen);

		private static ucol_getRulesExDelegate _ucol_getRulesEx;

		public static int ucol_getRulesEx(
			RuleBasedCollator.SafeRuleBasedCollatorHandle coll,
			UColRuleOption delta,
			IntPtr buffer,
			int bufferLen)
		{
			if (_ucol_getRulesEx == null)
				_ucol_getRulesEx = GetMethod<ucol_getRulesExDelegate>(IcuI18NLibHandle, "ucol_getRulesEx");
			return _ucol_getRulesEx(coll, delta, buffer, bufferLen);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int ucol_getBoundDelegate(byte[] source, int sourceLength,
			UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
			out ErrorCode status);

		private static ucol_getBoundDelegate _ucol_getBound;

		public static int ucol_getBound(byte[] source, int sourceLength,
			UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
			out ErrorCode status)
		{
			if (_ucol_getBound == null)
				_ucol_getBound = GetMethod<ucol_getBoundDelegate>(IcuI18NLibHandle, "ucol_getBound");
			return _ucol_getBound(source, sourceLength, boundType, noOfLevels, result, resultLength, out status);
		}

		#endregion // Unicode collator

		/*
			public enum CollationRuleOption
			{
				/// <summary>
				/// Retrieve tailoring only
				/// </summary>
				TailoringOnly,
				/// <summary>
				/// Retrieve UCA rules and tailoring
				/// </summary>
				FullRules
			}
		*/
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void u_InitDelegate(out ErrorCode errorCode);

		private static u_InitDelegate _u_Init;

		/// <summary>get the name of an ICU code point</summary>
		public static void u_Init(out ErrorCode errorCode)
		{
			if (_u_Init == null)
				_u_Init = GetMethod<u_InitDelegate>(IcuCommonLibHandle, "u_Init");
			_u_Init(out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void u_CleanupDelegate();

		private static u_CleanupDelegate _u_Cleanup;

		/// <summary>Clean up the ICU files that could be locked</summary>
		public static void u_Cleanup()
		{
			if (_u_Cleanup == null)
				_u_Cleanup = GetMethod<u_CleanupDelegate>(IcuCommonLibHandle, "u_Cleanup");
			_u_Cleanup();
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr u_GetDataDirectoryDelegate();

		private static u_GetDataDirectoryDelegate _u_GetDataDirectory;

		/// <summary>Return the ICU data directory</summary>
		public static IntPtr u_GetDataDirectory()
		{
			if (_u_GetDataDirectory == null)
				_u_GetDataDirectory = GetMethod<u_GetDataDirectoryDelegate>(IcuCommonLibHandle, "u_GetDataDirectory");
			return _u_GetDataDirectory();
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void u_SetDataDirectoryDelegate(
			[MarshalAs(UnmanagedType.LPStr)]string directory);

		private static u_SetDataDirectoryDelegate _u_SetDataDirectory;

		/// <summary>Set the ICU data directory</summary>
		public static void u_SetDataDirectory(
			[MarshalAs(UnmanagedType.LPStr)]string directory)
		{
			if (_u_SetDataDirectory == null)
				_u_SetDataDirectory = GetMethod<u_SetDataDirectoryDelegate>(IcuCommonLibHandle, "u_SetDataDirectory");
			_u_SetDataDirectory(directory);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_charNameDelegate(
			int code,
			Character.UCharNameChoice nameChoice,
			IntPtr buffer,
			int bufferLength,
			out ErrorCode errorCode);

		private static u_charNameDelegate _u_charName;

		/// <summary>get the name of an ICU code point</summary>
		public static int u_charName(
			int code,
			Character.UCharNameChoice nameChoice,
			IntPtr buffer,
			int bufferLength,
			out ErrorCode errorCode)
		{
			if (_u_charName == null)
				_u_charName = GetMethod<u_charNameDelegate>(IcuCommonLibHandle, "u_charName");
			return _u_charName(code, nameChoice, buffer, bufferLength, out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_digitDelegate(
			int characterCode,
			byte radix);

		private static u_digitDelegate _u_digit;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// get the numeric value for the Unicode digit
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static int u_digit(
			int characterCode,
			byte radix)
		{
			if (_u_digit == null)
				_u_digit = GetMethod<u_digitDelegate>(IcuCommonLibHandle, "u_digit");
			return _u_digit(characterCode, radix);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_getIntPropertyValueDelegate(
			int characterCode,
			Character.UProperty choice);

		private static u_getIntPropertyValueDelegate _u_getIntPropertyValue;

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
			if (_u_getIntPropertyValue == null)
				_u_getIntPropertyValue = GetMethod<u_getIntPropertyValueDelegate>(IcuCommonLibHandle, "u_getIntPropertyValue");
			return _u_getIntPropertyValue(characterCode, choice);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void u_getUnicodeVersionDelegate(out VersionInfo versionArray);

		private static u_getUnicodeVersionDelegate _u_getUnicodeVersion;

		public static void u_getUnicodeVersion(out VersionInfo versionArray)
		{
			if (_u_getUnicodeVersion == null)
				_u_getUnicodeVersion = GetMethod<u_getUnicodeVersionDelegate>(IcuCommonLibHandle, "u_getUnicodeVersion");
			_u_getUnicodeVersion(out versionArray);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void u_getVersionDelegate(out VersionInfo versionArray);

		private static u_getVersionDelegate _u_getVersion;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the ICU release version.
		/// </summary>
		/// <param name="versionArray">Stores the version information for ICU.</param>
		/// ------------------------------------------------------------------------------------
		public static void u_getVersion(out VersionInfo versionArray)
		{
			if (_u_getVersion == null)
				_u_getVersion = GetMethod<u_getVersionDelegate>(IcuCommonLibHandle, "u_getVersion");
			_u_getVersion(out versionArray);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_charTypeDelegate(int characterCode);

		private static u_charTypeDelegate _u_charType;

		/// <summary>
		/// Get the general character type.
		/// </summary>
		/// <param name="characterCode"></param>
		/// <returns></returns>
		public static int u_charType(int characterCode)
		{
			if (_u_charType == null)
				_u_charType = GetMethod<u_charTypeDelegate>(IcuCommonLibHandle, "u_charType");
			return _u_charType(characterCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate double u_getNumericValueDelegate(
			int characterCode);

		private static u_getNumericValueDelegate _u_getNumericValue;

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
			if (_u_getNumericValue == null)
				_u_getNumericValue = GetMethod<u_getNumericValueDelegate>(IcuCommonLibHandle, "u_getNumericValue");
			return _u_getNumericValue(characterCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
		// which are usually zero, especially in debug builds...but one day we will be sorry.
		[return: MarshalAs(UnmanagedType.I1)]
		private delegate bool u_ispunctDelegate(int characterCode);

		private static u_ispunctDelegate _u_ispunct;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	Determines whether the specified code point is a punctuation character.
		/// </summary>
		/// <param name="characterCode">the code point to be tested</param>
		/// ------------------------------------------------------------------------------------
		public static bool u_ispunct(
			int characterCode)
		{
			if (_u_ispunct == null)
				_u_ispunct = GetMethod<u_ispunctDelegate>(IcuCommonLibHandle, "u_ispunct");
			return _u_ispunct(characterCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
		// which are usually zero, especially in debug builds...but one day we will be sorry.
		[return: MarshalAs(UnmanagedType.I1)]
		private delegate bool u_isMirroredDelegate(int characterCode);

		private static u_isMirroredDelegate _u_isMirrored;

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
			if (_u_isMirrored == null)
				_u_isMirrored = GetMethod<u_isMirroredDelegate>(IcuCommonLibHandle, "u_isMirrored");
			return _u_isMirrored(characterCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
		// which are usually zero, especially in debug builds...but one day we will be sorry.
		[return: MarshalAs(UnmanagedType.I1)]
		private delegate bool u_iscntrlDelegate(int characterCode);

		private static u_iscntrlDelegate _u_iscntrl;

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
			if (_u_iscntrl == null)
				_u_iscntrl = GetMethod<u_iscntrlDelegate>(IcuCommonLibHandle, "u_iscntrl");
			return _u_iscntrl(characterCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
		// which are usually zero, especially in debug builds...but one day we will be sorry.
		[return: MarshalAs(UnmanagedType.I1)]
		private delegate bool u_isspaceDelegate(int characterCode);

		private static u_isspaceDelegate _u_isspace;

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
			if (_u_isspace == null)
				_u_isspace = GetMethod<u_isspaceDelegate>(IcuCommonLibHandle, "u_isspace");
			return _u_isspace(characterCode);
		}

		#region LCID

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int uloc_getLCIDDelegate([MarshalAs(UnmanagedType.LPStr)]string localeID);

		private static uloc_getLCIDDelegate _uloc_getLCID;

		/// ------------------------------------------------------------------------------------
		/// <summary>Get the ICU LCID for a locale</summary>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getLCID([MarshalAs(UnmanagedType.LPStr)]string localeID)
		{
			if (_uloc_getLCID == null)
				_uloc_getLCID = GetMethod<uloc_getLCIDDelegate>(IcuCommonLibHandle, "uloc_getLCID");
			return _uloc_getLCID(localeID);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int uloc_getLocaleForLCIDDelegate(int lcid,IntPtr locale,int localeCapacity,out ErrorCode err);

		private static uloc_getLocaleForLCIDDelegate _uloc_getLocaleForLCID;

		/// ------------------------------------------------------------------------------------
		/// <summary>Gets the ICU locale ID for the specified Win32 LCID value. </summary>
		/// ------------------------------------------------------------------------------------
		public static int uloc_getLocaleForLCID(int lcid, IntPtr locale, int localeCapacity, out ErrorCode err)
		{
			if (_uloc_getLocaleForLCID == null)
				_uloc_getLocaleForLCID = GetMethod<uloc_getLocaleForLCIDDelegate>(IcuCommonLibHandle, "uloc_getLocaleForLCID");
			return _uloc_getLocaleForLCID(lcid, locale, localeCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate IntPtr uloc_getISO3CountryDelegate(
			[MarshalAs(UnmanagedType.LPStr)]string locale);

		private static uloc_getISO3CountryDelegate _uloc_getISO3Country;

		/// ------------------------------------------------------------------------------------
		/// <summary>Return the ISO 3 char value, if it exists</summary>
		/// ------------------------------------------------------------------------------------
		public static IntPtr uloc_getISO3Country(
			[MarshalAs(UnmanagedType.LPStr)]string locale)
		{
			if (_uloc_getISO3Country == null)
				_uloc_getISO3Country = GetMethod<uloc_getISO3CountryDelegate>(IcuCommonLibHandle, "uloc_getISO3Country");
			return _uloc_getISO3Country(locale);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate IntPtr uloc_getISO3LanguageDelegate(
			[MarshalAs(UnmanagedType.LPStr)]string locale);

		private static uloc_getISO3LanguageDelegate _uloc_getISO3Language;

		/// ------------------------------------------------------------------------------------
		/// <summary>Return the ISO 3 char value, if it exists</summary>
		/// ------------------------------------------------------------------------------------
		public static IntPtr uloc_getISO3Language(
			[MarshalAs(UnmanagedType.LPStr)]string locale)
		{
			if (_uloc_getISO3Language == null)
				_uloc_getISO3Language = GetMethod<uloc_getISO3LanguageDelegate>(IcuCommonLibHandle, "uloc_getISO3Language");
			return _uloc_getISO3Language(locale);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_countAvailableDelegate();

		private static uloc_countAvailableDelegate _uloc_countAvailable;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the size of the all available locale list.
		/// </summary>
		/// <returns>the size of the locale list </returns>
		/// ------------------------------------------------------------------------------------
		public static int uloc_countAvailable()
		{
			if (_uloc_countAvailable == null)
				_uloc_countAvailable = GetMethod<uloc_countAvailableDelegate>(IcuCommonLibHandle, "uloc_countAvailable");
			return _uloc_countAvailable();
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate IntPtr uloc_getAvailableDelegate(int n);

		private static uloc_getAvailableDelegate _uloc_getAvailable;

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
			if (_uloc_getAvailable == null)
				_uloc_getAvailable = GetMethod<uloc_getAvailableDelegate>(IcuCommonLibHandle, "uloc_getAvailable");
			return _uloc_getAvailable(n);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getLanguageDelegate(string localeID, IntPtr language,
			int languageCapacity, out ErrorCode err);

		private static uloc_getLanguageDelegate _uloc_getLanguage;

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
			if (_uloc_getLanguage == null)
				_uloc_getLanguage = GetMethod<uloc_getLanguageDelegate>(IcuCommonLibHandle, "uloc_getLanguage");
			return _uloc_getLanguage(localeID, language, languageCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getScriptDelegate(string localeID, IntPtr script,
			int scriptCapacity, out ErrorCode err);

		private static uloc_getScriptDelegate _uloc_getScript;

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
			if (_uloc_getScript == null)
				_uloc_getScript = GetMethod<uloc_getScriptDelegate>(IcuCommonLibHandle, "uloc_getScript");
			return _uloc_getScript(localeID, script, scriptCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getCountryDelegate(string localeID, IntPtr country,
			int countryCapacity,out ErrorCode err);

		private static uloc_getCountryDelegate _uloc_getCountry;

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
			if (_uloc_getCountry == null)
				_uloc_getCountry = GetMethod<uloc_getCountryDelegate>(IcuCommonLibHandle, "uloc_getCountry");
			return _uloc_getCountry(localeID, country, countryCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getVariantDelegate(string localeID, IntPtr variant,
			int variantCapacity, out ErrorCode err);

		private static uloc_getVariantDelegate _uloc_getVariant;

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
			if (_uloc_getVariant == null)
				_uloc_getVariant = GetMethod<uloc_getVariantDelegate>(IcuCommonLibHandle, "uloc_getVariant");
			return _uloc_getVariant(localeID, variant, variantCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getDisplayNameDelegate(string localeID, string inLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err);

		private static uloc_getDisplayNameDelegate _uloc_getDisplayName;

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
			if (_uloc_getDisplayName == null)
				_uloc_getDisplayName = GetMethod<uloc_getDisplayNameDelegate>(IcuCommonLibHandle, "uloc_getDisplayName");
			return _uloc_getDisplayName(localeID, inLocaleID, result, maxResultSize, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getDisplayLanguageDelegate(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err);

		private static uloc_getDisplayLanguageDelegate _uloc_getDisplayLanguage;

		public static int uloc_getDisplayLanguage(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (_uloc_getDisplayLanguage == null)
				_uloc_getDisplayLanguage = GetMethod<uloc_getDisplayLanguageDelegate>(IcuCommonLibHandle, "uloc_getDisplayLanguage");
			return _uloc_getDisplayLanguage(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getDisplayScriptDelegate(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err);

		private static uloc_getDisplayScriptDelegate _uloc_getDisplayScript;

		public static int uloc_getDisplayScript(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (_uloc_getDisplayScript == null)
				_uloc_getDisplayScript = GetMethod<uloc_getDisplayScriptDelegate>(IcuCommonLibHandle, "uloc_getDisplayScript");
			return _uloc_getDisplayScript(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getDisplayCountryDelegate(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err);

		private static uloc_getDisplayCountryDelegate _uloc_getDisplayCountry;

		public static int uloc_getDisplayCountry(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (_uloc_getDisplayCountry == null)
				_uloc_getDisplayCountry = GetMethod<uloc_getDisplayCountryDelegate>(IcuCommonLibHandle, "uloc_getDisplayCountry");
			return _uloc_getDisplayCountry(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getDisplayVariantDelegate(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err);

		private static uloc_getDisplayVariantDelegate _uloc_getDisplayVariant;

		public static int uloc_getDisplayVariant(string localeID, string displayLocaleID,
			IntPtr result, int maxResultSize, out ErrorCode err)
		{
			if (_uloc_getDisplayVariant == null)
				_uloc_getDisplayVariant = GetMethod<uloc_getDisplayVariantDelegate>(IcuCommonLibHandle, "uloc_getDisplayVariant");
			return _uloc_getDisplayVariant(localeID, displayLocaleID, result, maxResultSize, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getNameDelegate(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err);

		private static uloc_getNameDelegate _uloc_getName;

		public static int uloc_getName(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (_uloc_getName == null)
				_uloc_getName = GetMethod<uloc_getNameDelegate>(IcuCommonLibHandle, "uloc_getName");
			return _uloc_getName(localeID, name, nameCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_getBaseNameDelegate(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err);

		private static uloc_getBaseNameDelegate _uloc_getBaseName;

		public static int uloc_getBaseName(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (_uloc_getBaseName == null)
				_uloc_getBaseName = GetMethod<uloc_getBaseNameDelegate>(IcuCommonLibHandle, "uloc_getBaseName");
			return _uloc_getBaseName(localeID, name, nameCapacity, out err);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private delegate int uloc_canonicalizeDelegate(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err);

		private static uloc_canonicalizeDelegate _uloc_canonicalize;

		public static int uloc_canonicalize(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err)
		{
			if (_uloc_canonicalize == null)
				_uloc_canonicalize = GetMethod<uloc_canonicalizeDelegate>(IcuCommonLibHandle, "uloc_canonicalize");
			var res = _uloc_canonicalize(localeID, name, nameCapacity, out err);
			return res;
		}

		#endregion

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_strToLowerDelegate(IntPtr dest, int destCapacity, string src, 
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);

		private static u_strToLowerDelegate _u_strToLower;

		/// <summary>Return the lower case equivalent of the string.</summary>
		public static int u_strToLower(IntPtr dest, int destCapacity, string src, 
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (_u_strToLower == null)
				_u_strToLower = GetMethod<u_strToLowerDelegate>(IcuCommonLibHandle, "u_strToLower");
			return _u_strToLower(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_strToTitleDelegate(IntPtr dest, int destCapacity, string src,
			int srcLength, IntPtr titleIter, [MarshalAs(UnmanagedType.LPStr)] string locale,
			out ErrorCode errorCode);

		private static u_strToTitleDelegate _u_strToTitle;

		/// <summary>Return the title case equivalent of the string.</summary>
		public static int u_strToTitle(IntPtr dest, int destCapacity, string src,
			int srcLength, IntPtr titleIter, [MarshalAs(UnmanagedType.LPStr)] string locale,
			out ErrorCode errorCode)
		{
			if (_u_strToTitle == null)
				_u_strToTitle = GetMethod<u_strToTitleDelegate>(IcuCommonLibHandle, "u_strToTitle", true);
			return _u_strToTitle(dest, destCapacity, src, srcLength, titleIter,
				locale, out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int u_strToUpperDelegate(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);

		private static u_strToUpperDelegate _u_strToUpper;

		/// <summary>Return the upper case equivalent of the string.</summary>
		public static int u_strToUpper(IntPtr dest, int destCapacity, string src,
			int srcLength, [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode)
		{
			if (_u_strToUpper == null)
				_u_strToUpper = GetMethod<u_strToUpperDelegate>(IcuCommonLibHandle, "u_strToUpper");
			return _u_strToUpper(dest, destCapacity, src, srcLength, locale, out errorCode);
		}

		#region normalize

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int unorm_normalizeDelegate(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, int options,
			IntPtr result, int resultLength, out ErrorCode errorCode);

		private static unorm_normalizeDelegate _unorm_normalize;

		/// <summary>
		/// Normalize a string according to the given mode and options.
		/// </summary>
		public static int unorm_normalize(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, int options,
			IntPtr result, int resultLength, out ErrorCode errorCode)
		{
			if (_unorm_normalize == null)
				_unorm_normalize = GetMethod<unorm_normalizeDelegate>(IcuCommonLibHandle, "unorm_normalize");
			return _unorm_normalize(source, sourceLength, mode, options, result,
				resultLength, out errorCode);
		}

		// Note that ICU's UBool type is typedef to an 8-bit integer.
		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate byte unorm_isNormalizedDelegate(string source,int sourceLength,
			Normalizer.UNormalizationMode mode,out ErrorCode errorCode);

		private static unorm_isNormalizedDelegate _unorm_isNormalized;

		/// <summary>
		/// Check whether a string is normalized according to the given mode and options.
		/// </summary>
		public static byte unorm_isNormalized(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, out ErrorCode errorCode)
		{
			if (_unorm_isNormalized == null)
				_unorm_isNormalized = GetMethod<unorm_isNormalizedDelegate>(IcuCommonLibHandle, "unorm_isNormalized");
			return _unorm_isNormalized(source, sourceLength, mode, out errorCode);
		}

		#endregion normalize

		#region Break iterator

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr ubrk_openDelegate(BreakIterator.UBreakIteratorType type,
			string locale, string text, int textLength, out ErrorCode errorCode);

		private static ubrk_openDelegate _ubrk_open;

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
			if (_ubrk_open == null)
				_ubrk_open = GetMethod<ubrk_openDelegate>(IcuCommonLibHandle, "ubrk_open", true);
			return _ubrk_open(type, locale, text, textLength, out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void ubrk_setTextDelegate(IntPtr bi, string text, int textLength, out ErrorCode errorCode);

		private static ubrk_setTextDelegate _ubrk_setText;

		/// <summary>
		/// Sets the iterator to a new text
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <param name="text">Text to examine</param>
		public static void ubrk_setText(IntPtr bi, string text, int textLength, out ErrorCode errorCode)
		{
			if (_ubrk_setText == null)
				_ubrk_setText = GetMethod<ubrk_setTextDelegate>(IcuCommonLibHandle, "ubrk_setText", true);

			_ubrk_setText(bi, text, textLength, out errorCode);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void ubrk_closeDelegate(IntPtr bi);

		private static ubrk_closeDelegate _ubrk_close;

		/// <summary>
		/// Close a UBreakIterator.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		public static void ubrk_close(IntPtr bi)
		{
			if (_ubrk_close == null)
				_ubrk_close = GetMethod<ubrk_closeDelegate>(IcuCommonLibHandle, "ubrk_close", true);
			_ubrk_close(bi);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int ubrk_firstDelegate(IntPtr bi);

		private static ubrk_firstDelegate _ubrk_first;

		/// <summary>
		/// Determine the index of the first character in the text being scanned.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_first(IntPtr bi)
		{
			if (_ubrk_first == null)
				_ubrk_first = GetMethod<ubrk_firstDelegate>(IcuCommonLibHandle, "ubrk_first", true);
			return _ubrk_first(bi);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int ubrk_nextDelegate(IntPtr bi);

		private static ubrk_nextDelegate _ubrk_next;

		/// <summary>
		/// Determine the text boundary following the current text boundary.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_next(IntPtr bi)
		{
			if (_ubrk_next == null)
				_ubrk_next = GetMethod<ubrk_nextDelegate>(IcuCommonLibHandle, "ubrk_next", true);
			return _ubrk_next(bi);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int ubrk_getRuleStatusDelegate(IntPtr bi);

		private static ubrk_getRuleStatusDelegate _ubrk_getRuleStatus;

		/// <summary>
		/// Return the status from the break rule that determined the most recently returned break position.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_getRuleStatus(IntPtr bi)
		{
			if (_ubrk_getRuleStatus == null)
				_ubrk_getRuleStatus = GetMethod<ubrk_getRuleStatusDelegate>(IcuCommonLibHandle, "ubrk_getRuleStatus", true);
			return _ubrk_getRuleStatus(bi);
		}

		#endregion Break iterator

		#region Unicode set

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void uset_closeDelegate(IntPtr set);

		private static uset_closeDelegate _uset_close;

		/// <summary>
		/// Disposes of the storage used by Unicode set.  This function should be called exactly once for objects returned by uset_open()
		/// </summary>
		/// <param name="set">Unicode set to dispose of </param>
		public static void uset_close(IntPtr set)
		{
			if (_uset_close == null)
				_uset_close = GetMethod<uset_closeDelegate>(IcuCommonLibHandle, "uset_close");
			_uset_close(set);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr uset_openDelegate(char start,char end);

		private static uset_openDelegate _uset_open;

		/// <summary>
		/// Creates a Unicode set that contains the range of characters start..end, inclusive.
		/// If start > end then an empty set is created (same as using uset_openEmpty().
		/// </summary>
		/// <param name="start">First character of the range, inclusive</param>
		/// <param name="end">Last character of the range, inclusive</param>
		/// <returns>Unicode set of characters.  The caller must call uset_close() on it when done</returns>
		public static IntPtr uset_open(char start, char end)
		{
			if (_uset_open == null)
				_uset_open = GetMethod<uset_openDelegate>(IcuCommonLibHandle, "uset_open");
			return _uset_open(start, end);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate IntPtr uset_openPatternDelegate(string pattern,int patternLength,ref ErrorCode status);

		private static uset_openPatternDelegate _uset_openPattern;

		/// <summary>
		/// Creates a set from the given pattern.
		/// </summary>
		/// <param name="pattern">A string specifying what characters are in the set</param>
		/// <param name="patternLength">Length of the pattern, or -1 if null terminated</param>
		/// <param name="status">The error code</param>
		/// <returns>Unicode set</returns>
		public static IntPtr uset_openPattern(string pattern, int patternLength, ref ErrorCode status)
		{
			if (_uset_openPattern == null)
				_uset_openPattern = GetMethod<uset_openPatternDelegate>(IcuCommonLibHandle, "uset_openPattern");
			return _uset_openPattern(pattern, patternLength, ref status);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void uset_addDelegate(IntPtr set,char c);

		private static uset_addDelegate _uset_add;

		/// <summary>
		/// Adds the given character to the given Unicode set.  After this call, uset_contains(set, c) will return TRUE.  A frozen set will not be modified.
		/// </summary>
		/// <param name="set">The object to which to add the character</param>
		/// <param name="c">The character to add</param>
		public static void uset_add(IntPtr set, char c)
		{
			if (_uset_add == null)
				_uset_add = GetMethod<uset_addDelegate>(IcuCommonLibHandle, "uset_add");
			_uset_add(set, c);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int uset_toPatternDelegate(IntPtr set, IntPtr result, int resultCapacity,
			bool escapeUnprintable, ref ErrorCode status);

		private static uset_toPatternDelegate _uset_toPattern;

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
			if (_uset_toPattern == null)
				_uset_toPattern = GetMethod<uset_toPatternDelegate>(IcuCommonLibHandle, "uset_toPattern");
			return _uset_toPattern(set, result, resultCapacity, escapeUnprintable, ref status);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate void uset_addStringDelegate(IntPtr set,string str,int strLen);

		private static uset_addStringDelegate _uset_addString;

		/// <summary>
		/// Adds the given string to the given Unicode set
		/// </summary>
		/// <param name="set">The Unicode set to which to add the string</param>
		/// <param name="str">The string to add</param>
		/// <param name="strLen">The length of the string or -1 if null</param>
		public static void uset_addString(IntPtr set, string str, int strLen)
		{
			if (_uset_addString == null)
				_uset_addString = GetMethod<uset_addStringDelegate>(IcuCommonLibHandle, "uset_addString");
			_uset_addString(set, str, strLen);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int uset_getItemDelegate(IntPtr set, int itemIndex, out int start,
			out int end, IntPtr str, int strCapacity, ref ErrorCode ec);

		private static uset_getItemDelegate _uset_getItem;

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
			if (_uset_getItem == null)
				_uset_getItem = GetMethod<uset_getItemDelegate>(IcuCommonLibHandle, "uset_getItem");
			return _uset_getItem(set, itemIndex, out start, out end, str, strCapacity, ref ec);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int uset_getItemCountDelegate(IntPtr set);

		private static uset_getItemCountDelegate _uset_getItemCount;

		/// <summary>
		/// Returns the number of items in this set.  An item is either a range of characters or a single multicharacter string
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <returns>A non-negative integer counting the character ranges and/or strings contained in the set</returns>
		public static int uset_getItemCount(IntPtr set)
		{
			if (_uset_getItemCount == null)
				_uset_getItemCount = GetMethod<uset_getItemCountDelegate>(IcuCommonLibHandle, "uset_getItemCount");
			return _uset_getItemCount(set);
		}

		#endregion // Unicode set
	}
}
