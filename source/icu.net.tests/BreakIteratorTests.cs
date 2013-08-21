// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class BreakIteratorTests
	{
		[Test]
		public void Split_Character()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.CHARACTER, "en-US", "abc");

			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "a", "b", "c"}));
		}

		[Test]
		public void Split_Word()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.WORD, "en-US", "Aa Bb. Cc");
			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa", "Bb", "Cc"}));
		}

		[Test]
		public void Split_Line()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.LINE, "en-US", "Aa Bb. Cc");
			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa ", "Bb. ", "Cc"}));
		}

		[Test]
		public void Split_Sentence()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.SENTENCE, "en-US", "Aa bb. Cc 3.5 x? Y?x! Z");
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa bb. ", "Cc 3.5 x? ", "Y?", "x! ","Z"}));
			Assert.That(parts.Count(), Is.EqualTo(5));
		}
	}
}
