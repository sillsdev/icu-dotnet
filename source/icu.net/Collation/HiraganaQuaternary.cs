// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu.Collation
{
	/// <summary>
	/// When turned on, this attribute positions Hiragana before all
	/// non-ignorables on quaternary level This is a sneaky way to produce JIS
	/// sort order
	/// </summary>
	[Obsolete("ICU 50 Implementation detail, cannot be set via API, was removed from implementation.")]
	public enum HiraganaQuaternary
	{
		/// <summary>
		/// Default value, does nothing.
		/// </summary>
		Default = -1,
		/// <summary>
		/// Turns off ability to position Hiragana before all non-ignorables on
		/// quaternary level.
		/// </summary>
		Off = 16,
		/// <summary>
		/// positions Hiragana before all non-ignorables on quaternary level.
		/// </summary>
		On = 17
	}
}
