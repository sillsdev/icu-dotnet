// Copyright (c) 2017 JEPA
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Text;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	class RegexMatcherTests
	{
		[Test]
		public void Match()
		{
			RegexMatcher rm = new RegexMatcher("(a|b)*c", 0);
			Assert.That(rm.Matches("c"));
			Assert.That(rm.Matches("abc"));
			Assert.That(rm.Matches("baabbac"));
			Assert.IsFalse(rm.Matches("ba"));
			Assert.IsFalse(rm.Matches("bacc"));
			Assert.IsFalse(rm.Matches("baab"));
			Assert.IsFalse(rm.Matches("baabcc"));
		}
		[Test]
		public void MatchWithCommentsAndSpace()
		{
			RegexMatcher rm = new RegexMatcher("(a | b)* c", (uint)RegexMatcher.URegexpFlag.UREGEX_COMMENTS);
			Assert.That(rm.Matches("c"));
			Assert.That(rm.Matches("abc"));
			Assert.That(rm.Matches("baabbac"));
			Assert.IsFalse(rm.Matches(" c"));
		}
	}
}
