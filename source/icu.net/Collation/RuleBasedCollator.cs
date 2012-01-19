using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Globalization;
namespace Icu.Collation
{
	public sealed class RuleBasedCollator : Collator
	{
		private sealed class SafeRuleBasedCollatorHandle : SafeHandle
		{
			public SafeRuleBasedCollatorHandle() :
					base(IntPtr.Zero, true) {}

			///<summary>
			///When overridden in a derived class, executes the code required to free the handle.
			///</summary>
			///<returns>
			///true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
			///</returns>
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			protected override bool ReleaseHandle()
			{
				NativeMethods.ucol_close(handle);
				return true;
			}

			///<summary>
			///When overridden in a derived class, gets a value indicating whether the handle value is invalid.
			///</summary>
			///<returns>
			///true if the handle is valid; otherwise, false.
			///</returns>
			public override bool IsInvalid
			{
				get { return (handle == IntPtr.Zero); }
			}
		}

		private SafeRuleBasedCollatorHandle collatorHandle;
		readonly ParseError parseError = new ParseError();

		/// <summary>
		/// RuleBasedCollator constructor.
		/// This takes the table rules and builds a collation table out of them.
		/// </summary>
		/// <param name="rules">the collation rules to build the collation table from</param>
		public RuleBasedCollator(string rules) :
				this(rules, CollationStrength.Default) {}

		/// <summary>
		/// RuleBasedCollator constructor.
		/// This takes the table rules and builds a collation table out of them.
		/// </summary>
		/// <param name="rules">the collation rules to build the collation table from</param>
		/// <param name="collationStrength">the collation strength to use</param>
		public RuleBasedCollator(string rules, CollationStrength collationStrength)
			: this(rules, NormalizationMode.Default, collationStrength) {}

		/// <summary>
		/// RuleBasedCollator constructor.
		/// This takes the table rules and builds a collation table out of them.
		/// </summary>
		/// <param name="rules">the collation rules to build the collation table from</param>
		/// <param name="normalizationMode">the normalization mode to use</param>
		/// <param name="collationStrength">the collation strength to use</param>
		public RuleBasedCollator(string rules,
								 NormalizationMode normalizationMode,
								 CollationStrength collationStrength)
		{
			ErrorCode status;
			collatorHandle = NativeMethods.ucol_openRules(rules,
														  rules.Length,
														  normalizationMode,
														  collationStrength,
														  ref parseError,
														  out status);
			ExceptionFromErrorCode.ThrowIfError(status, parseError.ToString(rules));
		}

		/// <summary>The collation strength.
		/// The usual strength for most locales (except Japanese) is tertiary.
		/// Quaternary strength is useful when combined with shifted setting
		/// for alternate handling attribute and for JIS x 4061 collation,
		/// when it is used to distinguish between Katakana and Hiragana
		/// (this is achieved by setting the HiraganaQuaternary mode to on.
		/// Otherwise, quaternary level is affected only by the number of
		/// non ignorable code points in the string.
		/// </summary>
		public override CollationStrength Strength
		{
			get { return (CollationStrength) GetAttribute(NativeMethods.CollationAttribute.Strength); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.Strength,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// Controls whether the normalization check and necessary normalizations
		/// are performed.
		/// </summary>
		public override NormalizationMode NormalizationMode
		{
			get { return (NormalizationMode) GetAttribute(NativeMethods.CollationAttribute.NormalizationMode); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.NormalizationMode,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// Direction of secondary weights - Necessary for French to make secondary weights be considered from back to front.
		/// </summary>
		public override FrenchCollation FrenchCollation
		{
			get { return (FrenchCollation) GetAttribute(NativeMethods.CollationAttribute.FrenchCollation); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.FrenchCollation,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// Controls whether an extra case level (positioned before the third
		/// level) is generated or not. Contents of the case level are affected by
		/// the value of CaseFirst attribute. A simple way to ignore
		/// accent differences in a string is to set the strength to Primary
		/// and enable case level.
		/// </summary>
		public override CaseLevel CaseLevel
		{
			get { return (CaseLevel) GetAttribute(NativeMethods.CollationAttribute.CaseLevel); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.CaseLevel,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// When turned on, this attribute positions Hiragana before all
		/// non-ignorables on quaternary level This is a sneaky way to produce JIS
		/// sort order
		/// </summary>
		public override HiraganaQuaternary HiraganaQuaternary
		{
			get { return (HiraganaQuaternary) GetAttribute(NativeMethods.CollationAttribute.HiraganaQuaternaryMode); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.HiraganaQuaternaryMode,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// When turned on, this attribute generates a collation key
		/// for the numeric value of substrings of digits.
		/// This is a way to get '100' to sort AFTER '2'.
		/// </summary>
		public override NumericCollation NumericCollation
		{
			get { return (NumericCollation) GetAttribute(NativeMethods.CollationAttribute.NumericCollation); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.NumericCollation,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// Controls the ordering of upper and lower case letters.
		/// </summary>
		public override CaseFirst CaseFirst
		{
			get { return (CaseFirst) GetAttribute(NativeMethods.CollationAttribute.CaseFirst); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.CaseFirst,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		/// <summary>
		/// Attribute for handling variable elements.
		/// </summary>
		public override AlternateHandling AlternateHandling
		{
			get { return (AlternateHandling) GetAttribute(NativeMethods.CollationAttribute.AlternateHandling); }
			set
			{
				SetAttribute(NativeMethods.CollationAttribute.AlternateHandling,
							 (NativeMethods.CollationAttributeValue) value);
			}
		}

		private byte[] keyData = new byte[1024];

		/// <summary>
		/// Get a sort key for the argument string.
		/// Sort keys may be compared using SortKey.Compare
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public override SortKey GetSortKey(string source)
		{
			if(source == null)
			{
				throw new ArgumentNullException();
			}
			int actualLength;
			for (;;)
			{
				actualLength = NativeMethods.ucol_getSortKey(collatorHandle,
															 source,
															 source.Length,
															 keyData,
															 keyData.Length);
				if (actualLength > keyData.Length)
				{
					keyData = new byte[keyData.Length*2];
					continue;
				}
				break;
			}
			return CreateSortKey(source, keyData, actualLength);
		}

		private NativeMethods.CollationAttributeValue GetAttribute(NativeMethods.CollationAttribute attr)
		{
			ErrorCode e;
			NativeMethods.CollationAttributeValue value = NativeMethods.ucol_getAttribute(collatorHandle, attr, out e);
			ExceptionFromErrorCode.ThrowIfError(e);
			return value;
		}

		private void SetAttribute(NativeMethods.CollationAttribute attr, NativeMethods.CollationAttributeValue value)
		{
			ErrorCode e;
			NativeMethods.ucol_setAttribute(collatorHandle, attr, value, out e);
			ExceptionFromErrorCode.ThrowIfError(e);
		}

		public static IList<string> GetAvailableCollationLocales()
		{
			List<string> locales = new List<string>();
			// The ucol_openAvailableLocales call failes when there are no locales available, so check first.
			if (NativeMethods.ucol_countAvailable() == 0)
			{
				return locales;
			}
			ErrorCode ec;
			SafeEnumeratorHandle en = NativeMethods.ucol_openAvailableLocales(out ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			try
			{
				string str = en.Next();
				while (str != null)
				{
					locales.Add(str);
					str = en.Next();
				}
			}
			finally
			{
				en.Close();
			}
			return locales;
		}

		private sealed class SafeEnumeratorHandle : SafeHandle
		{
			public SafeEnumeratorHandle()
				:
					base(IntPtr.Zero, true) { }

			///<summary>
			///When overridden in a derived class, executes the code required to free the handle.
			///</summary>
			///<returns>
			///true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
			///</returns>
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			protected override bool ReleaseHandle()
			{
				NativeMethods.uenum_close(handle);
				handle = IntPtr.Zero;
				return true;
			}

			///<summary>
			///When overridden in a derived class, gets a value indicating whether the handle value is invalid.
			///</summary>
			///<returns>
			///true if the handle is valid; otherwise, false.
			///</returns>
			public override bool IsInvalid
			{
				get { return (handle == IntPtr.Zero); }
			}

			public string Next()
			{
				ErrorCode e;
				int length;
				IntPtr str = NativeMethods.uenum_unext(this, out length, out e);
				if (str == IntPtr.Zero)
				{
					return null;
				}
				string result = Marshal.PtrToStringUni(str, length);
				ExceptionFromErrorCode.ThrowIfError(e);
				return result;
			}
		}

		#region ICloneable Members

		///<summary>
		///Creates a new object that is a copy of the current instance.
		///</summary>
		///
		///<returns>
		///A new object that is a copy of this instance.
		///</returns>
		public override object Clone()
		{
			RuleBasedCollator copy = new RuleBasedCollator();
			ErrorCode status;
			int buffersize = 512;
			copy.collatorHandle = NativeMethods.ucol_safeClone(collatorHandle,
															   IntPtr.Zero,
															   ref buffersize,
															   out status);
			ExceptionFromErrorCode.ThrowIfError(status);
			return copy;
		}

		public static new Collator Create(string localeId)
		{
			return Create(localeId, Fallback.NoFallback);
		}

		public static new Collator Create(string localeId, Fallback fallback)
		{
			RuleBasedCollator instance = new RuleBasedCollator();
			ErrorCode status;
			instance.collatorHandle = NativeMethods.ucol_open(localeId, out status);
			if(status == ErrorCode.USING_FALLBACK_WARNING && fallback == Fallback.NoFallback && instance.ValidId != localeId )
			{
				throw new ArgumentException("Could only create Collator '" +
											localeId +
											"' by falling back to '" +
											instance.ActualId +
											"'. You can use the fallback option to create this.");
			}
			if(status == ErrorCode.USING_DEFAULT_WARNING && fallback == Fallback.NoFallback && instance.ValidId != localeId &&
				 localeId.Length > 0 && localeId != "root")
			{
				throw new ArgumentException("Could only create Collator '" +
											localeId +
											"' by falling back to the default '" +
											instance.ActualId +
											"'. You can use the fallback option to create this.");
			}
			if (status == ErrorCode.INTERNAL_PROGRAM_ERROR && fallback == Fallback.FallbackAllowed)
			{
				instance = new RuleBasedCollator(string.Empty); // fallback to UCA
			}
			else
			{
				try
				{
					ExceptionFromErrorCode.ThrowIfError(status);
				}
				catch (Exception e)
				{
					throw new ArgumentException(
							"Unable to create a collator using the given localeId '"+localeId+"'.\nThis is likely because the ICU data file was created without collation rules for this locale. You can provide the rules yourself or replace the data dll.",
							e);
				}
			}
			return instance;
		}

		private RuleBasedCollator() {}

		private string ActualId
		{
		 get
		 {
			 ErrorCode status;
			 IntPtr resultAsIntPtr = NativeMethods.ucol_getLocaleByType(
				 collatorHandle,
				 NativeMethods.LocaleType.ActualLocale,
				 out status
			 );
			 string result = Marshal.PtrToStringAnsi(resultAsIntPtr);
			 if(status != ErrorCode.NoErrors)
			 {
				 return string.Empty;
			 }
			 return result;
		 }
		}
		private string ValidId
		{
		 get
		 {
			 ErrorCode status;
			 IntPtr resultAsIntPtr = NativeMethods.ucol_getLocaleByType(
				 collatorHandle,
				 NativeMethods.LocaleType.ValidLocale,
				 out status
			 );
			 string result = Marshal.PtrToStringAnsi(resultAsIntPtr);
			 if(status != ErrorCode.NoErrors)
			 {
				 return string.Empty;
			 }
			 return result;
		 }
		}

		#endregion

		#region IComparer<string> Members
		/// <summary>
		/// Compares two strings based on the rules of this RuleBasedCollator
		/// </summary>
		/// <param name="string1">The first string to compare</param>
		/// <param name="string2">The second string to compare</param>
		/// <returns></returns>
		/// <remarks>Comparing a null reference is allowed and does not generate an exception.
		/// A null reference is considered to be less than any reference that is not null.</remarks>
		public override int Compare(string string1, string string2)
		{
			if(string1 == null)
			{
				if(string2 == null)
				{
					return 0;
				}
				return -1;
			}
			if(string2 == null)
			{
				return 1;
			}
			return (int) NativeMethods.ucol_strcoll(collatorHandle,
													string1,
													string1.Length,
													string2,
													string2.Length);
		}

		#endregion

		private static class NativeMethods
		{
			private const string ICU_I18N_LIB = "icuin42.dll";
			private const string ICU_COMMON_LIB = "icuuc42.dll";
			#if ICU_VER_48
			private const string ICU_VERSION_SUFFIX = "_48";
			#elif ICU_VER_44
			private const string ICU_VERSION_SUFFIX = "_44";
			#else
			private const string ICU_VERSION_SUFFIX = "_4_2";
			#endif

			/**
			 * Function type declaration for uenum_close().
			 *
			 * This function should cleanup the enumerator object
			 *
			 * @param en enumeration to be closed
			 */
			[DllImport(ICU_COMMON_LIB, EntryPoint = "uenum_close" + ICU_VERSION_SUFFIX)]
			public static extern void uenum_close(IntPtr en);

			/**
			 * Function type declaration for uenum_unext().
			 *
			 * This function returns the next element as a UChar *,
			 * or NULL after all elements haven been enumerated.
			 *
			 * @param en enumeration
			 * @param resultLength pointer to result length
			 * @param status pointer to UErrorCode variable
			 * @return next element as UChar *,
			 *         or NULL after all elements haven been enumerated
			 */
			[DllImport(ICU_COMMON_LIB, EntryPoint = "uenum_unext" + ICU_VERSION_SUFFIX)]
			public static extern IntPtr uenum_unext(
				SafeEnumeratorHandle en,
				out int resultLength,
				out ErrorCode status);

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
			/// <param name="status">A pointer to an UErrorCode to receive any errors
			///</param>
			/// <returns>pointer to a Collator or 0 if an error occurred</returns>
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_open"+ICU_VERSION_SUFFIX)]
			public static extern SafeRuleBasedCollatorHandle ucol_open(
					[MarshalAs(UnmanagedType.LPStr)] string loc,
					out ErrorCode status);


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
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openRules"+ICU_VERSION_SUFFIX)]
			public static extern SafeRuleBasedCollatorHandle ucol_openRules(
					[MarshalAs(UnmanagedType.LPWStr)] string rules,
					int rulesLength,
					NormalizationMode normalizationMode,
					CollationStrength strength,
					ref ParseError parseError,
					out ErrorCode status);

/**
 * Open a collator defined by a short form string.
 * The structure and the syntax of the string is defined in the "Naming collators"
 * section of the users guide:
 * http://icu-project.org/userguide/Collate_Concepts.html#Naming_Collators
 * Attributes are overriden by the subsequent attributes. So, for "S2_S3", final
 * strength will be 3. 3066bis locale overrides individual locale parts.
 * The call to this function is equivalent to a call to ucol_open, followed by a
 * series of calls to ucol_setAttribute and ucol_setVariableTop.
 * @param definition A short string containing a locale and a set of attributes.
 *                   Attributes not explicitly mentioned are left at the default
 *                   state for a locale.
 * @param parseError if not NULL, structure that will get filled with error's pre
 *                   and post context in case of error.
 * @param forceDefaults if FALSE, the settings that are the same as the collator
 *                   default settings will not be applied (for example, setting
 *                   French secondary on a French collator would not be executed).
 *                   If TRUE, all the settings will be applied regardless of the
 *                   collator default value. If the definition
 *                   strings are to be cached, should be set to FALSE.
 * @param status     Error code. Apart from regular error conditions connected to
 *                   instantiating collators (like out of memory or similar), this
 *                   API will return an error if an invalid attribute or attribute/value
 *                   combination is specified.
 * @return           A pointer to a UCollator or 0 if an error occured (including an
 *                   invalid attribute).
 * @see ucol_open
 * @see ucol_setAttribute
 * @see ucol_setVariableTop
 * @see ucol_getShortDefinitionString
 * @see ucol_normalizeShortDefinitionString
 * @stable ICU 3.0
 *
 */

			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openFromShortString"+ICU_VERSION_SUFFIX))]
			public static extern SafeRuleBasedCollatorHandle ucol_openFromShortString(
					[MarshalAs(UnmanagedType.LPStr)] string definition,
					[MarshalAs(UnmanagedType.I1)] bool forceDefaults,
					ref ParseError parseError,
					out ErrorCode status);
			*/

			/**
 * Close a UCollator.
 * Once closed, a UCollator should not be used. Every open collator should
 * be closed. Otherwise, a memory leak will result.
 * @param coll The UCollator to close.
 * @stable ICU 2.0
 */

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_close"+ICU_VERSION_SUFFIX)]
			public static extern void ucol_close(IntPtr collator);

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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_strcoll"+ICU_VERSION_SUFFIX)]
			public static extern CollationResult ucol_strcoll(SafeRuleBasedCollatorHandle collator,
															  [MarshalAs(UnmanagedType.LPWStr)] string source,
															  Int32 sourceLength,
															  [MarshalAs(UnmanagedType.LPWStr)] string target,
															  Int32 targetLength);

			/**
 * Get the collation strength used in a UCollator.
 * The strength influences how strings are compared.
 * @param coll The UCollator to query.
 * @return The collation strength; one of UCOL_PRIMARY, UCOL_SECONDARY,
 * UCOL_TERTIARY, UCOL_QUATERNARY, UCOL_IDENTICAL
 * @see ucol_setStrength
 * @stable ICU 2.0
 */
			/*

[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getStrength"+ICU_VERSION_SUFFIX))]
public static extern CollationStrength ucol_getStrength(SafeRuleBasedCollatorHandle collator);
*/

/**
* Set the collation strength used in a UCollator.
* The strength influences how strings are compared.
* @param coll The UCollator to set.
* @param strength The desired collation strength; one of UCOL_PRIMARY,
* UCOL_SECONDARY, UCOL_TERTIARY, UCOL_QUATERNARY, UCOL_IDENTICAL, UCOL_DEFAULT
* @see ucol_getStrength
* @stable ICU 2.0
*/
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setStrength"+ICU_VERSION_SUFFIX))]
			public static extern void ucol_setStrength(SafeRuleBasedCollatorHandle collator,
													   CollationStrength strength);
			*/

			/**
 * Get the display name for a UCollator.
 * The display name is suitable for presentation to a user.
 * @param objLoc The locale of the collator in question.
 * @param dispLoc The locale for display.
 * @param result A pointer to a buffer to receive the attribute.
 * @param resultLength The maximum size of result.
 * @param status A pointer to an UErrorCode to receive any errors
 * @return The total buffer size needed; if greater than resultLength,
 * the output was truncated.
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getDisplayName"+ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_getDisplayName([MarshalAs(UnmanagedType.LPStr)] string objLoc,
														   [MarshalAs(UnmanagedType.LPStr)] string dispLoc,
														   [MarshalAs(UnmanagedType.LPWStr)] StringBuilder result,
														   Int32 resultLength,
														   out ErrorCode status);
			*/
/**
 * Get a locale for which collation rules are available.
 * A UCollator in a locale returned by this function will perform the correct
 * collation for the locale.
 * @param index The index of the desired locale.
 * @return A locale for which collation rules are available, or 0 if none.
 * @see ucol_countAvailable
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getAvailable"+ICU_VERSION_SUFFIX)]
			[return : MarshalAs(UnmanagedType.LPStr)]
			public static extern string ucol_getAvailable(Int32 index);
			*/

/**
 * Determine how many locales have collation rules available.
 * This function is most useful as determining the loop ending condition for
 * calls to {@link #ucol_getAvailable }.
 * @return The number of locales for which collation rules are available.
 * @see ucol_getAvailable
 * @stable ICU 2.0
 */
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_countAvailable"+ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_countAvailable();


			/**
 * Create a string enumerator of all locales for which a valid
 * collator may be opened.
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openAvailableLocales"+ICU_VERSION_SUFFIX)]
			public static extern SafeEnumeratorHandle ucol_openAvailableLocales(out ErrorCode status);


			/**
 * Create a string enumerator of all possible keywords that are relevant to
 * collation. At this point, the only recognized keyword for this
 * service is "collation".
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getKeywords"+ICU_VERSION_SUFFIX))]
			public static extern IntPtr ucol_getKeywords(out ErrorCode status);
			*/


			/**
 * Given a keyword, create a string enumeration of all values
 * for that keyword that are currently in use.
 * @param keyword a particular keyword as enumerated by
 * ucol_getKeywords. If any other keyword is passed in, *status is set
 * to U_ILLEGAL_ARGUMENT_ERROR.
 * @param status input-output error code
 * @return a string enumeration over collation keyword values, or NULL
 * upon error. The caller is responsible for closing the result.
 * @stable ICU 3.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getKeywordValues"+ICU_VERSION_SUFFIX))]
			public static extern IntPtr ucol_getKeywordValues([MarshalAs(UnmanagedType.LPStr)] string keyword,
															  out ErrorCode status);
			*/



			/**
 * Return the functionally equivalent locale for the given
 * requested locale, with respect to given keyword, for the
 * collation service.  If two locales return the same result, then
 * collators instantiated for these locales will behave
 * equivalently.  The converse is not always true; two collators
 * may in fact be equivalent, but return different results, due to
 * internal details.  The return result has no other meaning than
 * that stated above, and implies nothing as to the relationship
 * between the two locales.  This is intended for use by
 * applications who wish to cache collators, or otherwise reuse
 * collators when possible.  The functional equivalent may change
 * over time.  For more information, please see the <a
 * href="http://icu-project.org/userguide/locale.html#services">
 * Locales and Services</a> section of the ICU User Guide.
 * @param result fillin for the functionally equivalent locale
 * @param resultCapacity capacity of the fillin buffer
 * @param keyword a particular keyword as enumerated by
 * ucol_getKeywords.
 * @param locale the requested locale
 * @param isAvailable if non-NULL, pointer to a fillin parameter that
 * indicates whether the requested locale was 'available' to the
 * collation service. A locale is defined as 'available' if it
 * physically exists within the collation locale data.
 * @param status pointer to input-output error code
 * @return the actual buffer size needed for the locale.  If greater
 * than resultCapacity, the returned full name will be truncated and
 * an error code will be returned.
 * @stable ICU 3.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getFunctionalEquivalent" + ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_getFunctionalEquivalent(
					[MarshalAs(UnmanagedType.LPStr)] StringBuilder result,
					Int32 resultCapacity,
					[MarshalAs(UnmanagedType.LPStr)] string keyword,
					[MarshalAs(UnmanagedType.LPStr)] string locale,
					[MarshalAs(UnmanagedType.I1)] out bool isAvailable,
					out ErrorCode status);
			*/

/**
 * Get the collation rules from a UCollator.
 * The rules will follow the rule syntax.
 * @param coll The UCollator to query.
 * @param length
 * @return The collation rules.
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getRules" + ICU_VERSION_SUFFIX)]
			public static extern string ucol_getRules(SafeRuleBasedCollatorHandle collator,
													  out Int32 length);
			*/

/** Get the short definition string for a collator. This API harvests the collator's
 *  locale and the attribute set and produces a string that can be used for opening
 *  a collator with the same properties using the ucol_openFromShortString API.
 *  This string will be normalized.
 *  The structure and the syntax of the string is defined in the "Naming collators"
 *  section of the users guide:
 *  http://icu-project.org/userguide/Collate_Concepts.html#Naming_Collators
 *  This API supports preflighting.
 *  @param coll a collator
 *  @param locale a locale that will appear as a collators locale in the resulting
 *                short string definition. If NULL, the locale will be harvested
 *                from the collator.
 *  @param buffer space to hold the resulting string
 *  @param capacity capacity of the buffer
 *  @param status for returning errors. All the preflighting errors are featured
 *  @return length of the resulting string
 *  @see ucol_openFromShortString
 *  @see ucol_normalizeShortDefinitionString
 *  @stable ICU 3.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getShortDefinitionString" + ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_getShortDefinitionString(SafeRuleBasedCollatorHandle collator,
																	 [MarshalAs(UnmanagedType.LPStr)] string locale,
																	 [In,Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder
																			 buffer,
																	 Int32 capacity,
																	 out ErrorCode status);
			*/
/** Verifies and normalizes short definition string.
 *  Normalized short definition string has all the option sorted by the argument name,
 *  so that equivalent definition strings are the same.
 *  This API supports preflighting.
 *  @param source definition string
 *  @param destination space to hold the resulting string
 *  @param capacity capacity of the buffer
 *  @param parseError if not NULL, structure that will get filled with error's pre
 *                   and post context in case of error.
 *  @param status     Error code. This API will return an error if an invalid attribute
 *                    or attribute/value combination is specified. All the preflighting
 *                    errors are also featured
 *  @return length of the resulting normalized string.
 *
 *  @see ucol_openFromShortString
 *  @see ucol_getShortDefinitionString
 *
 *  @stable ICU 3.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_normalizeShortDefinitionString" + ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_normalizeShortDefinitionString(
					[MarshalAs(UnmanagedType.LPStr)] string source,
					[MarshalAs(UnmanagedType.LPStr)] StringBuilder destination,
					Int32 capacity,
					ref ParseError parseError,
					out ErrorCode status);
			*/
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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getSortKey" + ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_getSortKey(SafeRuleBasedCollatorHandle collator,
													   [MarshalAs(UnmanagedType.LPWStr)] string source,
													   Int32 sourceLength,
													   [Out, MarshalAs(UnmanagedType.LPArray)] byte[]
															   result,
													   Int32 resultLength);

/** Gets the next count bytes of a sort key. Caller needs
 *  to preserve state array between calls and to provide
 *  the same type of UCharIterator set with the same string.
 *  The destination buffer provided must be big enough to store
 *  the number of requested bytes. Generated sortkey is not
 *  compatible with sortkeys generated using ucol_getSortKey
 *  API, since we don't do any compression. If uncompressed
 *  sortkeys are required, this API can be used.
 *  @param coll The UCollator containing the collation rules.
 *  @param iter UCharIterator containing the string we need
 *              the sort key to be calculated for.
 *  @param state Opaque state of sortkey iteration.
 *  @param dest Buffer to hold the resulting sortkey part
 *  @param count number of sort key bytes required.
 *  @param status error code indicator.
 *  @return the actual number of bytes of a sortkey. It can be
 *          smaller than count if we have reached the end of
 *          the sort key.
 *  @stable ICU 2.6
 */
			/*
[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_nextSortKeyPart"+ICU_VERSION_SUFFIX))]
		static public extern Int32
ucol_nextSortKeyPart(SafeRuleBasedCollatorHandle collator,
					 UCharIterator *iter,
					 [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] UInt32[] state,
					 [Out][MarshalAs(UnmanagedType.LPArray)] byte[]       dest,
					 Int32 count,
					 out ErrorCode status);
			*/
			/*
			public enum CollationBoundMode
			{
				/// <summary>lower bound</summary>
				Lower = 0,
				/// <summary>upper bound that will match strings of exact size</summary>
				Upper = 1,
				/// <summary>upper bound that will match all the strings that have the same initial substring as the given string </summary>
				UpperLong = 2,
			}
			*/
/**
 * Produce a bound for a given sortkey and a number of levels.
 * Return value is always the number of bytes needed, regardless of
 * whether the result buffer was big enough or even valid.<br>
 * Resulting bounds can be used to produce a range of strings that are
 * between upper and lower bounds. For example, if bounds are produced
 * for a sortkey of string "smith", strings between upper and lower
 * bounds with one level would include "Smith", "SMITH", "sMiTh".<br>
 * There are two upper bounds that can be produced. If UCOL_BOUND_UPPER
 * is produced, strings matched would be as above. However, if bound
 * produced using UCOL_BOUND_UPPER_LONG is used, the above example will
 * also match "Smithsonian" and similar.<br>
 * For more on usage, see example in cintltst/capitst.c in procedure
 * TestBounds.
 * Sort keys may be compared using <TT>strcmp</TT>.
 * @param source The source sortkey.
 * @param sourceLength The length of source, or -1 if null-terminated.
 *                     (If an unmodified sortkey is passed, it is always null
 *                      terminated).
 * @param boundType Type of bound required. It can be UCOL_BOUND_LOWER, which
 *                  produces a lower inclusive bound, UCOL_BOUND_UPPER, that
 *                  produces upper bound that matches strings of the same length
 *                  or UCOL_BOUND_UPPER_LONG that matches strings that have the
 *                  same starting substring as the source string.
 * @param noOfLevels  Number of levels required in the resulting bound (for most
 *                    uses, the recommended value is 1). See users guide for
 *                    explanation on number of levels a sortkey can have.
 * @param result A pointer to a buffer to receive the resulting sortkey.
 * @param resultLength The maximum size of result.
 * @param status Used for returning error code if something went wrong. If the
 *               number of levels requested is higher than the number of levels
 *               in the source key, a warning (U_SORT_KEY_TOO_SHORT_WARNING) is
 *               issued.
 * @return The size needed to fully store the bound.
 * @see ucol_keyHashCode
 * @stable ICU 2.1
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getBound" + ICU_VERSION_SUFFIX)]
			public static extern Int32
					ucol_getBound([MarshalAs(UnmanagedType.LPArray)] byte[] source,
								  Int32 sourceLength,
								  CollationBoundMode boundType,
								  UInt32 noOfLevels,
								  [Out][MarshalAs(UnmanagedType.LPArray)] byte[] result,
								  Int32 resultLength,
								  out ErrorCode status);
			*/
/**
 * Gets the version information for a Collator. Version is currently
 * an opaque 32-bit number which depends, among other things, on major
 * versions of the collator tailoring and UCA.
 * @param coll The UCollator to query.
 * @param info the version # information, the result will be filled in
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getVersion" + ICU_VERSION_SUFFIX)]
			public static extern void ucol_getVersion(
					SafeRuleBasedCollatorHandle collator,
					out VersionInfo info);
			*/
/**
 * Gets the UCA version information for a Collator. Version is the
 * UCA version number (3.1.1, 4.0).
 * @param coll The UCollator to query.
 * @param info the version # information, the result will be filled in
 * @stable ICU 2.8
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getUCAVersion" + ICU_VERSION_SUFFIX)]
			public static extern void ucol_getUCAVersion(
					SafeRuleBasedCollatorHandle collator,
					out VersionInfo info);
			*/
/**
 * Merge two sort keys. The levels are merged with their corresponding counterparts
 * (primaries with primaries, secondaries with secondaries etc.). Between the values
 * from the same level a separator is inserted.
 * example (uncompressed):
 * 191B1D 01 050505 01 910505 00 and 1F2123 01 050505 01 910505 00
 * will be merged as
 * 191B1D 02 1F212301 050505 02 050505 01 910505 02 910505 00
 * This allows for concatenating of first and last names for sorting, among other things.
 * If the destination buffer is not big enough, the results are undefined.
 * If any of source lengths are zero or any of source pointers are NULL/undefined,
 * result is of size zero.
 * @param src1 pointer to the first sortkey
 * @param src1Length length of the first sortkey
 * @param src2 pointer to the second sortkey
 * @param src2Length length of the second sortkey
 * @param dest buffer to hold the result
 * @param destCapacity size of the buffer for the result
 * @return size of the result. If the buffer is big enough size is always
 *         src1Length+src2Length-1
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_mergeSortkeys" + ICU_VERSION_SUFFIX)]
			public static extern Int32
					ucol_mergeSortkeys([MarshalAs(UnmanagedType.LPArray)] byte[] src1,
									   Int32 src1Length,
									   [MarshalAs(UnmanagedType.LPArray)] byte[] src2,
									   Int32 src2Length,
									   [Out][MarshalAs(UnmanagedType.LPArray)] byte[] dest,
									   Int32 destCapacity);
			*/
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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setAttribute" + ICU_VERSION_SUFFIX)]
			public static extern void ucol_setAttribute(
					SafeRuleBasedCollatorHandle collator,
					CollationAttribute attr,
					CollationAttributeValue value,
					out ErrorCode status);

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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getAttribute" + ICU_VERSION_SUFFIX)]
			public static extern CollationAttributeValue ucol_getAttribute(
					SafeRuleBasedCollatorHandle collator,
					CollationAttribute attr,
					out ErrorCode status);

/** Variable top
 * is a two byte primary value which causes all the codepoints with primary values that
 * are less or equal than the variable top to be shifted when alternate handling is set
 * to UCOL_SHIFTED.
 * Sets the variable top to a collation element value of a string supplied.
 * @param coll collator which variable top needs to be changed
 * @param varTop one or more (if contraction) UChars to which the variable top should be set
 * @param len length of variable top string. If -1 it is considered to be zero terminated.
 * @param status error code. If error code is set, the return value is undefined.
 *               Errors set by this function are: <br>
 *    U_CE_NOT_FOUND_ERROR if more than one character was passed and there is no such
 *    a contraction<br>
 *    U_PRIMARY_TOO_LONG_ERROR if the primary for the variable top has more than two bytes
 * @return a 32 bit value containing the value of the variable top in upper 16 bits.
 *         Lower 16 bits are undefined
 * @see ucol_getVariableTop
 * @see ucol_restoreVariableTop
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setVariableTop" + ICU_VERSION_SUFFIX)]
			public static extern UInt32 ucol_setVariableTop(
					SafeRuleBasedCollatorHandle collator,
					[MarshalAs(UnmanagedType.LPWStr)] string varTop,
					Int32 len,
					out ErrorCode status);
			*/
/**
 * Gets the variable top value of a Collator.
 * Lower 16 bits are undefined and should be ignored.
 * @param coll collator which variable top needs to be retrieved
 * @param status error code (not changed by function). If error code is set,
 *               the return value is undefined.
 * @return the variable top value of a Collator.
 * @see ucol_setVariableTop
 * @see ucol_restoreVariableTop
 * @stable ICU 2.0
 */
/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getVariableTop" + ICU_VERSION_SUFFIX)]
			public static extern UInt32 ucol_getVariableTop(
					SafeRuleBasedCollatorHandle collator,
					out ErrorCode status);
			*/
/**
 * Sets the variable top to a collation element value supplied. Variable top is
 * set to the upper 16 bits.
 * Lower 16 bits are ignored.
 * @param coll collator which variable top needs to be changed
 * @param varTop CE value, as returned by ucol_setVariableTop or ucol)getVariableTop
 * @param status error code (not changed by function)
 * @see ucol_getVariableTop
 * @see ucol_setVariableTop
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_restoreVariableTop" + ICU_VERSION_SUFFIX)]
			public static extern void ucol_restoreVariableTop(
					SafeRuleBasedCollatorHandle collator,
					UInt32 varTop,
					out ErrorCode status);
			*/
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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_safeClone" + ICU_VERSION_SUFFIX)]
			public static extern SafeRuleBasedCollatorHandle ucol_safeClone(SafeRuleBasedCollatorHandle collator,
																			IntPtr stackBuffer,
																			ref Int32 pBufferSize,
																			out ErrorCode status);

/**
 * Returns current rules. Delta defines whether full rules are returned or just the tailoring.
 * Returns number of UChars needed to store rules. If buffer is NULL or bufferLen is not enough
 * to store rules, will store up to available space.
 * @param coll collator to get the rules from
 * @param delta one of UCOL_TAILORING_ONLY, UCOL_FULL_RULES.
 * @param buffer buffer to store the result in. If NULL, you'll get no rules.
 * @param bufferLen lenght of buffer to store rules in. If less then needed you'll get only the part that fits in.
 * @return current rules
 * @stable ICU 2.0
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getRulesEx" + ICU_VERSION_SUFFIX)]
			public static extern Int32 ucol_getRulesEx(
					SafeRuleBasedCollatorHandle collator,
					CollationRuleOption delta,
					[MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer,
					Int32 bufferLen);
			*/
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

			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getLocaleByType" + ICU_VERSION_SUFFIX)]
			public static extern IntPtr
					ucol_getLocaleByType(SafeRuleBasedCollatorHandle collator, LocaleType type,
										 out ErrorCode status);

/**
 * Get an Unicode set that contains all the characters and sequences tailored in
 * this collator. The result must be disposed of by using uset_close.
 * @param coll        The UCollator for which we want to get tailored chars
 * @param status      error code of the operation
 * @return a pointer to newly created USet. Must be be disposed by using uset_close
 * @see ucol_openRules
 * @see uset_close
 * @stable ICU 2.4
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getTailoredSet" + ICU_VERSION_SUFFIX)]
			public static extern IntPtr
					ucol_getTailoredSet(SafeRuleBasedCollatorHandle collator, out ErrorCode status);
			*/

/** Creates a binary image of a collator. This binary image can be stored and
 *  later used to instantiate a collator using ucol_openBinary.
 *  This API supports preflighting.
 *  @param coll Collator
 *  @param buffer a fill-in buffer to receive the binary image
 *  @param capacity capacity of the destination buffer
 *  @param status for catching errors
 *  @return size of the image
 *  @see ucol_openBinary
 *  @stable ICU 3.2
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_cloneBinary" + ICU_VERSION_SUFFIX)]
			public static extern Int32
					ucol_cloneBinary(SafeRuleBasedCollatorHandle collator,
									 [MarshalAs(UnmanagedType.U1)] out int buffer, Int32 capacity,
									 out ErrorCode status);
			*/
/** Opens a collator from a collator binary image created using
 *  ucol_cloneBinary. Binary image used in instantiation of the
 *  collator remains owned by the user and should stay around for
 *  the lifetime of the collator. The API also takes a base collator
 *  which usualy should be UCA.
 *  @param bin binary image owned by the user and required through the
 *             lifetime of the collator
 *  @param length size of the image. If negative, the API will try to
 *                figure out the length of the image
 *  @param base fallback collator, usually UCA. Base is required to be
 *              present through the lifetime of the collator. Currently
 *              it cannot be NULL.
 *  @param status for catching errors
 *  @return newly created collator
 *  @see ucol_cloneBinary
 *  @stable ICU 3.2
 */
			/*
			[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openBinary" + ICU_VERSION_SUFFIX)]
			public static extern SafeRuleBasedCollatorHandle
					ucol_openBinary(byte[] bin, Int32 length,
									SafeRuleBasedCollatorHandle baseCollator,
									out ErrorCode status);
			*/

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
			public enum  LocaleType {
				/// <summary>
				/// This is locale the data actually comes from
				/// </summary>
				ActualLocale    = 0,
				/// <summary>
				/// This is the most specific locale supported by ICU
				/// </summary>
				ValidLocale    = 1,
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

				Off = 16,
				//Turn the feature off - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode
				On = 17,
				//Turn the feature on - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode

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
		}
	}
}
