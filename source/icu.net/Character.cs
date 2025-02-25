// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;

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
		/// <remarks>
		/// Note: UProperty constants are parsed by preparseucd.py.
		/// It matches lines like
		///		UCHAR_{Unicode property name}={integer},
		/// </remarks>
		public enum UProperty
		{
			/* Comments of the form "Binary property Dash",
			"Enumerated property Script", "Double property Numeric_Value",
			and "String property Age" are read by genpname. */

			/* Note!!!
			Place ALPHABETIC before BINARY_START so that debuggers display
			ALPHABETIC as the symbolic name for 0, rather than BINARY_START.
			Likewise for other *_START identifiers. */

			/* Binary Unicode properties (0-999) */

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

			/* Enumerated/integer Unicode properties (0x1000-1999) */

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

			/* Bit-mask Unicode properties (0x2000-2999) */

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

			/* Double Unicode properties (0x3000-3999) */

			/// <summary>Corresponds to u_getNumericValue.</summary>
			NUMERIC_VALUE = 0x3000,
			/// <summary>First constant for double Unicode properties.</summary>
			DOUBLE_START = NUMERIC_VALUE,
			/// <summary>One more than the last constant for double Unicode properties.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			DOUBLE_LIMIT = 0x3001,

			/* String Unicode properties (0x4000-4999) */

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

			/* Unicode properties with unusual value types (0x7000-7999) */

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

			/* Nonexistent/invalid property (-1) */

			/// <summary>Represents a nonexistent or invalid property or property value.</summary>
			INVALID_CODE = -1
		}

		/// <summary>
		/// enumerated Unicode general category types.
		/// See http://www.unicode.org/Public/UNIDATA/UnicodeData.html .
		/// </summary>
		/// <remarks>
		/// Note: UCharCategory constants and their API comments are parsed by preparseucd.py.
		/// It matches pairs of lines like
		/// 	/ ** {Unicode 2-letter General_Category value} comment... * /
		/// 	U_{[A-Z_]+} = {integer},
		/// </remarks>
		public enum UCharCategory
		{
			/* Stable in ICU 2.0 */

			///<summary>Non-category for unassigned and non-character code points.</summary>
			UNASSIGNED = 0,
			/// <summary>
			/// Cn "Other, Not Assigned (no characters in [UnicodeData.txt] have this property)"
			/// </summary>
			GENERAL_OTHER_TYPES = UNASSIGNED,
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
			/// <summary>One higher than the last enum UCharCategory constant.
			/// This numeric value is stable (will not change), see
			/// http://www.unicode.org/policies/stability_policy.html#Property_Value</summary>
			CHAR_CATEGORY_COUNT
		}

		/// <summary>
		/// BIDI direction constants
		/// </summary>
		/// <remarks>
		/// Note: UCharDirection constants and their API comments are parsed by preparseucd.py.
		/// It matches pairs of lines like
		/// 	/ ** {Unicode 1..3-letter Bidi_Class value} comment... * /
		/// 	U_{[A-Z_]+} = {integer},
		/// </remarks>
		public enum UCharDirection
		{
			/* Stable in ICU 2.0 */

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

			/* Stable in ICU 52 */

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
		/// Bidi Paired Bracket Type constants.
		/// </summary>
		/// <remarks>
		/// Note: UBidiPairedBracketType constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_BPT_{Unicode Bidi_Paired_Bracket_Type value name}
		/// </remarks>
		public enum UBidiPairedBracketType
		{
			/* Stable in ICU 52 */

			/// <summary>Not a paired bracket</summary>
			NONE,
			/// <summary>Open paired bracket</summary>
			OPEN,
			/// <summary>Close paired bracket</summary>
			CLOSE,

			/// <summary>One more than the highest normal UBidiPairedBracketType value</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Constants for Unicode blocks, see the Unicode Data file Blocks.txt
		/// </summary>
		/// <remarks>
		/// Note: UBlockCode constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	UBLOCK_{Unicode Block value name} = {integer},
		/// </remarks>
		public enum UBlockCode
		{
			/* Stable in ICU 2.0 */

			/// <summary>[none] Special range indicating No_Block</summary>
			NO_BLOCK = 0,
			/// <summary>[0000]</summary>
			BASIC_LATIN = 1,
			/// <summary>[0080]</summary>
			LATIN_1_SUPPLEMENT = 2,
			/// <summary>[0100]</summary>
			LATIN_EXTENDED_A = 3,
			/// <summary>[0180]</summary>
			LATIN_EXTENDED_B = 4,
			/// <summary>[0250]</summary>
			IPA_EXTENSIONS = 5,
			/// <summary>[02B0]</summary>
			SPACING_MODIFIER_LETTERS = 6,
			/// <summary>[0300]</summary>
			COMBINING_DIACRITICAL_MARKS = 7,
			/// <summary>[0370] Unicode 3.2 renames this block to "Greek and Coptic".</summary>
			GREEK = 8,
			/// <summary>[0400]</summary>
			CYRILLIC = 9,
			/// <summary>[0530]</summary>
			ARMENIAN = 10,
			/// <summary>[0590]</summary>
			HEBREW = 11,
			/// <summary>[0600]</summary>
			ARABIC = 12,
			/// <summary>[0700]</summary>
			SYRIAC = 13,
			/// <summary>[0780]</summary>
			THAANA = 14,
			/// <summary>[0900]</summary>
			DEVANAGARI = 15,
			/// <summary>[0980]</summary>
			BENGALI = 16,
			/// <summary>[0A00]</summary>
			GURMUKHI = 17,
			/// <summary>[0A80]</summary>
			GUJARATI = 18,
			/// <summary>[0B00]</summary>
			ORIYA = 19,
			/// <summary>[0B80]</summary>
			TAMIL = 20,
			/// <summary>[0C00]</summary>
			TELUGU = 21,
			/// <summary>[0C80]</summary>
			KANNADA = 22,
			/// <summary>[0D00]</summary>
			MALAYALAM = 23,
			/// <summary>[0D80]</summary>
			SINHALA = 24,
			/// <summary>[0E00]</summary>
			THAI = 25,
			/// <summary>[0E80]</summary>
			LAO = 26,
			/// <summary>[0F00]</summary>
			TIBETAN = 27,
			/// <summary>[1000]</summary>
			MYANMAR = 28,
			/// <summary>[10A0]</summary>
			GEORGIAN = 29,
			/// <summary>[1100]</summary>
			HANGUL_JAMO = 30,
			/// <summary>[1200]</summary>
			ETHIOPIC = 31,
			/// <summary>[13A0]</summary>
			CHEROKEE = 32,
			/// <summary>[1400]</summary>
			UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS = 33,
			/// <summary>[1680]</summary>
			OGHAM = 34,
			/// <summary>[16A0]</summary>
			RUNIC = 35,
			/// <summary>[1780]</summary>
			KHMER = 36,
			/// <summary>[1800]</summary>
			MONGOLIAN = 37,
			/// <summary>[1E00]</summary>
			LATIN_EXTENDED_ADDITIONAL = 38,
			/// <summary>[1F00]</summary>
			GREEK_EXTENDED = 39,
			/// <summary>[2000]</summary>
			GENERAL_PUNCTUATION = 40,
			/// <summary>[2070]</summary>
			SUPERSCRIPTS_AND_SUBSCRIPTS = 41,
			/// <summary>[20A0]</summary>
			CURRENCY_SYMBOLS = 42,
			/// <summary>
			/// [20D0] Unicode 3.2 renames this block to "Combining Diacritical Marks for Symbols".
			/// </summary>
			COMBINING_MARKS_FOR_SYMBOLS = 43,
			/// <summary>[2100]</summary>
			LETTERLIKE_SYMBOLS = 44,
			/// <summary>[2150]</summary>
			NUMBER_FORMS = 45,
			/// <summary>[2190]</summary>
			ARROWS = 46,
			/// <summary>[2200]</summary>
			MATHEMATICAL_OPERATORS = 47,
			/// <summary>[2300]</summary>
			MISCELLANEOUS_TECHNICAL = 48,
			/// <summary>[2400]</summary>
			CONTROL_PICTURES = 49,
			/// <summary>[2440]</summary>
			OPTICAL_CHARACTER_RECOGNITION = 50,
			/// <summary>[2460]</summary>
			ENCLOSED_ALPHANUMERICS = 51,
			/// <summary>[2500]</summary>
			BOX_DRAWING = 52,
			/// <summary>[2580]</summary>
			BLOCK_ELEMENTS = 53,
			/// <summary>[25A0]</summary>
			GEOMETRIC_SHAPES = 54,
			/// <summary>[2600]</summary>
			MISCELLANEOUS_SYMBOLS = 55,
			/// <summary>[2700]</summary>
			DINGBATS = 56,
			/// <summary>[2800]</summary>
			BRAILLE_PATTERNS = 57,
			/// <summary>[2E80]</summary>
			CJK_RADICALS_SUPPLEMENT = 58,
			/// <summary>[2F00]</summary>
			KANGXI_RADICALS = 59,
			/// <summary>[2FF0]</summary>
			IDEOGRAPHIC_DESCRIPTION_CHARACTERS = 60,
			/// <summary>[3000]</summary>
			CJK_SYMBOLS_AND_PUNCTUATION = 61,
			/// <summary>[3040]</summary>
			HIRAGANA = 62,
			/// <summary>[30A0]</summary>
			KATAKANA = 63,
			/// <summary>[3100]</summary>
			BOPOMOFO = 64,
			/// <summary>[3130]</summary>
			HANGUL_COMPATIBILITY_JAMO = 65,
			/// <summary>[3190]</summary>
			KANBUN = 66,
			/// <summary>[31A0]</summary>
			BOPOMOFO_EXTENDED = 67,
			/// <summary>[3200]</summary>
			ENCLOSED_CJK_LETTERS_AND_MONTHS = 68,
			/// <summary>[3300]</summary>
			CJK_COMPATIBILITY = 69,
			/// <summary>[3400]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A = 70,
			/// <summary>[4E00]</summary>
			CJK_UNIFIED_IDEOGRAPHS = 71,
			/// <summary>[A000]</summary>
			YI_SYLLABLES = 72,
			/// <summary>[A490]</summary>
			YI_RADICALS = 73,
			/// <summary>[AC00]</summary>
			HANGUL_SYLLABLES = 74,
			/// <summary>[D800]</summary>
			HIGH_SURROGATES = 75,
			/// <summary>[DB80]</summary>
			HIGH_PRIVATE_USE_SURROGATES = 76,
			/// <summary>[DC00]</summary>
			LOW_SURROGATES = 77,
			/// <summary>
			/// [E000] Unicode 3.2 renames the block for the BMP PUA to "Private Use Area"
			/// and adds separate blocks for the supplementary PUAs.</summary>
			PRIVATE_USE_AREA = 78,
			/// <summary>
			/// Until Unicode 3.1.1, the corresponding block name was "Private Use",
			/// and multiple code point ranges had this block.
			/// </summary>
			PRIVATE_USE = PRIVATE_USE_AREA,
			/// <summary>[F900]</summary>
			CJK_COMPATIBILITY_IDEOGRAPHS = 79,
			/// <summary>[FB00]</summary>
			ALPHABETIC_PRESENTATION_FORMS = 80,
			/// <summary>[FB50]</summary>
			ARABIC_PRESENTATION_FORMS_A = 81,
			/// <summary>[FE20]</summary>
			COMBINING_HALF_MARKS = 82,
			/// <summary>[FE30]</summary>
			CJK_COMPATIBILITY_FORMS = 83,
			/// <summary>[FE50]</summary>
			SMALL_FORM_VARIANTS = 84,
			/// <summary>[FE70]</summary>
			ARABIC_PRESENTATION_FORMS_B = 85,
			/// <summary>[FFF0]</summary>
			SPECIALS = 86,
			/// <summary>[FF00]</summary>
			HALFWIDTH_AND_FULLWIDTH_FORMS = 87,

			/* New blocks in Unicode 3.1 */

			/// <summary>[10300]</summary>
			OLD_ITALIC = 88,
			/// <summary>[10330]</summary>
			GOTHIC = 89,
			/// <summary>[10400]</summary>
			DESERET = 90,
			/// <summary>[1D000]</summary>
			BYZANTINE_MUSICAL_SYMBOLS = 91,
			/// <summary>[1D100]</summary>
			MUSICAL_SYMBOLS = 92,
			/// <summary>[1D400]</summary>
			MATHEMATICAL_ALPHANUMERIC_SYMBOLS = 93,
			/// <summary>[20000]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B = 94,
			/// <summary>[2F800]</summary>
			CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT = 95,
			/// <summary>[E0000]</summary>
			TAGS = 96,

			/* New blocks in Unicode 3.2 */

			/// <summary>
			/// [0500]
			/// Unicode 4.0.1 renames the "Cyrillic Supplementary" block to "Cyrillic Supplement".
			/// </summary>
			CYRILLIC_SUPPLEMENT = 97,
			/// <summary>[1700]</summary>
			TAGALOG = 98,
			/// <summary>[1720]</summary>
			HANUNOO = 99,
			/// <summary>[1740]</summary>
			BUHID = 100,
			/// <summary>[1760]</summary>
			TAGBANWA = 101,
			/// <summary>[27C0]</summary>
			MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A = 102,
			/// <summary>[27F0]</summary>
			SUPPLEMENTAL_ARROWS_A = 103,
			/// <summary>[2900]</summary>
			SUPPLEMENTAL_ARROWS_B = 104,
			/// <summary>[2980]</summary>
			MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B = 105,
			/// <summary>[2A00]</summary>
			SUPPLEMENTAL_MATHEMATICAL_OPERATORS = 106,
			/// <summary>[31F0]</summary>
			KATAKANA_PHONETIC_EXTENSIONS = 107,
			/// <summary>[FE00]</summary>
			VARIATION_SELECTORS = 108,
			/// <summary>[F0000]</summary>
			SUPPLEMENTARY_PRIVATE_USE_AREA_A = 109,
			/// <summary>[100000]</summary>
			SUPPLEMENTARY_PRIVATE_USE_AREA_B = 110,

			/* New blocks in Unicode 4.0 */

			/// <summary>[1900]</summary>
			LIMBU = 111,
			/// <summary>[1950]</summary>
			TAI_LE = 112,
			/// <summary>[19E0]</summary>
			KHMER_SYMBOLS = 113,
			/// <summary>[1D00]</summary>
			PHONETIC_EXTENSIONS = 114,
			/// <summary>[2B00]</summary>
			MISCELLANEOUS_SYMBOLS_AND_ARROWS = 115,
			/// <summary>[4DC0]</summary>
			YIJING_HEXAGRAM_SYMBOLS = 116,
			/// <summary>[10000]</summary>
			LINEAR_B_SYLLABARY = 117,
			/// <summary>[10080]</summary>
			LINEAR_B_IDEOGRAMS = 118,
			/// <summary>[10100]</summary>
			AEGEAN_NUMBERS = 119,
			/// <summary>[10380]</summary>
			UGARITIC = 120,
			/// <summary>[10450]</summary>
			SHAVIAN = 121,
			/// <summary>[10480]</summary>
			OSMANYA = 122,
			/// <summary>[10800]</summary>
			CYPRIOT_SYLLABARY = 123,
			/// <summary>[1D300]</summary>
			TAI_XUAN_JING_SYMBOLS = 124,
			/// <summary>[E0100]</summary>
			VARIATION_SELECTORS_SUPPLEMENT = 125,

			/* New blocks in Unicode 4.1 */

			/// <summary>[1D200]</summary>
			ANCIENT_GREEK_MUSICAL_NOTATION = 126,
			/// <summary>[10140]</summary>
			ANCIENT_GREEK_NUMBERS = 127,
			/// <summary>[0750]</summary>
			ARABIC_SUPPLEMENT = 128,
			/// <summary>[1A00]</summary>
			BUGINESE = 129,
			/// <summary>[31C0]</summary>
			CJK_STROKES = 130,
			/// <summary>[1DC0]</summary>
			COMBINING_DIACRITICAL_MARKS_SUPPLEMENT = 131,
			/// <summary>[2C80]</summary>
			COPTIC = 132,
			/// <summary>[2D80]</summary>
			ETHIOPIC_EXTENDED = 133,
			/// <summary>[1380]</summary>
			ETHIOPIC_SUPPLEMENT = 134,
			/// <summary>[2D00]</summary>
			GEORGIAN_SUPPLEMENT = 135,
			/// <summary>[2C00]</summary>
			GLAGOLITIC = 136,
			/// <summary>[10A00]</summary>
			KHAROSHTHI = 137,
			/// <summary>[A700]</summary>
			MODIFIER_TONE_LETTERS = 138,
			/// <summary>[1980]</summary>
			NEW_TAI_LUE = 139,
			/// <summary>[103A0]</summary>
			OLD_PERSIAN = 140,
			/// <summary>[1D80]</summary>
			PHONETIC_EXTENSIONS_SUPPLEMENT = 141,
			/// <summary>[2E00]</summary>
			SUPPLEMENTAL_PUNCTUATION = 142,
			/// <summary>[A800]</summary>
			SYLOTI_NAGRI = 143,
			/// <summary>[2D30]</summary>
			TIFINAGH = 144,
			/// <summary>[FE10]</summary>
			VERTICAL_FORMS = 145,

			/* New blocks in Unicode 5.0 */

			/// <summary>[07C0]</summary>
			NKO = 146,
			/// <summary>[1B00]</summary>
			BALINESE = 147,
			/// <summary>[2C60]</summary>
			LATIN_EXTENDED_C = 148,
			/// <summary>[A720]</summary>
			LATIN_EXTENDED_D = 149,
			/// <summary>[A840]</summary>
			PHAGS_PA = 150,
			/// <summary>[10900]</summary>
			PHOENICIAN = 151,
			/// <summary>[12000]</summary>
			CUNEIFORM = 152,
			/// <summary>[12400]</summary>
			CUNEIFORM_NUMBERS_AND_PUNCTUATION = 153,
			/// <summary>[1D360]</summary>
			COUNTING_ROD_NUMERALS = 154,

			/* New blocks in Unicode 5.1 */

			/// <summary>[1B80]</summary>
			SUNDANESE = 155,
			/// <summary>[1C00]</summary>
			LEPCHA = 156,
			/// <summary>[1C50]</summary>
			OL_CHIKI = 157,
			/// <summary>[2DE0]</summary>
			CYRILLIC_EXTENDED_A = 158,
			/// <summary>[A500]</summary>
			VAI = 159,
			/// <summary>[A640]</summary>
			CYRILLIC_EXTENDED_B = 160,
			/// <summary>[A880]</summary>
			SAURASHTRA = 161,
			/// <summary>[A900]</summary>
			KAYAH_LI = 162,
			/// <summary>[A930]</summary>
			REJANG = 163,
			/// <summary>[AA00]</summary>
			CHAM = 164,
			/// <summary>[10190]</summary>
			ANCIENT_SYMBOLS = 165,
			/// <summary>[101D0]</summary>
			PHAISTOS_DISC = 166,
			/// <summary>[10280]</summary>
			LYCIAN = 167,
			/// <summary>[102A0]</summary>
			CARIAN = 168,
			/// <summary>[10920]</summary>
			LYDIAN = 169,
			/// <summary>[1F000]</summary>
			MAHJONG_TILES = 170,
			/// <summary>[1F030]</summary>
			DOMINO_TILES = 171,

			/* New blocks in Unicode 5.2 */

			/// <summary>[0800]</summary>
			SAMARITAN = 172,
			/// <summary>[18B0]</summary>
			UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED = 173,
			/// <summary>[1A20]</summary>
			TAI_THAM = 174,
			/// <summary>[1CD0]</summary>
			VEDIC_EXTENSIONS = 175,
			/// <summary>[A4D0]</summary>
			LISU = 176,
			/// <summary>[A6A0]</summary>
			BAMUM = 177,
			/// <summary>[A830]</summary>
			COMMON_INDIC_NUMBER_FORMS = 178,
			/// <summary>[A8E0]</summary>
			DEVANAGARI_EXTENDED = 179,
			/// <summary>[A960]</summary>
			HANGUL_JAMO_EXTENDED_A = 180,
			/// <summary>[A980]</summary>
			JAVANESE = 181,
			/// <summary>[AA60]</summary>
			MYANMAR_EXTENDED_A = 182,
			/// <summary>[AA80]</summary>
			TAI_VIET = 183,
			/// <summary>[ABC0]</summary>
			MEETEI_MAYEK = 184,
			/// <summary>[D7B0]</summary>
			HANGUL_JAMO_EXTENDED_B = 185,
			/// <summary>[10840]</summary>
			IMPERIAL_ARAMAIC = 186,
			/// <summary>[10A60]</summary>
			OLD_SOUTH_ARABIAN = 187,
			/// <summary>[10B00]</summary>
			AVESTAN = 188,
			/// <summary>[10B40]</summary>
			INSCRIPTIONAL_PARTHIAN = 189,
			/// <summary>[10B60]</summary>
			INSCRIPTIONAL_PAHLAVI = 190,
			/// <summary>[10C00]</summary>
			OLD_TURKIC = 191,
			/// <summary>[10E60]</summary>
			RUMI_NUMERAL_SYMBOLS = 192,
			/// <summary>[11080]</summary>
			KAITHI = 193,
			/// <summary>[13000]</summary>
			EGYPTIAN_HIEROGLYPHS = 194,
			/// <summary>[1F100]</summary>
			ENCLOSED_ALPHANUMERIC_SUPPLEMENT = 195,
			/// <summary>[1F200]</summary>
			ENCLOSED_IDEOGRAPHIC_SUPPLEMENT = 196,
			/// <summary>[2A700]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C = 197,

			/* New blocks in Unicode 6.0 */

			/// <summary>[0840]</summary>
			MANDAIC = 198,
			/// <summary>[1BC0]</summary>
			BATAK = 199,
			/// <summary>[AB00]</summary>
			ETHIOPIC_EXTENDED_A = 200,
			/// <summary>[11000]</summary>
			BRAHMI = 201,
			/// <summary>[16800]</summary>
			BAMUM_SUPPLEMENT = 202,
			/// <summary>[1B000]</summary>
			KANA_SUPPLEMENT = 203,
			/// <summary>[1F0A0]</summary>
			PLAYING_CARDS = 204,
			/// <summary>[1F300]</summary>
			MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS = 205,
			/// <summary>[1F600]</summary>
			EMOTICONS = 206,
			/// <summary>[1F680]</summary>
			TRANSPORT_AND_MAP_SYMBOLS = 207,
			/// <summary>[1F700]</summary>
			ALCHEMICAL_SYMBOLS = 208,
			/// <summary>[2B740]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D = 209,

			/* New blocks in Unicode 6.1 */

			/// <summary>[08A0]</summary>
			ARABIC_EXTENDED_A = 210,
			/// <summary>[1EE00]</summary>
			ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS = 211,
			/// <summary>[11100]</summary>
			CHAKMA = 212,
			/// <summary>[AAE0]</summary>
			MEETEI_MAYEK_EXTENSIONS = 213,
			/// <summary>[109A0]</summary>
			MEROITIC_CURSIVE = 214,
			/// <summary>[10980]</summary>
			MEROITIC_HIEROGLYPHS = 215,
			/// <summary>[16F00]</summary>
			MIAO = 216,
			/// <summary>[11180]</summary>
			SHARADA = 217,
			/// <summary>[110D0]</summary>
			SORA_SOMPENG = 218,
			/// <summary>[1CC0]</summary>
			SUNDAINESE_SUPPLEMENT = 219,
			/// <summary>[11680]</summary>
			TAKRI = 220,

			/* New blocks in Unicode 7.0 */

			/// <summary>[16AD0]</summary>
			BASSA_VAH = 221,
			/// <summary>[10530]</summary>
			CAUCASIAN_ALBANIAN = 222,
			/// <summary>[102E0]</summary>
			COPTIC_EPACT_NUMBERS = 223,
			/// <summary>[1AB0]</summary>
			COMBINING_DIACRITICAL_MARKS_EXTENDED = 224,
			/// <summary>[1BC00]</summary>
			DUPLOYAN = 225,
			/// <summary>[10500]</summary>
			ELBASAN = 226,
			/// <summary>[1F780]</summary>
			GEOMETRIC_SHAPES_EXTENDED = 227,
			/// <summary>[11300]</summary>
			GRANTHA = 228,
			/// <summary>[11200]</summary>
			KHOJKI = 229,
			/// <summary>[112B0]</summary>
			KHUDAWADI = 230,
			/// <summary>[AB30]</summary>
			LATIN_EXTENDED_E = 231,
			/// <summary>[10600]</summary>
			LINEAR_A = 232,
			/// <summary>[11150]</summary>
			MAHAJANI = 233,
			/// <summary>[10AC0]</summary>
			MANICHAEAN = 234,
			/// <summary>[1E800]</summary>
			MENDE_KIKAKUI = 235,
			/// <summary>[11600]</summary>
			MODI = 236,
			/// <summary>[16A40]</summary>
			MRO = 237,
			/// <summary>[A9E0]</summary>
			MYANMAR_EXTENDED_B = 238,
			/// <summary>[10880]</summary>
			NABATAEAN = 239,
			/// <summary>[10A80]</summary>
			OLD_NORTH_ARABIAN = 240,
			/// <summary>[10350]</summary>
			OLD_PERMIC = 241,
			/// <summary>[1F650]</summary>
			ORNAMENTAL_DINGBATS = 242,
			/// <summary>[16B00]</summary>
			PAHAWH_HMONG = 243,
			/// <summary>[10860]</summary>
			PALMYRENE = 244,
			/// <summary>[11AC0]</summary>
			PAU_CIN_HAU = 245,
			/// <summary>[10B80]</summary>
			PSALTER_PAHLAVI = 246,
			/// <summary>[1BCA0]</summary>
			SHORTHAND_FORMAT_CONTROLS = 247,
			/// <summary>[11580]</summary>
			SIDDHAM = 248,
			/// <summary>[111E0]</summary>
			SINHALA_ARCHAIC_NUMBERS = 249,
			/// <summary>[1F800]</summary>
			SUPPLEMENTAL_ARROWS_C = 250,
			/// <summary>[11480]</summary>
			TIRHUTA = 251,
			/// <summary>[118A0]</summary>
			WARANG_CITI = 252,

			/* New blocks in Unicode 8.0 */

			/// <summary>[11700]</summary>
			AHOM = 253,
			/// <summary>[14400]</summary>
			ANATOLIAN_HIEROGLYPHS = 254,
			/// <summary>[AB70]</summary>
			CHEROKEE_SUPPLEMENT = 255,
			/// <summary>[2B820]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_E = 256,
			/// <summary>[12480]</summary>
			EARLY_DYNASTIC_CUNEIFORM = 257,
			/// <summary>[108E0]</summary>
			HATRAN = 258,
			/// <summary>[11280]</summary>
			MULTANI = 259,
			/// <summary>[10C80]</summary>
			OLD_HUNGARIAN = 260,
			/// <summary>[1F900]</summary>
			SUPPLEMENTAL_SYMBOLS_AND_PICTOGRAPHS = 261,
			/// <summary>[1D800]</summary>
			SUTTON_SIGNWRITING = 262,

			/* New blocks in Unicode 9.0 */

			/// <summary>[1E900]</summary>
			ADLAM = 263,
			/// <summary>[11C00]</summary>
			BHAIKSUKI = 264,
			/// <summary>[1C80]</summary>
			CYRILLIC_EXTENDED_C = 265,
			/// <summary>[1E000]</summary>
			GLAGOLITIC_SUPPLEMENT = 266,
			/// <summary>[16FE0]</summary>
			IDEOGRAPHIC_SYMBOLS_AND_PUNCTUATION = 267,
			/// <summary>[11C70]</summary>
			MARCHEN = 268,
			/// <summary>[11660]</summary>
			MONGOLIAN_SUPPLEMENT = 269,
			/// <summary>[11400]</summary>
			NEWA = 270,
			/// <summary>[104B0]</summary>
			OSAGE = 271,
			/// <summary>[17000]</summary>
			TANGUT = 272,
			/// <summary>[18800]</summary>
			TANGUT_COMPONENTS = 273,

			/* New blocks in Unicode 10.0 */

			/// <summary>[2CEB0]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_F = 274,
			/// <summary>[1B100]</summary>
			KANA_EXTENDED_A = 275,
			/// <summary>[11D00]</summary>
			MASARAM_GONDI = 276,
			/// <summary>[1B170]</summary>
			NUSHU = 277,
			/// <summary>[11A50]</summary>
			SOYOMBO = 278,
			/// <summary>[0860]</summary>
			SYRIAC_SUPPLEMENT = 279,
			/// <summary>[11A00]</summary>
			ZANABAZAR_SQUARE = 280,
			/// <summary>[1FA00]</summary>

			/* New blocks in Unicode 11.0 */

			CHESS_SYMBOLS = 281,
			/// <summary>[11800]</summary>
			DOGRA = 282,
			/// <summary>[1C90]</summary>
			GEORGIAN_EXTENDED = 283,
			/// <summary>[11D60]</summary>
			GUNJALA_GONDI = 284,
			/// <summary>[10D00]</summary>
			HANIFI_ROHINGYA = 285,
			/// <summary>[1EC70]</summary>
			INDIC_SIYAQ_NUMBERS = 286,
			/// <summary>[11EE0]</summary>
			MAKASAR = 287,
			/// <summary>[1D2E0]</summary>
			MAYAN_NUMERALS = 288,
			/// <summary>[16E40]</summary>
			MEDEFAIDRIN = 289,
			/// <summary>[10F00]</summary>
			OLD_SOGDIAN = 290,
			/// <summary>[10F30]</summary>
			SOGDIAN = 291,

			/* New blocks in Unicode 12.0 */

			/// <summary>[13430]</summary>
			EGYPTIAN_HIEROGLYPH_FORMAT_CONTROLS = 292,
			/// <summary>[10FE0]</summary>
			ELYMAIC = 293,
			/// <summary>[119A0]</summary>
			NANDINAGARI = 294,
			/// <summary>[1E100]</summary>
			NYIAKENG_PUACHUE_HMONG = 295,
			/// <summary>[1ED00]</summary>
			OTTOMAN_SIYAQ_NUMBERS = 296,
			/// <summary>[1B130]</summary>
			SMALL_KANA_EXTENSION = 297,
			/// <summary>[1FA70]</summary>
			SYMBOLS_AND_PICTOGRAPHS_EXTENDED_A = 298,
			/// <summary>[11FC0]</summary>
			TAMIL_SUPPLEMENT = 299,
			/// <summary>[1E2C0]</summary>
			WANCHO = 300,

			/* New blocks in Unicode 13.0 */

			/// <summary>[10FB0]</summary>
			CHORASMIAN = 301,
			/// <summary>[30000]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_G = 302,
			/// <summary>[11900]</summary>
			DIVES_AKURU = 303,
			/// <summary>[18B00]</summary>
			KHITAN_SMALL_SCRIPT = 304,
			/// <summary>[11FB0]</summary>
			LISU_SUPPLEMENT = 305,
			/// <summary>[1FB00]</summary>
			SYMBOLS_FOR_LEGACY_COMPUTING = 306,
			/// <summary>[18D00]</summary>
			TANGUT_SUPPLEMENT = 307,
			/// <summary>[10E80]</summary>
			YEZIDI = 308,

			/* New blocks in Unicode 14.0 */

			/// <summary>[0870]</summary>
			ARABIC_EXTENDED_B = 309,
			/// <summary>[12F90]</summary>
			CYPRO_MINOAN = 310,
			/// <summary>[1E7E0]</summary>
			ETHIOPIC_EXTENDED_B = 311,
			/// <summary>[1AFF0]</summary>
			KANA_EXTENDED_B = 312,
			/// <summary>[10780]</summary>
			LATIN_EXTENDED_F = 313,
			/// <summary>[1DF00]</summary>
			LATIN_EXTENDED_G = 314,
			/// <summary>[10F70]</summary>
			OLD_UYGHUR = 315,
			/// <summary>[16A70]</summary>
			TANGSA = 316,
			/// <summary>[1E290]</summary>
			TOTO = 317,
			/// <summary>[11AB0]</summary>
			UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED_A = 318,
			/// <summary>[10570]</summary>
			VITHKUQI = 319,
			/// <summary>[1CF00]</summary>
			ZNAMENNY_MUSICAL_NOTATION = 320,

			/* New blocks in Unicode 15.0 */

			/// <summary>[10EC0]</summary>
			ARABIC_EXTENDED_C = 321,
			/// <summary>[31350]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_H = 322,
			/// <summary>[1E030]</summary>
			CYRILLIC_EXTENDED_D = 323,
			/// <summary>[11B00]</summary>
			DEVANAGARI_EXTENDED_A = 324,
			/// <summary>[1D2C0]</summary>
			KAKTOVIK_NUMERALS = 325,
			/// <summary>[11F00]</summary>
			KAWI = 326,
			/// <summary>[1E4D0]</summary>
			NAG_MUNDARI = 327,

			/* New block in Unicode 15.1 */

			/// <summary>[2EBF0]</summary>
			CJK_UNIFIED_IDEOGRAPHS_EXTENSION_I = 328,

			/* New blocks in Unicode 16.0 */

			/// <summary>[13460]</summary>
			EGYPTIAN_HIEROGLYPHS_EXTENDED_A = 329,
			/// <summary>[10D40]</summary>
			GARAY = 330,
			/// <summary>[16100]</summary>
			GURUNG_KHEMA = 331,
			/// <summary>[16D40]</summary>
			KIRAT_RAI = 332,
			/// <summary>[116D0]</summary>
			MYANMAR_EXTENDED_C = 333,
			/// <summary>[1E5D0]</summary>
			OL_ONAL = 334,
			/// <summary>[11BC0]</summary>
			SUNUWAR = 335,
			/// <summary>[1CC00]</summary>
			SYMBOLS_FOR_LEGACY_COMPUTING_SUPPLEMENT = 336,
			/// <summary>[105C0]</summary>
			TODHRI = 337,
			/// <summary>[11380]</summary>
			TULU_TIGALARI = 338,

			/// <summary>One more than the highest normal UBlockCode value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_BLOCK).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			UBLOCK_COUNT = 339,
			/// <summary>Invalid code</summary>
			INVALID_CODE = -1
		}

		/// <summary>
		/// East Asian Width constants.
		/// </summary>
		/// <remarks>
		/// Note: UEastAsianWidth constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_EA_{Unicode East_Asian_Width value name}
		/// </remarks>
		public enum UEastAsianWidth
		{
			/* Stable in ICU 2.2 */

			/// <summary>[N]</summary>
			NEUTRAL,
			/// <summary>[A]</summary>
			AMBIGUOUS,
			/// <summary>[H]</summary>
			HALFWIDTH,
			/// <summary>[F]</summary>
			FULLWIDTH,
			/// <summary>[Na]</summary>
			NARROW,
			/// <summary>[W]</summary>
			WIDE,

			/// <summary>One more than the highest normal UEastAsianWidth value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_EAST_ASIAN_WIDTH).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
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
		/// Selector constants for u_getPropertyName() and u_getPropertyValueName().
		/// These selectors are used to choose which name is returned for a given property or value.
		/// All properties and values have a long name. Most have a short name, but some do not.
		/// Unicode allows for additional names, beyond the long and short name, which would be indicated by U_LONG_PROPERTY_NAME + i, where i=1, 2,...
		/// </summary>
		public enum UPropertyNameChoice
		{
			/* Stable in ICU 2.4 */

			SHORT_PROPERTY_NAME,
			LONG_PROPERTY_NAME,

			/// <summary>One more than the highest normal UPropertyNameChoice value.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			PROPERTY_NAME_CHOICE_COUNT
		}

		/// <summary>
		/// Decomposition Type constants.
		/// </summary>
		/// <remarks>
		/// Note: UDecompositionType constants are parsed by preparseucd.py.
		/// It matches lines like
		///		U_DT_{Unicode Decomposition_Type value name}
		/// </remarks>
		public enum UDecompositionType
		{
			/* Stable in ICU 2.2 */

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

			/// <summary>One more than the highest normal UDecompositionType value.</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Joining Type constants.
		/// </summary>
		/// <remarks>
		/// Note: UJoiningType constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_JT_{Unicode Joining_Type value name}
		/// </remarks>
		public enum UJoiningType
		{
			/* Stable in ICU 2.2 */

			/// <summary>[U]</summary>
			NON_JOINING,
			/// <summary>[C]</summary>
			JOIN_CAUSING,
			/// <summary>[D]</summary>
			DUAL_JOINING,
			/// <summary>[L]</summary>
			LEFT_JOINING,
			/// <summary>[R]</summary>
			RIGHT_JOINING,
			/// <summary>[T]</summary>
			TRANSPARENT,

			/// <summary>One more than the highest normal UJoiningType value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_JOINING_TYPE).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Joining Group constants.
		/// </summary>
		/// <remarks>
		/// Note: UJoiningGroup constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_JG_{Unicode Joining_Group value name}
		/// </remarks>
		public enum UJoiningGroup
		{
			/* Stable in ICU 2.2 */

			NO_JOINING_GROUP,
			AIN,
			ALAPH,
			ALEF,
			BEH,
			BETH,
			DAL,
			DALATH_RISH,
			E,
			FEH,
			FINAL_SEMKATH,
			GAF,
			GAMAL,
			HAH,
			TEH_MARBUTA_GOAL, // Stable in ICU 4.6
			HAMZA_ON_HEH_GOAL = TEH_MARBUTA_GOAL,
			HE,
			HEH,
			HEH_GOAL,
			HETH,
			KAF,
			KAPH,
			KNOTTED_HEH,
			LAM,
			LAMADH,
			MEEM,
			MIM,
			NOON,
			NUN,
			PE,
			QAF,
			QAPH,
			REH,
			REVERSED_PE,
			SAD,
			SADHE,
			SEEN,
			SEMKATH,
			SHIN,
			SWASH_KAF,
			SYRIAC_WAW,
			TAH,
			TAW,
			TEH_MARBUTA,
			TETH,
			WAW,
			YEH,
			YEH_BARREE,
			YEH_WITH_TAIL,
			YUDH,
			YUDH_HE,
			ZAIN,

			/* Stable in ICU 2.6 */

			FE,
			KHAPH,
			ZHAIN,

			/* Stable in ICU 4.0 */

			BURUSHASKI_YEH_BARREE,

			/* Stable in ICU 4.4 */

			FARSI_YEH,
			NYA,

			/* Stable in ICU 49 */

			ROHINGYA_YEH,

			/* Stable in ICU 54 */

			MANICHAEAN_ALEPH,
			MANICHAEAN_AYIN,
			MANICHAEAN_BETH,
			MANICHAEAN_DALETH,
			MANICHAEAN_DHAMEDH,
			MANICHAEAN_FIVE,
			MANICHAEAN_GIMEL,
			MANICHAEAN_HETH,
			MANICHAEAN_HUNDRED,
			MANICHAEAN_KAPH,
			MANICHAEAN_LAMEDH,
			MANICHAEAN_MEM,
			MANICHAEAN_NUN,
			MANICHAEAN_ONE,
			MANICHAEAN_PE,
			MANICHAEAN_QOPH,
			MANICHAEAN_RESH,
			MANICHAEAN_SADHE,
			MANICHAEAN_SAMEKH,
			MANICHAEAN_TAW,
			MANICHAEAN_TEN,
			MANICHAEAN_TETH,
			MANICHAEAN_THAMEDH,
			MANICHAEAN_TWENTY,
			MANICHAEAN_WAW,
			MANICHAEAN_YODH,
			MANICHAEAN_ZAYIN,
			STRAIGHT_WAW,

			/* Stable in ICU 58 */

			AFRICAN_FEH,
			AFRICAN_NOON,
			AFRICAN_QAF,

			/* Stable in ICU 60 */

			MALAYALAM_BHA,
			MALAYALAM_JA,
			MALAYALAM_LLA,
			MALAYALAM_LLLA,
			MALAYALAM_NGA,
			MALAYALAM_NNA,
			MALAYALAM_NNNA,
			MALAYALAM_NYA,
			MALAYALAM_RA,
			MALAYALAM_SSA,
			MALAYALAM_TTA,

			/* Stable in ICU 62 */

			HANIFI_ROHINGYA_KINNA_YA,
			HANIFI_ROHINGYA_PA,

			/* Stable in ICU 70 */

			THIN_YEH,
			VERTICAL_TAIL,

			/* Stable in ICU 76 */

			KASHMIRI_YEH,

			/// <summary>One more than the highest normal UJoiningGroup value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_JOINING_GROUP).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Grapheme Cluster Break constants.
		/// </summary>
		/// <remarks>
		/// Note: UGraphemeClusterBreak constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_GCB_{Unicode Grapheme_Cluster_Break value name}
		/// </remarks>
		public enum UGraphemeClusterBreak
		{
			/* Stable in ICU 3.4 */

			/// <summary>[XX]</summary>
			OTHER = 0,
			/// <summary>[CN]</summary>
			CONTROL = 1,
			/// <summary>[CR]</summary>
			CR = 2,
			/// <summary>[EX]</summary>
			EXTEND = 3,
			/// <summary>[L]</summary>
			L = 4,
			/// <summary>[LF]</summary>
			LF = 5,
			/// <summary>[LV]</summary>
			LV = 6,
			/// <summary>[LVT]</summary>
			LVT = 7,
			/// <summary>[T]</summary>
			T = 8,
			/// <summary>[V]</summary>
			V = 9,

			/* New in Unicode 5.1/ICU 4.0 */

			/// <summary>[SM]</summary>
			SPACING_MARK = 10,
			/// <summary>[PP]</summary>
			PREPEND = 11,

			/* New in Unicode 6.2/ICU 50 */

			/// <summary>[RI]</summary>
			REGIONAL_INDICATOR = 12,

			/* New in Unicode 9.0/ICU 58 */

			/// <summary>[EB]</summary>
			E_BASE = 13,
			/// <summary>[EBG]</summary>
			E_BASE_GAZ = 14,
			/// <summary>[EM]</summary>
			E_MODIFIER = 15,
			/// <summary>[GAZ]</summary>
			GLUE_AFTER_ZWJ = 16,
			/// <summary>[ZWJ]</summary>
			ZWJ = 17,

			/// <summary>One more than the highest normal UGraphemeClusterBreak value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_GRAPHEME_CLUSTER_BREAK).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT = 18
		}

		/// <summary>
		/// Word Break constants.
		/// </summary>
		/// <remarks>
		/// Note: UWordBreakValues constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_WB_{Unicode Word_Break value name}
		/// </remarks>
		public enum UWordBreakValues
		{
			/* Stable in ICU 3.4 */

			/// <summary>[XX]</summary>
			OTHER = 0,
			/// <summary>[LE]</summary>
			ALETTER = 1,
			/// <summary>[FO]</summary>
			FORMAT = 2,
			/// <summary>[KA]</summary>
			KATAKANA = 3,
			/// <summary>[ML]</summary>
			MIDLETTER = 4,
			/// <summary>[MN]</summary>
			MIDNUM = 5,
			/// <summary>[NU]</summary>
			NUMERIC = 6,
			/// <summary>[EX]</summary>
			EXTENDNUMLET = 7,

			/* New in Unicode 5.1/ICU 4.0 */

			/// <summary>[CR]</summary>
			CR = 8,
			/// <summary>[Extend]</summary>
			EXTEND = 9,
			/// <summary>[LF]</summary>
			LF = 10,
			/// <summary>[MB]</summary>
			MIDNUMLET = 11,
			/// <summary>[NL]</summary>
			NEWLINE = 12,

			/* New in Unicode 6.2/ICU 50 */

			/// <summary>[RI]</summary>
			REGIONAL_INDICATOR = 13,

			/* New in Unicode 6.3/ICU 52 */

			/// <summary>[HL]</summary>
			HEBREW_LETTER = 14,
			/// <summary>[SQ]</summary>
			SINGLE_QUOTE = 15,
			/// <summary>[DQ]</summary>
			DOUBLE_QUOTE = 16,

			/* New in Unicode 9.0/ICU 58 */

			/// <summary>[EB]</summary>
			E_BASE = 17,
			/// <summary>[EBG]</summary>
			E_BASE_GAZ = 18,
			/// <summary>[EM]</summary>
			E_MODIFIER = 19,
			/// <summary>[GAZ]</summary>
			GLUE_AFTER_ZWJ = 20,
			/// <summary>[ZWJ]</summary>
			ZWJ = 21,
			/// <summary>[WSEGSPACE]</summary>
			WSEGSPACE = 22,

			/// <summary>One more than the highest normal UWordBreakValues value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_WORD_BREAK).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT = 23
		}

		/// <summary>
		/// Sentence Break constants.
		/// </summary>
		/// <remarks>
		/// Note: USentenceBreak constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_SB_{Unicode Sentence_Break value name}
		/// </remarks>
		public enum USentenceBreak
		{
			/* Stable in ICU 3.4 */

			/// <summary>[XX]</summary>
			OTHER = 0,
			/// <summary>[AT]</summary>
			ATERM = 1,
			/// <summary>[CL]</summary>
			CLOSE = 2,
			/// <summary>[FO]</summary>
			FORMAT = 3,
			/// <summary>[LO]</summary>
			LOWER = 4,
			/// <summary>[NU]</summary>
			NUMERIC = 5,
			/// <summary>[LE]</summary>
			OLETTER = 6,
			/// <summary>[SE]</summary>
			SEP = 7,
			/// <summary>[SP]</summary>
			SP = 8,
			/// <summary>[ST]</summary>
			STERM = 9,
			/// <summary>[UP]</summary>
			UPPER = 10,

			/* New in Unicode 5.1/ICU 4.0 */

			/// <summary>[CR]</summary>
			CR = 11,
			/// <summary>[EX]</summary>
			EXTEND = 12,
			/// <summary>[LF]</summary>
			LF = 13,
			/// <summary>[SC]</summary>
			SCONTINUE = 14,

			/// <summary>One more than the highest normal USentenceBreak value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_SENTENCE_BREAK).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT = 15
		}

		/// <summary>
		/// Line Break constants.
		/// </summary>
		/// <remarks>
		/// Note: ULineBreak constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_LB_{Unicode Line_Break value name}
		/// </remarks>
		public enum ULineBreak
		{
			/// <summary>[XX]</summary>
			UNKNOWN = 0,
			/// <summary>[AI]</summary>
			AMBIGUOUS = 1,
			/// <summary>[AL]</summary>
			ALPHABETIC = 2,
			/// <summary>[B2]</summary>
			BREAK_BOTH = 3,
			/// <summary>[BA]</summary>
			BREAK_AFTER = 4,
			/// <summary>[BB]</summary>
			BREAK_BEFORE = 5,
			/// <summary>[BK]</summary>
			MANDATORY_BREAK = 6,
			/// <summary>[CB]</summary>
			CONTINGENT_BREAK = 7,
			/// <summary>[CL]</summary>
			CLOSE_PUNCTUATION = 8,
			/// <summary>[CM]</summary>
			COMBINING_MARK = 9,
			/// <summary>[CR]</summary>
			CARRIAGE_RETURN = 10,
			/// <summary>[EX]</summary>
			EXCLAMATION = 11,
			/// <summary>[GL]</summary>
			GLUE = 12,
			/// <summary>[HY]</summary>
			HYPHEN = 13,
			/// <summary>[ID]</summary>
			IDEOGRAPHIC = 14,
			/// <summary>[IN]</summary>
			INSEPARABLE = 15,
			[Obsolete("Renamed the misspelled 'inseperable' in Unicode 4.0.1/ICU 3.0")]
			INSEPERABLE = INSEPARABLE,
			/// <summary>[IS]</summary>
			INFIX_NUMERIC = 16,
			/// <summary>[LF]</summary>
			LINE_FEED = 17,
			/// <summary>[NS]</summary>
			NONSTARTER = 18,
			/// <summary>[NU]</summary>
			NUMERIC = 19,
			/// <summary>[OP]</summary>
			OPEN_PUNCTUATION = 20,
			/// <summary>[PO]</summary>
			POSTFIX_NUMERIC = 21,
			/// <summary>[PR]</summary>
			PREFIX_NUMERIC = 22,
			/// <summary>[QU]</summary>
			QUOTATION = 23,
			/// <summary>[SA]</summary>
			COMPLEX_CONTEXT = 24,
			/// <summary>[SG]</summary>
			SURROGATE = 25,
			/// <summary>[SP]</summary>
			SPACE = 26,
			/// <summary>[SY]</summary>
			BREAK_SYMBOLS = 27,
			/// <summary>[ZW]</summary>
			ZWSPACE = 28,

			/* New in Unicode 4.0/ICU 2.6 */

			/// <summary>[NL]</summary>
			NEXT_LINE = 29,
			/// <summary>[WJ]</summary>
			WORD_JOINER = 30,

			/* New in Unicode 4.1/ICU 3.4 */

			/// <summary>[H2]</summary>
			H2 = 31,
			/// <summary>[H3]</summary>
			H3 = 32,
			/// <summary>[JL]</summary>
			JL = 33,
			/// <summary>[JT]</summary>
			JT = 34,
			/// <summary>[JV]</summary>
			JV = 35,

			/* New in Unicode 5.2/ICU 4.4 */

			/// <summary>[CP]</summary>
			CLOSE_PARENTHESIS = 36,

			/* New in Unicode 6.1/ICU 49 */

			/// <summary>[CJ]</summary>
			CONDITIONAL_JAPANESE_STARTER = 37,
			/// <summary>[HL]</summary>
			HEBREW_LETTER = 38,

			/* New in Unicode 6.2/ICU 50 */

			/// <summary>[RI]</summary>
			REGIONAL_INDICATOR = 39,

			/* New in Unicode 9.0/ICU 58 */

			/// <summary>[EB]</summary>
			E_BASE = 40,
			/// <summary>[EM]</summary>
			E_MODIFIER = 41,
			/// <summary>[ZWJ]</summary>
			ZWJ = 42,
			/// <summary>[AK]</summary>
			AKSARA = 43,
			/// <summary>[AP]</summary>
			AKSARA_PREBASE = 44,
			/// <summary>[AS]</summary>
			AKSARA_START = 45,
			/// <summary>[VF]</summary>
			VIRAMA_FINAL = 46,
			/// <summary>[VI]</summary>
			VIRAMA = 47,

			/// <summary>One more than the highest normal ULineBreak value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_LINE_BREAK).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT = 48
		}

		/// <summary>
		/// Numeric Type constants
		/// </summary>
		/// <remarks>
		/// Note: UNumericType constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_NT_{Unicode Numeric_Type value name}
		/// </remarks>
		public enum UNumericType
		{
			/* Stable in ICU 2.2 */

			/// <summary>[None]</summary>
			NONE,
			/// <summary>[de]</summary>
			DECIMAL,
			/// <summary>[di]</summary>
			DIGIT,
			/// <summary>[nu]</summary>
			NUMERIC,

			/// <summary>One more than the highest normal UNumericType value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_NUMERIC_TYPE).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Hangul Syllable Type constants.
		/// </summary>
		/// <remarks>
		/// Note: UHangulSyllableType constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_HST_{Unicode Hangul_Syllable_Type value name}
		/// </remarks>
		public enum UHangulSyllableType
		{
			/* Stable in ICU 2.6 */

			/// <summary>[NA]</summary>
			NOT_APPLICABLE,
			/// <summary>[L]</summary>
			LEADING_JAMO,
			/// <summary>[V]</summary>
			VOWEL_JAMO,
			/// <summary>[T]</summary>
			TRAILING_JAMO,
			/// <summary>[LV]</summary>
			LV_SYLLABLE,
			/// <summary>[LVT]</summary>
			LVT_SYLLABLE,

			/// <summary>One more than the highest normal UHangulSyllableType value.
			/// The highest value is available via u_getIntPropertyMaxValue(UCHAR_HANGUL_SYLLABLE_TYPE).</summary>
			[Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
			COUNT
		}

		/// <summary>
		/// Indic Positional Category constants.
		/// </summary>
		/// <remarks>
		/// Note: UIndicPositionalCategory constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_INPC_{Unicode Indic_Positional_Category value name}
		/// </remarks>
		public enum UIndicPositionalCategory
		{
			/* Stable in ICU 63 */

			NA,
			BOTTOM,
			BOTTOM_AND_LEFT,
			BOTTOM_AND_RIGHT,
			LEFT,
			LEFT_AND_RIGHT,
			OVERSTRUCK,
			RIGHT,
			TOP,
			TOP_AND_BOTTOM,
			TOP_AND_BOTTOM_AND_RIGHT,
			TOP_AND_LEFT,
			TOP_AND_LEFT_AND_RIGHT,
			TOP_AND_RIGHT,
			VISUAL_ORDER_LEFT,

			/* Stable in ICU 66 */

			TOP_AND_BOTTOM_AND_LEFT
		}

		/// <summary>
		/// Indic Positional Category constants.
		/// </summary>
		/// <remarks>
		/// Note: UIndicPositionalCategory constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_INPC_{Unicode Indic_Positional_Category value name}
		/// </remarks>
		public enum UIndicSyllabicCategory
		{
			/* Stable in ICU 63 */

			OTHER,
			AVAGRAHA,
			BINDU,
			BRAHMI_JOINING_NUMBER,
			CANTILLATION_MARK,
			CONSONANT,
			CONSONANT_DEAD,
			CONSONANT_FINAL,
			CONSONANT_HEAD_LETTER,
			CONSONANT_INITIAL_POSTFIXED,
			CONSONANT_KILLER,
			CONSONANT_MEDIAL,
			CONSONANT_PLACEHOLDER,
			CONSONANT_PRECEDING_REPHA,
			CONSONANT_PREFIXED,
			CONSONANT_SUBJOINED,
			CONSONANT_SUCCEEDING_REPHA,
			CONSONANT_WITH_STACKER,
			GEMINATION_MARK,
			INVISIBLE_STACKER,
			JOINER,
			MODIFYING_LETTER,
			NON_JOINER,
			NUKTA,
			NUMBER,
			NUMBER_JOINER,
			PURE_KILLER,
			REGISTER_SHIFTER,
			SYLLABLE_MODIFIER,
			TONE_LETTER,
			TONE_MARK,
			VIRAMA,
			VISARGA,
			VOWEL,
			VOWEL_DEPENDENT,
			VOWEL_INDEPENDENT,

			/* Stable in ICU 76 */

			REORDERING_KILLER
		}

		/// <summary>
		/// Indic Conjunct Break constants. (ICU 76 draft; not stable.)
		/// </summary>
		/// <remarks>
		/// Note: UIndicConjunctBreak constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_INCB_{Unicode Indic_Conjunct_Break value name}
		/// </remarks>
		public enum UIndicConjunctBreak
		{
			/* Draft in ICU 76 */

			NONE,
			CONSONANT,
			EXTEND,
			LINKER
		}

		/// <summary>
		/// Vertical Orientation constants.
		/// </summary>
		/// <remarks>
		/// Note: UVerticalOrientation constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_VO_{Unicode Vertical_Orientation value name}
		/// </remarks>
		public enum UVerticalOrientation
		{
			/* Stable in ICU 63 */

			ROTATED,
			TRANSFORMED_ROTATED,
			TRANSFORMED_UPRIGHT,
			UPRIGHT
		}

		/// <summary>
		/// Identifier Status constants.
		/// See https://www.unicode.org/reports/tr39/#Identifier_Status_and_Type.
		/// </summary>
		/// <remarks>
		/// Note: UIdentifierStatus constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_ID_STATUS_{Unicode Identifier_Status value name}
		/// </remarks>
		public enum UIdentifierStatus
		{
			/* Stable in ICU 75 */

			RESTRICTED,
			ALLOWED
		}

		/// <summary>
		/// Identifier Type constants.
		/// See https://www.unicode.org/reports/tr39/#Identifier_Status_and_Type.
		/// </summary>
		/// <remarks>
		/// Note: UIdentifierType constants are parsed by preparseucd.py.
		/// It matches lines like
		/// 	U_ID_TYPE_{Unicode Identifier_Type value name}
		/// </remarks>
		public enum UIdentifierType
		{
			/* Stable in ICU 75 */

			NOT_CHARACTER,
			DEPRECATED,
			DEFAULT_IGNORABLE,
			NOT_NFKC,
			NOT_XID,
			EXCLUSION,
			OBSOLETE,
			TECHNICAL,
			UNCOMMON_USE,
			LIMITED_USE,
			INCLUSION,
			RECOMMENDED
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
		/// <param name="characterCode">The Unicode code point to be checked.</param>
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
		/// <param name="characterCode">The Unicode code point to be checked.</param>
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
		/// <param name="characterCode">The Unicode code point to be checked.</param>
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
		/// <param name="characterCode">The Unicode code point to be checked.</param>
		public static bool IsPunct(int characterCode)
		{
			return NativeMethods.u_ispunct(characterCode);
		}

		/// <summary>Determines whether the code point has the Bidi_Mirrored property. </summary>
		/// <param name="characterCode">The Unicode code point to be checked.</param>
		public static bool IsMirrored(int characterCode)
		{
			return NativeMethods.u_isMirrored(characterCode);
		}

		/// <summary>Determines whether the specified code point is a control character, as
		/// defined by the ICU NativeMethods.u_iscntrl function.</summary>
		/// <param name="characterCode">The Unicode code point to be checked.</param>
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
		/// <param name="chr">The Unicode character to be checked.</param>
		public static bool IsControl(string chr)
		{
			return !string.IsNullOrEmpty(chr) && chr.Length == 1 && IsControl(chr[0]);
		}

		/// <summary>Determines whether the specified character is a space character, as
		/// defined by the ICU NativeMethods.u_isspace function.</summary>
		/// <param name="characterCode">The Unicode code point to be checked.</param>
		public static bool IsSpace(int characterCode)
		{
			return NativeMethods.u_isspace(characterCode);
		}

		/// <summary>
		///	Determines whether the specified character is a space character.
		/// </summary>
		/// <param name="chr">The Unicode character to be checked.</param>
		public static bool IsSpace(string chr)
		{
			return !string.IsNullOrEmpty(chr) && chr.Length == 1 && IsSpace(chr[0]);
		}

		/// <summary>
		/// Get the general character category value for the given code point.
		/// </summary>
		/// <param name="ch">the code point to be checked</param>
		public static UCharCategory GetCharType(int ch)
		{
			return (UCharCategory)NativeMethods.u_charType(ch);
		}

		/// <summary>
		/// Get the numeric value for a given code point.
		/// </summary>
		/// <param name="characterCode">The Unicode code point to be checked.</param>
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
		/// <param name="chr">The Unicode character to be checked.</param>
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
		/// <param name="code">The Unicode code point to be checked.</param>
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
