// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu
{
	/// <summary>
	/// UnicodeString is a string class that stores Unicode characters directly
	/// and provides similar functionality as the Java String and
	/// StringBuffer/StringBuilder classes.
	/// </summary>
	public static class UnicodeString
	{

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Convert the string to lower case, using the convention of the specified locale.
		/// This may be null for the universal locale, or "" for a 'root' locale (whatever that means).
		/// </summary>
		/// <param name="src"></param>
		/// <param name="locale"></param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static string ToLower(string src, Locale locale)
		{
			return ToLower(src, locale.Id);
		}

		/// <summary>
		/// Convert the characters in <paramref name="src"/> to lower case
		/// following the conventions of a specific locale.
		/// </summary>
		/// <param name="src">String to convert.</param>
		/// <param name="locale">The locale to consider, or "" for the root
		/// locale or NULL for the default locale.</param>
		public static string ToLower(string src, string locale)
		{
			return GetString(NativeMethods.u_strToLower, src, locale);
		}

		/// <summary>
		/// Convert the string to upper case, using the convention of the specified locale.
		/// This may be null for the universal locale, or "" for a 'root' locale (whatever that means).
		/// </summary>
		/// <param name="src">String to convert.</param>
		/// <param name="locale">The locale containing the conventions to use.</param>
		public static string ToUpper(string src, Locale locale)
		{
			return ToUpper(src, locale.Id);
		}

		/// <summary>
		/// Convert the characters in <paramref name="src"/> to UPPER CASE
		/// following the conventions of a specific locale.
		/// </summary>
		/// <param name="src">String to convert.</param>
		/// <param name="locale">The locale to consider, or "" for the root
		/// locale or NULL for the default locale.</param>
		public static string ToUpper(string src, string locale)
		{
			return GetString(NativeMethods.u_strToUpper, src, locale);
		}

		/// <summary>
		/// Convert the string to title case, using the convention of the specified locale.
		/// This may be null for the universal locale, or "" for a 'root' locale (whatever that means).
		/// </summary>
		/// <param name="src"></param>
		/// <param name="locale"></param>
		/// <returns></returns>
		public static string ToTitle(string src, Locale locale)
		{
			return ToTitle(src, locale.Id);
		}

		/// <summary>
		/// Titlecase a string. Casing is locale-dependent and context-sensitive.
		/// Titlecasing uses a break iterator to find the first characters of
		/// words that are to be titlecased. It titlecases those characters and
		/// lowercases all others.
		/// </summary>
		/// <param name="src">The original string</param>
		/// <param name="locale">The locale to consider, or "" for the root
		/// locale or NULL for the default locale.</param>
		/// <returns></returns>
		public static string ToTitle(string src, string locale)
		{
			return GetString(NativeMethods.u_strToTitle, src, locale);
		}

		/// <summary>
		/// Titlecase a string and then normalizes the string using the given
		/// normalization mode.
		/// </summary>
		/// <param name="src">The original string.</param>
		/// <param name="locale">The locale to consider, or "" for the root
		/// locale or NULL for the default locale.</param>
		/// <param name="normMode">Normalization mode to apply to titlecased string.</param>
		/// <returns></returns>
		public static string ToTitle(string src, string locale, Normalizer.UNormalizationMode normMode)
		{
			src = Normalizer.Normalize(src, Normalizer.UNormalizationMode.UNORM_NFC);
			var result = ToTitle(src, locale);

			// The dotted I in Turkish and other characters like it are not handled properly unless we are NFC, since
			// by default ICU only looks at the first character.
			return Normalizer.Normalize(result, normMode);
		}

		private delegate int GetStringMethod(IntPtr dest, int destCapacity, string src,
			int srcLength, string locale, out ErrorCode errorCode);

		private static string GetString(GetStringMethod method, string src, string locale)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
				{
					length = method(ptr, length, src, src.Length, locale, out var err);
					return new Tuple<ErrorCode, int>(err, length);
				}, src.Length + 10);
		}

	}
}
