// Copyright (c) 2016-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace Icu.Tests
{
	/// <summary>
	/// Tests to ensure compatibility with JDK 7
	/// </summary>
	// Ignore tests until a JavaBreakIterator using RuleBasedBreakIterator is written. The rules for JavaBreakIterator can be found at:
	// http://grepcode.com/file/repository.grepcode.com/java/root/jdk/openjdk/7u40-b43/sun/text/resources/BreakIteratorRules.java/
	// and http://grepcode.com/file/repository.grepcode.com/java/root/jdk/openjdk/7u40-b43/sun/text/resources/BreakIteratorRules_th.java#BreakIteratorRules_th
	[TestFixture]
	[Category("Full ICU")]
	[Ignore("Needs implementation of JavaBreakIterator using RuleBasedBreakIterator. See comment in code.")]
	public class BreakIteratorTests_JDKCompatibility
	{
		static readonly String TEXT =
			"Apache Lucene(TM) is a high-performance, full-featured text search engine library written entirely in Java.";

		private BreakIterator GetWordInstance(System.Globalization.CultureInfo locale)
		{
			return BreakIterator.CreateWordInstance(new Locale(locale.Name));
		}

		[Test]
		public void TestWordIteration()
		{
			using var bi = GetWordInstance(System.Globalization.CultureInfo.InvariantCulture);

			// Test empty
			Assert.AreEqual(0, bi.Current);
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());
			Assert.AreEqual(0, bi.Current);

			bi.SetText(TEXT);

			// Ensure position starts at 0 when initialized
			Assert.AreEqual(0, bi.Current);

			// Check first boundary (Apache^)
			Assert.AreEqual(6, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(6, bi.Current);

			// Check second boundary (^Lucene)
			Assert.AreEqual(7, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(7, bi.Current);

			// Check third boundary (Lucene^)
			Assert.AreEqual(13, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(13, bi.Current);

			// Check fourth boundary (^TM)
			Assert.AreEqual(14, bi.MoveNext());

			// Check fifth boundary (TM^)
			Assert.AreEqual(16, bi.MoveNext());

			// Check sixth boundary (TM)^
			Assert.AreEqual(17, bi.MoveNext());

			// Check seventh boundary (^is)
			Assert.AreEqual(18, bi.MoveNext());

			// Move to (^high-performance)
			bi.MoveNext();
			bi.MoveNext();
			bi.MoveNext();

			// Check next boundary (^high-performance)
			Assert.AreEqual(23, bi.MoveNext());

			// Ensure we don't break on hyphen (high-performance^)
			Assert.AreEqual(39, bi.MoveNext());


			// Check MoveLast()
			Assert.AreEqual(107, bi.MoveLast());

			// Check going past last boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());

			// Check we are still at last boundary
			Assert.AreEqual(107, bi.Current);


			// Check MoveFirst()
			Assert.AreEqual(0, bi.MoveFirst());

			// Check going past first boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MovePrevious());

			// Check we are still at first boundary
			Assert.AreEqual(0, bi.Current);
		}

		[Test]
		public void TestWordIterationThai()
		{
			using var bi = GetWordInstance(new System.Globalization.CultureInfo("th"));

			// Test empty
			Assert.AreEqual(0, bi.Current);
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());
			Assert.AreEqual(0, bi.Current);

			bi.SetText("บริษัทMicrosoftบริการดีที่สุด");

			// Ensure position starts at 0 when initialized
			Assert.AreEqual(0, bi.Current);

			// Check first boundary (บริษัท^Microsoft)
			Assert.AreEqual(6, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(6, bi.Current);

			// Check second boundary (Microsoft^บริการ)
			Assert.AreEqual(15, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(15, bi.Current);

			// Check third boundary (บริการ^ดี)
			Assert.AreEqual(21, bi.MoveNext());

			// Ensure Current returns the last boundary iterated to
			Assert.AreEqual(21, bi.Current);

			// Check fourth boundary (ดี^ที่สุด)
			Assert.AreEqual(23, bi.MoveNext());

			// Check fifth boundary (ดีที่สุด^)
			Assert.AreEqual(29, bi.MoveNext());

			// Check beyond last boundary (ดีที่สุด)^
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());

			// Check we are still at last boundary
			Assert.AreEqual(29, bi.Current);

			// Check MovePrevious() (ดี^ที่สุด)
			Assert.AreEqual(23, bi.MovePrevious());


			// Check MoveFirst()
			Assert.AreEqual(0, bi.MoveFirst());

			// Check going past first boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MovePrevious());

			// Check we are still at first boundary
			Assert.AreEqual(0, bi.Current);


			// Check Numerals
			bi.SetText("๑23๔๕๖7");

			// Ensure position starts at 0 when initialized
			Assert.AreEqual(0, bi.Current);

			// Ensure Hindu and Thai numerals stay in one group
			Assert.AreEqual(7, bi.MoveNext());
		}


		static readonly String SENTENCE_TEXT =
			"Apache Lucene(TM) is a high-performance, full-featured text\nsearch engine library written entirely in Java. " +
			"It is a technology suitable for nearly any application that requires" +
			"full-text search, especially cross-platform. Apache Lucene is an open source project available for free download.\n" +
			"Lucene makes finding things easy. Lucene is powerful. Lucene is exciting. Lucene is cool. Where be Lucene now?";

		private BreakIterator GetSentenceInstance(System.Globalization.CultureInfo locale)
		{
			return BreakIterator.CreateSentenceInstance(new Locale(locale.Name));
		}

		[Test]
		public void TestSentenceIteration()
		{
			using var bi = GetSentenceInstance(System.Globalization.CultureInfo.InvariantCulture);

			// Test empty
			Assert.AreEqual(0, bi.Current);
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());
			Assert.AreEqual(0, bi.Current);

			bi.SetText(SENTENCE_TEXT);

			// Ensure position starts at 0 when initialized
			Assert.AreEqual(0, bi.Current);

			// Check first boundary (in Java.^) - Ensure we don't break on \n
			Assert.AreEqual(108, bi.MoveNext());

			// Ensure Current returns the most recent boundary
			Assert.AreEqual(108, bi.Current);

			// Check next boundary (especially cross-platform.^)
			Assert.AreEqual(221, bi.MoveNext());

			// Check next boundary (free download.^)
			Assert.AreEqual(290, bi.MoveNext());

			// Check next boundary (things easy.^)
			Assert.AreEqual(324, bi.MoveNext());

			// Check next boundary (is powerful.^)
			Assert.AreEqual(344, bi.MoveNext());

			// Check next boundary (is exciting.^)
			Assert.AreEqual(364, bi.MoveNext());

			// Check next boundary (is cool.^)
			Assert.AreEqual(380, bi.MoveNext());

			// Check last boundary (Lucene now?^)
			Assert.AreEqual(400, bi.MoveNext());

			// Check move past last boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());

			// Ensure we are still at last boundary
			Assert.AreEqual(400, bi.Current);


			// Check MovePrevious
			Assert.AreEqual(380, bi.MovePrevious());

			// Ensure we get the same value for Current as the last move
			Assert.AreEqual(380, bi.Current);


			// Check MoveFirst
			Assert.AreEqual(0, bi.MoveFirst());

			// Ensure we get the same value for Current as the last move
			Assert.AreEqual(0, bi.Current);

			// Check moving beyond first boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MovePrevious());

			// Ensure we are still at first boundary
			Assert.AreEqual(0, bi.Current);


			// Check MoveLast()
			Assert.AreEqual(400, bi.MoveLast());
		}

		static readonly String LINE_TEXT =
			"Apache\tLucene(TM) is a high-\nperformance, full-featured text search engine library written entirely in Java.";

		private BreakIterator GetLineInstance(System.Globalization.CultureInfo locale)
		{
			return BreakIterator.CreateLineInstance(new Locale(locale.Name));
		}

		[Test]
		public void TestLineIteration()
		{
			using var bi = GetLineInstance(System.Globalization.CultureInfo.InvariantCulture);

			// Test empty
			Assert.AreEqual(0, bi.Current);
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());
			Assert.AreEqual(0, bi.Current);

			bi.SetText(LINE_TEXT);

			// Ensure position starts at 0 when initialized
			Assert.AreEqual(0, bi.Current);

			// Check first boundary (Apache\t^Lucene) - Ensure we break on \t
			Assert.AreEqual(7, bi.MoveNext());

			// Ensure Current returns the most recent boundary
			Assert.AreEqual(7, bi.Current);

			// Check next boundary (Lucene^(TM))
			Assert.AreEqual(13, bi.MoveNext());

			// Ensure Current returns the most recent boundary
			Assert.AreEqual(13, bi.Current);

			// Check next boundary (Lucene(TM) ^is a)
			Assert.AreEqual(18, bi.MoveNext());

			// Ensure Current returns the most recent boundary
			Assert.AreEqual(18, bi.Current);

			// Move to start of high-performance
			bi.MoveNext();
			bi.MoveNext();

			// Check next boundary (high-\n^performance)
			Assert.AreEqual(29, bi.MoveNext());


			// Check last boundary (in Java.^)
			Assert.AreEqual(108, bi.MoveLast());


			// Check move past last boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());

			// Ensure we are still at last boundary
			Assert.AreEqual(108, bi.Current);


			// Check MovePrevious
			Assert.AreEqual(103, bi.MovePrevious());

			// Ensure we get the same value for Current as the last move
			Assert.AreEqual(103, bi.Current);


			// Check MoveFirst
			Assert.AreEqual(0, bi.MoveFirst());

			// Ensure we get the same value for Current as the last move
			Assert.AreEqual(0, bi.Current);


			// Check moving beyond first boundary
			Assert.AreEqual(BreakIterator.DONE, bi.MovePrevious());

			// Ensure we are still at first boundary
			Assert.AreEqual(0, bi.Current);


			// Check MoveLast()
			Assert.AreEqual(108, bi.MoveLast());
		}
	}
}
