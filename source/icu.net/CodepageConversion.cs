// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Icu
{
	public static class CodepageConversion
	{
		/// <summary>
		/// Get an ICU UEnumeration pointer that will enumerate all converter IDs (canonical names).
		/// </summary>
		/// <returns>The opaque enumerator handle. Closing it properly is the responsibility of
		/// the caller.</returns>
		private static SafeEnumeratorHandle GetEnumerator()
		{
			var result = NativeMethods.ucnv_openAllNames(out var status);
			ExceptionFromErrorCode.ThrowIfError(status);
			return result;
		}

		/// <summary>
		/// Get the standard name for an encoding converter, according to some standard.
		/// </summary>
		/// <param name="name">The canonical name of the converter</param>
		/// <param name="standard">The name of the standard, e.g. "IANA"</param>
		/// <returns>The name as a string, or <c>null</c> if no name could be determined.</returns>
		public static string GetStandardName(string name, string standard)
		{
			var resultPtr = NativeMethods.ucnv_getStandardName(name, standard, out var status);
			ExceptionFromErrorCode.ThrowIfError(status);
			return resultPtr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(resultPtr);
		}

		/// <summary>
		/// Return all IDs (canonical names) and names (standard IANA names) of the converters that ICU knows about.
		/// </summary>
		/// <param name="standard">The name of the standard, e.g. "IANA"</param>
		/// <returns>An IEnumerable of IcuIdAndName objects, which have Id and Name properties.</returns>
		public static IEnumerable<(string id, string name)> GetIdsAndNames(string standard)
		{
			using (var icuEnumerator = GetEnumerator())
			{
				for (var id = icuEnumerator.Next(); !string.IsNullOrEmpty(id); id = icuEnumerator.Next())
				{
					var name = GetStandardName(id, standard);
					if (name != null)
						yield return (id, name);
				}
			}
		}
	}
}
