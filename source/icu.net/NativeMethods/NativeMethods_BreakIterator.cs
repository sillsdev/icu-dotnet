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
		private class BreakIteratorMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ubrk_openDelegate(BreakIterator.UBreakIteratorType type,
				[MarshalAs(UnmanagedType.LPStr)] string locale, string text, int textLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ubrk_openRulesDelegate(string rules, int rulesLength,
				string text, int textLength, out ParseError parseError, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubrk_closeDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubrk_firstDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubrk_nextDelegate(IntPtr bi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubrk_getRuleStatusDelegate(IntPtr bi);

			/// <summary>
			/// Get the statuses from the break rules that determined the most
			/// recently returned break position.  The values appear in the rule
			/// source within brackets, {123}, for example.The default status value
			/// for rules that do not explicitly provide one is zero.
			///
			/// For word break iterators, the possible values are defined in
			/// <see cref="Icu.BreakIterator.UWordBreak"/>
			/// </summary>
			/// <returns>The number of rule status values that determined the most recent
			/// boundary returned from the break iterator.</returns>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubrk_getRuleStatusVecDelegate(IntPtr bi,
				[Out, MarshalAs(UnmanagedType.LPArray)]int[] fillInVector,
				int capacity,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr ubrk_safeCloneDelegate(IntPtr bi, IntPtr stackBuffer,
				IntPtr bufferSize, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ubrk_setTextDelegate(IntPtr bi, string text, int textLength,
				out ErrorCode errorCode);

			internal ubrk_openDelegate ubrk_open;
			internal ubrk_openRulesDelegate ubrk_openRules;
			internal ubrk_closeDelegate ubrk_close;
			internal ubrk_firstDelegate ubrk_first;
			internal ubrk_nextDelegate ubrk_next;
			internal ubrk_getRuleStatusDelegate ubrk_getRuleStatus;
			internal ubrk_getRuleStatusVecDelegate ubrk_getRuleStatusVec;
			internal ubrk_safeCloneDelegate ubrk_safeClone;
			internal ubrk_setTextDelegate ubrk_setText;
		}

		// ReSharper disable once InconsistentNaming
		private static BreakIteratorMethodsContainer BreakIteratorMethods = new BreakIteratorMethodsContainer();

		#region Break iterator

		/// <summary>
		/// Open a new UBreakIterator for locating text boundaries for a specified locale.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The text.</param>
		/// <param name="textLength">Length of the text.</param>
		/// <param name="errorCode">The error code.</param>
		/// <returns></returns>
		public static IntPtr ubrk_open(BreakIterator.UBreakIteratorType type,
			string locale, string text, int textLength, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (BreakIteratorMethods.ubrk_open == null)
				BreakIteratorMethods.ubrk_open = GetMethod<BreakIteratorMethodsContainer.ubrk_openDelegate>(IcuCommonLibHandle, "ubrk_open", true);
			return BreakIteratorMethods.ubrk_open(type, locale, text, textLength, out errorCode);
		}

		/// <summary>
		/// Open a new BreakIterator that uses the given rules to break text.
		/// </summary>
		/// <param name="rules">The rules to use for break iterator</param>
		/// <param name="rulesLength">The length of the rules.</param>
		/// <param name="text">The text.</param>
		/// <param name="textLength">Length of the text.</param>
		/// <param name="parseError">Receives position and context information for any syntax errors detected while parsing the rules.</param>
		/// <param name="errorCode">The error code.</param>
		/// <returns></returns>
		public static IntPtr ubrk_openRules(
			string rules, int rulesLength,
			string text, int textLength,
			out ParseError parseError, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (BreakIteratorMethods.ubrk_openRules == null)
				BreakIteratorMethods.ubrk_openRules = GetMethod<BreakIteratorMethodsContainer.ubrk_openRulesDelegate>(IcuCommonLibHandle, "ubrk_openRules", true);
			return BreakIteratorMethods.ubrk_openRules(rules, rulesLength, text, textLength, out parseError, out errorCode);
		}

		/// <summary>
		/// Thread safe cloning operation.
		/// </summary>
		/// <param name="bi">iterator to be cloned</param>
		/// <param name="stackBuffer">Deprecated. Should be IntPtr.Zero.</param>
		/// <param name="bufferSize">Deprecated. Should be IntPtr.Zero.</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The new clone</returns>
		public static IntPtr ubrk_safeClone(IntPtr bi, IntPtr stackBuffer, IntPtr bufferSize,
			out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (BreakIteratorMethods.ubrk_safeClone == null)
				BreakIteratorMethods.ubrk_safeClone = GetMethod<BreakIteratorMethodsContainer.ubrk_safeCloneDelegate>(IcuCommonLibHandle, "ubrk_safeClone", true);
			return BreakIteratorMethods.ubrk_safeClone(bi, stackBuffer, bufferSize, out errorCode);
		}

		/// <summary>
		/// Sets the iterator to a new text
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <param name="text">Text to examine</param>
		/// <param name="textLength">The length of the text</param>
		/// <param name="errorCode">The error code</param>
		public static void ubrk_setText(IntPtr bi, string text, int textLength, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (BreakIteratorMethods.ubrk_setText == null)
				BreakIteratorMethods.ubrk_setText = GetMethod<BreakIteratorMethodsContainer.ubrk_setTextDelegate>(IcuCommonLibHandle, "ubrk_setText", true);
			BreakIteratorMethods.ubrk_setText(bi, text, textLength, out errorCode);
		}

		/// <summary>
		/// Close a UBreakIterator.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		public static void ubrk_close(IntPtr bi)
		{
			if (BreakIteratorMethods.ubrk_close == null)
				BreakIteratorMethods.ubrk_close = GetMethod<BreakIteratorMethodsContainer.ubrk_closeDelegate>(IcuCommonLibHandle, "ubrk_close", true);
			BreakIteratorMethods.ubrk_close(bi);
		}

		/// <summary>
		/// Determine the index of the first character in the text being scanned.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_first(IntPtr bi)
		{
			if (BreakIteratorMethods.ubrk_first == null)
				BreakIteratorMethods.ubrk_first = GetMethod<BreakIteratorMethodsContainer.ubrk_firstDelegate>(IcuCommonLibHandle, "ubrk_first", true);
			return BreakIteratorMethods.ubrk_first(bi);
		}

		/// <summary>
		/// Determine the text boundary following the current text boundary.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_next(IntPtr bi)
		{
			if (BreakIteratorMethods.ubrk_next == null)
				BreakIteratorMethods.ubrk_next = GetMethod<BreakIteratorMethodsContainer.ubrk_nextDelegate>(IcuCommonLibHandle, "ubrk_next", true);
			return BreakIteratorMethods.ubrk_next(bi);
		}

		/// <summary>
		/// Return the status from the break rule that determined the most recently returned break position.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <returns></returns>
		public static int ubrk_getRuleStatus(IntPtr bi)
		{
			if (BreakIteratorMethods.ubrk_getRuleStatus == null)
				BreakIteratorMethods.ubrk_getRuleStatus = GetMethod<BreakIteratorMethodsContainer.ubrk_getRuleStatusDelegate>(IcuCommonLibHandle, "ubrk_getRuleStatus", true);
			return BreakIteratorMethods.ubrk_getRuleStatus(bi);
		}

		/// <summary>
		/// Return the status from the break rule that determined the most recently returned break position.
		/// </summary>
		/// <param name="bi">The break iterator.</param>
		/// <param name="fillInVector">An array to be filled in with the status values.</param>
		/// <param name="capacity">The length of the supplied vector. A length of zero causes the function to return the number of status values, in the normal way, without attempting to store any values.</param>
		/// <param name="status">Receives error codes.</param>
		/// <returns>The number of rule status values from rules that determined the most recent boundary returned by the break iterator.</returns>
		public static int ubrk_getRuleStatusVec(IntPtr bi,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] fillInVector,
			int capacity,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (BreakIteratorMethods.ubrk_getRuleStatusVec == null)
				BreakIteratorMethods.ubrk_getRuleStatusVec = GetMethod<BreakIteratorMethodsContainer.ubrk_getRuleStatusVecDelegate>(IcuCommonLibHandle, "ubrk_getRuleStatusVec", true);
			return BreakIteratorMethods.ubrk_getRuleStatusVec(bi, fillInVector, capacity, out status);
		}

		#endregion Break iterator

	}
}
