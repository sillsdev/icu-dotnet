// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Icu.Collation
{
	/// <summary>
	/// The Collator class performs locale-sensitive string comparison.
	/// You use this class to build searching and sorting routines for natural
	/// language text.
	/// </summary>
	public abstract class Collator : IComparer<string>, IDisposable
#if FEATURE_ICLONEABLE
		, ICloneable
#endif
	{
		/// <summary>
		/// Gets or sets the minimum strength that will be used in comparison
		/// or transformation.
		/// </summary>
		public abstract CollationStrength Strength{ get; set; }

		/// <summary>
		/// Gets or sets the NormalizationMode
		/// </summary>
		public abstract NormalizationMode NormalizationMode{ get; set; }

		/// <summary>
		/// Gets or sets the FrenchCollation. Attribute for direction of
		/// secondary weights - used in Canadian French.
		/// </summary>
		public abstract FrenchCollation FrenchCollation{ get; set; }

		/// <summary>
		/// Gets or sets the CaseLevel mode. Controls whether an extra case
		/// level (positioned before the third level) is generated or not.
		/// </summary>
		/// <remarks></remarks>
		public abstract CaseLevel CaseLevel{ get; set; }

		/// <summary>
		/// Gets or sets HiraganaQuaternary mode. When turned on, this attribute
		/// positions Hiragana before all non-ignorables on quaternary level
		/// This is a sneaky way to produce JIS sort order.
		/// </summary>
		[Obsolete("ICU 50 Implementation detail, cannot be set via API, was removed from implementation.")]
		public abstract HiraganaQuaternary HiraganaQuaternary{ get; set; }

		/// <summary>
		/// Gets or sets NumericCollation mode. When turned on, this attribute
		/// makes substrings of digits sort according to their numeric values.
		/// </summary>
		public abstract NumericCollation NumericCollation{ get; set; }

		/// <summary>
		/// Controls the ordering of upper and lower case letters.
		/// </summary>
		public abstract CaseFirst CaseFirst{ get; set; }

		/// <summary>
		/// Gets or sets attribute for handling variable elements.
		/// </summary>
		public abstract AlternateHandling AlternateHandling{ get; set; }

		/// <summary>
		/// Get a sort key for a string from the collator.
		/// </summary>
		/// <param name="source">The text</param>
		public abstract SortKey GetSortKey(string source);

		/// <summary>
		/// Compare two strings.
		/// </summary>
		/// <param name="source">The source string</param>
		/// <param name="target">The target string</param>
		/// <returns>The result of comparing the strings; 0 if source == target;
		/// 1 if source &gt; target; -1 source &lt; target</returns>
		public abstract int Compare(string source, string target);

		/// <summary>
		/// Thread safe cloning operation.
		/// </summary>
		/// <returns>The result is a clone of a given collator.</returns>
		public abstract object Clone();

		/// <summary>
		/// Specifies whether locale fallback is allowed.
		/// For more information, see: http://userguide.icu-project.org/locale#TOC-Fallback
		/// </summary>
		public enum Fallback
		{
			/// <summary>Do not use Locale fallback</summary>
			NoFallback,
			/// <summary>Use locale fallback algorithm</summary>
			FallbackAllowed
		}

		/// <summary>
		/// Creates a collator with the current culture.  Does not allow
		/// locale fallback.
		/// </summary>
		public static Collator Create()
		{
			return Create(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Creates a collator with the specified locale. Does not allow
		/// locale fallback.
		/// </summary>
		/// <param name="localeId">Locale to use</param>
		public static Collator Create(string localeId)
		{
			return Create(localeId, Fallback.NoFallback);
		}

		/// <summary>
		/// Creates a collator with the given locale and whether to use locale
		/// fallback when creating collator.
		/// </summary>
		/// <param name="localeId">The locale</param>
		/// <param name="fallback">Whether to use locale fallback or not.</param>
		public static Collator Create(string localeId, Fallback fallback)
		{
			if (localeId == null)
			{
				throw new ArgumentNullException();
			}
			return RuleBasedCollator.Create(localeId, fallback);
		}

		/// <summary>
		/// Creates a collator with the given CultureInfo
		/// </summary>
		/// <param name="cultureInfo">Culture to use.</param>
		public static Collator Create(CultureInfo cultureInfo)
		{
			return Create(cultureInfo, Fallback.NoFallback);
		}

		/// <summary>
		/// Creates a collator with the given CultureInfo and fallback
		/// </summary>
		/// <param name="cultureInfo">Culture to use</param>
		/// <param name="fallback">Whether to use locale fallback or not.</param>
		public static Collator Create(CultureInfo cultureInfo, Fallback fallback)
		{
			if (cultureInfo == null)
			{
				throw new ArgumentNullException();
			}
			return Create(cultureInfo.Name, fallback);
		}

		/// <summary>
		/// Creates a SortKey with the given string and keyData
		/// </summary>
		/// <param name="originalString">String to use</param>
		/// <param name="keyData">Data of the SortKey</param>
		static public SortKey CreateSortKey(string originalString, byte[] keyData)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			return CreateSortKey(originalString, keyData, keyData.Length);
		}

		/// <summary>
		/// Creates a SortKey with the given string and keyData
		/// </summary>
		/// <param name="originalString">String to use</param>
		/// <param name="keyData">Data of the SortKey</param>
		/// <param name="keyDataLength">Length to use from keyData</param>
		static public SortKey CreateSortKey(string originalString, byte[] keyData, int keyDataLength)
		{
			if (originalString == null)
			{
				throw new ArgumentNullException("originalString");
			}
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (0 > keyDataLength || keyDataLength > keyData.Length)
			{
				throw new ArgumentOutOfRangeException("keyDataLength");
			}

            CompareOptions options = CompareOptions.None;

#if NETSTANDARD1_6
            SortKey sortKey = new SortKey(CultureInfo.InvariantCulture.Name, originalString, options, keyData);
#else
            SortKey sortKey = CultureInfo.InvariantCulture.CompareInfo.GetSortKey(string.Empty, options);
            SetInternalOriginalStringField(sortKey, originalString);
			SetInternalKeyDataField(sortKey, keyData, keyDataLength);
#endif

            return sortKey;
		}

#if !NETSTANDARD1_6
        private static void SetInternalKeyDataField(SortKey sortKey, byte[] keyData, int keyDataLength)
		{
			byte[] keyDataCopy = new byte[keyDataLength];
			Array.Copy(keyData, keyDataCopy, keyDataLength);

			string propertyName = "SortKey.KeyData";
			string monoInternalFieldName = "key";
			string netInternalFieldName = "m_KeyData";
			SetInternalFieldForPublicProperty(sortKey,
											  propertyName,
											  netInternalFieldName,
											  monoInternalFieldName,
											  keyDataCopy);

		}

		private static void SetInternalOriginalStringField(SortKey sortKey, string originalString)
		{
			string propertyName = "SortKey.OriginalString";
			string monoInternalFieldName = "source";
			string netInternalFieldName = "m_String";
			SetInternalFieldForPublicProperty(sortKey,
											  propertyName,
											  netInternalFieldName,
											  monoInternalFieldName,
											  originalString);
		}

		private static void SetInternalFieldForPublicProperty<T,P>(
			T instance,
			string propertyName,
			string netInternalFieldName,
			string monoInternalFieldName,
			P value)
		{
			Type type = instance.GetType();

			string fieldName = IsRunningOnMono() ? monoInternalFieldName : netInternalFieldName;

			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

			Debug.Assert(fieldInfo != null,
						 "Unsupported runtime",
						 "Could not figure out an internal field for" + propertyName);

			if (fieldInfo == null)
			{
				throw new NotImplementedException("Not implemented for this runtime");
			}

			fieldInfo.SetValue(instance, value);
		}
        
		private static bool IsRunningOnMono()
		{
			return Type.GetType("Mono.Runtime") != null;
		}
#endif

        /// <summary>
        /// Simple class to allow passing collation error info back to the caller of CheckRules.
        /// </summary>
        public class CollationRuleErrorInfo
		{
			/// <summary>Line number (1-based) containing the error</summary>
			public int Line;
			/// <summary>Character offset (1-based) on Line where the error was detected</summary>
			public int Offset;
			/// <summary>Characters preceding the the error</summary>
			public String PreContext;
			/// <summary>Characters following the the error</summary>
			public String PostContext;
		}

		// REVIEW: We might want to integrate the methods below in a better way.

		/// <summary>
		/// Test collation rules and return an object with error information if it fails.
		/// </summary>
		/// <param name="rules">String containing the collation rules to check</param>
		/// <returns>A CollationRuleErrorInfo object with error information; or <c>null</c> if
		/// no errors are found.</returns>
		public static CollationRuleErrorInfo CheckRules(string rules)
		{
			if (rules == null)
				return null;

			ErrorCode err;
			var parseError = new ParseError();
			using (NativeMethods.ucol_openRules(rules, rules.Length, NormalizationMode.Default,
				CollationStrength.Default, ref parseError, out err))
			{
				if (err == ErrorCode.NoErrors)
					return null;

				return new CollationRuleErrorInfo
				{
					Line = parseError.Line + 1,
					Offset = parseError.Offset + 1,
					PreContext = parseError.PreContext,
					PostContext = parseError.PostContext
				};
			}
		}

		/// <summary>
		/// Gets the collation rules for the specified locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="collatorRuleOption">UColRuleOption to use. Default is UColRuleOption.UCOL_TAILORING_ONLY</param>
		public static string GetCollationRules(string locale, UColRuleOption collatorRuleOption = UColRuleOption.UCOL_TAILORING_ONLY)
		{
			return GetCollationRules(new Locale(locale), collatorRuleOption);
		}

		/// <summary>
		/// Gets the collation rules for the specified locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="collatorRuleOption">UColRuleOption to use. Default is UColRuleOption.UCOL_TAILORING_ONLY</param>
		public static string GetCollationRules(Locale locale, UColRuleOption collatorRuleOption = UColRuleOption.UCOL_TAILORING_ONLY)
		{
			ErrorCode err;
			using (RuleBasedCollator.SafeRuleBasedCollatorHandle coll = NativeMethods.ucol_open(locale.Id, out err))
			{
				if (coll.IsInvalid || err.IsFailure())
					return null;

				const int len = 1000;
				IntPtr buffer = Marshal.AllocCoTaskMem(len * 2);
				try
				{
					int actualLen = NativeMethods.ucol_getRulesEx(coll, collatorRuleOption, buffer, len);
					if (actualLen > len)
					{
						Marshal.FreeCoTaskMem(buffer);
						buffer = Marshal.AllocCoTaskMem(actualLen * 2);
						NativeMethods.ucol_getRulesEx(coll, collatorRuleOption, buffer, actualLen);
					}
					return Marshal.PtrToStringUni(buffer, actualLen);
				}
				finally
				{
					Marshal.FreeCoTaskMem(buffer);
				}
			}
		}

		/// <summary>
		/// Produces a bound for a given sort key.
		/// </summary>
		/// <param name="sortKey">The sort key.</param>
		/// <param name="boundType">Type of the bound.</param>
		/// <param name="result">The result.</param>
		public static void GetSortKeyBound(byte[] sortKey, UColBoundMode boundType, ref byte[] result)
		{
			ErrorCode err;
			int size = NativeMethods.ucol_getBound(sortKey, sortKey.Length, boundType, 1, result, result.Length, out err);
			if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
				throw new Exception("Collator.GetSortKeyBound() failed with code " + err);
			if (size > result.Length)
			{
				result = new byte[size + 1];
				NativeMethods.ucol_getBound(sortKey, sortKey.Length, boundType, 1, result, result.Length, out err);
				if (err != ErrorCode.NoErrors)
					throw new Exception("Collator.GetSortKeyBound() failed with code " + err);
			}
		}

		#region IDisposable Support

		/// <summary>
		/// Dispose of managed/unmanaged resources.
		/// Allow any inheriting classes to dispose of manage
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by Collator.
		/// </summary>
		/// <param name="disposing">true to release managed and unmanaged
		/// resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing) { }

		#endregion
	}
}
