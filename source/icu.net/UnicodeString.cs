// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// UnicodeString is a string class that stores Unicode characters directly
	/// and provides similar functionality as the Java String and
	/// StringBuffer/StringBuilder classes.
	/// </summary>
	public class UnicodeString
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
			if (src == null)
				return string.Empty;

			int length = src.Length + 10;
			IntPtr resPtr = Marshal.AllocCoTaskMem(length * 2);
			try
			{
				ErrorCode err;
				int outLength = NativeMethods.u_strToLower(resPtr, length, src, src.Length, locale, out err);
				if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
					throw new Exception("UnicodeString.ToLower() failed with code " + err);
				if (outLength > length)
				{
					Marshal.FreeCoTaskMem(resPtr);
					length = outLength + 1;
					resPtr = Marshal.AllocCoTaskMem(length * 2);
					NativeMethods.u_strToLower(resPtr, length, src, src.Length, locale, out err);
				}
				if (err > 0)
					throw new Exception("UnicodeString.ToLower() failed with code " + err);

				string result = Marshal.PtrToStringUni(resPtr);

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
			if (src == null)
				return string.Empty;

			int length = src.Length + 10;
			IntPtr resPtr = Marshal.AllocCoTaskMem(length * 2);
			try
			{
				ErrorCode err;
				int outLength = NativeMethods.u_strToUpper(resPtr, length, src, src.Length, locale, out err);
				if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
					throw new Exception("UnicodeString.ToUpper() failed with code " + err);
				if (outLength > length)
				{
					err = ErrorCode.NoErrors; // ignore possible U_BUFFER_OVERFLOW_ERROR
					Marshal.FreeCoTaskMem(resPtr);
					length = outLength + 1;
					resPtr = Marshal.AllocCoTaskMem(length * 2);
					NativeMethods.u_strToUpper(resPtr, length, src, src.Length, locale, out err);
				}
				if (err > 0)
					throw new Exception("UnicodeString.ToUpper() failed with code " + err);

				string result = Marshal.PtrToStringUni(resPtr);
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
			if (src == null)
				return string.Empty;

			int length = src.Length + 10;
			IntPtr resPtr = Marshal.AllocCoTaskMem(length * 2);
			try
			{
				ErrorCode err;
				int outLength = NativeMethods.u_strToTitle(resPtr, length, src, src.Length, IntPtr.Zero, locale, out err);
				if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
					throw new Exception("UnicodeString.ToTitle() failed with code " + err);
				if (outLength > length)
				{
					err = ErrorCode.NoErrors; // ignore possible U_BUFFER_OVERFLOW_ERROR
					Marshal.FreeCoTaskMem(resPtr);
					length = outLength + 1;
					resPtr = Marshal.AllocCoTaskMem(length * 2);
					NativeMethods.u_strToTitle(resPtr, length, src, src.Length, IntPtr.Zero, locale, out err);
				}
				if (err > 0)
					throw new Exception("UnicodeString.ToTitle() failed with code " + err);

				string result = Marshal.PtrToStringUni(resPtr);
				if (result != null)
				{
					// Strip any garbage left over at the end of the string.
					if (err == ErrorCode.STRING_NOT_TERMINATED_WARNING)
						result = result.Substring(0, outLength);
				}
				return result;
			}
			finally
			{
				Marshal.FreeCoTaskMem(resPtr);
			}
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

	}
}
