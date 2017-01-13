using System;

namespace Icu
{
	/// <summary>
	/// Exceptions related to BreakIterator functionality.
	/// </summary>
	public class BreakException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public BreakException(string message) : base(message)
		{ }
	}
}
