// Copyright (c) 2017 JEPA
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Text;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	[Category("Regular Expression")]
	class RegexMatcherTests
	{
		[Test]
		[TestCase("c", ExpectedResult = true)]
		[TestCase("abc", ExpectedResult = true)]
		[TestCase("baabbac", ExpectedResult = true)]
		[TestCase("ba", ExpectedResult = false)]
		[TestCase("baab", ExpectedResult = false)]
		[TestCase("baabcc", ExpectedResult = false)]
		public bool MatchSimpleTest(string text)
		{
			RegexMatcher rm = new RegexMatcher("(a|b)*c");
			return rm.Matches(text);
		}
		[Test]
		[TestCase("c", ExpectedResult = true)]
		[TestCase("abc", ExpectedResult = true)]
		[TestCase("baabbac", ExpectedResult = true)]
		[TestCase(" c", ExpectedResult = false)]
		public bool MatchWithCommentsAndSpace(string text)
		{
			RegexMatcher rm = new RegexMatcher("(?# space is ignored )(a | b)* c", RegexMatcher.URegexpFlag.COMMENTS);
			return rm.Matches(text);
		}
		[Test]
		[TestCase("a", "\\p{L}", ExpectedResult = true)]
		[TestCase("a", "\\p{Lowercase_Letter}", ExpectedResult = true)]
		[TestCase("\u3042", "\\p{Hiragana}", ExpectedResult = true)]
		[TestCase("\u30a2", "\\p{Hiragana}", ExpectedResult = false)]
		[TestCase("\u30a2", "\\p{InKatakana}", ExpectedResult = true)]
		[TestCase("\u3042", "\\p{InKatakana}", ExpectedResult = false)]
		[TestCase("\u6751", "\\p{InCJK_Unified_Ideographs}", ExpectedResult = true)]
		[TestCase("\u30a2", "\\p{Script_Extensions=Katakana}", ExpectedResult = true)]
		[TestCase("\u3042", "\\p{sc=Hira}", ExpectedResult = true)]
		[TestCase("A", "[ABC--DE]", ExpectedResult = true)]
		[TestCase("D", "[ABC--DE]", ExpectedResult = false)]
		public bool MatchAdvancedTest(string text, string regexp)
		{
			RegexMatcher rm = new RegexMatcher(regexp);
			return rm.Matches(text);
		}
}
}
