// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu
{
	/// <summary>
	/// Exceptions indicating that there was a warning returned from icu.
	/// </summary>
	public class WarningException : Exception
	{
		/// <summary>
		/// Creates exception with the provided message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public WarningException(string message) : base(message)
		{ }
	}
}
