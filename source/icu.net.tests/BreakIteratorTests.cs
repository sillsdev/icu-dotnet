// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	[Category("Full ICU")]
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

		[Test]
		public void GetBoundaries_Character()
		{
			var text = "abc? 1";
			var expected = new[] {
				new Boundary(0, 1), new Boundary(1, 2), new Boundary(2, 3), new Boundary(3, 4), new Boundary(4, 5), new Boundary(5, 6)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.CHARACTER, new Locale("en-US"), text);

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetBoundaries_Word()
		{
			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.WORD, new Locale("en-US"), WordBoundaryTestData.Text);

			Assert.That(parts.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedOnlyWords.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedOnlyWords));
		}

		[Test]
		public void GetBoundaries_Line()
		{
			var text = "Aa bb. Ccdef 3.5 x? Y?x! Z";
			var expected = new[] {
				new Boundary(0, 3), new Boundary(3, 7), new Boundary(7, 13), new Boundary(13, 17), new Boundary(17, 20),
				new Boundary(20, 22), new Boundary(22, 25), new Boundary(25, 26)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.LINE, new Locale("en-US"), text);

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetBoundaries_Sentence()
		{
			var text = "Aa bb. Ccdef 3.5 x? Y?x! Z";
			var expected = new[] {
				new Boundary(0, 7), new Boundary(7, 20), new Boundary(20, 22), new Boundary(22, 25), new Boundary(25, 26)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.SENTENCE, new Locale("en-US"), text);
			
			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetWordBoundaries_IgnoreSpacesAndPunctuation()
		{
			var onlyWords = BreakIterator.GetWordBoundaries(new Locale("en-US"), WordBoundaryTestData.Text, false);

			Assert.That(onlyWords.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedOnlyWords.Length));
			Assert.That(onlyWords.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedOnlyWords));
		}

		[Test]
		public void GetWordBoundaries_IncludeSpacesAndPunctuation()
		{
			var allBoundaries = BreakIterator.GetWordBoundaries(new Locale("en-US"), WordBoundaryTestData.Text, true);

			Assert.That(allBoundaries.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedAllBoundaries.Length));
			Assert.That(allBoundaries.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedAllBoundaries));
		}

		/// <summary>
		/// The hypenated text case tests the difference between Word and Line
		/// breaks described in:
		/// http://userguide.icu-project.org/boundaryanalysis#TOC-Line-break-Boundary
		/// </summary>
		[Test]
		public void GetWordAndLineBoundariesWithHyphenatedText()
		{
			var text = "Good-day, kind sir !";
			var expectedWords = new[] {
				new Boundary(0, 4), new Boundary(5, 8), new Boundary(10, 14), new Boundary(15, 18)
			};
			var expectedLines = new[] {
				new Boundary(0, 5), new Boundary(5, 10), new Boundary(10, 15), new Boundary(15, 20)
			};

			var wordBoundaries = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.WORD, new Locale("en-US"), text);
			var lineBoundaries = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.LINE, new Locale("en-US"), text);

			Assert.That(wordBoundaries.Count(), Is.EqualTo(expectedWords.Length));
			Assert.That(wordBoundaries.ToArray(), Is.EquivalentTo(expectedWords));

			Assert.That(lineBoundaries.Count(), Is.EqualTo(expectedLines.Length));
			Assert.That(lineBoundaries.ToArray(), Is.EquivalentTo(expectedLines));
		}

		[Test]
		public void CreateChracterInstanceTest()
		{
			var locale = new Locale("de-DE");
			var text = "Good-bye, dear!";
			var expected = new[] {
				new Boundary(0, 1), new Boundary(1, 2), new Boundary(2, 3), new Boundary(3, 4),
				new Boundary(4, 5), new Boundary(5, 6), new Boundary(6, 7), new Boundary(7, 8),
				new Boundary(8, 9), new Boundary(9, 10), new Boundary(10, 11), new Boundary(11, 12),
				new Boundary(12, 13), new Boundary(13, 14), new Boundary(14, 15)
			};

			using (var bi = BreakIterator.CreateCharacterInstance(locale, text))
			{
				Assert.AreEqual(locale, bi.Locale);
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		/// <summary>
		/// Checking that when a break iterator is created with null or ""
		/// that it returns the correct properties.
		/// </summary>
		[Test]
		public void BreakIteratorThatIsEmptyOrNull()
		{
			var locale = new Locale("de-DE");

			Action<BreakIterator, string> Verify = (BreakIterator iterator, string t) => 
			{
				Assert.AreEqual(locale, iterator.Locale);
				Assert.AreEqual(t, iterator.Text);
				Assert.AreEqual(0, iterator.Boundaries.Length);

				Assert.Null(iterator.Current);
				Assert.Null(iterator.MoveNext());
				Assert.Null(iterator.MoveFirst());
				Assert.Null(iterator.MoveLast());
				Assert.Null(iterator.MovePrevious());
			};

			using (var bi = BreakIterator.CreateCharacterInstance(locale, string.Empty))
			{
				Verify(bi, string.Empty);
			}

			using (var bi = BreakIterator.CreateWordInstance(locale, null, true))
			{
				Verify(bi, null);
			}
		}

		[Test]
		public void CanIterateForwards()
		{
			var locale = new Locale("de-DE");
			var text = "Good-day, kind sir !";
			var expected = new[] {
				new Boundary(0, 4), new Boundary(5, 8), new Boundary(10, 14), new Boundary(15, 18)
			};

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: false))
			{
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				int current = 0;
				var currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.Current);
				
				// increment the index and verify that the next Boundary is correct.
				current++;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);

				current++;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);

				current++;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);

				// We've moved past the last word, it should return null.
				Assert.Null(bi.MoveNext());
				Assert.Null(bi.Current);

				// Verify that the first element is correct now that we've moved to the end.
				Assert.AreEqual(expected[0], bi.MoveFirst());
				Assert.AreEqual(expected[0], bi.Current);
			}
		}

		[Test]
		public void CanIterateBackwards()
		{
			var locale = new Locale("de-DE");
			var text = "Good-day, kind sir !";
			var expected = new[] {
				new Boundary(0, 5), new Boundary(5, 10), new Boundary(10, 15), new Boundary(15, 20)
			};

			using (var bi = BreakIterator.CreateLineInstance(locale, text))
			{
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				int current = 0;
				var currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.Current);

				// Increment the index and verify that the next Boundary is correct.
				current++;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);

				current++;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);

				current--;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MovePrevious());
				Assert.AreEqual(currentBoundary, bi.Current);

				current--;
				currentBoundary = expected[current];
				Assert.AreEqual(currentBoundary, bi.MovePrevious());
				Assert.AreEqual(currentBoundary, bi.Current);

				// We've moved past the first word, it should return null.
				Assert.Null(bi.MovePrevious());
				Assert.Null(bi.Current);

				// Verify that the element is correct now that we've moved to the end.
				Assert.AreEqual(expected[3], bi.MoveLast());
				Assert.AreEqual(expected[3], bi.Current);
			}
		}

		[Test]
		public void CanSetNewText()
		{
			var locale = new Locale("en-US");
			var text = "Good-day, kind sir !  Can I have a glass of water?  I am very parched.";
			var expected = new[] { new Boundary(0, 22), new Boundary(22, 52), new Boundary(52, 70) };

			var secondText = "It is my birthday!  I hope something exciting happens.";
			var secondExpected = new[] { new Boundary(0, 20), new Boundary(20, 54) };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				// Move the iterator to the next boundary
				Assert.AreEqual(expected[1], bi.MoveNext());
				Assert.AreEqual(expected[1], bi.Current);

				// Assert that the new set of boundaries were found.
				bi.SetText(secondText);
				Assert.AreEqual(secondText, bi.Text);

				// Assert that the iterator was reset back to the first element
				// when we set new text.
				Assert.AreEqual(secondExpected[0], bi.Current);

				CollectionAssert.AreEqual(secondExpected, bi.Boundaries);
			}
		}

		/// <summary>
		/// Assert that when we set the text to empty that it will reset all the values.
		/// </summary>
		[Test]
		[TestCase("")]
		[TestCase(null)]
		public void CanSetNewText_EmptyOrNull(string secondText)
		{
			var locale = new Locale("en-US");
			var text = "Good-day, kind sir !  Can I have a glass of water?  I am very parched.";
			var expected = new[] { new Boundary(0, 22), new Boundary(22, 52), new Boundary(52, 70) };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				// Move the iterator to the next boundary
				Assert.AreEqual(expected[1], bi.MoveNext());
				Assert.AreEqual(expected[1], bi.Current);

				// Assert that the new set of boundaries were found.
				bi.SetText(secondText);
				Assert.AreEqual(secondText, bi.Text);

				// Assert that the iterator was reset back to the first element
				// and is now null.
				Assert.Null(bi.Current);
				Assert.Null(bi.MoveNext());
				Assert.Null(bi.MoveFirst());
				Assert.Null(bi.MoveLast());
				Assert.Null(bi.MovePrevious());

				CollectionAssert.IsEmpty(bi.Boundaries);
			}
		}

		[Test]
		public void CreateSentenceInstanceTest()
		{
			var locale = new Locale("de-DE");
			var text = "Good-bye, dear! That was a delicious dinner.";
			var expected = new[] { new Boundary(0, 16), new Boundary(16, 44) };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(locale, bi.Locale);
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		public void CreateWordInstanceTest()
		{
			var locale = new Locale("de-DE");
			var text = "Good-day, kind sir !";
			var expectedWords = new[] {
				new Boundary(0, 4), new Boundary(5, 8), new Boundary(10, 14), new Boundary(15, 18)
			};

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: false))
			{
				Assert.AreEqual(locale, bi.Locale);
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expectedWords, bi.Boundaries);
			}
		}

		[Test]
		public void CreateWordInstanceTest_IncludeSpacesAndPunctuation()
		{
			var locale = new Locale("de-DE");
			var text = "Good-day, kind sir !";
			var expected = new[] {
				new Boundary(0, 4), new Boundary(4, 5),
				new Boundary(5, 8), new Boundary(8, 9), new Boundary(9, 10),
				new Boundary(10, 14), new Boundary(14, 15), new Boundary(15, 18),
				new Boundary(18, 19), new Boundary(19, 20)
			};

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: true))
			{
				Assert.AreEqual(locale, bi.Locale);
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		public void CreateLineInstanceTest()
		{
			var locale = new Locale("de-DE");
			var text = "Good-day, kind sir !";
			var expected = new[] {
				new Boundary(0, 5), new Boundary(5, 10), new Boundary(10, 15), new Boundary(15, 20)
			};

			using (var bi = BreakIterator.CreateLineInstance(locale, text))
			{
				Assert.AreEqual(locale, bi.Locale);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		/// <summary>
		/// Test data for GetBoundaries_Word and GetWordBoundaries  tests
		/// </summary>
		internal static class WordBoundaryTestData
		{
			public const string Text = "Aa bb. Ccdef 3.5 x? Y?x! Z";

			public static readonly Boundary[] ExpectedOnlyWords = new[] {
				new Boundary(0, 2), new Boundary(3, 5), new Boundary(7, 12), new Boundary(13, 16),
				new Boundary(17, 18), new Boundary(20, 21), new Boundary(22, 23), new Boundary(25, 26)
			};

			public static readonly Boundary[] ExpectedAllBoundaries = new[] {
				new Boundary(0, 2), new Boundary(2, 3), new Boundary(3, 5), new Boundary(5, 6),
				new Boundary(6, 7), new Boundary(7, 12), new Boundary(12, 13), new Boundary(13, 16),
				new Boundary(16, 17), new Boundary(17, 18), new Boundary(18, 19), new Boundary(19, 20),
				new Boundary(20, 21), new Boundary(21, 22), new Boundary(22, 23), new Boundary(23, 24),
				new Boundary(24, 25), new Boundary(25, 26)
			};
		}
	}
}
