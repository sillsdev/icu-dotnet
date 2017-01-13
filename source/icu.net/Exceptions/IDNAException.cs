using System;

namespace Icu
{
	/// <summary>
	/// Exception thrown when an IDNA ErrorCode is thrown.
	/// </summary>
	public class IDNAException : Exception
	{
		/// <summary>
		/// Create an IDNA Exception with the following message.
		/// </summary>
		/// <param name="message">Message to pass to exception</param>
		public IDNAException(string message) : base(message)
		{ }
	}
}
