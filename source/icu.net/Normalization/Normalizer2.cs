// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;

namespace Icu.Normalization
{
	/// <summary>
	/// Unicode normalization functionality for standard Unicode normalization or for using
	/// custom mapping tables.
	/// </summary>
	public class Normalizer2
	{
		/// <summary>
		/// Constants for normalization modes.
		/// </summary>
		/// <remarks>
		/// For details about standard Unicode normalization forms and about the algorithms which
		/// are also used with custom mapping tables
		/// <see href="http: //www.unicode.org/unicode/reports/tr15/"/>
		/// </remarks>
		public enum Mode
		{
			/// <summary>
			/// Decomposition followed by composition.
			/// Same as standard NFC when using an "nfc" instance. Same as standard NFKC when
			/// using an "nfkc" instance.
			/// </summary>
			COMPOSE = 0,

			/// <summary>
			/// Map, and reorder canonically.
			/// Same as standard NFD when using an "nfc" instance. Same as standard NFKD when
			/// using an "nfkc" instance.
			/// </summary>
			DECOMPOSE = 1,

			/// <summary>
			/// "Fast C or D" form.
			/// If a string is in this form, then further decomposition without reordering would
			/// yield the same form as DECOMPOSE. Text in "Fast C or D" form can be processed
			/// efficiently with data tables that are "canonically closed", that is, that provide
			/// equivalent data for equivalent text, without having to be fully normalized. Not a
			/// standard Unicode normalization form. Not a unique form: Different FCD strings can
			/// be canonically equivalent.
			/// </summary>
			FCD = 2,

			/// <summary>
			/// Compose only contiguously.
			/// Also known as "FCC" or "Fast C Contiguous". The result will often but not always
			/// be in NFC. The result will conform to FCD which is useful for processing. Not a
			/// standard Unicode normalization form.
			/// </summary>
			COMPOSE_CONTIGUOUS = 3
		}

		private readonly IntPtr _Normalizer;

		/// <summary>
		/// Returns a Normalizer2 instance for Unicode NFC normalization. Same as
		/// GetInstance(null, "nfc", Mode.COMPOSE).
		/// </summary>
		/// <returns>the requested Normalizer2, if successful</returns>
		public static Normalizer2 GetNFCInstance()
		{
			return GetInstance(null, "nfc", Mode.COMPOSE);
		}

		/// <summary>
		/// Returns a Normalizer2 instance for Unicode NFD normalization. Same as
		/// GetInstance(null, "nfc", Mode.DECOMPOSE).
		/// </summary>
		/// <returns>the requested Normalizer2, if successful</returns>
		public static Normalizer2 GetNFDInstance()
		{
			return GetInstance(null, "nfc", Mode.DECOMPOSE);
		}

		/// <summary>
		/// Returns a Normalizer2 instance for Unicode NFKC normalization. Same as
		/// GetInstance(null, "nfkc", Mode.COMPOSE).
		/// </summary>
		/// <returns>the requested Normalizer2, if successful</returns>
		public static Normalizer2 GetNFKCInstance()
		{
			return GetInstance(null, "nfkc", Mode.COMPOSE);
		}

		/// <summary>
		/// Returns a Normalizer2 instance for Unicode NFKD normalization. Same as
		/// GetInstance(null, "nfkc", Mode.DECOMPOSE).
		/// </summary>
		/// <returns>the requested Normalizer2, if successful</returns>
		public static Normalizer2 GetNFKDInstance()
		{
			return GetInstance(null, "nfkc", Mode.DECOMPOSE);
		}

		/// <summary>
		/// Returns a Normalizer2 instance for Unicode NFKC_Casefold normalization. Same as
		/// GetInstance(null, "nfkc_cf", Mode.COMPOSE).
		/// </summary>
		/// <returns>the requested Normalizer2, if successful</returns>
		public static Normalizer2 GetNFKCCasefoldInstance()
		{
			return GetInstance(null, "nfkc_cf", Mode.COMPOSE);
		}

		/// <summary>
		/// Returns a Normalizer instance which uses the specified data file and which composes or
		/// decomposes text according to the specified mode.
		/// Use <paramref name="packageName"/>=<c>null</c> for data files that are part of ICU's
		/// own data.
		/// Use <paramref name="name"/> = "nfc" and COMPOSE/DECOMPOSE for Unicode
		/// standard NFC/NFD.
		/// Use <paramref name="name"/> = "nfkc" and COMPOSE/DECOMPOSE for Unicode
		/// standard NFKC/NFKD.
		/// Use <paramref name="name"/> = "nfkc_cf" and COMPOSE for Unicode standard
		/// NFKC_CF=NFKC_Casefold.
		/// </summary>
		/// <param name="packageName"><c>null</c> for ICU built-in data, otherwise application
		/// data package name</param>
		/// <param name="name">"nfc" or "nfkc" or "nfkc_cf" or name of custom data file</param>
		/// <param name="mode">normalization mode (compose or decompose etc.)</param>
		/// <returns>the requested Normalizer, if successful</returns>
		public static Normalizer2 GetInstance(string packageName, string name, Mode mode)
		{
			return new Normalizer2(packageName, name, mode);
		}

		private Normalizer2(string packageName, string name, Mode mode)
		{
			_Normalizer = NativeMethods.unorm2_getInstance(packageName, name, mode, out var error);
			ExceptionFromErrorCode.ThrowIfError(error);
		}

		/// <summary>
		/// Tests if the character always has a normalization boundary after it, regardless of
		/// context.
		/// If <c>true</c>, then the character does not normalization-interact with following
		/// characters. In other words, a string containing this character can be normalized by
		/// processing portions up to this character and after this character independently. This
		/// is used for iterative normalization. Note that this operation may be significantly
		/// slower than <see cref="hasBoundaryBefore"/>.
		/// </summary>
		/// <param name="codePoint">character to test</param>
		/// <returns><c>true</c> if c has a normalization boundary after it</returns>
		public bool HasBoundaryAfter(int codePoint)
		{
			return NativeMethods.unorm2_hasBoundaryAfter(_Normalizer, codePoint);
		}

		/// <summary>
		/// Tests if the character always has a normalization boundary before it, regardless of
		/// context.
		/// If <c>true</c>, then the character does not normalization-interact with preceding
		/// characters. In other words, a string containing this character can be normalized by
		/// processing portions before this character and starting from this character
		/// independently. This is used for iterative normalization.
		/// </summary>
		/// <param name="codePoint">character to test</param>
		/// <returns><c>true</c> if c has a normalization boundary after it</returns>
		public bool HasBoundaryBefore(int codePoint)
		{
			return NativeMethods.unorm2_hasBoundaryBefore(_Normalizer, codePoint);
		}

		/// <summary>
		/// Gets the decomposition mapping of c. Roughly equivalent to normalizing the String
		/// form of c on a Mode.DECOMPOSE Normalizer2 instance, but much faster, and except that
		/// this function returns null if c does not have a decomposition mapping in this
		/// instance's data. This function is independent of the mode of the Normalizer2.
		/// </summary>
		/// <param name="codePoint">code point </param>
		/// <returns>c's decomposition mapping, if any; otherwise <c>null</c></returns>
		public string GetDecomposition(int codePoint)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.unorm2_getDecomposition(_Normalizer, codePoint,
					ptr, length, out var err);
				return new Tuple<ErrorCode, int>(err, length);
			}, 10);
		}

		/// <summary>
		/// Tests if the string is normalized.
		/// </summary>
		/// <param name="src">input string</param>
		/// <returns><c>true</c> if normalized</returns>
		public bool IsNormalized(string src)
		{
			if (string.IsNullOrEmpty(src))
				return true;

			var isNormalized = NativeMethods.unorm2_isNormalized(_Normalizer, src, src.Length,
				out var err);
			ExceptionFromErrorCode.ThrowIfError(err);
			return isNormalized;
		}

		/// <summary>
		/// Gets the combining class of c. The default implementation returns 0 but all standard
		/// implementations return the Unicode Canonical_Combining_Class value.
		/// </summary>
		/// <param name="c">code point</param>
		/// <returns>c's combining class</returns>
		public byte GetCombiningClass(int c)
		{
			return (byte)NativeMethods.unorm2_getCombiningClass(_Normalizer, c);
		}

		/// <summary>
		/// Normalize the string according to the mode of the current normalizer.
		/// </summary>
		/// <param name="src">source string</param>
		/// <returns>Returns the normalized form of the source string.</returns>
		public string Normalize(string src)
		{
			if (string.IsNullOrEmpty(src))
				return string.Empty;

			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.unorm2_normalize(_Normalizer, src, src.Length,
					ptr, length, out var status);
				return new Tuple<ErrorCode, int>(status, length);
			}, src.Length + 10);
		}

	}
}
