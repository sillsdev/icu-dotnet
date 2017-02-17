// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
	/// <summary>
	/// When turned on, this attribute positions Hiragana before all
	/// non-ignorables on quaternary level This is a sneaky way to produce JIS
	/// sort order
	/// </summary>
	public enum HiraganaQuaternary
	{
		/// <summary>
		/// Default value, does nothing.
		/// </summary>
		Default = -1,
		/// <summary>
		/// Turns off ability to position positions Hiragana before all
		/// non-ignorables on quaternary level.
		/// </summary>
		Off = 16,
		/// <summary>
		/// positions Hiragana before all non-ignorables on quaternary level.
		/// </summary>
		On = 17
	}
}
