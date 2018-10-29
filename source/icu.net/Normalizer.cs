// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Unicode normalization functionality for standard Unicode normalization or for using custom mapping tables.
	/// </summary>
	public class Normalizer
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
			UNORM_FCD = 6,
			/// NFKC followed by case folding
			UNORM_NFKC_CF = 7

		}

		internal enum UNormalization2Mode
		{
			UNORM2_COMPOSE = 0,
			UNORM2_DECOMPOSE = 1,
			UNORM2_FCD = 2,
			UNORM2_COMPOSE_CONTIGUOUS = 3
		}

		private static IntPtr GetNormalizer(UNormalizationMode mode)
		{
			ErrorCode errorCode;
			var ret = NativeMethods.unorm2_getInstance(IntPtr.Zero,
				(mode == UNormalizationMode.UNORM_NFC || mode == UNormalizationMode.UNORM_NFD) ? "nfc" : (mode == UNormalizationMode.UNORM_NFKC_CF ? "nfkc_cf" : "nfkc"),
				(mode == UNormalizationMode.UNORM_NFC || mode == UNormalizationMode.UNORM_NFKC || mode == UNormalizationMode.UNORM_NFKC_CF) ?
					UNormalization2Mode.UNORM2_COMPOSE : UNormalization2Mode.UNORM2_DECOMPOSE,
				out errorCode);
			if (errorCode != ErrorCode.NoErrors)
				throw new Exception("Normalizer.Normalize() failed with code " + errorCode);
			return ret;
		}

		/// <summary>
		/// Normalize the string according to the given mode.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static string Normalize(string src, UNormalizationMode mode)
		{
			if (string.IsNullOrEmpty(src))
				return string.Empty;

			var length = src.Length + 10;
			var resPtr = Marshal.AllocCoTaskMem(length * 2);
			try
			{
				ErrorCode err;
				var normalizer = GetNormalizer(mode);
				var outLength = NativeMethods.unorm2_normalize(normalizer, src, src.Length, resPtr, length, out err);
				if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
					throw new Exception("Normalizer.Normalize() failed with code " + err);
				if (outLength >= length)
				{
					Marshal.FreeCoTaskMem(resPtr);
					length = outLength + 1; // allow room for the terminating NUL (FWR-505)
					resPtr = Marshal.AllocCoTaskMem(length * 2);
					outLength = NativeMethods.unorm2_normalize(normalizer, src, src.Length, resPtr, length, out err);
				}
				if (err > 0)
					throw new Exception("Normalizer.Normalize() failed with code " + err);

				var result = Marshal.PtrToStringUni(resPtr);
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
		/// Check whether the string is normalized according to the given mode.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool IsNormalized(string src, UNormalizationMode mode)
		{
			if (string.IsNullOrEmpty(src))
				return true;

			ErrorCode err;
			var fIsNorm = NativeMethods.unorm2_isNormalized(GetNormalizer(mode), src, src.Length, out err);
			if (err != ErrorCode.NoErrors)
				throw new Exception("Normalizer.IsNormalized() failed with code " + err);
			return fIsNorm != 0;
		}
	}
}
