// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
	/// <summary>
	/// When turned on, this attribute generates a collation key
	/// for the numeric value of substrings of digits.
	/// This is a way to get '100' to sort AFTER '2'.
	/// </summary>
	public enum NumericCollation
	{
		/// <summary>
		/// Default value, does nothing.
		/// </summary>
		Default = -1,
		/// <summary>
		/// Turns off NumericCollation behaviour.
		/// </summary>
		Off = 16,
		/// <summary>
		/// Generates a collation for the numeric value of substrings of digits.
		/// </summary>
		On = 17
	}
}
