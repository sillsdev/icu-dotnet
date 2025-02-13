// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Provides access to Unicode Character Database.
	/// In addition to raw property values, some convenience functions calculate
	/// derived properties.
	/// </summary>
	public static class Character
	{
		/// <summary>
		/// Defined in ICU uchar.h
		/// http://icu-project.org/apiref/icu4c/uchar_8h.html
		/// </summary>
		public enum UProperty
		{
			/*  See note !!.  Comments of the form "Binary property Dash",
			"Enumerated property Script", "Double property Numeric_Value",
			and "String property Age" are read by genpname. */

			/*  Note: Place ALPHABETIC before BINARY_START so that
			debuggers display ALPHABETIC as the symbolic name for 0,
			rather than BINARY_START.  Likewise for other *_START
			identifiers. */
			/// <summary>
			/// Same as u_isUAlphabetic, different from u_isalpha.
			/// Lu+Ll+Lt+Lm+Lo+Nl+Other_Alphabetic
			/// </summary>
			ALPHABETIC = 0,
			/// <summary>First constant for binary Unicode properties.</summary>
			BINARY_START = ALPHABETIC,
			/// <summary>0-9 A-F a-f</summary>
			ASCII_HEX_DIGIT = 1,
			/// <summary>Format controls which have specific functions in the Bidi Algorithm.</summary>
			BIDI_CONTROL = 2,
			/// <summary>
			/// Characters that may change display in RTL text. Same as
			/// u_isMirrored. See Bidi Algorithm, UTR 9.
			/// </summary>
			BIDI_MIRRORED = 3,
			/// <summary>Variations of dashes.</summary>
			DASH = 4,
			/// <summary>Ignorable in most processing.</summary>
			/// <example>&lt;2060..206F, FFF0..FFFB, E0000..E0FFF&gt;+Other_Default_Ignorable_Code_Point+(Cf+Cc+Cs-White_Space)</example>
			DEFAULT_IGNORABLE_CODE_POINT = 5,
			/// <summary>The usage of deprecated characters is strongly discouraged.</summary>
			DEPRECATED = 6,
			/// <summary>
			/// Characters that linguistically modify the meaning of another
			/// character to which they apply.
			/// </summary>
			DIACRITIC = 7,
			/// <summary>
			/// Extend the value or shape of a preceding alphabetic character,
			/// e.g., length and iteration marks.
			/// </summary>
			EXTENDER = 8,
			/// <summary>CompositionExclusions.txt+Singleton Decompositions+ Non-Starter Decompositions.</summary>
			FULL_COMPOSITION_EXCLUSION = 9,
			/// <summary>For programmatic determination of grapheme cluster boundaries.</summary>
			/// <example>[0..10FFFF]-Cc-Cf-Cs-Co-Cn-Zl-Zp-Grapheme_Link-Grapheme_Extend-CGJ</example>
			GRAPHEME_BASE = 10,
			/// <summary>For programmatic determination of grapheme cluster boundaries.</summary>
			/// <example>Me+Mn+Mc+Other_Grapheme_Extend-Grapheme_Link-CGJ</example>
			GRAPHEME_EXTEND = 11,
			/// <summary>For programmatic determination of grapheme cluster boundaries.</summary>
			GRAPHEME_LINK = 12,
			/// <summary>Characters commonly used for hexadecimal numbers.</summary>
			HEX_DIGIT = 13,
			/// <summary>
			/// Dashes used to mark connections between pieces of words, plus the
			/// Katakana middle dot.
			/// </summary>
			HYPHEN = 14,
			/// <summary>
			/// Characters that can continue an identifier.
			/// DerivedCoreProperties.txt also says "NOTE: Cf characters should
			/// be filtered out."
			/// </summary>
			/// <example>ID_Start+Mn+Mc+Nd+Pc</example>
			ID_CONTINUE = 15,
			/// <summary>Characters that can start an identifier.</summary>
			/// <example>Lu+Ll+Lt+Lm+Lo+Nl</example>
			ID_START = 16,
			/// <summary>CJKV ideographs.</summary>
			IDEOGRAPHIC = 17,
			/// <summary>For programmatic determination of Ideographic Description Sequences.</summary>
			IDS_BINARY_OPERATOR = 18,
			/// <summary>For programmatic determination of Ideographic Description Sequences.</summary>
			IDS_TRINARY_OPERATOR = 19,
			/// <summary>Format controls for cursive joining and ligation.</summary>
			JOIN_CONTROL = 20,
			/// <summary>
			/// Characters that do not use logical order and require special
			/// handling in most processing.
			/// </summary>
			LOGICAL_ORDER_EXCEPTION = 21,
			/// <summary>Same as u_isULowercase, different from u_islower.</summary>
			/// <example>Ll+Other_Lowercase</example>
			LOWERCASE = 22,
			/// <summary>Sm+Other_Math</summary>
			MATH = 23,
			/// <summary>Code points that are explicitly defined as illegal for the encoding of characters.</summary>
			NONCHARACTER_CODE_POINT = 24,
			/// <summary>Binary property Quotation_Mark.</summary>
			QUOTATION_MARK = 25,
			/// <summary>For programmatic determination of Ideographic Description Sequences.</summary>
			RADICAL = 26,
			/// <summary>
			/// Characters with a "soft dot", like i or j. An accent placed on
			/// these characters causes the dot to disappear.
			/// </summary>
			SOFT_DOTTED = 27,
			/// <summary>Punctuation characters that generally mark the end of textual units.</summary>
			TERMINAL_PUNCTUATION = 28,
			/// <summary>For programmatic determination of Ideographic Description Sequences.</summary>
			UNIFIED_IDEOGRAPH = 29,
			/// <summary>Same as u_isUUppercase, different from u_isupper. Lu+Other_Uppercase</summary>
			UPPERCASE = 30,
			/// <summary>
			/// Same as u_isUWhiteSpace, different from u_isspace and
			/// u_isWhitespace. Space characters+TAB+CR+LF-ZWSP-ZWNBSP
			/// </summary>
			WHITE_SPACE = 31,
			/// <summary>ID_Continue modified to allow closure under normalization forms NFKC and NFKD.</summary>
			XID_CONTINUE = 32,
			/// <summary>ID_Start modified to allow closure under normalization forms NFKC and NFKD.</summary>
			XID_START = 33,
			/// <summary>
			/// Either the source of a case mapping or in the target of a case
			/// mapping. Not the same as the general category Cased_Letter.
			/// </summary>
			CASE_SENSITIVE = 34,
			/// <summary>Sentence Terminal. Used in UAX #29: Text Boundaries (http://www.unicode.org/reports/tr29/)</summary>
			S_TERM = 35,
			/// <summary>
			/// ICU-specific property for characters that are inert under NFD,
			/// i.e., they do not interact with adjacent characters. See the
			/// documentation for the Normalizer2 class and the
			/// Normalizer2::isInert() method.
			/// http://www.icu-project.org/apiref/icu4c/classicu_1_1Normalizer2.html
			/// </summary>
			VARIATION_SELECTOR = 36,
			/// <summary>
			/// ICU-specific property for characters that are inert under NFD,
			/// i.e., they do not interact with adjacent characters. See the
			/// documentation for the Normalizer2 class and the
			/// Normalizer2::isInert() method.
			/// http://www.icu-project.org/apiref/icu4c/classicu_1_1Normalizer2.html
			/// </summary>
			NFD_INERT = 37,
			/// <summary>
			/// ICU-specific property for characters that are inert under NFKD,
			/// i.e., they do not interact with adjacent characters. See the
			/// documentation for the Normalizer2 class and the
			/// Normalizer2::isInert() method.
			/// http://www.icu-project.org/apiref/icu4c/classicu_1_1Normalizer2.html
			/// </summary>
			NFKD_INERT = 38,
			/// <summary>
			/// ICU-specific property for characters that are inert under NFC,
			/// i.e., they do not interact with adjacent characters. See the
			/// documentation for the Normalizer2 class and the
			/// Normalizer2::isInert() method.
			/// http://www.icu-project.org/apiref/icu4c/classicu_1_1Normalizer2.html
			/// </summary>
			NFC_INERT = 39,
			/// <summary>
			/// ICU-specific property for characters that are inert under NFKC,
			/// i.e., they do not interact with adjacent characters. See the
			/// documentation for the Normalizer2 class and the
			/// Normalizer2::isInert() method.
			/// http://www.icu-project.org/apiref/icu4c/classicu_1_1Normalizer2.html
			/// </summary>
			NFKC_INERT = 40,
			/// <summary>
			/// ICU-specific property for characters that are starters in terms
			/// of Unicode normalization and combining character sequences. They
			/// have ccc=0 and do not occur in non-initial position of the
			/// canonical decomposition of any character (like a-umlaut in NFD
			/// and a Jamo T in an NFD(Hangul LVT)). ICU uses this property for
			/// segmenting a string for generating a set of canonically
			/// equivalent strings, e.g. for canonical closure while processing
			/// collation tailoring rules.
			/// </summary>
			SEGMENT_STARTER = 41,
			/// <summary>See UAX #31 Identifier and Pattern Syntax (http://www.unicode.org/reports/tr31/)</summary>
			PATTERN_SYNTAX = 42,
			/// <summary>See UAX #31 Identifier and Pattern Syntax (http://www.unicode.org/reports/tr31/)</summary>
			PATTERN_WHITE_SPACE = 43,
			/// <summary>
			/// Implemented according to the UTS #18 Annex C Standard
			/// Recommendation. See the uchar.h file documentation.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html
			/// </summary>
			POSIX_ALNUM = 44,
			/// <summary>
			/// Implemented according to the UTS #18 Annex C Standard
			/// Recommendation. See the uchar.h file documentation.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html
			/// </summary>
			POSIX_BLANK = 45,
			/// <summary>
			/// Implemented according to the UTS #18 Annex C Standard
			/// Recommendation. See the uchar.h file documentation.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html
			/// </summary>
			POSIX_GRAPH = 46,
			/// <summary>
			/// Implemented according to the UTS #18 Annex C Standard
			/// Recommendation. See the uchar.h file documentation.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html
			/// </summary>
			POSIX_PRINT = 47,
			/// <summary>
			/// Implemented according to the UTS #18 Annex C Standard
			/// Recommendation. See the uchar.h file documentation.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html
			/// </summary>
			POSIX_XDIGIT = 48,
			/// <summary>For Lowercase, Uppercase and Titlecase characters.</summary>
			CASED = 49,
			/// <summary>Used in context-sensitive case mappings.</summary>
			CASE_IGNORABLE = 50,
			/// <summary>Binary property Changes_When_Lowercased.</summary>
			CHANGES_WHEN_LOWERCASED = 51,
			/// <summary>Binary property Changes_When_Uppercased.</summary>
			CHANGES_WHEN_UPPERCASED = 52,
			/// <summary>Binary property Changes_When_Titlecased.</summary>
			CHANGES_WHEN_TITLECASED = 53,
			/// <summary>Binary property Changes_When_Casefolded.</summary>
			CHANGES_WHEN_CASEFOLDED = 54,
			/// <summary>Binary property Changes_When_Casemapped.</summary>
			CHANGES_WHEN_CASEMAPPED = 55,
			/// <summary>Binary property Changes_When_NFKC_Casefolded.</summary>
			CHANGES_WHEN_NFKC_CASEFOLDED = 56,
			/// <summary>Binary property EMOJI.</summary>
			EMOJI = 57,
			/// <summary>Binary property EMOJI_PRESENTATION.</summary>
			EMOJI_PRESENTATION = 58,
			/// <summary>Binary property EMOJI_MODIFIER.</summary>
			EMOJI_MODIFIER = 59,
			/// <summary>Binary property EMOJI_MODIFIER_BASE.</summary>
			EMOJI_MODIFIER_BASE = 60,
			/// <summary>Binary property EMOJI_COMPONENT.</summary>
			EMOJI_COMPONENT = 61,
			/// <summary>Binary property REGIONAL_INDICATOR.</summary>
			REGIONAL_INDICATOR = 62,
			/// <summary>Binary property PREPENDED_CONCATENATION_MARK.</summary>
			PREPENDED_CONCATENATION_MARK = 63,
			/// <summary>Binary property EXTENDED_PICTOGRAPHIC.</summary>
			EXTENDED_PICTOGRAPHIC = 64,
			/// <summary>One more than the last constant for binary Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			BINARY_LIMIT,

			/// <summary>Same as u_charDirection, returns UCharDirection values.</summary>
			BIDI_CLASS = 0x1000,
			/// <summary>First constant for enumerated/integer Unicode properties.</summary>
			INT_START = BIDI_CLASS,
			/// <summary>Same as ublock_getCode, returns UBlockCode values.</summary>
			BLOCK = 0x1001,
			/// <summary>Same as u_getCombiningClass, returns 8-bit numeric values.</summary>
			CANONICAL_COMBINING_CLASS = 0x1002,
			/// <summary>Returns UDecompositionType values.</summary>
			DECOMPOSITION_TYPE = 0x1003,
			/// <summary>See http://www.unicode.org/reports/tr11/ Returns UEastAsianWidth values.</summary>
			EAST_ASIAN_WIDTH = 0x1004,
			/// <summary>Same as u_charType, returns UCharCategory values.</summary>
			GENERAL_CATEGORY = 0x1005,
			/// <summary>Returns UJoiningGroup values.</summary>
			JOINING_GROUP = 0x1006,
			/// <summary>Returns UJoiningType values.</summary>
			JOINING_TYPE = 0x1007,
			/// <summary>Returns ULineBreak values.</summary>
			LINE_BREAK = 0x1008,
			/// <summary>Returns UNumericType values.</summary>
			NUMERIC_TYPE = 0x1009,
			/// <summary>Same as uscript_getScript, returns UScriptCode values.</summary>
			SCRIPT = 0x100A,
			/// <summary>Returns UHangulSyllableType values.</summary>
			HANGUL_SYLLABLE_TYPE = 0x100B,
			/// <summary>Returns UNormalizationCheckResult values.</summary>
			NFD_QUICK_CHECK = 0x100C,
			/// <summary>Returns UNormalizationCheckResult values.</summary>
			NFKD_QUICK_CHECK = 0x100D,
			/// <summary>Returns UNormalizationCheckResult values.</summary>
			NFC_QUICK_CHECK = 0x100E,
			/// <summary>Returns UNormalizationCheckResult values.</summary>
			NFKC_QUICK_CHECK = 0x100F,
			/// <summary>
			/// ICU-specific property for the ccc of the first code point of the
			/// decomposition, or lccc(c)=ccc(NFD(c)[0]). Useful for checking
			/// for canonically ordered text; see UNORM_FCD and
			/// http://www.unicode.org/notes/tn5/#FCD . Returns 8-bit numeric
			/// values like UCHAR_CANONICAL_COMBINING_CLASS.
			/// </summary>
			LEAD_CANONICAL_COMBINING_CLASS = 0x1010,
			/// <summary>
			/// ICU-specific property for the ccc of the last code point of the
			/// decomposition, or tccc(c)=ccc(NFD(c)[last]). Useful for checking
			/// for canonically ordered text; see UNORM_FCD and
			/// http://www.unicode.org/notes/tn5/#FCD . Returns 8-bit numeric
			/// values like UCHAR_CANONICAL_COMBINING_CLASS.
			/// </summary>
			TRAIL_CANONICAL_COMBINING_CLASS = 0x1011,
			/// <summary>
			/// Used in UAX #29: Text Boundaries (http://www.unicode.org/reports/tr29/)
			/// Returns UGraphemeClusterBreak values.
			/// </summary>
			GRAPHEME_CLUSTER_BREAK = 0x1012,
			/// <summary>
			/// Used in UAX #29: Text Boundaries (http://www.unicode.org/reports/tr29/)
			/// Returns USentenceBreak values
			/// </summary>
			SENTENCE_BREAK = 0x1013,
			/// <summary>
			/// Used in UAX #29: Text Boundaries (http://www.unicode.org/reports/tr29/)
			/// Returns UWordBreakValues values.
			/// </summary>
			WORD_BREAK = 0x1014,
			/// <summary>
			/// Used in UAX #9: Unicode Bidirectional Algorithm (http://www.unicode.org/reports/tr9/)
			/// Returns UBidiPairedBracketType values.
			/// </summary>
			BIDI_PAIRED_BRACKET_TYPE = 0x1015,
			/// <summary>One more than the last constant for enumerated/integer Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			INT_LIMIT = 0x1016,

			/// <summary>
			/// This is the General_Category property returned as a bit mask.
			/// When used in u_getIntPropertyValue(c), same as
			/// U_MASK(u_charType(c)), returns bit masks for UCharCategory
			/// values where exactly one bit is set. When used with
			/// u_getPropertyValueName() and u_getPropertyValueEnum(), a
			/// multi-bit mask is used for sets of categories like "Letters".
			/// Mask values should be cast to uint32_t.
			/// http://icu-project.org/apiref/icu4c/uchar_8h.html#a3f694e48867909fbe555586f2b3565be
			/// </summary>
			GENERAL_CATEGORY_MASK = 0x2000,
			/// <summary>First constant for bit-mask Unicode properties.</summary>
			MASK_START = GENERAL_CATEGORY_MASK,
			/// <summary>One more than the last constant for bit-mask Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			MASK_LIMIT = 0x2001,

			/// <summary>Corresponds to u_getNumericValue.</summary>
			NUMERIC_VALUE = 0x3000,
			/// <summary>First constant for double Unicode properties.</summary>
			DOUBLE_START = NUMERIC_VALUE,
			/// <summary>One more than the last constant for double Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			DOUBLE_LIMIT = 0x3001,

			/// <summary>Corresponds to u_charAge.</summary>
			AGE = 0x4000,
			/// <summary>First constant for string Unicode properties.</summary>
			STRING_START = AGE,
			/// <summary>Corresponds to u_charMirror.</summary>
			BIDI_MIRRORING_GLYPH = 0x4001,
			/// <summary>Corresponds to u_strFoldCase in ustring.h (http://icu-project.org/apiref/icu4c/ustring_8h.html).</summary>
			CASE_FOLDING = 0x4002,
			/// <summary>Corresponds to u_getISOComment.</summary>
			[Obsolete("ICU 49")]
			ISO_COMMENT = 0x4003,
			/// <summary>Corresponds to u_strToLower in ustring.h (http://icu-project.org/apiref/icu4c/ustring_8h.html).</summary>
			LOWERCASE_MAPPING = 0x4004,
			/// <summary>Corresponds to u_charName.</summary>
			NAME = 0x4005,
			/// <summary>Corresponds to u_foldCase.</summary>
			SIMPLE_CASE_FOLDING = 0x4006,
			/// <summary> Corresponds to u_tolower.</summary>
			SIMPLE_LOWERCASE_MAPPING = 0x4007,
			/// <summary> Corresponds to u_totitle.</summary>
			SIMPLE_TITLECASE_MAPPING = 0x4008,
			/// <summary> Corresponds to u_toupper.</summary>
			SIMPLE_UPPERCASE_MAPPING = 0x4009,
			/// <summary>Corresponds to u_strToTitle in ustring.h  (http://icu-project.org/apiref/icu4c/ustring_8h.html).</summary>
			TITLECASE_MAPPING = 0x400A,
			/// <summary>
			/// This property is of little practical value. Beginning with
			/// ICU 49, ICU APIs return an empty string for this property.
			/// Corresponds to u_charName(U_UNICODE_10_CHAR_NAME).
			/// </summary>
			[Obsolete("ICU 49")]
			UNICODE_1_NAME = 0x400B,
			/// <summary>Corresponds to u_strToUpper in ustring.h (http://icu-project.org/apiref/icu4c/ustring_8h.html).</summary>
			UPPERCASE_MAPPING = 0x400C,
			/// <summary>Corresponds to u_getBidiPairedBracket.</summary>
			BIDI_PAIRED_BRACKET = 0x400D,
			/// <summary>One more than the last constant for string Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			STRING_LIMIT = 0x400E,

			/// <summary>
			/// Some characters are commonly used in multiple scripts. For more
			/// information, see UAX #24: http://www.unicode.org/reports/tr24/.
			/// Corresponds to uscript_hasScript and uscript_getScriptExtensions
			/// in uscript.h. http://icu-project.org/apiref/icu4c/uscript_8h.html
			/// </summary>
			SCRIPT_EXTENSIONS = 0x7000,
			/// <summary>First constant for Unicode properties with unusual value types</summary>
			OTHER_PROPERTY_START = SCRIPT_EXTENSIONS,
			/// <summary>One more than the last constant for Unicode properties with unusual value types.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			OTHER_PROPERTY_LIMIT = 0x7001,
			/// <summary>Represents a nonexistent or invalid property or property value.</summary>
			INVALID_CODE = -1
		}

		///<summary>
		/// enumerated Unicode general category types.
		/// See http://www.unicode.org/Public/UNIDATA/UnicodeData.html .
		/// </summary>
		public enum UCharCategory
		{
			///<summary>Non-category for unassigned and non-character code points.</summary>
			UNASSIGNED = 0,
			///<summary>Cn "Other, Not Assigned (no characters in [UnicodeData.txt] have this property)" (same as U_UNASSIGNED!)</summary>
			GENERAL_OTHER_TYPES = 0,
			///<summary>Lu</summary>
			UPPERCASE_LETTER = 1,
			///<summary>Ll</summary>
			LOWERCASE_LETTER = 2,
			///<summary>Lt</summary>
			TITLECASE_LETTER = 3,
			///<summary>Lm</summary>
			MODIFIER_LETTER = 4,
			///<summary>Lo</summary>
			OTHER_LETTER = 5,
			///<summary>Mn</summary>
			NON_SPACING_MARK = 6,
			///<summary>Me</summary>
			ENCLOSING_MARK = 7,
			///<summary>Mc</summary>
			COMBINING_SPACING_MARK = 8,
			///<summary>Nd</summary>
			DECIMAL_DIGIT_NUMBER = 9,
			///<summary>Nl</summary>
			LETTER_NUMBER = 10,
			///<summary>No</summary>
			OTHER_NUMBER = 11,
			///<summary>Zs</summary>
			SPACE_SEPARATOR = 12,
			///<summary>Zl</summary>
			LINE_SEPARATOR = 13,
			///<summary>Zp</summary>
			PARAGRAPH_SEPARATOR = 14,
			///<summary>Cc</summary>
			CONTROL_CHAR = 15,
			///<summary>Cf</summary>
			FORMAT_CHAR = 16,
			///<summary>Co</summary>
			PRIVATE_USE_CHAR = 17,
			///<summary>Cs</summary>
			SURROGATE = 18,
			///<summary>Pd</summary>
			DASH_PUNCTUATION = 19,
			///<summary>Ps</summary>
			START_PUNCTUATION = 20,
			///<summary>Pe</summary>
			END_PUNCTUATION = 21,
			///<summary>Pc</summary>
			CONNECTOR_PUNCTUATION = 22,
			///<summary>Po</summary>
			OTHER_PUNCTUATION = 23,
			///<summary>Sm</summary>
			MATH_SYMBOL = 24,
			///<summary>Sc</summary>
			CURRENCY_SYMBOL = 25,
			///<summary>Sk</summary>
			MODIFIER_SYMBOL = 26,
			///<summary>So</summary>
			OTHER_SYMBOL = 27,
			///<summary>Pi</summary>
			INITIAL_PUNCTUATION = 28,
			///<summary>Pf</summary>
			FINAL_PUNCTUATION = 29,
			///<summary>One higher than the last enum UCharCategory constant.</summary>
			CHAR_CATEGORY_COUNT
		}

		/// <summary>
		/// Selector constants for u_charName().
		/// u_charName() returns the "modern" name of a Unicode character; or the name that was
		/// defined in Unicode version 1.0, before the Unicode standard merged with ISO-10646; or
		/// an "extended" name that gives each Unicode code point a unique name.
		/// </summary>
		public enum UCharNameChoice
		{
			/// <summary>Unicode character name (Name property). </summary>
			UNICODE_CHAR_NAME,
			/// <summary>The Unicode_1_Name property value which is of little practical value.
			/// Beginning with ICU 49, ICU APIs return an empty string for this name choice. </summary>
			[Obsolete("ICU 49")]
			UNICODE_10_CHAR_NAME,
			/// <summary>Standard or synthetic character name. </summary>
			EXTENDED_CHAR_NAME,
			/// <summary>Corrected name from NameAliases.txt. </summary>
			NAME_ALIAS,
			/// <summary>One more than the highest normal UCharNameChoice value. </summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			CHAR_NAME_CHOICE_COUNT
		}

		/// <summary>
		/// Decomposition Type constants.
		/// </summary>
		/// <remarks>
		/// Note: UDecompositionType constants are parsed by preparseucd.py.
		/// It matches lines like U_DT_&lt;Unicode Decomposition_Type value name&gt;
		/// </remarks>
		public enum UDecompositionType
		{
			/// <summary>[none]</summary>
			NONE,
			/// <summary>[can]</summary>
			CANONICAL,
			/// <summary>[com]</summary>
			COMPAT,
			/// <summary>[enc]</summary>
			CIRCLE,
			/// <summary>[fin]</summary>
			FINAL,
			/// <summary>[font]</summary>
			FONT,
			/// <summary>[fra]</summary>
			FRACTION,
			/// <summary>[init]</summary>
			INITIAL,
			/// <summary>[iso]</summary>
			ISOLATED,
			/// <summary>[med]</summary>
			MEDIAL,
			/// <summary>[nar]</summary>
			NARROW,
			/// <summary>[nb]</summary>
			NOBREAK,
			/// <summary>[sml]</summary>
			SMALL,
			/// <summary>[sqr]</summary>
			SQUARE,
			/// <summary>[sub]</summary>
			SUB,
			/// <summary>[sup]</summary>
			SUPER,
			/// <summary>[vert]</summary>
			VERTICAL,
			/// <summary>[wide]</summary>
			WIDE,
			/// <summary>18</summary>
			COUNT
		}

		/// <summary>
		/// Numeric Type constants
		/// </summary>
		/// <remarks>Note: UNumericType constants are parsed by preparseucd.py.
		/// It matches lines like U_NT_&lt;Unicode Numeric_Type value name&gt;</remarks>
		public enum UNumericType
		{
			/// <summary>[None]</summary>
			NONE,
			/// <summary>[de]</summary>
			DECIMAL,
			/// <summary>[di]</summary>
			DIGIT,
			/// <summary>[nu]</summary>
			NUMERIC,
			/// <summary></summary>
			COUNT
		}

		/// <summary>
		/// BIDI direction constants
		/// </summary>
		public enum UCharDirection
		{
			/// <summary>L.</summary>
			LEFT_TO_RIGHT = 0,

			/// <summary>R.</summary>
			RIGHT_TO_LEFT = 1,

			/// <summary>EN.</summary>
			EUROPEAN_NUMBER = 2,

			/// <summary>ES.</summary>
			EUROPEAN_NUMBER_SEPARATOR = 3,

			/// <summary>ET.</summary>
			EUROPEAN_NUMBER_TERMINATOR = 4,

			/// <summary>AN.</summary>
			ARABIC_NUMBER = 5,

			/// <summary>CS.</summary>
			COMMON_NUMBER_SEPARATOR = 6,

			/// <summary>B.</summary>
			BLOCK_SEPARATOR = 7,

			/// <summary>S.</summary>
			SEGMENT_SEPARATOR = 8,

			/// <summary>WS.</summary>
			WHITE_SPACE_NEUTRAL = 9,

			/// <summary>ON.</summary>
			OTHER_NEUTRAL = 10,

			/// <summary>LRE.</summary>
			LEFT_TO_RIGHT_EMBEDDING = 11,

			/// <summary>LRO.</summary>
			LEFT_TO_RIGHT_OVERRIDE = 12,

			/// <summary>AL.</summary>
			RIGHT_TO_LEFT_ARABIC = 13,

			/// <summary>RLE.</summary>
			RIGHT_TO_LEFT_EMBEDDING = 14,

			/// <summary>RLO.</summary>
			RIGHT_TO_LEFT_OVERRIDE = 15,

			/// <summary>PDF.</summary>
			POP_DIRECTIONAL_FORMAT = 16,

			/// <summary>NSM.</summary>
			DIR_NON_SPACING_MARK = 17,

			/// <summary>BN.</summary>
			BOUNDARY_NEUTRAL = 18,

			/// <summary>FSI.</summary>
			FIRST_STRONG_ISOLATE = 19,

			/// <summary>LRI.</summary>
			LEFT_TO_RIGHT_ISOLATE = 20,

			/// <summary>RLI.</summary>
			RIGHT_TO_LEFT_ISOLATE = 21,

			/// <summary>PDI.</summary>
			POP_DIRECTIONAL_ISOLATE = 22,

			/// <summary>One more than the highest UCharDirection value.
			/// The highest value is available via u_getIntPropertyMaxValue(BIDI_CLASS).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			CHAR_DIRECTION_COUNT
		}

		/// <summary>
		/// Special value that is returned by <see cref="GetNumericValue(int)"/>
		/// when no numeric value is defined for a code point.
		/// </summary>
		public const double NO_NUMERIC_VALUE = -123456789;

		/// <summary>
		/// Returns the decimal digit value of the code point in the specified radix.
		/// </summary>
		/// <param name="characterCode">The code point to be tested</param>
		/// <param name="radix">The radix</param>
		public static int Digit(int characterCode, byte radix)
		{
			return NativeMethods.u_digit(characterCode, radix);
		}

		/// <summary>
		/// Determines whether the specified character code is alphabetic, based on the
		/// UProperty.ALPHABETIC property.
		/// </summary>
		/// <param name="characterCode">The character code.</param>
		public static bool IsAlphabetic(int characterCode)
		{
			return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.ALPHABETIC) != 0;
		}

		/// <summary>
		/// Determines whether the specified character code is ideographic, based on the
		/// UProperty.IDEOGRAPHIC property.
		/// </summary>
		/// <param name="characterCode">The character code.</param>
		public static bool IsIdeographic(int characterCode)
		{
			return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.IDEOGRAPHIC) != 0;
		}

		/// <summary>
		/// Determines whether the specified character code is alphabetic, based on the
		/// UProperty.DIACRITIC property.
		/// </summary>
		/// <param name="characterCode">The character code.</param>
		public static bool IsDiacritic(int characterCode)
		{
			return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.DIACRITIC) != 0;
		}

		/// <summary>
		///	Determines whether the specified code point is a symbol character
		/// </summary>
		/// <param name="characterCode">the code point to be tested</param>
		public static bool IsSymbol(int characterCode)
		{
			switch (GetCharType(characterCode))
			{
				case UCharCategory.MATH_SYMBOL:
				case UCharCategory.CURRENCY_SYMBOL:
				case UCharCategory.MODIFIER_SYMBOL:
				case UCharCategory.OTHER_SYMBOL:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determines whether the specified character is a letter, i.e. if code point is in the
		/// category Lu, Ll, Lt, Lm and Lo.
		/// </summary>
		public static bool IsLetter(int characterCode)
		{
			switch (GetCharType(characterCode))
			{
				case UCharCategory.UPPERCASE_LETTER:
				case UCharCategory.LOWERCASE_LETTER:
				case UCharCategory.TITLECASE_LETTER:
				case UCharCategory.MODIFIER_LETTER:
				case UCharCategory.OTHER_LETTER:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determines whether the specified character is a mark, i.e. if code point is in the
		/// category Mn, Me and Mc.
		/// </summary>
		public static bool IsMark(int characterCode)
		{
			switch (GetCharType(characterCode))
			{
				case UCharCategory.NON_SPACING_MARK:
				case UCharCategory.ENCLOSING_MARK:
				case UCharCategory.COMBINING_SPACING_MARK:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determines whether the specified character is a separator, i.e. if code point is in
		/// the category Zs, Zl and Zp.
		/// </summary>
		public static bool IsSeparator(int characterCode)
		{
			switch (GetCharType(characterCode))
			{
				case UCharCategory.SPACE_SEPARATOR:
				case UCharCategory.LINE_SEPARATOR:
				case UCharCategory.PARAGRAPH_SEPARATOR:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determines whether the specified character code is numeric, based on the
		/// UProperty.NUMERIC_TYPE property.
		/// </summary>
		/// <param name="characterCode">The character code.</param>
		public static bool IsNumeric(int characterCode)
		{
			return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.NUMERIC_TYPE) != 0;
		}

		/// <summary>Determines whether the specified code point is a punctuation character, as
		/// defined by the ICU NativeMethods.u_ispunct function.</summary>
		public static bool IsPunct(int characterCode)
		{
			return NativeMethods.u_ispunct(characterCode);
		}

		/// <summary>Determines whether the code point has the Bidi_Mirrored property. </summary>
		public static bool IsMirrored(int characterCode)
		{
			return NativeMethods.u_isMirrored(characterCode);
		}

		/// <summary>Determines whether the specified code point is a control character, as
		/// defined by the ICU NativeMethods.u_iscntrl function.</summary>
		public static bool IsControl(int characterCode)
		{
			return NativeMethods.u_iscntrl(characterCode);
		}

		/// <summary>
		///	Determines whether the specified character is a control character. A control
		///	character is one of the following:
		/// <list>
		///	<item>ISO 8-bit control character (U+0000..U+001f and U+007f..U+009f)</item>
		///	<item>U_CONTROL_CHAR (Cc)</item>
		///	<item>U_FORMAT_CHAR (Cf)</item>
		///	<item>U_LINE_SEPARATOR (Zl)</item>
		///	<item>U_PARAGRAPH_SEPARATOR (Zp)</item>
		///	</list>
		/// </summary>
		public static bool IsControl(string chr)
		{
			return !string.IsNullOrEmpty(chr) && chr.Length == 1 && IsControl(chr[0]);
		}

		/// <summary>Determines whether the specified character is a space character, as
		/// defined by the ICU NativeMethods.u_isspace function.</summary>
		public static bool IsSpace(int characterCode)
		{
			return NativeMethods.u_isspace(characterCode);
		}

		/// <summary>
		///	Determines whether the specified character is a space character.
		/// </summary>
		public static bool IsSpace(string chr)
		{
			return !string.IsNullOrEmpty(chr) && chr.Length == 1 && IsSpace(chr[0]);
		}

		///<summary>
		/// Get the general character category value for the given code point.
		///</summary>
		///<param name="ch">the code point to be checked</param>
		///<returns></returns>
		public static UCharCategory GetCharType(int ch)
		{
			return (UCharCategory)NativeMethods.u_charType(ch);
		}

		/// <summary></summary>
		public static double GetNumericValue(int characterCode)
		{
			return NativeMethods.u_getNumericValue(characterCode);
		}

		/// <summary>
		/// Returns the bidirectional category value for the code point, which is used in the
		/// Unicode bidirectional algorithm (UAX #9 http://www.unicode.org/reports/tr9/).
		/// </summary>
		/// <param name="code">the code point to be tested </param>
		/// <returns>the bidirectional category (UCharDirection) value</returns>
		/// <remarks><para>Note that some unassigned code points have bidi values of R or AL because
		/// they are in blocks that are reserved for Right-To-Left scripts.</para>
		/// <para>Same as java.lang.Character.getDirectionality()</para></remarks>
		public static UCharDirection CharDirection(int code)
		{
			return (UCharDirection)NativeMethods.u_charDirection(code);
		}

		/// <summary>
		/// Get the description for a given ICU code point.
		/// </summary>
		/// <param name="code">the code point to get description/name of</param>
		/// <param name="nameChoice">what type of information to retrieve</param>
		/// <param name="name">return string</param>
		/// <returns>length of string</returns>
		private static int CharName(int code, UCharNameChoice nameChoice, out string name)
		{
			name = NativeMethods.GetAnsiString((ptr, length) =>
			{
				length = NativeMethods.u_charName(code, nameChoice, ptr, length, out var err);
				return new Tuple<ErrorCode, int>(err, length);
			});
			return name.Length;
		}

		/// <summary>
		/// Gets the ICU display name of the specified character.
		/// </summary>
		public static string GetPrettyICUCharName(string chr)
		{
			if (!string.IsNullOrEmpty(chr) && chr.Length == 1)
			{
				string name;
				if (CharName(chr[0], UCharNameChoice.UNICODE_CHAR_NAME, out name) > 0)
				{
					var lowercase = CultureInfo.CurrentUICulture.TextInfo.ToLower(name);
					return UnicodeString.ToTitle(lowercase, new Locale());
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the raw ICU display name of the specified character code.
		/// </summary>
		public static string GetCharName(int code)
		{
			string name;
			if (CharName(code, UCharNameChoice.UNICODE_CHAR_NAME, out name) > 0)
			{
				return name;
			}
			return null;
		}

		/// <summary>
		/// Get the property value for an enumerated or integer Unicode property for a code point.
		/// Also returns binary and mask property values.
		/// </summary>
		/// <param name="codePoint">Code point to test. </param>
		/// <param name="which">UProperty selector constant, identifies which property to check.
		/// Must be UProperty.BINARY_START &lt;= which &lt; UProperty.BINARY_LIMIT or
		/// UProperty.INT_START &lt;= which &lt; UProperty.INT_LIMIT or
		/// UProperty.MASK_START &lt;= which &lt; UProperty.MASK_LIMIT.</param>
		/// <returns>
		/// Numeric value that is directly the property value or, for enumerated properties,
		/// corresponds to the numeric value of the enumerated constant of the respective property
		/// value enumeration type (cast to enum type if necessary).
		/// Returns 0 or 1 (for <c>false</c>/<c>true</c>) for binary Unicode properties.
		/// Returns a bit-mask for mask properties.
		/// Returns 0 if 'which' is out of bounds or if the Unicode version does not have data for
		/// the property at all, or not for this code point.
		/// </returns>
		/// <remarks>
		/// Unicode, especially in version 3.2, defines many more properties than the original set
		/// in UnicodeData.txt.
		/// The properties APIs are intended to reflect Unicode properties as defined in the
		/// Unicode Character Database (UCD) and Unicode Technical Reports (UTR). For details
		/// about the properties <see href="http: //www.unicode.org/"/> . For names of Unicode
		/// properties see the UCD file <see cref="PropertyAliases.txt"/>.
		/// </remarks>
		public static int GetIntPropertyValue(int codePoint, UProperty which)
		{
			return NativeMethods.u_getIntPropertyValue(codePoint, which);
		}

		/// <summary>
		/// The given character is mapped to its lowercase equivalent according to UnicodeData.txt;
		/// if the character has no lowercase equivalent, the character itself is returned.
		///
		/// This function only returns the simple, single-code point case mapping. Full case
		/// mappings should be used whenever possible because they produce better results by
		/// working on whole strings. They take into account the string context and the language
		/// and can map to a result string with a different length as appropriate. Full case
		/// mappings are applied by the string case mapping functions, <see cref="UnicodeString"/>
		/// See also the User Guide chapter on C/POSIX migration:
		/// <seealso href="http: //icu-project.org/userguide/posix.html#case_mappings"/>
		/// </summary>
		/// <param name="codePoint">the code point to be mapped </param>
		/// <returns>the Simple_Lowercase_Mapping of the code point, if any; otherwise the code
		/// point itself. </returns>
		public static int ToLower(int codePoint)
		{
			return NativeMethods.u_tolower(codePoint);
		}

		/// <summary>
		/// The given character is mapped to its titlecase equivalent according to UnicodeData.txt;
		/// if the character has no lowercase equivalent, the character itself is returned.
		///
		/// This function only returns the simple, single-code point case mapping. Full case
		/// mappings should be used whenever possible because they produce better results by
		/// working on whole strings. They take into account the string context and the language
		/// and can map to a result string with a different length as appropriate. Full case
		/// mappings are applied by the string case mapping functions, <see cref="UnicodeString"/>
		/// See also the User Guide chapter on C/POSIX migration:
		/// <seealso href="http: //icu-project.org/userguide/posix.html#case_mappings"/>
		/// </summary>
		/// <param name="codePoint">the code point to be mapped </param>
		/// <returns>the Simple_Titlecase_Mapping of the code point, if any; otherwise the code
		/// point itself. </returns>
		public static int ToTitle(int codePoint)
		{
			return NativeMethods.u_totitle(codePoint);
		}

		/// <summary>
		/// The given character is mapped to its uppercase equivalent according to UnicodeData.txt;
		/// if the character has no lowercase equivalent, the character itself is returned.
		///
		/// This function only returns the simple, single-code point case mapping. Full case
		/// mappings should be used whenever possible because they produce better results by
		/// working on whole strings. They take into account the string context and the language
		/// and can map to a result string with a different length as appropriate. Full case
		/// mappings are applied by the string case mapping functions, <see cref="UnicodeString"/>
		/// See also the User Guide chapter on C/POSIX migration:
		/// <seealso href="http: //icu-project.org/userguide/posix.html#case_mappings"/>
		/// </summary>
		/// <param name="codePoint">the code point to be mapped </param>
		/// <returns>the Simple_Uppercase_Mapping of the code point, if any; otherwise the code
		/// point itself. </returns>
		public static int ToUpper(int codePoint)
		{
			return NativeMethods.u_toupper(codePoint);
		}

	}
}
