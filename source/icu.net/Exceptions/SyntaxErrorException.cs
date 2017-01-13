using System;

namespace Icu
{
	/// <summary>
	/// Exception for syntax errors in a format pattern (ie. Number, exponent patterns.)
	/// </summary>
	public class SyntaxErrorException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public SyntaxErrorException(string message) : base(message)
		{ }
	}
}
