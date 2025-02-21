// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
	/// <summary>
	/// Options for retrieving the rule string.
	/// </summary>
	public enum UColRuleOption
	{
		/// <summary>
		/// Retrieves the tailoring rules only.
		/// </summary>
		UCOL_TAILORING_ONLY = 0,
		/// <summary>
		/// Retrieves the "UCA rules" concatenated with the tailoring rules.
		/// The "UCA rules" are an approximation of the root collator's sort
		/// order. They are almost never used or useful at runtime and can be
		/// removed from the data.
		/// See http://userguide.icu-project.org/collation/customization#TOC-Building-on-Existing-Locales
		/// </summary>
		UCOL_FULL_RULES
	}
}
