using System;

namespace Icu
{
	/// <summary>
	/// Exceptions for Transliterator errors.
	/// </summary>
	public class TransliteratorParseException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public TransliteratorParseException(string message) : base(message)
		{ }
	}
}
