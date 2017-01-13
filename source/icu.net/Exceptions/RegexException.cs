using System;

namespace Icu
{
	/// <summary>
	/// Exceptions indicating Regexp failures
	/// </summary>
	public class RegexException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public RegexException(string message) : base(message)
		{ }
	}
}
