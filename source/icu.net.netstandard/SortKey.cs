// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;

namespace Icu
{
	/// <summary>
	/// Replacement for System.Globalization.SortKey, which does not exist in
	/// .NET Standard 1.5. Will be brought back in .NET Standard 2.0.
	/// See https://github.com/dotnet/corefx/issues/10065 for more information.
	/// </summary>
	public class SortKey
	{
		private readonly string localeName;
		private readonly CompareOptions options;
		private readonly byte[] m_KeyData;
		private readonly string m_String;

		internal SortKey(string localeName, string str, CompareOptions options, byte[] keyData)
		{
			var copy = new byte[keyData.Length];
			keyData.CopyTo(copy, 0);

			this.m_KeyData = copy;
			this.localeName = localeName;
			this.options = options;
			this.m_String = str;
		}

		/// <summary>
		/// Gets the byte array representing the current System.Globalization.SortKey object.
		/// </summary>
		public virtual byte[] KeyData
		{
			get
			{
				var copy = new byte[m_KeyData.Length];
				m_KeyData.CopyTo(copy, 0);

				return copy;
			}
		}

		/// <summary>
		/// Gets the original string used to create the current System.Globalization.SortKey
		/// object.
		/// </summary>
		public virtual string OriginalString { get { return m_String; } }

		/// <summary>
		/// Compares two sort keys.
		/// </summary>
		/// <param name="sortkey1">The first sort key to compare.</param>
		/// <param name="sortkey2">The second sort key to compare.</param>
		/// <returns>A signed integer that indicates the relationship between sortkey1 and sortkey2.
		/// Value
		/// Condition Less than zero:	sortkey1 is less than sortkey2.
		/// Zero					:	sortkey1 is equal to sortkey2.
		/// Greater than zero		:	sortkey1 is greater than sortkey2.
		///</returns>
		public static int Compare(SortKey sortkey1, SortKey sortkey2)
		{
			if (sortkey1 == null || sortkey2 == null)
			{
				throw new ArgumentNullException("A value is required to compare both values");
			}

			var keyData1 = sortkey1.KeyData;
			var keyData2 = sortkey2.KeyData;

			if (keyData1.Length == 0)
			{
				return keyData2.Length == 0 ? 0 : -1;
			}

			if (keyData2.Length == 0)
			{
				return keyData1.Length == 0 ? 0 : 1;
			}

			var length = Math.Min(keyData1.Length, keyData2.Length);

			for (int i = 0; i < length; i++)
			{
				var value = keyData1[i];
				var value2 = keyData2[i];

				if (value > value2)
				{
					return 1;
				}

				if (value < value2)
				{
					return -1;
				}
			}

			return 0;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current
		/// System.Globalization.SortKey object.
		/// </summary>
		/// <param name="value">The object to compare with the current
		/// System.Globalization.SortKey object.</param>
		/// <returns>true if the value parameter is equal to the current
		/// System.Globalization.SortKey object; otherwise, false.</returns>
		public override bool Equals(object value)
		{
			var obj = value as SortKey;

			if (obj == null)
				return false;

			return Compare(this, obj) == 0;
		}

		/// <summary>
		/// Serves as a hash function for the current System.Globalization.SortKey object
		/// that is suitable for hashing algorithms and data structures such as a hash table.
		/// </summary>
		/// <returns>A hash code for the current System.Globalization.SortKey object.</returns>
		public override int GetHashCode()
		{
			return CompareInfo.GetCompareInfo(localeName).GetHashCode(m_String, options);
		}

		/// <summary>
		/// Returns a string that represents the current System.Globalization.SortKey object.
		/// </summary>
		/// <returns>A string that represents the current System.Globalization.SortKey object.</returns>
		public override string ToString()
		{
			return string.Format("SortKey - {0}, {1}, {3}", localeName, options, OriginalString);
		}
	}
}
