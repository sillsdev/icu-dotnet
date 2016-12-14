// Copyright (c) 2013-2017 SIL International
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

		public int Line { get { return _error.Line; } }
		public int Offset { get { return _error.Offset; } }
		public string PreContext { get { return _error.PreContext; } }
		public string PostContext { get { return _error.PostContext; } }
		public string Rules { get { return _rules; } }

		public override string ToString()
		{
			return Message + Environment.NewLine + _error.ToString(_rules);
		}
	}
}
