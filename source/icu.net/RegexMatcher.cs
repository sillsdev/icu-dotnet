// Copyright (c) 2017 JEPA
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icu
{
	/// <summary>
	/// Regular Expression Matcher
	/// </summary>
	public class RegexMatcher
	{
		/// <summary>
		/// Constants for Regular Expression Match Modes.
		/// </summary>
		[Flags]
		public enum URegexpFlag : uint
		{
			/// <summary>
			/// Enable case insensitive matching.
			/// </summary>
			UREGEX_CASE_INSENSITIVE = 2,
			/// <summary>
			/// Allow white space and comments within patterns
			/// </summary>
			UREGEX_COMMENTS = 4,
			/// <summary>
			/// If set, '.' matches line terminators,  otherwise '.' matching stops at line end.
			/// </summary>
			UREGEX_DOTALL = 32,
			/// <summary>
			///  If set, treat the entire pattern as a literal string.  		
			///  Metacharacters or escape sequences in the input sequence will be given 
			///  no special meaning. 
			/// 
			///  The flag UREGEX_CASE_INSENSITIVE retains its impact
			///  on matching when used in conjunction with this flag.
			///  The other flags become superfluous.
			/// </summary>
			UREGEX_LITERAL = 16,

			/// <summary>
			///  Control behavior of "$" and "^"
			/// If set, recognize line terminators within string,
			///  otherwise, match only at start and end of input string.
			/// </summary>
			UREGEX_MULTILINE = 8,

			/// <summary>
			/// Unix-only line endings.
			/// When this mode is enabled, only \\u000a is recognized as a line ending
			/// in the behavior of ., ^, and $.
			/// </summary>
			UREGEX_UNIX_LINES = 1,

			/// <summary>
			/// Unicode word boundaries.
			/// If set, \b uses the Unicode TR 29 definition of word boundaries.
			/// Warning: Unicode word boundaries are quite different from
			/// traditional regular expression word boundaries.  See
			/// http://unicode.org/reports/tr29/#Word_Boundaries
			/// </summary>
			UREGEX_UWORD = 256,

			/// <summary>
			/// Error on Unrecognized backslash escapes.
			/// If set, fail with an error on patterns that contain
			/// backslash-escaped ASCII letters without a known special
			/// meaning.  If this flag is not set, these
			/// escaped letters represent themselves.
			/// </summary>
			UREGEX_ERROR_ON_UNKNOWN_ESCAPES = 512
		}
		private string _regexp;
		private IntPtr _regexMatcher = IntPtr.Zero;

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="regexp">a unicode regular expression</param>
		/// <param name="flags">an array of flags</param>
		public RegexMatcher(string regexp, uint flags)
		{
			_regexp = regexp;
			ErrorCode e;
			ParseError parseError;

			_regexMatcher = NativeMethods.uregex_open(_regexp, _regexp.Length, flags, out parseError, out e);
			ExceptionFromErrorCode.ThrowIfError(e);
		}

		/// <summary>
		/// Set the subject text string upon which the regular expression will look for matches.
		/// This function may be called any number of times, allowing the regular
		/// expression pattern to be applied to different strings.
		/// </summary>
		/// <param name="str">The subject text string.</param>
		public void SetText(string str)
		{
			ErrorCode e;

			NativeMethods.uregex_setText(_regexMatcher, str, str.Length, out e);
			ExceptionFromErrorCode.ThrowIfError(e);
		}

		/// <summary>
		/// Attempts to match the input string against the pattern.
		/// To succeed, the match must extend to the end of the string,
		/// or cover the complete match region.
		/// 
		/// If startIndex >= zero the match operation starts at the specified
		/// index and must extend to the end of the input string.  Any region
		/// that has been specified is reset.
		/// 
		/// If startIndex = -1 the match must cover the input region, or the entire
		/// input string if no region has been set.This directly corresponds to
		/// Matcher.matches() in Java
		/// </summary>
		/// <param name="startIndex">The input string (native) index at which to begin 
		///              matching, or -1 to match the input Region.</param>
		/// <returns>true if match suceeds; false, otherwise.</returns>
		public bool Matches(int startIndex)
		{
			ErrorCode errorCode;
			bool result;

			result = NativeMethods.uregex_matches(_regexMatcher, startIndex, out errorCode);
			if (errorCode.IsFailure())
			{
				throw new Exception("Match failed");
			}
			return result;
		}

		/// <summary>
		/// Attempts to match the entire subject string against the pattern.
		/// </summary>
		/// <param name="str">The subject string.</param>
		/// <returns>true if match suceeds; false, otherwise.</returns>
		public bool Matches(string str)
		{
			SetText(str);
			return Matches(-1);
		}

		/// <summary>
		/// Dispose of managed/unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Dispose managed state (managed objects), if any.
			}

			if (_regexMatcher != IntPtr.Zero)
			{
				NativeMethods.uregex_close(_regexMatcher);
				_regexMatcher = IntPtr.Zero;
			}
		}

		~RegexMatcher()
		{
			Dispose(false);
		}
	}
}
