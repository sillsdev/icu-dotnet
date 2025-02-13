// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu
{
	/// <summary>
	/// Exception when RuleBasedBreakIterator is unable to parse the rules.
	/// </summary>
	public class ParseErrorException : Exception
	{
		private readonly ParseError _error;
		private readonly string _rules;

		/// <summary>
		/// Creates a exception with the given message, error and rules used.
		/// </summary>
		/// <param name="message">Message to add with exception.</param>
		/// <param name="error">ParseError containing detailed error information.</param>
		/// <param name="rules">Rules resulting in parse error.</param>
		public ParseErrorException(string message, ParseError error, string rules)
			: base(message)
		{
			if (rules == null)
			{
				throw new ArgumentNullException("rules");
			}

			_error = error;
			_rules = rules;
		}

		/// <summary>
		/// Line that the error occurred at.
		/// </summary>
		public int Line { get { return _error.Line; } }
		/// <summary>
		/// Gets the offset in the line that the syntax error is at.
		/// </summary>
		public int Offset { get { return _error.Offset; } }
		/// <summary>
		/// Textual context before the error or empty if there is none.
		/// </summary>
		public string PreContext { get { return _error.PreContext; } }
		/// <summary>
		/// The error itself or textual information after the error. Empty string
		/// if there is none.
		/// </summary>
		public string PostContext { get { return _error.PostContext; } }
		/// <summary>
		/// Gets the rule that resulted in the error.
		/// </summary>
		public string Rules { get { return _rules; } }

		/// <summary>
		/// Gets a string representation of the exception message with rule and
		/// its line and offset information.
		/// </summary>
		public override string ToString()
		{
			return Message + Environment.NewLine + _error.ToString(_rules);
		}
	}
}
