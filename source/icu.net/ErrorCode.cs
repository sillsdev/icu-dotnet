// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;

namespace Icu
{
	/// <summary>
	/// Error code to replace exception handling, so that the code is compatible
	/// with all C++ compilers, and to use the same mechanism for C and C++.
	/// Error codes should be tested using <see cref="ErrorCodeExtensions.IsFailure(ErrorCode)"/>
	/// and <see cref="ErrorCodeExtensions.IsSuccess(ErrorCode)"/>
	/// </summary>
	public enum ErrorCode
	{
		/// <summary>A resource bundle lookup returned a fallback result (not an error)</summary>
		USING_FALLBACK_WARNING = -128,
		/// <summary>Start of information results (semantically successful)</summary>
		ERROR_WARNING_START = -128,
		/// <summary>A resource bundle lookup returned a result from the root locale (not an error)</summary>
		USING_DEFAULT_WARNING = -127,
		/// <summary>A SafeClone operation required allocating memory (informational only)</summary>
		SAFECLONE_ALLOCATED_WARNING = -126,
		/// <summary>ICU has to use compatibility layer to construct the service. Expect performance/memory usage degradation. Consider upgrading</summary>
		STATE_OLD_WARNING = -125,
		/// <summary>An output string could not be NUL-terminated because output length==destCapacity.</summary>
		STRING_NOT_TERMINATED_WARNING = -124,
		/// <summary>Number of levels requested in getBound is higher than the number of levels in the sort key</summary>
		SORT_KEY_TOO_SHORT_WARNING = -123,
		/// <summary>This converter alias can go to different converter implementations</summary>
		AMBIGUOUS_ALIAS_WARNING = -122,
		/// <summary>ucol_open encountered a mismatch between UCA version and collator image version, so the collator was constructed from rules. No impact to further function</summary>
		DIFFERENT_UCA_VERSION = -121,
		/// <summary>This must always be the last warning value to indicate the limit for UErrorCode warnings (last warning code +1)</summary>
		ERROR_WARNING_LIMIT,
		/// <summary>No error, no warning.</summary>

		ZERO_ERROR = 0,
		/// <summary>No error, no warning.</summary>
		NoErrors = ZERO_ERROR,
		/// <summary>Start of codes indicating failure</summary>
		ILLEGAL_ARGUMENT_ERROR = 1,
		/// <summary>The requested resource cannot be found</summary>
		MISSING_RESOURCE_ERROR = 2,
		/// <summary>Data format is not what is expected</summary>
		INVALID_FORMAT_ERROR = 3,
		/// <summary>The requested file cannot be found</summary>
		FILE_ACCESS_ERROR = 4,
		/// <summary>Indicates a bug in the library code</summary>
		INTERNAL_PROGRAM_ERROR = 5,
		/// <summary>Unable to parse a message (message format)</summary>
		MESSAGE_PARSE_ERROR = 6,
		/// <summary>Memory allocation error</summary>
		MEMORY_ALLOCATION_ERROR = 7,
		/// <summary>Trying to access the index that is out of bounds</summary>
		INDEX_OUTOFBOUNDS_ERROR = 8,
		/// <summary>Equivalent to Java ParseException</summary>
		PARSE_ERROR = 9,
		/// <summary>Character conversion: Unmappable input sequence. In other APIs: Invalid character.</summary>
		INVALID_CHAR_FOUND = 10,
		/// <summary>Character conversion: Incomplete input sequence.</summary>
		TRUNCATED_CHAR_FOUND = 11,
		/// <summary>Character conversion: Illegal input sequence/combination of input units.</summary>
		ILLEGAL_CHAR_FOUND = 12,
		/// <summary>Conversion table file found, but corrupted</summary>
		INVALID_TABLE_FORMAT = 13,
		/// <summary>Conversion table file not found</summary>
		INVALID_TABLE_FILE = 14,
		/// <summary>A result would not fit in the supplied buffer</summary>
		BUFFER_OVERFLOW_ERROR = 15,
		/// <summary>Requested operation not supported in current context</summary>
		UNSUPPORTED_ERROR = 16,
		/// <summary>an operation is requested over a resource that does not support it</summary>
		RESOURCE_TYPE_MISMATCH = 17,
		/// <summary>ISO-2022 illegal escape sequence</summary>
		ILLEGAL_ESCAPE_SEQUENCE = 18,
		/// <summary>ISO-2022 unsupported escape sequence</summary>
		UNSUPPORTED_ESCAPE_SEQUENCE = 19,
		/// <summary>No space available for in-buffer expansion for Arabic shaping</summary>
		NO_SPACE_AVAILABLE = 20,
		/// <summary>Currently used only while setting variable top, but can be used generally</summary>
		CE_NOT_FOUND_ERROR = 21,
		/// <summary>User tried to set variable top to a primary that is longer than two bytes</summary>
		PRIMARY_TOO_LONG_ERROR = 22,
		/// <summary>ICU cannot construct a service from this state, as it is no longer supported</summary>
		STATE_TOO_OLD_ERROR = 23,
		/// <summary>
		/// There are too many aliases in the path to the requested resource.
		/// It is very possible that a circular alias definition has occurred
		/// </summary>
		TOO_MANY_ALIASES_ERROR = 24,
		/// <summary>UEnumeration out of sync with underlying collection</summary>
		ENUM_OUT_OF_SYNC_ERROR = 25,
		/// <summary>Unable to convert a UChar* string to char* with the invariant converter.</summary>
		INVARIANT_CONVERSION_ERROR = 26,
		/// <summary>Requested operation can not be completed with ICU in its current state</summary>
		INVALID_STATE_ERROR = 27,
		/// <summary>Collator version is not compatible with the base version</summary>
		COLLATOR_VERSION_MISMATCH = 28,
		/// <summary>Collator is options only and no base is specified</summary>
		USELESS_COLLATOR_ERROR = 29,
		/// <summary>Attempt to modify read-only or constant data.</summary>
		NO_WRITE_PERMISSION = 30,
		/// <summary>This must always be the last value to indicate the limit for standard errors</summary>
		STANDARD_ERROR_LIMIT,
		/*
		 * the error code range 0x10000 0x10100 are reserved for Transliterator
		 */
		/// <summary>Missing '$' or duplicate variable name</summary>
		BAD_VARIABLE_DEFINITION = 0x10000,
		/// <summary>Start of Transliterator errors</summary>
		PARSE_ERROR_START = 0x10000,
		/// <summary>Elements of a rule are misplaced</summary>
		MALFORMED_RULE,
		/// <summary>A UnicodeSet pattern is invalid</summary>
		MALFORMED_SET,
		/// <summary>UNUSED as of ICU 2.4</summary>
		MALFORMED_SYMBOL_REFERENCE,
		/// <summary>A Unicode escape pattern is invalid</summary>
		MALFORMED_UNICODE_ESCAPE,
		/// <summary>A variable definition is invalid</summary>
		MALFORMED_VARIABLE_DEFINITION,
		/// <summary>A variable reference is invalid</summary>
		MALFORMED_VARIABLE_REFERENCE,
		/// <summary>UNUSED as of ICU 2.4</summary>
		MISMATCHED_SEGMENT_DELIMITERS,
		/// <summary>A start anchor appears at an illegal position</summary>
		MISPLACED_ANCHOR_START,
		/// <summary>A cursor offset occurs at an illegal position</summary>
		MISPLACED_CURSOR_OFFSET,
		/// <summary>A quantifier appears after a segment close delimiter</summary>
		MISPLACED_QUANTIFIER,
		/// <summary>A rule contains no operator</summary>
		MISSING_OPERATOR,
		/// <summary>UNUSED as of ICU 2.4</summary>
		MISSING_SEGMENT_CLOSE,
		/// <summary>More than one ante context</summary>
		MULTIPLE_ANTE_CONTEXTS,
		/// <summary>More than one cursor</summary>
		MULTIPLE_CURSORS,
		/// <summary>More than one post context</summary>
		MULTIPLE_POST_CONTEXTS,
		/// <summary>A dangling backslash</summary>
		TRAILING_BACKSLASH,
		/// <summary>A segment reference does not correspond to a defined segment</summary>
		UNDEFINED_SEGMENT_REFERENCE,
		/// <summary>A variable reference does not correspond to a defined variable</summary>
		UNDEFINED_VARIABLE,
		/// <summary>A special character was not quoted or escaped</summary>
		UNQUOTED_SPECIAL,
		/// <summary>A closing single quote is missing</summary>
		UNTERMINATED_QUOTE,
		/// <summary>A rule is hidden by an earlier more general rule</summary>
		RULE_MASK_ERROR,
		/// <summary>A compound filter is in an invalid location</summary>
		MISPLACED_COMPOUND_FILTER,
		/// <summary>More than one compound filter</summary>
		MULTIPLE_COMPOUND_FILTERS,
		/// <summary>A "::id" rule was passed to the RuleBasedTransliterator parser</summary>
		INVALID_RBT_SYNTAX,
		/// <summary>UNUSED as of ICU 2.4</summary>
		INVALID_PROPERTY_PATTERN,
		/// <summary>A 'use' pragma is invalid</summary>
		MALFORMED_PRAGMA,
		/// <summary>A closing ')' is missing</summary>
		UNCLOSED_SEGMENT,
		/// <summary>UNUSED as of ICU 2.4</summary>
		ILLEGAL_CHAR_IN_SEGMENT,
		/// <summary>Too many stand-ins generated for the given variable range</summary>
		VARIABLE_RANGE_EXHAUSTED,
		/// <summary>The variable range overlaps characters used in rules</summary>
		VARIABLE_RANGE_OVERLAP,
		/// <summary>A special character is outside its allowed context</summary>
		ILLEGAL_CHARACTER,
		/// <summary>Internal transliterator system error</summary>
		INTERNAL_TRANSLITERATOR_ERROR,
		/// <summary>A "::id" rule specifies an unknown transliterator</summary>
		INVALID_ID,
		/// <summary>A "&amp;fn()" rule specifies an unknown transliterator</summary>
		INVALID_FUNCTION,
		/// <summary>The limit for Transliterator errors</summary>
		PARSE_ERROR_LIMIT,
		/*
		 * the error code range 0x10100 0x10200 are reserved for formatting API parsing error
		 */
		/// <summary>Syntax error in format pattern</summary>
		UNEXPECTED_TOKEN = 0x10100,
		/// <summary>Start of format library errors</summary>
		FMT_PARSE_ERROR_START = 0x10100,
		/// <summary>More than one decimal separator in number pattern</summary>
		MULTIPLE_DECIMAL_SEPARATORS,
		/// <summary>More than one exponent symbol in number pattern</summary>
		MULTIPLE_EXPONENTIAL_SYMBOLS,
		/// <summary>Grouping symbol in exponent pattern</summary>
		MALFORMED_EXPONENTIAL_PATTERN,
		/// <summary>More than one percent symbol in number pattern</summary>
		MULTIPLE_PERCENT_SYMBOLS,
		/// <summary>More than one permill symbol in number pattern</summary>
		MULTIPLE_PERMILL_SYMBOLS,
		/// <summary>More than one pad symbol in number pattern</summary>
		MULTIPLE_PAD_SPECIFIERS,
		/// <summary>Syntax error in format pattern</summary>
		PATTERN_SYNTAX_ERROR,
		/// <summary>Pad symbol misplaced in number pattern</summary>
		ILLEGAL_PAD_POSITION,
		/// <summary>Braces do not match in message pattern</summary>
		UNMATCHED_BRACES,
		/// <summary>UNUSED as of ICU 2.4</summary>
		UNSUPPORTED_PROPERTY,
		/// <summary>UNUSED as of ICU 2.4</summary>
		UNSUPPORTED_ATTRIBUTE,
		/// <summary>Argument name and argument index mismatch in MessageFormat functions.</summary>
		ARGUMENT_TYPE_MISMATCH,
		/// <summary>Duplicate keyword in PluralFormat.</summary>
		DUPLICATE_KEYWORD,
		/// <summary>Undefined Plural keyword.</summary>
		UNDEFINED_KEYWORD,
		/// <summary>Missing DEFAULT rule in plural rules.</summary>
		DEFAULT_KEYWORD_MISSING,
		/// <summary>The limit for format library errors</summary>
		FMT_PARSE_ERROR_LIMIT,
		/*
		 * the error code range 0x10200 0x102ff are reserved for Break Iterator related error
		 */
		/// <summary>An internal error (bug) was detected.</summary>
		BRK_INTERNAL_ERROR = 0x10200,
		/// <summary>Start of codes indicating Break Iterator failures</summary>
		BRK_ERROR_START = 0x10200,
		/// <summary>Hex digits expected as part of a escaped char in a rule.</summary>
		BRK_HEX_DIGITS_EXPECTED,
		/// <summary>Missing ';' at the end of an RBBI rule.</summary>
		BRK_SEMICOLON_EXPECTED,
		/// <summary>Syntax error in RBBI rule.</summary>
		BRK_RULE_SYNTAX,
		/// <summary>UnicodeSet writing an RBBI rule missing a closing ']'.</summary>
		BRK_UNCLOSED_SET,
		/// <summary>Syntax error in RBBI rule assignment statement.</summary>
		BRK_ASSIGN_ERROR,
		/// <summary>RBBI rule $Variable redefined.</summary>
		BRK_VARIABLE_REDFINITION,
		/// <summary>Mis-matched parentheses in an RBBI rule.</summary>
		BRK_MISMATCHED_PAREN,
		/// <summary>Missing closing quote in an RBBI rule.</summary>
		BRK_NEW_LINE_IN_QUOTED_STRING,
		/// <summary>Use of an undefined $Variable in an RBBI rule.</summary>
		BRK_UNDEFINED_VARIABLE,
		/// <summary>Initialization failure.  Probable missing ICU Data.</summary>
		BRK_INIT_ERROR,
		/// <summary>Rule contains an empty Unicode Set.</summary>
		BRK_RULE_EMPTY_SET,
		/// <summary>!!option in RBBI rules not recognized.</summary>
		BRK_UNRECOGNIZED_OPTION,
		/// <summary>The {nnn} tag on a rule is mal formed</summary>
		BRK_MALFORMED_RULE_TAG,
		/// <summary>This must always be the last value to indicate the limit for Break Iterator failures</summary>
		BRK_ERROR_LIMIT,
		/*
		 * The error codes in the range 0x10300-0x103ff are reserved for regular expression related errors
		 */
		/// <summary>An internal error (bug) was detected.</summary>
		REGEX_INTERNAL_ERROR = 0x10300,
		/// <summary>Start of codes indicating Regexp failures</summary>
		REGEX_ERROR_START = 0x10300,
		/// <summary>Syntax error in regexp pattern.</summary>
		REGEX_RULE_SYNTAX,
		/// <summary>RegexMatcher in invalid state for requested operation</summary>
		REGEX_INVALID_STATE,
		/// <summary>Unrecognized backslash escape sequence in pattern</summary>
		REGEX_BAD_ESCAPE_SEQUENCE,
		/// <summary>Incorrect Unicode property</summary>
		REGEX_PROPERTY_SYNTAX,
		/// <summary>Use of regexp feature that is not yet implemented.</summary>
		REGEX_UNIMPLEMENTED,
		/// <summary>Incorrectly nested parentheses in regexp pattern.</summary>
		REGEX_MISMATCHED_PAREN,
		/// <summary>Decimal number is too large.</summary>
		REGEX_NUMBER_TOO_BIG,
		/// <summary>Error in {min,max} interval</summary>
		REGEX_BAD_INTERVAL,
		/// <summary>In {min,max}, max is less than min.</summary>
		REGEX_MAX_LT_MIN,
		/// <summary>Back-reference to a non-existent capture group.</summary>
		REGEX_INVALID_BACK_REF,
		/// <summary>Invalid value for match mode flags.</summary>
		REGEX_INVALID_FLAG,
		/// <summary>Look-Behind pattern matches must have a bounded maximum length.</summary>
		REGEX_LOOK_BEHIND_LIMIT,
		/// <summary>Regexps cannot have UnicodeSets containing strings.</summary>
		REGEX_SET_CONTAINS_STRING,
		/// <summary>Octal character constants must be &lt;= 0377.</summary>
		REGEX_OCTAL_TOO_BIG,
		/// <summary>Missing closing bracket on a bracket expression.</summary>
		REGEX_MISSING_CLOSE_BRACKET,
		/// <summary>In a character range [x-y], x is greater than y.</summary>
		REGEX_INVALID_RANGE,
		/// <summary>Regular expression backtrack stack overflow.</summary>
		REGEX_STACK_OVERFLOW,
		/// <summary>Maximum allowed match time exceeded.</summary>
		REGEX_TIME_OUT,
		/// <summary>Matching operation aborted by user callback fn.</summary>
		REGEX_STOPPED_BY_CALLER,
		/// <summary>This must always be the last value to indicate the limit for regexp errors</summary>
		REGEX_ERROR_LIMIT,

		/*
		 * The error code in the range 0x10400-0x104ff are reserved for IDNA related error codes
		 */
		/// <summary></summary>
		IDNA_PROHIBITED_ERROR = 0x10400,
		/// <summary>Start of codes indicating IDNA failures</summary>
		IDNA_ERROR_START = 0x10400,
		/// <summary></summary>
		IDNA_UNASSIGNED_ERROR,
		/// <summary></summary>
		IDNA_CHECK_BIDI_ERROR,
		/// <summary></summary>
		IDNA_STD3_ASCII_RULES_ERROR,
		/// <summary></summary>
		IDNA_ACE_PREFIX_ERROR,
		/// <summary></summary>
		IDNA_VERIFICATION_ERROR,
		/// <summary></summary>
		IDNA_LABEL_TOO_LONG_ERROR,
		/// <summary></summary>
		IDNA_ZERO_LENGTH_LABEL_ERROR,
		/// <summary></summary>
		IDNA_DOMAIN_NAME_TOO_LONG_ERROR,
		/// <summary>This must always be the last value to indicate the limit for IDNA errors</summary>
		IDNA_ERROR_LIMIT,
		/*
		 * Aliases for StringPrep
		 */
		/// <summary></summary>
		STRINGPREP_PROHIBITED_ERROR = IDNA_PROHIBITED_ERROR,
		/// <summary></summary>
		STRINGPREP_UNASSIGNED_ERROR = IDNA_UNASSIGNED_ERROR,
		/// <summary></summary>
		STRINGPREP_CHECK_BIDI_ERROR = IDNA_CHECK_BIDI_ERROR,

		/// <summary>This must always be the last value to indicate the limit for UErrorCode (last error code +1)</summary>
		ERROR_LIMIT = IDNA_ERROR_LIMIT
	}

	internal static class ErrorCodeExtensions
	{
		/// <summary>
		/// Determines whether the operation was successful or not.
		/// http://icu-project.org/apiref/icu4c/utypes_8h_source.html#l00709
		/// </summary>
		public static bool IsSuccess(this ErrorCode errorCode)
		{
			return errorCode <= ErrorCode.ZERO_ERROR;
		}

		/// <summary>
		/// Determines whether the operation resulted in an error.
		/// http://icu-project.org/apiref/icu4c/utypes_8h_source.html#l00714
		/// </summary>
		public static bool IsFailure(this ErrorCode errorCode)
		{
			return errorCode > ErrorCode.ZERO_ERROR;
		}
	}

	internal class ExceptionFromErrorCode
	{
		public static void ThrowIfError(ErrorCode e)
		{
			ThrowIfError(e, string.Empty, false);
		}

		public static void ThrowIfErrorOrWarning(ErrorCode e)
		{
			ThrowIfError(e, string.Empty, true);
		}

		public static void ThrowIfError(ErrorCode e, string extraInfo)
		{
			ThrowIfError(e, extraInfo, false);
		}

		public static void ThrowIfErrorOrWarning(ErrorCode e, string extraInfo)
		{
			ThrowIfError(e, extraInfo, true);
		}

		private static void ThrowIfError(ErrorCode e, string extraInfo, bool throwOnWarnings)
		{
			switch (e)
			{
				case ErrorCode.ZERO_ERROR: // the only case to not throw!
					break;
				case ErrorCode.USING_FALLBACK_WARNING:
					if (throwOnWarnings)
						throw new WarningException("Warning: A resource bundle lookup returned a fallback result " + extraInfo);

					break;
				case ErrorCode.USING_DEFAULT_WARNING:
					if (throwOnWarnings)
						throw new WarningException("Warning: A resource bundle lookup returned a result from the root locale " + extraInfo);

					break;
				case ErrorCode.SAFECLONE_ALLOCATED_WARNING:
					if (throwOnWarnings)
						throw new WarningException("Notice: A SafeClone operation required allocating memory " + extraInfo);

					break;
				case ErrorCode.STATE_OLD_WARNING:
					if (throwOnWarnings)
						throw new WarningException("ICU has to use compatibility layer to construct the service. Expect performance/memory usage degradation. Consider upgrading " + extraInfo);

					break;
				case ErrorCode.STRING_NOT_TERMINATED_WARNING:
					if (throwOnWarnings)
						throw new WarningException("An output string could not be NUL-terminated because output length==destCapacity. " + extraInfo);

					break;
				case ErrorCode.SORT_KEY_TOO_SHORT_WARNING:
					if (throwOnWarnings)
						throw new WarningException("Number of levels requested in getBound is higher than the number of levels in the sort key " + extraInfo);

					break;
				case ErrorCode.AMBIGUOUS_ALIAS_WARNING:
					if (throwOnWarnings)
						throw new WarningException("This converter alias can go to different converter implementations " + extraInfo);

					break;
				case ErrorCode.DIFFERENT_UCA_VERSION:
					if (throwOnWarnings)
						throw new WarningException(
								"Warning: ucol_open encountered a mismatch between UCA version and collator image version, so the collator was constructed from rules. No impact to further function " + extraInfo);

					break;
				case ErrorCode.ILLEGAL_ARGUMENT_ERROR:
					throw new ArgumentException(extraInfo);
				case ErrorCode.MISSING_RESOURCE_ERROR:
					throw new MissingResourceException("The requested resource cannot be found " + extraInfo);
				case ErrorCode.INVALID_FORMAT_ERROR:
					throw new ArgumentException("Data format is not what is expected " + extraInfo);
				case ErrorCode.FILE_ACCESS_ERROR:
					throw new FileNotFoundException("The requested file cannot be found " + extraInfo);
				case ErrorCode.INTERNAL_PROGRAM_ERROR:
					throw new InvalidOperationException("Indicates a bug in the library code " + extraInfo);
				case ErrorCode.MESSAGE_PARSE_ERROR:
					throw new ArgumentException("Unable to parse a message (message format) " + extraInfo);
				case ErrorCode.MEMORY_ALLOCATION_ERROR:
					throw new OutOfMemoryException(extraInfo);
				case ErrorCode.INDEX_OUTOFBOUNDS_ERROR:
					throw new IndexOutOfRangeException(extraInfo);
				case ErrorCode.PARSE_ERROR:
					throw new SyntaxErrorException("Parse Error " + extraInfo);
				case ErrorCode.INVALID_CHAR_FOUND:
					throw new ArgumentException("Character conversion: Unmappable input sequence. In other APIs: Invalid character. " + extraInfo);
				case ErrorCode.TRUNCATED_CHAR_FOUND:
					throw new ArgumentException("Character conversion: Incomplete input sequence. " + extraInfo);
				case ErrorCode.ILLEGAL_CHAR_FOUND:
					throw new ArgumentException("Character conversion: Illegal input sequence/combination of input units. " + extraInfo);
				case ErrorCode.INVALID_TABLE_FORMAT:
					throw new InvalidDataException("Conversion table file found, but corrupted " + extraInfo);
				case ErrorCode.INVALID_TABLE_FILE:
					throw new FileNotFoundException("Conversion table file not found " + extraInfo);
				case ErrorCode.BUFFER_OVERFLOW_ERROR:
					throw new OverflowException("A result would not fit in the supplied buffer " + extraInfo);
				case ErrorCode.UNSUPPORTED_ERROR:
					throw new InvalidOperationException("Requested operation not supported in current context " + extraInfo);
				case ErrorCode.RESOURCE_TYPE_MISMATCH:
					throw new InvalidOperationException("an operation is requested over a resource that does not support it " + extraInfo);
				case ErrorCode.ILLEGAL_ESCAPE_SEQUENCE:
					throw new ArgumentException("ISO-2022 illegal escape sequence " + extraInfo);
				case ErrorCode.UNSUPPORTED_ESCAPE_SEQUENCE:
					throw new ArgumentException("ISO-2022 unsupported escape sequence " + extraInfo);
				case ErrorCode.NO_SPACE_AVAILABLE:
					throw new ArgumentException("No space available for in-buffer expansion for Arabic shaping " + extraInfo);
				case ErrorCode.CE_NOT_FOUND_ERROR:
					throw new ArgumentException("Collation Element not found " + extraInfo);
				case ErrorCode.PRIMARY_TOO_LONG_ERROR:
					throw new ArgumentException("User tried to set variable top to a primary that is longer than two bytes " + extraInfo);
				case ErrorCode.STATE_TOO_OLD_ERROR:
					throw new NotSupportedException("ICU cannot construct a service from this state, as it is no longer supported " + extraInfo);
				case ErrorCode.TOO_MANY_ALIASES_ERROR:
					throw new ArgumentException("There are too many aliases in the path to the requested resource.\nIt is very possible that a circular alias definition has occured " + extraInfo);
				case ErrorCode.ENUM_OUT_OF_SYNC_ERROR:
					throw new InvalidOperationException("Enumeration out of sync with underlying collection " + extraInfo);
				case ErrorCode.INVARIANT_CONVERSION_ERROR:
					throw new ArgumentException("Unable to convert a UChar* string to char* with the invariant converter " + extraInfo);
				case ErrorCode.INVALID_STATE_ERROR:
					throw new InvalidOperationException("Requested operation can not be completed with ICU in its current state " + extraInfo);
				case ErrorCode.COLLATOR_VERSION_MISMATCH:
					throw new InvalidOperationException("Collator version is not compatible with the base version " + extraInfo);
				case ErrorCode.USELESS_COLLATOR_ERROR:
					throw new ArgumentException("Collator is options only and no base is specified " + extraInfo);
				case ErrorCode.NO_WRITE_PERMISSION:
					throw new InvalidOperationException("Attempt to modify read-only or constant data. " + extraInfo);
				case ErrorCode.BAD_VARIABLE_DEFINITION:
					throw new TransliteratorParseException("Transliterator Parse Error: Missing '$' or duplicate variable name " + extraInfo);
				case ErrorCode.MALFORMED_RULE:
					throw new TransliteratorParseException("Transliterator Parse Error: Elements of a rule are misplaced " + extraInfo);
				case ErrorCode.MALFORMED_SET:
					throw new TransliteratorParseException("Transliterator Parse Error: A UnicodeSet pattern is invalid " + extraInfo);
				case ErrorCode.MALFORMED_UNICODE_ESCAPE:
					throw new TransliteratorParseException("Transliterator Parse Error: A Unicode escape pattern is invalid " + extraInfo);
				case ErrorCode.MALFORMED_VARIABLE_DEFINITION:
					throw new TransliteratorParseException("Transliterator Parse Error: A variable definition is invalid " + extraInfo);
				case ErrorCode.MALFORMED_VARIABLE_REFERENCE:
					throw new TransliteratorParseException("Transliterator Parse Error: A variable reference is invalid " + extraInfo);
				case ErrorCode.MISPLACED_ANCHOR_START:
					throw new TransliteratorParseException("Transliterator Parse Error: A start anchor appears at an illegal position " + extraInfo);
				case ErrorCode.MISPLACED_CURSOR_OFFSET:
					throw new TransliteratorParseException("Transliterator Parse Error: A cursor offset occurs at an illegal position " + extraInfo);
				case ErrorCode.MISPLACED_QUANTIFIER:
					throw new TransliteratorParseException("Transliterator Parse Error: A quantifier appears after a segment close delimiter " + extraInfo);
				case ErrorCode.MISSING_OPERATOR:
					throw new TransliteratorParseException("Transliterator Parse Error: A rule contains no operator " + extraInfo);
				case ErrorCode.MULTIPLE_ANTE_CONTEXTS:
					throw new TransliteratorParseException("Transliterator Parse Error: More than one ante context " + extraInfo);
				case ErrorCode.MULTIPLE_CURSORS:
					throw new TransliteratorParseException("Transliterator Parse Error: More than one cursor " + extraInfo);
				case ErrorCode.MULTIPLE_POST_CONTEXTS:
					throw new TransliteratorParseException("Transliterator Parse Error: More than one post context " + extraInfo);
				case ErrorCode.TRAILING_BACKSLASH:
					throw new TransliteratorParseException("Transliterator Parse Error: A dangling backslash " + extraInfo);
				case ErrorCode.UNDEFINED_SEGMENT_REFERENCE:
					throw new TransliteratorParseException("Transliterator Parse Error: A segment reference does not correspond to a defined segment " + extraInfo);
				case ErrorCode.UNDEFINED_VARIABLE:
					throw new TransliteratorParseException("Transliterator Parse Error: A variable reference does not correspond to a defined variable " + extraInfo);
				case ErrorCode.UNQUOTED_SPECIAL:
					throw new TransliteratorParseException("Transliterator Parse Error: A special character was not quoted or escaped " + extraInfo);
				case ErrorCode.UNTERMINATED_QUOTE:
					throw new TransliteratorParseException("Transliterator Parse Error: A closing single quote is missing " + extraInfo);
				case ErrorCode.RULE_MASK_ERROR:
					throw new TransliteratorParseException("Transliterator Parse Error: A rule is hidden by an earlier more general rule " + extraInfo);
				case ErrorCode.MISPLACED_COMPOUND_FILTER:
					throw new TransliteratorParseException("Transliterator Parse Error: A compound filter is in an invalid location " + extraInfo);
				case ErrorCode.MULTIPLE_COMPOUND_FILTERS:
					throw new TransliteratorParseException("Transliterator Parse Error: More than one compound filter " + extraInfo);
				case ErrorCode.INVALID_RBT_SYNTAX:
					throw new TransliteratorParseException("Transliterator Parse Error: A '::id' rule was passed to the RuleBasedTransliterator parser " + extraInfo);
				case ErrorCode.MALFORMED_PRAGMA:
					throw new TransliteratorParseException("Transliterator Parse Error: A 'use' pragma is invalid " + extraInfo);
				case ErrorCode.UNCLOSED_SEGMENT:
					throw new TransliteratorParseException("Transliterator Parse Error: A closing ')' is missing " + extraInfo);
				case ErrorCode.VARIABLE_RANGE_EXHAUSTED:
					throw new TransliteratorParseException("Transliterator Parse Error: Too many stand-ins generated for the given variable range " + extraInfo);
				case ErrorCode.VARIABLE_RANGE_OVERLAP:
					throw new TransliteratorParseException("Transliterator Parse Error: The variable range overlaps characters used in rules " + extraInfo);
				case ErrorCode.ILLEGAL_CHARACTER:
					throw new TransliteratorParseException("Transliterator Parse Error: A special character is outside its allowed context " + extraInfo);
				case ErrorCode.INTERNAL_TRANSLITERATOR_ERROR:
					throw new TransliteratorParseException("Transliterator Parse Error: Internal transliterator system error " + extraInfo);
				case ErrorCode.INVALID_ID:
					throw new TransliteratorParseException("Transliterator Parse Error: A '::id' rule specifies an unknown transliterator " + extraInfo);
				case ErrorCode.INVALID_FUNCTION:
					throw new TransliteratorParseException("Transliterator Parse Error: A '&fn()' rule specifies an unknown transliterator " + extraInfo);
				case ErrorCode.UNEXPECTED_TOKEN:
					throw new SyntaxErrorException("Format Parse Error: Unexpected token in format pattern " + extraInfo);
				case ErrorCode.MULTIPLE_DECIMAL_SEPARATORS:
					throw new SyntaxErrorException("Format Parse Error: More than one decimal separator in number pattern " + extraInfo);
				case ErrorCode.MULTIPLE_EXPONENTIAL_SYMBOLS:
					throw new SyntaxErrorException("Format Parse Error: More than one exponent symbol in number pattern " + extraInfo);
				case ErrorCode.MALFORMED_EXPONENTIAL_PATTERN:
					throw new SyntaxErrorException("Format Parse Error: Grouping symbol in exponent pattern " + extraInfo);
				case ErrorCode.MULTIPLE_PERCENT_SYMBOLS:
					throw new SyntaxErrorException("Format Parse Error: More than one percent symbol in number pattern " + extraInfo);
				case ErrorCode.MULTIPLE_PERMILL_SYMBOLS:
					throw new SyntaxErrorException("Format Parse Error: More than one permill symbol in number pattern " + extraInfo);
				case ErrorCode.MULTIPLE_PAD_SPECIFIERS:
					throw new SyntaxErrorException("Format Parse Error: More than one pad symbol in number pattern " + extraInfo);
				case ErrorCode.PATTERN_SYNTAX_ERROR:
					throw new SyntaxErrorException("Format Parse Error: Syntax error in format pattern " + extraInfo);
				case ErrorCode.ILLEGAL_PAD_POSITION:
					throw new SyntaxErrorException("Format Parse Error: Pad symbol misplaced in number pattern " + extraInfo);
				case ErrorCode.UNMATCHED_BRACES:
					throw new SyntaxErrorException("Format Parse Error: Braces do not match in message pattern " + extraInfo);
				case ErrorCode.ARGUMENT_TYPE_MISMATCH:
					throw new SyntaxErrorException("Format Parse Error: Argument name and argument index mismatch in MessageFormat functions. " + extraInfo);
				case ErrorCode.DUPLICATE_KEYWORD:
					throw new SyntaxErrorException("Format Parse Error: Duplicate keyword in PluralFormat. " + extraInfo);
				case ErrorCode.UNDEFINED_KEYWORD:
					throw new SyntaxErrorException("Format Parse Error: Undefined Plural keyword. " + extraInfo);
				case ErrorCode.DEFAULT_KEYWORD_MISSING:
					throw new SyntaxErrorException("Format Parse Error: Missing DEFAULT rule in plural rules. " + extraInfo);
				case ErrorCode.BRK_INTERNAL_ERROR:
					throw new BreakException("Break Error: An internal error (bug) was detected. " + extraInfo);
				case ErrorCode.BRK_HEX_DIGITS_EXPECTED:
					throw new BreakException("Break Error: Hex digits expected as part of a escaped char in a rule. " + extraInfo);
				case ErrorCode.BRK_SEMICOLON_EXPECTED:
					throw new BreakException("Break Error: Missing ';' at the end of an RBBI rule. " + extraInfo);
				case ErrorCode.BRK_RULE_SYNTAX:
					throw new BreakException("Break Error: Syntax error in RBBI rule. " + extraInfo);
				case ErrorCode.BRK_UNCLOSED_SET:
					throw new BreakException("Break Error: UnicodeSet writing an RBBI rule missing a closing ']'. " + extraInfo);
				case ErrorCode.BRK_ASSIGN_ERROR:
					throw new BreakException("Break Error: Syntax error in RBBI rule assignment statement. " + extraInfo);
				case ErrorCode.BRK_VARIABLE_REDFINITION:
					throw new BreakException("Break Error: RBBI rule $Variable redefined. " + extraInfo);
				case ErrorCode.BRK_MISMATCHED_PAREN:
					throw new BreakException("Break Error: Mis-matched parentheses in an RBBI rule. " + extraInfo);
				case ErrorCode.BRK_NEW_LINE_IN_QUOTED_STRING:
					throw new BreakException("Break Error: Missing closing quote in an RBBI rule. " + extraInfo);
				case ErrorCode.BRK_UNDEFINED_VARIABLE:
					throw new BreakException("Break Error: Use of an undefined $Variable in an RBBI rule. " + extraInfo);
				case ErrorCode.BRK_INIT_ERROR:
					throw new BreakException("Break Error: Initialization failure.  Probable missing ICU Data. " + extraInfo);
				case ErrorCode.BRK_RULE_EMPTY_SET:
					throw new BreakException("Break Error: Rule contains an empty Unicode Set. " + extraInfo);
				case ErrorCode.BRK_UNRECOGNIZED_OPTION:
					throw new BreakException("Break Error: !!option in RBBI rules not recognized. " + extraInfo);
				case ErrorCode.BRK_MALFORMED_RULE_TAG:
					throw new BreakException("Break Error: The {nnn} tag on a rule is mal formed " + extraInfo);
				case ErrorCode.BRK_ERROR_LIMIT:
					throw new BreakException("Break Error: This must always be the last value to indicate the limit for Break Iterator failures " + extraInfo);
				case ErrorCode.REGEX_INTERNAL_ERROR:
					throw new RegexException("RegEx Error: An internal error (bug) was detected. " + extraInfo);
				case ErrorCode.REGEX_RULE_SYNTAX:
					throw new RegexException("RegEx Error: Syntax error in regexp pattern. " + extraInfo);
				case ErrorCode.REGEX_INVALID_STATE:
					throw new RegexException("RegEx Error: RegexMatcher in invalid state for requested operation " + extraInfo);
				case ErrorCode.REGEX_BAD_ESCAPE_SEQUENCE:
					throw new RegexException("RegEx Error: Unrecognized backslash escape sequence in pattern " + extraInfo);
				case ErrorCode.REGEX_PROPERTY_SYNTAX:
					throw new RegexException("RegEx Error: Incorrect Unicode property " + extraInfo);
				case ErrorCode.REGEX_UNIMPLEMENTED:
					throw new RegexException("RegEx Error: Use of regexp feature that is not yet implemented. " + extraInfo);
				case ErrorCode.REGEX_MISMATCHED_PAREN:
					throw new RegexException("RegEx Error: Incorrectly nested parentheses in regexp pattern. " + extraInfo);
				case ErrorCode.REGEX_NUMBER_TOO_BIG:
					throw new RegexException("RegEx Error: Decimal number is too large. " + extraInfo);
				case ErrorCode.REGEX_BAD_INTERVAL:
					throw new RegexException("RegEx Error: Error in {min,max} interval " + extraInfo);
				case ErrorCode.REGEX_MAX_LT_MIN:
					throw new RegexException("RegEx Error: In {min,max}, max is less than min. " + extraInfo);
				case ErrorCode.REGEX_INVALID_BACK_REF:
					throw new RegexException("RegEx Error: Back-reference to a non-existent capture group. " + extraInfo);
				case ErrorCode.REGEX_INVALID_FLAG:
					throw new RegexException("RegEx Error: Invalid value for match mode flags. " + extraInfo);
				case ErrorCode.REGEX_LOOK_BEHIND_LIMIT:
					throw new RegexException("RegEx Error: Look-Behind pattern matches must have a bounded maximum length. " + extraInfo);
				case ErrorCode.REGEX_SET_CONTAINS_STRING:
					throw new RegexException("RegEx Error: Regexps cannot have UnicodeSets containing strings. " + extraInfo);
				case ErrorCode.REGEX_OCTAL_TOO_BIG:
					throw new RegexException("Regex Error: Octal character constants must be <= 0377. " + extraInfo);
				case ErrorCode.REGEX_MISSING_CLOSE_BRACKET:
					throw new RegexException("Regex Error: Missing closing bracket on a bracket expression. " + extraInfo);
				case ErrorCode.REGEX_INVALID_RANGE:
					throw new RegexException("Regex Error: In a character range [x-y], x is greater than y. " + extraInfo);
				case ErrorCode.REGEX_STACK_OVERFLOW:
					throw new RegexException("Regex Error: Regular expression backtrack stack overflow. " + extraInfo);
				case ErrorCode.REGEX_TIME_OUT:
					throw new RegexException("Regex Error: Maximum allowed match time exceeded. " + extraInfo);
				case ErrorCode.REGEX_STOPPED_BY_CALLER:
					throw new RegexException("Regex Error: Matching operation aborted by user callback fn. " + extraInfo);
				case ErrorCode.REGEX_ERROR_LIMIT:
					throw new RegexException("RegEx Error:  " + extraInfo);
				case ErrorCode.IDNA_PROHIBITED_ERROR:
					throw new IDNAException("IDNA Error: Prohibited " + extraInfo);
				case ErrorCode.IDNA_UNASSIGNED_ERROR:
					throw new IDNAException("IDNA Error: Unassigned " + extraInfo);
				case ErrorCode.IDNA_CHECK_BIDI_ERROR:
					throw new IDNAException("IDNA Error: Check Bidi " + extraInfo);
				case ErrorCode.IDNA_STD3_ASCII_RULES_ERROR:
					throw new IDNAException("IDNA Error: Std3 Ascii Rules Error " + extraInfo);
				case ErrorCode.IDNA_ACE_PREFIX_ERROR:
					throw new IDNAException("IDNA Error: Ace Prefix Error " + extraInfo);
				case ErrorCode.IDNA_VERIFICATION_ERROR:
					throw new IDNAException("IDNA Error: Verification Error " + extraInfo);
				case ErrorCode.IDNA_LABEL_TOO_LONG_ERROR:
					throw new IDNAException("IDNA Error: Label too long " + extraInfo);
				case ErrorCode.IDNA_ZERO_LENGTH_LABEL_ERROR:
					throw new IDNAException("IDNA Error: Zero length label " + extraInfo);
				case ErrorCode.IDNA_DOMAIN_NAME_TOO_LONG_ERROR:
					throw new IDNAException("IDNA Error: Domain name too long " + extraInfo);
				default:
					throw new NotSupportedException("Missing implementation for ErrorCode " + e);
			}
		}
	}
}
