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
		private class UnicodeSetMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void uset_closeDelegate(IntPtr set);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uset_openDelegate(char start, char end);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr uset_openPatternDelegate(string pattern, int patternLength,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uset_addDelegate(IntPtr set, char c);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uset_toPatternDelegate(IntPtr set, IntPtr result, int resultCapacity,
				bool escapeUnprintable, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void uset_addStringDelegate(IntPtr set, string str, int strLen);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int uset_getItemDelegate(IntPtr set, int itemIndex, out int start,
				out int end, IntPtr str, int strCapacity, out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int uset_getItemCountDelegate(IntPtr set);

			internal uset_closeDelegate uset_close;
			internal uset_openDelegate uset_open;
			internal uset_openPatternDelegate uset_openPattern;
			internal uset_addDelegate uset_add;
			internal uset_toPatternDelegate uset_toPattern;
			internal uset_addStringDelegate uset_addString;
			internal uset_getItemDelegate uset_getItem;
			internal uset_getItemCountDelegate uset_getItemCount;
		}

		// ReSharper disable once InconsistentNaming
		private static UnicodeSetMethodsContainer UnicodeSetMethods = new UnicodeSetMethodsContainer();

		/// <summary>
		/// Disposes of the storage used by Unicode set.  This function should be called exactly once for objects returned by uset_open()
		/// </summary>
		/// <param name="set">Unicode set to dispose of</param>
		public static void uset_close(IntPtr set)
		{
			if (UnicodeSetMethods.uset_close == null)
				UnicodeSetMethods.uset_close = GetMethod<UnicodeSetMethodsContainer.uset_closeDelegate>(IcuCommonLibHandle, "uset_close");
			UnicodeSetMethods.uset_close(set);
		}

		/// <summary>
		/// Creates a Unicode set that contains the range of characters start..end, inclusive.
		/// If start > end then an empty set is created (same as using uset_openEmpty().
		/// </summary>
		/// <param name="start">First character of the range, inclusive</param>
		/// <param name="end">Last character of the range, inclusive</param>
		/// <returns>Unicode set of characters.  The caller must call uset_close() on it when done</returns>
		public static IntPtr uset_open(char start, char end)
		{
			if (UnicodeSetMethods.uset_open == null)
				UnicodeSetMethods.uset_open = GetMethod<UnicodeSetMethodsContainer.uset_openDelegate>(IcuCommonLibHandle, "uset_open");
			return UnicodeSetMethods.uset_open(start, end);
		}

		/// <summary>
		/// Creates a set from the given pattern.
		/// </summary>
		/// <param name="pattern">A string specifying what characters are in the set</param>
		/// <param name="patternLength">Length of the pattern, or -1 if null terminated</param>
		/// <param name="status">The error code</param>
		/// <returns>Unicode set</returns>
		public static IntPtr uset_openPattern(string pattern, int patternLength, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (UnicodeSetMethods.uset_openPattern == null)
				UnicodeSetMethods.uset_openPattern = GetMethod<UnicodeSetMethodsContainer.uset_openPatternDelegate>(IcuCommonLibHandle, "uset_openPattern");
			return UnicodeSetMethods.uset_openPattern(pattern, patternLength, out status);
		}

		/// <summary>
		/// Adds the given character to the given Unicode set.  After this call,
		/// uset_contains(set, c) will return TRUE.  A frozen set will not be modified.
		/// </summary>
		/// <param name="set">The object to which to add the character</param>
		/// <param name="c">The character to add</param>
		public static void uset_add(IntPtr set, char c)
		{
			if (UnicodeSetMethods.uset_add == null)
				UnicodeSetMethods.uset_add = GetMethod<UnicodeSetMethodsContainer.uset_addDelegate>(IcuCommonLibHandle, "uset_add");
			UnicodeSetMethods.uset_add(set, c);
		}

		/// <summary>
		/// Returns a string representation of this set.  If the result of calling this function is
		/// passed to a uset_openPattern(), it will produce another set that is equal to this one.
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <param name="result">The string to receive the rules, may be NULL</param>
		/// <param name="resultCapacity">The capacity of result, may be 0 if result is NULL</param>
		/// <param name="escapeUnprintable">if TRUE then convert unprintable characters to their hex escape representations,
		/// \uxxxx or \Uxxxxxxxx. Unprintable characters are those other than U+000A, U+0020..U+007E.</param>
		/// <param name="status">Error code</param>
		/// <returns>Length of string, possibly larger than resultCapacity</returns>
		[SuppressMessage("ReSharper", "CommentTypo")]
		public static int uset_toPattern(IntPtr set, IntPtr result, int resultCapacity,
			bool escapeUnprintable, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (UnicodeSetMethods.uset_toPattern == null)
				UnicodeSetMethods.uset_toPattern = GetMethod<UnicodeSetMethodsContainer.uset_toPatternDelegate>(IcuCommonLibHandle, "uset_toPattern");
			return UnicodeSetMethods.uset_toPattern(set, result, resultCapacity, escapeUnprintable, out status);
		}

		/// <summary>
		/// Adds the given string to the given Unicode set
		/// </summary>
		/// <param name="set">The Unicode set to which to add the string</param>
		/// <param name="str">The string to add</param>
		/// <param name="strLen">The length of the string or -1 if null</param>
		public static void uset_addString(IntPtr set, string str, int strLen)
		{
			if (UnicodeSetMethods.uset_addString == null)
				UnicodeSetMethods.uset_addString = GetMethod<UnicodeSetMethodsContainer.uset_addStringDelegate>(IcuCommonLibHandle, "uset_addString");
			UnicodeSetMethods.uset_addString(set, str, strLen);
		}

		/// <summary>
		/// Returns an item of this Unicode set.  An item is either a range of characters or a single multi-character string.
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <param name="itemIndex">A non-negative integer in the range 0..uset_getItemCount(set)-1</param>
		/// <param name="start">Pointer to variable to receive first character in range, inclusive</param>
		/// <param name="end">Pointer to variable to receive the last character in range, inclusive</param>
		/// <param name="str">Buffer to receive the string, may be NULL</param>
		/// <param name="strCapacity">Capacity of str, or 0 if str is NULL</param>
		/// <param name="status">Error Code</param>
		/// <returns>The length of the string (>=2), or 0 if the item is a range, in which case it
		///  is the range *start..*end, or -1 if itemIndex is out of range</returns>
		public static int uset_getItem(IntPtr set, int itemIndex, out int start,
			out int end, IntPtr str, int strCapacity, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (UnicodeSetMethods.uset_getItem == null)
				UnicodeSetMethods.uset_getItem = GetMethod<UnicodeSetMethodsContainer.uset_getItemDelegate>(IcuCommonLibHandle, "uset_getItem");
			return UnicodeSetMethods.uset_getItem(set, itemIndex, out start, out end, str, strCapacity, out status);
		}

		/// <summary>
		/// Returns the number of items in this set.  An item is either a range of characters or a single multi-character string
		/// </summary>
		/// <param name="set">The Unicode set</param>
		/// <returns>A non-negative integer counting the character ranges and/or strings contained in the set</returns>
		public static int uset_getItemCount(IntPtr set)
		{
			if (UnicodeSetMethods.uset_getItemCount == null)
				UnicodeSetMethods.uset_getItemCount = GetMethod<UnicodeSetMethodsContainer.uset_getItemCountDelegate>(IcuCommonLibHandle, "uset_getItemCount");
			return UnicodeSetMethods.uset_getItemCount(set);
		}
	}
}
