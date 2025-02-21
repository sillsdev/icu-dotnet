// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Icu.Collation;

// ReSharper disable once CheckNamespace
namespace Icu
{
	internal static partial class NativeMethods
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class CollatorMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openDelegate(
				[MarshalAs(UnmanagedType.LPStr)] string loc, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openRulesDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string rules, int rulesLength,
				NormalizationMode normalizationMode, CollationStrength strength,
				ref ParseError parseError, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucol_closeDelegate(IntPtr coll);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate CollationResult ucol_strcollDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				[MarshalAs(UnmanagedType.LPWStr)] string source, int sourceLength,
				[MarshalAs(UnmanagedType.LPWStr)] string target, int targetLength);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_countAvailableDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate SafeEnumeratorHandle ucol_openAvailableLocalesDelegate(out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_getSortKeyDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				[MarshalAs(UnmanagedType.LPWStr)] string source, int sourceLength,
				[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result, int resultLength);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucol_setAttributeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				CollationAttribute attr, CollationAttributeValue value, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate CollationAttributeValue ucol_getAttributeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator, CollationAttribute attr,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeCloneDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
				IntPtr stackBuffer, ref int pBufferSize, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ucol_getLocaleByTypeDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle collator, LocaleType type,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_getRulesExDelegate(
				RuleBasedCollator.SafeRuleBasedCollatorHandle coll, UColRuleOption delta,
				IntPtr buffer, int bufferLen);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucol_getBoundDelegate(byte[] source, int sourceLength,
				UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
				out ErrorCode status);

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
		}

		// ReSharper disable once InconsistentNaming
		private static CollatorMethodsContainer CollatorMethods = new CollatorMethodsContainer();

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
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_open == null)
				CollatorMethods.ucol_open = GetMethod<CollatorMethodsContainer.ucol_openDelegate>(IcuI18NLibHandle, "ucol_open");
			return CollatorMethods.ucol_open(loc, out status);
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
		/// <param name="parseError">A pointer to ParseError to receive information about errors
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
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_openRules == null)
				CollatorMethods.ucol_openRules = GetMethod<CollatorMethodsContainer.ucol_openRulesDelegate>(IcuI18NLibHandle, "ucol_openRules");
			return CollatorMethods.ucol_openRules(rules, rulesLength, normalizationMode, strength, ref parseError,
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
			if (CollatorMethods.ucol_close == null)
				CollatorMethods.ucol_close = GetMethod<CollatorMethodsContainer.ucol_closeDelegate>(IcuI18NLibHandle, "ucol_close");
			CollatorMethods.ucol_close(coll);
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
			int sourceLength,
			[MarshalAs(UnmanagedType.LPWStr)] string target,
			int targetLength)
		{
			if (CollatorMethods.ucol_strcoll == null)
				CollatorMethods.ucol_strcoll = GetMethod<CollatorMethodsContainer.ucol_strcollDelegate>(IcuI18NLibHandle, "ucol_strcoll");
			return CollatorMethods.ucol_strcoll(collator, source, sourceLength, target, targetLength);
		}

		/**
		 * Determine how many locales have collation rules available.
		 * This function is most useful as determining the loop ending condition for
		 * calls to {@link #ucol_getAvailable }.
		 * @return The number of locales for which collation rules are available.
		 * @see ucol_getAvailable
		 * @stable ICU 2.0
		 */
		public static int ucol_countAvailable()
		{
			if (CollatorMethods.ucol_countAvailable == null)
				CollatorMethods.ucol_countAvailable = GetMethod<CollatorMethodsContainer.ucol_countAvailableDelegate>(IcuI18NLibHandle, "ucol_countAvailable");
			return CollatorMethods.ucol_countAvailable();
		}

		/**
		 * Create a string enumerator of all locales for which a valid
		 * collator may be opened.
		 * @param status input-output error code
		 * @return a string enumeration over locale strings. The caller is
		 * responsible for closing the result.
		 * @stable ICU 3.0
		 */
		public static SafeEnumeratorHandle ucol_openAvailableLocales(out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_openAvailableLocales == null)
				CollatorMethods.ucol_openAvailableLocales = GetMethod<CollatorMethodsContainer.ucol_openAvailableLocalesDelegate>(IcuI18NLibHandle, "ucol_openAvailableLocales");
			return CollatorMethods.ucol_openAvailableLocales(out status);
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
		public static int ucol_getSortKey(
			RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
			[MarshalAs(UnmanagedType.LPWStr)] string source, int sourceLength,
			[Out, MarshalAs(UnmanagedType.LPArray)] byte[] result, int resultLength)
		{
			if (CollatorMethods.ucol_getSortKey == null)
				CollatorMethods.ucol_getSortKey = GetMethod<CollatorMethodsContainer.ucol_getSortKeyDelegate>(IcuI18NLibHandle, "ucol_getSortKey");
			return CollatorMethods.ucol_getSortKey(collator, source, sourceLength, result, resultLength);
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
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_setAttribute == null)
				CollatorMethods.ucol_setAttribute = GetMethod<CollatorMethodsContainer.ucol_setAttributeDelegate>(IcuI18NLibHandle, "ucol_setAttribute");
			CollatorMethods.ucol_setAttribute(collator, attr, value, out status);
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
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_getAttribute == null)
				CollatorMethods.ucol_getAttribute = GetMethod<CollatorMethodsContainer.ucol_getAttributeDelegate>(IcuI18NLibHandle, "ucol_getAttribute");
			return CollatorMethods.ucol_getAttribute(collator, attr, out status);
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
			ref int pBufferSize,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_safeClone == null)
				CollatorMethods.ucol_safeClone = GetMethod<CollatorMethodsContainer.ucol_safeCloneDelegate>(IcuI18NLibHandle, "ucol_safeClone");
			return CollatorMethods.ucol_safeClone(collator, stackBuffer, ref pBufferSize, out status);
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
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_getLocaleByType == null)
				CollatorMethods.ucol_getLocaleByType = GetMethod<CollatorMethodsContainer.ucol_getLocaleByTypeDelegate>(IcuI18NLibHandle, "ucol_getLocaleByType");
			return CollatorMethods.ucol_getLocaleByType(collator, type, out status);
		}

		public static int ucol_getRulesEx(
			RuleBasedCollator.SafeRuleBasedCollatorHandle coll,
			UColRuleOption delta,
			IntPtr buffer, int bufferLen)
		{
			if (CollatorMethods.ucol_getRulesEx == null)
				CollatorMethods.ucol_getRulesEx = GetMethod<CollatorMethodsContainer.ucol_getRulesExDelegate>(IcuI18NLibHandle, "ucol_getRulesEx");
			return CollatorMethods.ucol_getRulesEx(coll, delta, buffer, bufferLen);
		}

		public static int ucol_getBound(byte[] source, int sourceLength,
			UColBoundMode boundType, int noOfLevels, byte[] result, int resultLength,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (CollatorMethods.ucol_getBound == null)
				CollatorMethods.ucol_getBound = GetMethod<CollatorMethodsContainer.ucol_getBoundDelegate>(IcuI18NLibHandle, "ucol_getBound");
			return CollatorMethods.ucol_getBound(source, sourceLength, boundType, noOfLevels, result, resultLength, out status);
		}

		#endregion // Unicode collator

	}
}
