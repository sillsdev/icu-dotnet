using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Icu.Collation
{
	public abstract class Collator : IComparer<string>, ICloneable
	{
		public abstract CollationStrength Strength{get;set;}
		public abstract NormalizationMode NormalizationMode{get;set;}
		public abstract FrenchCollation FrenchCollation{get;set;}
		public abstract CaseLevel CaseLevel{get;set;}
		public abstract HiraganaQuaternary HiraganaQuaternary{get;set;}
		public abstract NumericCollation NumericCollation{get;set;}
		public abstract CaseFirst CaseFirst{get;set;}
		public abstract AlternateHandling AlternateHandling{get;set;}
		public abstract SortKey GetSortKey(string source);
		public abstract int Compare(string source, string target);
		public abstract object Clone();

		public enum Fallback
		{
			NoFallback,
			FallbackAllowed
		}

		public static Collator Create()
		{
			return Create(CultureInfo.CurrentCulture);
		}

		public static Collator Create(string localeId)
		{
			return Create(localeId, Fallback.NoFallback);
		}

		public static Collator Create(string localeId, Fallback fallback)
		{
			if (localeId == null)
			{
				throw new ArgumentNullException();
			}
			return RuleBasedCollator.Create(localeId, fallback);
		}

		public static Collator Create(CultureInfo cultureInfo)
		{
			return Create(cultureInfo, Fallback.NoFallback);
		}

		public static Collator Create(CultureInfo cultureInfo, Fallback fallback)
		{
			if (cultureInfo == null)
			{
				throw new ArgumentNullException();
			}
			return Create(cultureInfo.IetfLanguageTag, fallback);
		}

		static public SortKey CreateSortKey(string originalString, byte[] keyData)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			return CreateSortKey(originalString, keyData, keyData.Length);
		}

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
			if(0 > keyDataLength || keyDataLength > keyData.Length)
			{
				throw new ArgumentOutOfRangeException("keyDataLength");
			}

			SortKey sortKey = CultureInfo.InvariantCulture.CompareInfo.GetSortKey(string.Empty);
			FieldInfo originalStringField =
					sortKey.GetType().GetField("m_String", BindingFlags.Instance | BindingFlags.NonPublic);
			originalStringField.SetValue(sortKey, originalString);

			byte[] keyDataCopy = new byte[keyDataLength];
			Array.Copy(keyData, keyDataCopy, keyDataLength);

			FieldInfo keyDataField =
					sortKey.GetType().GetField("m_KeyData", BindingFlags.Instance | BindingFlags.NonPublic);
			keyDataField.SetValue(sortKey, keyDataCopy);
			return sortKey;
		}

	}
}
