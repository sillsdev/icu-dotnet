// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using Icu;
using Icu.Collation;

namespace icu.net.android.tests.Collation;

public class RuleBasedCollatorTests
{
	private const string SerbianRules = "& C < č <<< Č < ć <<< Ć";

	static RuleBasedCollatorTests()
	{
		AndroidIcuSetup.Configure();
	}

	[Theory]
	[InlineData("", null, "a", -1)]
	[InlineData("", "a", null, 1)]
	[InlineData("", null, null, 0)]
	[InlineData("", "ČUKIĆ SLOBODAN", "CUKIĆ SVETOZAR", -1)]
	[InlineData(SerbianRules, "ČUKIĆ SLOBODAN", "CUKIĆ SVETOZAR", 1)]
	public void Compare(string rules, string? string1, string? string2, int expected)
	{
		using var ucaCollator = new RuleBasedCollator(rules);
		Assert.Equal(expected, ucaCollator.Compare(string1, string2));
	}
}
