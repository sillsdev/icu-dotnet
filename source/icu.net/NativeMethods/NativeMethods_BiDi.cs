// Copyright (c) 2019 Jeff Skaistis
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;

namespace Icu
{
	internal static partial class NativeMethods
	{
		private class BiDiMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr ubidi_openDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr ubidi_openSizedDelegate(int maxLength, int maxRunCount, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_closeDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate BiDi.BiDiReorderingMode ubidi_getReorderingModeDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_setReorderingModeDelegate(IntPtr bidi, BiDi.BiDiReorderingMode reorderingMode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate BiDi.BiDiReorderingOption ubidi_getReorderingOptionsDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_setReorderingOptionsDelegate(IntPtr bidi, BiDi.BiDiReorderingOption reorderingOptions);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate bool ubidi_isInverseDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_setInverseDelegate(IntPtr bidi, bool isInverse);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate bool ubidi_isOrderParagraphsLTRDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_orderParagraphsLTRDelegate(IntPtr bidi, bool orderParagraphsLTR);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate BiDi.BiDiDirection ubidi_getDirectionDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_setParaDelegate(IntPtr bidi, IntPtr text, int length, byte paraLevel, [MarshalAs(UnmanagedType.LPArray)]byte[] embeddingLevels, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_setLineDelegate(IntPtr bidi, int start, int limit, out IntPtr lineBiDi, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getLengthDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getProcessedLengthDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getResultLengthDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr ubidi_getTextDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr ubidi_getLevelsDelegate(IntPtr bidi, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate byte ubidi_getLevelAtDelegate(IntPtr bidi, int charIndex);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate byte ubidi_getParaLevelDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_countParagraphsDelegate(IntPtr bidi);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getParagraphDelegate(IntPtr bidi, int charIndex, out int paraStart, out int paraLimit, out byte paraLevel, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_getParagraphByIndexDelegate(IntPtr bidi, int paraIndex, out int paraStart, out int paraLimit, out byte paraLevel, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_countRunsDelegate(IntPtr bidi, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getLogicalIndexDelegate(IntPtr bidi, int visualIndex, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_getLogicalMapDelegate(IntPtr bidi, [Out, MarshalAs(UnmanagedType.LPArray)]int[] indexMap, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_getLogicalRunDelegate(IntPtr bidi, int logicalPosition, out int logicalLimit, out byte level);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int ubidi_getVisualIndexDelegate(IntPtr bidi, int logicalIndex, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ubidi_getVisualMapDelegate(IntPtr bidi, [Out, MarshalAs(UnmanagedType.LPArray)]int[] indexMap, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate BiDi.BiDiDirection ubidi_getVisualRunDelegate(IntPtr bidi, int runIndex, out int logicalStart, out int length);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubidi_writeReorderedDelegate(IntPtr bidi, [Out, MarshalAs(UnmanagedType.LPArray)]char[] dest, int destSize, ushort options, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ubidi_writeReverseDelegate(string src, int srcLength, [Out, MarshalAs(UnmanagedType.LPArray)]char[] dest, int destSize, ushort options, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate BiDi.BiDiDirection ubidi_getBaseDirectionDelegate(string text, int length);

			internal ubidi_openDelegate ubidi_open;
			internal ubidi_openSizedDelegate ubidi_openSized;
			internal ubidi_closeDelegate ubidi_close;
			internal ubidi_getReorderingModeDelegate ubidi_getReorderingMode;
			internal ubidi_setReorderingModeDelegate ubidi_setReorderingMode;
			internal ubidi_getReorderingOptionsDelegate ubidi_getReorderingOptions;
			internal ubidi_setReorderingOptionsDelegate ubidi_setReorderingOptions;
			internal ubidi_isInverseDelegate ubidi_isInverse;
			internal ubidi_setInverseDelegate ubidi_setInverse;
			internal ubidi_isOrderParagraphsLTRDelegate ubidi_isOrderParagraphsLTR;
			internal ubidi_orderParagraphsLTRDelegate ubidi_orderParagraphsLTR;
			internal ubidi_getDirectionDelegate ubidi_getDirection;
			internal ubidi_setParaDelegate ubidi_setPara;
			internal ubidi_setLineDelegate ubidi_setLine;
			internal ubidi_getLengthDelegate ubidi_getLength;
			internal ubidi_getProcessedLengthDelegate ubidi_getProcessedLength;
			internal ubidi_getResultLengthDelegate ubidi_getResultLength;
			internal ubidi_getTextDelegate ubidi_getText;
			internal ubidi_getLevelsDelegate ubidi_getLevels;
			internal ubidi_getLevelAtDelegate ubidi_getLevelAt;
			internal ubidi_getParaLevelDelegate ubidi_getParaLevel;
			internal ubidi_countParagraphsDelegate ubidi_countParagraphs;
			internal ubidi_getParagraphDelegate ubidi_getParagraph;
			internal ubidi_getParagraphByIndexDelegate ubidi_getParagraphByIndex;
			internal ubidi_countRunsDelegate ubidi_countRuns;
			internal ubidi_getLogicalIndexDelegate ubidi_getLogicalIndex;
			internal ubidi_getLogicalMapDelegate ubidi_getLogicalMap;
			internal ubidi_getLogicalRunDelegate ubidi_getLogicalRun;
			internal ubidi_getVisualIndexDelegate ubidi_getVisualIndex;
			internal ubidi_getVisualMapDelegate ubidi_getVisualMap;
			internal ubidi_getVisualRunDelegate ubidi_getVisualRun;
			internal ubidi_writeReorderedDelegate ubidi_writeReordered;
			internal ubidi_writeReverseDelegate ubidi_writeReverse;
			internal ubidi_getBaseDirectionDelegate ubidi_getBaseDirection;
		}

		private static BiDiMethodsContainer _BiDiMethods;

		private static BiDiMethodsContainer BiDiMethods => _BiDiMethods ?? (_BiDiMethods = new BiDiMethodsContainer());

		/// <summary>
		/// Create a UBiDi structure.
		/// </summary>
		/// <returns>An created empty UBiDi structure.</returns>
		public static IntPtr ubidi_open()
		{
			if (BiDiMethods.ubidi_open == null)
				BiDiMethods.ubidi_open = GetMethod<BiDiMethodsContainer.ubidi_openDelegate>(IcuCommonLibHandle, "ubidi_open");
			return BiDiMethods.ubidi_open();
		}

		/// <summary>
		/// Creates a new UBiDi structure with pre-allocated internal structures.
		/// </summary>
		/// <param name="maxLength">The maximum text or line length that internal memory will be preallocated for</param>
		/// <param name="maxRunCount">The maximum anticipated number of same-level runs that internal memory will be preallocated for</param>
		/// <param name="errorCode">The error code</param>
		/// <returns></returns>
		public static IntPtr ubidi_openSized(int maxLength, int maxRunCount, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_openSized == null)
				BiDiMethods.ubidi_openSized = GetMethod<BiDiMethodsContainer.ubidi_openSizedDelegate>(IcuCommonLibHandle, "ubidi_openSized");
			return BiDiMethods.ubidi_openSized(maxLength, maxRunCount, out errorCode);
		}

		/// <summary>
		/// Free the memory associated with a UBiDi object.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		public static void ubidi_close(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_close == null)
				BiDiMethods.ubidi_close = GetMethod<BiDiMethodsContainer.ubidi_closeDelegate>(IcuCommonLibHandle, "ubidi_close");
			BiDiMethods.ubidi_close(bidi);
		}

		/// <summary>
		/// Gets the reordering mode of a given Bidi object.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>A combination of zero or more of the reordering options</returns>
		public static BiDi.BiDiReorderingMode ubidi_getReorderingMode(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getReorderingMode == null)
				BiDiMethods.ubidi_getReorderingMode = GetMethod<BiDiMethodsContainer.ubidi_getReorderingModeDelegate>(IcuCommonLibHandle, "ubidi_getReorderingMode");
			return BiDiMethods.ubidi_getReorderingMode(bidi);
		}

		/// <summary>
		/// Modify the operation of the Bidi algorithm such that it implements some variant to the basic Bidi algorithm or approximates an "inverse Bidi" algorithm, depending on different values of the "reordering mode".
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="reorderingMode">A combination of zero or more of the reordering options</param>
		public static void ubidi_setReorderingMode(IntPtr bidi, BiDi.BiDiReorderingMode reorderingMode)
		{
			if (BiDiMethods.ubidi_setReorderingMode == null)
				BiDiMethods.ubidi_setReorderingMode = GetMethod<BiDiMethodsContainer.ubidi_setReorderingModeDelegate>(IcuCommonLibHandle, "ubidi_setReorderingMode");
			BiDiMethods.ubidi_setReorderingMode(bidi, reorderingMode);
		}

		/// <summary>
		/// Gets the reordering options of a given Bidi object.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>A combination of zero or more of the reordering options</returns>
		public static BiDi.BiDiReorderingOption ubidi_getReorderingOptions(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getReorderingOptions == null)
				BiDiMethods.ubidi_getReorderingOptions = GetMethod<BiDiMethodsContainer.ubidi_getReorderingOptionsDelegate>(IcuCommonLibHandle, "ubidi_getReorderingOptions");
			return BiDiMethods.ubidi_getReorderingOptions(bidi);
		}

		/// <summary>
		/// Specify which of the reordering options should be applied during Bidi transformations.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="reorderingOptions">A combination of zero or more of the reordering options</param>
		public static void ubidi_setReorderingOptions(IntPtr bidi, BiDi.BiDiReorderingOption reorderingOptions)
		{
			if (BiDiMethods.ubidi_setReorderingOptions == null)
				BiDiMethods.ubidi_setReorderingOptions = GetMethod<BiDiMethodsContainer.ubidi_setReorderingOptionsDelegate>(IcuCommonLibHandle, "ubidi_setReorderingOptions");
			BiDiMethods.ubidi_setReorderingOptions(bidi, reorderingOptions);
		}

		/// <summary>
		/// Is this Bidi object set to perform the inverse Bidi algorithm?
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>True if the object is using the inverse Bidi algorithm</returns>
		public static bool ubidi_isInverse(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_isInverse == null)
				BiDiMethods.ubidi_isInverse = GetMethod<BiDiMethodsContainer.ubidi_isInverseDelegate>(IcuCommonLibHandle, "ubidi_isInverse");
			return BiDiMethods.ubidi_isInverse(bidi);
		}

		/// <summary>
		/// Modify the operation of the Bidi algorithm such that it approximates an "inverse Bidi" algorithm.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="isInverse">True to use the inverse algorithm</param>
		public static void ubidi_setInverse(IntPtr bidi, bool isInverse)
		{
			if (BiDiMethods.ubidi_setInverse == null)
				BiDiMethods.ubidi_setInverse = GetMethod<BiDiMethodsContainer.ubidi_setInverseDelegate>(IcuCommonLibHandle, "ubidi_setInverse");
			BiDiMethods.ubidi_setInverse(bidi, isInverse);
		}

		/// <summary>
		/// Is this Bidi object set to allocate level 0 to block separators so that successive paragraphs progress from left to right?
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>True if the object is using level 0 for block separators</returns>
		public static bool ubidi_isOrderParagraphsLTR(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_isOrderParagraphsLTR == null)
				BiDiMethods.ubidi_isOrderParagraphsLTR = GetMethod<BiDiMethodsContainer.ubidi_isOrderParagraphsLTRDelegate>(IcuCommonLibHandle, "ubidi_isOrderParagraphsLTR");
			return BiDiMethods.ubidi_isOrderParagraphsLTR(bidi);
		}

		/// <summary>
		/// Specify whether block separators must be allocated level zero, so that successive paragraphs will progress from left to right.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="orderParagraphsLTR">True to use level 0 for block separators</param>
		public static void ubidi_orderParagraphsLTR(IntPtr bidi, bool orderParagraphsLTR)
		{
			if (BiDiMethods.ubidi_orderParagraphsLTR == null)
				BiDiMethods.ubidi_orderParagraphsLTR = GetMethod<BiDiMethodsContainer.ubidi_orderParagraphsLTRDelegate>(IcuCommonLibHandle, "ubidi_orderParagraphsLTR");
			BiDiMethods.ubidi_orderParagraphsLTR(bidi, orderParagraphsLTR);
		}

		/// <summary>
		/// Get the directionality of the text.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The directionality of the text</returns>
		public static BiDi.BiDiDirection ubidi_getDirection(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getDirection == null)
				BiDiMethods.ubidi_getDirection = GetMethod<BiDiMethodsContainer.ubidi_getDirectionDelegate>(IcuCommonLibHandle, "ubidi_getDirection");
			return BiDiMethods.ubidi_getDirection(bidi);
		}

		/// <summary>
		/// Perform the Unicode Bidi algorithm.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="text">The text to analyze</param>
		/// <param name="length">The text length.</param>
		/// <param name="paraLevel">The base paragraph level.</param>
		/// <param name="embeddingLevels">Preset embedding and override levels</param>
		/// <param name="errorCode">The error code</param>
		public static void ubidi_setPara(IntPtr bidi, IntPtr text, int length, byte paraLevel, byte[] embeddingLevels, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_setPara == null)
				BiDiMethods.ubidi_setPara = GetMethod<BiDiMethodsContainer.ubidi_setParaDelegate>(IcuCommonLibHandle, "ubidi_setPara");
			BiDiMethods.ubidi_setPara(bidi, text, length, paraLevel, embeddingLevels, out errorCode);
		}

		/// <summary>
		/// Sets a BiDi object to contain the reordering information, especially the resolved levels, for all the characters in a line of text.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="start">The line's first index into the text</param>
		/// <param name="limit">Index just behind the line's last index into the text (its last index +1)</param>
		/// <param name="lineBidi">The new line BiDi object</param>
		/// <param name="errorCode">The error code</param>
		public static void ubidi_setLine(IntPtr bidi, int start, int limit, out IntPtr lineBidi, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_setLine == null)
				BiDiMethods.ubidi_setLine = GetMethod<BiDiMethodsContainer.ubidi_setLineDelegate>(IcuCommonLibHandle, "ubidi_setLine");
			BiDiMethods.ubidi_setLine(bidi, start, limit, out lineBidi, out errorCode);
		}

		/// <summary>
		/// Get the length of the text.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The text length</returns>
		public static int ubidi_getLength(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getLength == null)
				BiDiMethods.ubidi_getLength = GetMethod<BiDiMethodsContainer.ubidi_getLengthDelegate>(IcuCommonLibHandle, "ubidi_getLength");
			return BiDiMethods.ubidi_getLength(bidi);
		}

		/// <summary>
		/// Get the length of the source text processed by the last call to ubidi_setPara().
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The source text length</returns>
		public static int ubidi_getProcessedLength(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getProcessedLength == null)
				BiDiMethods.ubidi_getProcessedLength = GetMethod<BiDiMethodsContainer.ubidi_getProcessedLengthDelegate>(IcuCommonLibHandle, "ubidi_getProcessedLength");
			return BiDiMethods.ubidi_getProcessedLength(bidi);
		}

		/// <summary>
		/// Get the length of the reordered text resulting from the last call to ubidi_setPara().
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The result text length</returns>
		public static int ubidi_getResultLength(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getResultLength == null)
				BiDiMethods.ubidi_getResultLength = GetMethod<BiDiMethodsContainer.ubidi_getResultLengthDelegate>(IcuCommonLibHandle, "ubidi_getResultLength");
			return BiDiMethods.ubidi_getResultLength(bidi);
		}

		/// <summary>
		/// Get the pointer to the text
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The text</returns>
		public static IntPtr ubidi_getText(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getText == null)
				BiDiMethods.ubidi_getText = GetMethod<BiDiMethodsContainer.ubidi_getTextDelegate>(IcuCommonLibHandle, "ubidi_getText");
			return BiDiMethods.ubidi_getText(bidi);
		}

		/// <summary>
		/// Get an array of levels for each character.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The bidi levels</returns>
		public static IntPtr ubidi_getLevels(IntPtr bidi, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getLevels == null)
				BiDiMethods.ubidi_getLevels = GetMethod<BiDiMethodsContainer.ubidi_getLevelsDelegate>(IcuCommonLibHandle, "ubidi_getLevels");
			return BiDiMethods.ubidi_getLevels(bidi, out errorCode);
		}

		/// <summary>
		/// Get the level for one character.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="charIndex">The index of the character in the text.</param>
		/// <returns>The bidi level</returns>
		public static byte ubidi_getLevelAt(IntPtr bidi, int charIndex)
		{
			if (BiDiMethods.ubidi_getLevelAt == null)
				BiDiMethods.ubidi_getLevelAt = GetMethod<BiDiMethodsContainer.ubidi_getLevelAtDelegate>(IcuCommonLibHandle, "ubidi_getLevelAt");
			return BiDiMethods.ubidi_getLevelAt(bidi, charIndex);
		}

		/// <summary>
		/// Get the paragraph level of the text.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The paragraph level</returns>
		public static byte ubidi_getParaLevel(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_getParaLevel == null)
				BiDiMethods.ubidi_getParaLevel = GetMethod<BiDiMethodsContainer.ubidi_getParaLevelDelegate>(IcuCommonLibHandle, "ubidi_getParaLevel");
			return BiDiMethods.ubidi_getParaLevel(bidi);
		}

		/// <summary>
		/// Get the number of paragraphs.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <returns>The number of paragraphs</returns>
		public static int ubidi_countParagraphs(IntPtr bidi)
		{
			if (BiDiMethods.ubidi_countParagraphs == null)
				BiDiMethods.ubidi_countParagraphs = GetMethod<BiDiMethodsContainer.ubidi_countParagraphsDelegate>(IcuCommonLibHandle, "ubidi_countParagraphs");
			return BiDiMethods.ubidi_countParagraphs(bidi);
		}

		/// <summary>
		/// Get a paragraph, given a position within the text.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="charIndex">The index of a character within the text, in the range [0..ubidi_getProcessedLength(pBiDi)-1]</param>
		/// <param name="paraStart">Will receive the index of the first character of the paragraph in the text</param>
		/// <param name="paraLimit">Will receive the limit of the paragraph</param>
		/// <param name="paraLevel">Will receive the level of the paragraph</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The index of the paragraph containing the specified position</returns>
		public static int ubidi_getParagraph(IntPtr bidi, int charIndex, out int paraStart, out int paraLimit, out byte paraLevel, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getParagraph == null)
				BiDiMethods.ubidi_getParagraph = GetMethod<BiDiMethodsContainer.ubidi_getParagraphDelegate>(IcuCommonLibHandle, "ubidi_getParagraph");
			return BiDiMethods.ubidi_getParagraph(bidi, charIndex, out paraStart, out paraLimit, out paraLevel, out errorCode);
		}

		/// <summary>
		/// Get a paragraph, given the index of this paragraph.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="paraIndex">The number of the paragraph, in the range [0..ubidi_countParagraphs(pBiDi)-1]</param>
		/// <param name="paraStart">Will receive the index of the first character of the paragraph in the text</param>
		/// <param name="paraLimit">Will receive the limit of the paragraph</param>
		/// <param name="paraLevel">Will receive the level of the paragraph</param>
		/// <param name="errorCode">The error code</param>
		public static void ubidi_getParagraphByIndex(IntPtr bidi, int paraIndex, out int paraStart, out int paraLimit, out byte paraLevel, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getParagraphByIndex == null)
				BiDiMethods.ubidi_getParagraphByIndex = GetMethod<BiDiMethodsContainer.ubidi_getParagraphByIndexDelegate>(IcuCommonLibHandle, "ubidi_getParagraphByIndex");
			BiDiMethods.ubidi_getParagraphByIndex(bidi, paraIndex, out paraStart, out paraLimit, out paraLevel, out errorCode);
		}

		/// <summary>
		/// Get the number of runs.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The number of runs</returns>
		public static int ubidi_countRuns(IntPtr bidi, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_countRuns == null)
				BiDiMethods.ubidi_countRuns = GetMethod<BiDiMethodsContainer.ubidi_countRunsDelegate>(IcuCommonLibHandle, "ubidi_countRuns");
			return BiDiMethods.ubidi_countRuns(bidi, out errorCode);
		}

		/// <summary>
		/// Get the logical text position from a visual position.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="visualIndex">The visual position of a character</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The index of this character in the text</returns>
		public static int ubidi_getLogicalIndex(IntPtr bidi, int visualIndex, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getLogicalIndex == null)
				BiDiMethods.ubidi_getLogicalIndex = GetMethod<BiDiMethodsContainer.ubidi_getLogicalIndexDelegate>(IcuCommonLibHandle, "ubidi_getLogicalIndex");
			return BiDiMethods.ubidi_getLogicalIndex(bidi, visualIndex, out errorCode);
		}

		/// <summary>
		/// Get a logical-to-visual index map (array) for the characters in the UBiDi (paragraph or line) object.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="indexMap">An array of ubidi_getProcessedLength() indexes which will reflect the reordering of the characters;
		/// if option UBIDI_OPTION_INSERT_MARKS is set, the number of elements allocated in indexMap must be no less than ubidi_getResultLength()</param>
		/// <param name="errorCode">The error code</param>
		public static void ubidi_getLogicalMap(IntPtr bidi, [Out, MarshalAs(UnmanagedType.LPArray)]int[] indexMap, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getLogicalMap == null)
				BiDiMethods.ubidi_getLogicalMap = GetMethod<BiDiMethodsContainer.ubidi_getLogicalMapDelegate>(IcuCommonLibHandle, "ubidi_getLogicalMap");
			BiDiMethods.ubidi_getLogicalMap(bidi, indexMap, out errorCode);
		}

		/// <summary>
		/// Get a logical run.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="logicalPosition">A logical position within the source text</param>
		/// <param name="logicalLimit">Will receive the limit of the corresponding run</param>
		/// <param name="level">Will receive the level of the corresponding run</param>
		public static void ubidi_getLogicalRun(IntPtr bidi, int logicalPosition, out int logicalLimit, out byte level)
		{
			if (BiDiMethods.ubidi_getLogicalRun == null)
				BiDiMethods.ubidi_getLogicalRun = GetMethod<BiDiMethodsContainer.ubidi_getLogicalRunDelegate>(IcuCommonLibHandle, "ubidi_getLogicalRun");
			BiDiMethods.ubidi_getLogicalRun(bidi, logicalPosition, out logicalLimit, out level);
		}

		/// <summary>
		/// Get the visual position from a logical text position..
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="logicalIndex">The index of a character in the tex</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>The visual position of this character</returns>
		public static int ubidi_getVisualIndex(IntPtr bidi, int logicalIndex, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getVisualIndex == null)
				BiDiMethods.ubidi_getVisualIndex = GetMethod<BiDiMethodsContainer.ubidi_getVisualIndexDelegate>(IcuCommonLibHandle, "ubidi_getVisualIndex");
			return BiDiMethods.ubidi_getVisualIndex(bidi, logicalIndex, out errorCode);
		}

		/// <summary>
		/// Get a visual-to-logical index map (array) for the characters in the UBiDi (paragraph or line) object.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="indexMap">An array of ubidi_getResultLength() indexes which will reflect the reordering of the characters</param>
		/// <param name="errorCode">The error code</param>
		public static void ubidi_getVisualMap(IntPtr bidi, [Out, MarshalAs(UnmanagedType.LPArray)]int[] indexMap, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_getVisualMap == null)
				BiDiMethods.ubidi_getVisualMap = GetMethod<BiDiMethodsContainer.ubidi_getVisualMapDelegate>(IcuCommonLibHandle, "ubidi_getVisualMap");
			BiDiMethods.ubidi_getVisualMap(bidi, indexMap, out errorCode);
		}

		/// <summary>
		/// Get one run's logical start, length, and directionality, which can be 0 for LTR or 1 for RTL.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="runIndex">The number of the run in visual order, in the range [0..ubidi_countRuns(pBiDi)-1]</param>
		/// <param name="logicalStart">The first logical character index in the text</param>
		/// <param name="length">The number of characters (at least one) in the run</param>
		/// <returns></returns>
		public static BiDi.BiDiDirection ubidi_getVisualRun(IntPtr bidi, int runIndex, out int logicalStart, out int length)
		{
			if (BiDiMethods.ubidi_getVisualRun == null)
				BiDiMethods.ubidi_getVisualRun = GetMethod<BiDiMethodsContainer.ubidi_getVisualRunDelegate>(IcuCommonLibHandle, "ubidi_getVisualRun");
			return BiDiMethods.ubidi_getVisualRun(bidi, runIndex, out logicalStart, out length);
		}

		/// <summary>
		/// Take a BiDi object containing the reordering information for a piece of text (one or more paragraphs) set by ubidi_setPara() or for a line of text set by ubidi_setLine() and write a reordered string to the destination buffer.
		/// </summary>
		/// <param name="bidi">The BiDi object</param>
		/// <param name="dest">Destination buffer</param>
		/// <param name="destSize">Destination buffer size</param>
		/// <param name="options">Reordering options</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>Length of the output string</returns>
		public static int ubidi_writeReordered(IntPtr bidi, char[] dest, int destSize, ushort options, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_writeReordered == null)
				BiDiMethods.ubidi_writeReordered = GetMethod<BiDiMethodsContainer.ubidi_writeReorderedDelegate>(IcuCommonLibHandle, "ubidi_writeReordered");
			return BiDiMethods.ubidi_writeReordered(bidi, dest, destSize, options, out errorCode);
		}

		/// <summary>
		/// Reverse a Right-To-Left run of Unicode text.
		/// </summary>
		/// <param name="src">The RTL run text</param>
		/// <param name="srcLength">The length of the RTL run</param>
		/// <param name="dest">Destination buffer</param>
		/// <param name="destSize">Destination buffer size</param>
		/// <param name="options">Reordering options</param>
		/// <param name="errorCode">The error code</param>
		/// <returns>Length of the output string</returns>
		public static int ubidi_writeReverse(string src, int srcLength, char[] dest, int destSize, ushort options, out ErrorCode errorCode)
		{
			if (BiDiMethods.ubidi_writeReverse == null)
				BiDiMethods.ubidi_writeReverse = GetMethod<BiDiMethodsContainer.ubidi_writeReverseDelegate>(IcuCommonLibHandle, "ubidi_writeReverse");
			return BiDiMethods.ubidi_writeReverse(src, srcLength, dest, destSize, options, out errorCode);
		}

		/// <summary>
		/// Gets the base direction of the text provided according to the Unicode Bidirectional Algorithm.
		/// </summary>
		/// <param name="text">The text whose base direction is needed</param>
		/// <param name="length">The length of the text</param>
		/// <returns>Base direction of the text</returns>
		public static BiDi.BiDiDirection ubidi_getBaseDirection(string text, int length)
		{
			if (BiDiMethods.ubidi_getBaseDirection == null)
				BiDiMethods.ubidi_getBaseDirection = GetMethod<BiDiMethodsContainer.ubidi_getBaseDirectionDelegate>(IcuCommonLibHandle, "ubidi_getBaseDirection");
			return BiDiMethods.ubidi_getBaseDirection(text, length);
		}
	}
}
