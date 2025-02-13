// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;
using Icu.Normalization;

namespace Icu
{
	/// <summary>
	/// Unicode normalization functionality for standard Unicode normalization or for using custom mapping tables.
	/// </summary>
	public static class Normalizer
	{
		/// <summary>
		/// Normalization mode constants.
		/// </summary>
		public enum UNormalizationMode
		{
			/// <summary>No decomposition/composition.</summary>
			UNORM_NONE = 1,
			/// <summary>Canonical decomposition.</summary>
			UNORM_NFD = 2,
			/// <summary>Compatibility decomposition.</summary>
			UNORM_NFKD = 3,
			/// <summary>Canonical decomposition followed by canonical composition.</summary>
			UNORM_NFC = 4,
			/// <summary>Default normalization.</summary>
			UNORM_DEFAULT = UNORM_NFC,
			///<summary>Compatibility decomposition followed by canonical composition.</summary>
			UNORM_NFKC = 5,
			/// <summary>"Fast C or D" form.</summary>
			UNORM_FCD = 6
		}

		private static Normalizer2 GetNormalizer(UNormalizationMode mode)
		{
			return Normalizer2.GetInstance(null,
				mode == UNormalizationMode.UNORM_NFC || mode == UNormalizationMode.UNORM_NFD
					? "nfc"
					: "nfkc",
				mode == UNormalizationMode.UNORM_NFC || mode == UNormalizationMode.UNORM_NFKC
					? Normalizer2.Mode.COMPOSE
					: Normalizer2.Mode.DECOMPOSE);
		}

		/// <summary>
		/// Normalize the string according to the given mode.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static string Normalize(string src, UNormalizationMode mode)
		{
			var normalizer = GetNormalizer(mode);
			return normalizer.Normalize(src);
		}

		/// <summary>
		/// Check whether the string is normalized according to the given mode.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool IsNormalized(string src, UNormalizationMode mode)
		{
			var normalizer = GetNormalizer(mode);
			return normalizer.IsNormalized(src);
		}
	}
}
