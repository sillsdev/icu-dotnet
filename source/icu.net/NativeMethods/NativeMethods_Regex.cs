// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Icu
{
	internal static partial class NativeMethods
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class RegexMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uregex_openDelegate(string pattern, int patternLength,
				uint flags, out ParseError parseError, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			// Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
			// which are usually zero, especially in debug builds...but one day we will be sorry.
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool uregex_matchesDelegate(IntPtr regexp, int startIndex, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uregex_setTextDelegate(IntPtr regexp,
				string text, int textLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uregex_closeDelegate(IntPtr regexp);

			internal uregex_openDelegate uregex_open;
			internal uregex_matchesDelegate uregex_matches;
			internal uregex_setTextDelegate uregex_setText;
			internal uregex_closeDelegate uregex_close;
		}

		// ReSharper disable once InconsistentNaming
		private static RegexMethodsContainer RegexMethods = new RegexMethodsContainer();

		/// <summary>
		/// Open (compile) an ICU regular expression.
		/// </summary>
		public static IntPtr uregex_open(string pattern, int patternLength, uint flags,
			out ParseError parseError, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (RegexMethods.uregex_open == null)
				RegexMethods.uregex_open = GetMethod<RegexMethodsContainer.uregex_openDelegate>(IcuI18NLibHandle, "uregex_open", true);
			return RegexMethods.uregex_open(pattern, patternLength, flags, out parseError, out errorCode);
		}

		/// <summary>
		/// Attempts to match the input string against the pattern.
		/// </summary>
		public static bool uregex_matches(IntPtr regexp, int startIndex, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (RegexMethods.uregex_matches == null)
				RegexMethods.uregex_matches = GetMethod<RegexMethodsContainer.uregex_matchesDelegate>(IcuI18NLibHandle, "uregex_matches", true);
			return RegexMethods.uregex_matches(regexp, startIndex, out errorCode);
		}

		public static void uregex_setText(IntPtr regexp, string text, int textLength,
			out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (RegexMethods.uregex_setText == null)
				RegexMethods.uregex_setText = GetMethod<RegexMethodsContainer.uregex_setTextDelegate>(IcuI18NLibHandle, "uregex_setText", true);
			RegexMethods.uregex_setText(regexp, text, textLength, out errorCode);
		}

		public static void uregex_close(IntPtr regexp)
		{
			if (RegexMethods.uregex_close == null)
				RegexMethods.uregex_close = GetMethod<RegexMethodsContainer.uregex_closeDelegate>(IcuI18NLibHandle, "uregex_close", true);
			RegexMethods.uregex_close(regexp);
		}

	}
}
