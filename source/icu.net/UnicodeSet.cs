// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// A mutable set of Unicode characters and multicharacter strings.
	/// </summary>
	public static class UnicodeSet
	{
		/// <summary>
		/// Returns a string representation of this Unicode set
		/// </summary>
		/// <param name="set">Unicode set to convert.  Null set throws an exception</param>
		/// <returns>pattern string</returns>
		public static string ToPattern(IEnumerable<string> set)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}
			// uset_openEmpty unavailable, so this is equivalent
			IntPtr uset = NativeMethods.uset_open('1', '0');
			try
			{
				foreach (string str in set)
				{
					if (!string.IsNullOrEmpty(str))
					{
						if (str.Length == 1)
							NativeMethods.uset_add(uset, str.First());
						else
							NativeMethods.uset_addString(uset, str, str.Length);
					}
				}

				return NativeMethods.GetUnicodeString((ptr, length) =>
					{
						length = NativeMethods.uset_toPattern(uset, ptr, length, true, out var err);
						return new Tuple<ErrorCode, int>(err, length);
					});
			}
			finally
			{
				NativeMethods.uset_close(uset);
			}
		}

		/// <summary>
		/// Creates a Unicode set from the given pattern
		/// </summary>
		/// <param name="pattern">A string specifying what characters are in the set.  Null pattern returns an empty set</param>
		/// <returns>Unicode set of characters.</returns>
		public static IEnumerable<string> ToCharacters(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
			{
				return Enumerable.Empty<string>();
			}

			var uset = NativeMethods.uset_openPattern(pattern, -1, out var err);
			try
			{
				if (err.IsFailure())
					throw new ArgumentException(nameof(pattern));

				var output = new List<string>();

				// Parse the number of items in the Unicode set
				var itemCount = NativeMethods.uset_getItemCount(uset);
				for (var i = 0; i < itemCount; i++)
				{
					var strLength = NativeMethods.uset_getItem(uset, i, out var startChar, out var endChar, IntPtr.Zero, 0, out err);

					if (strLength == 0 && err.IsSuccess())
					{
						// Add a character range to the set
						for (var j = startChar; j <= endChar; j++)
						{
							output.Add(char.ConvertFromUtf32(j));
						}
					}
					else
					{
						// Add a multiple-character string to the set
						var index = i;
						output.Add(NativeMethods.GetUnicodeString((ptr, length) =>
							{
								length = NativeMethods.uset_getItem(uset, index, out startChar,
									out endChar, ptr, length, out var errorCode);
								return new Tuple<ErrorCode, int>(errorCode, length);
							}, strLength * 2));
					}
				}
				return output;
			}
			finally
			{
				NativeMethods.uset_close(uset);
			}
		}
	}
}
