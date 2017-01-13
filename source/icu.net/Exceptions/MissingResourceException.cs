using System;

namespace Icu
{
	/// <summary>
	/// Exception when icu4c cannot find a resource.
	/// </summary>
	public class MissingResourceException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public MissingResourceException(string message) : base(message)
		{ }
	}
}
