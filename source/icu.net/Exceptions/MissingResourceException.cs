// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
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
