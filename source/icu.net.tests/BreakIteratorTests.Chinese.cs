using NUnit.Framework;
using System.Linq;

namespace Icu.Tests
{
	[TestFixture]
	[Category("Full ICU")]
	class BreakIteratorTests_Chinese
	{
		[Test]
		public void Split_Character()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.CHARACTER, "zh-HK", "今晚、我會睡著。狗");
			var expected = new[] { "今", "晚", "、", "我", "會", "睡", "著", "。", "狗" };

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void Split_Word()
		{
			if (string.CompareOrdinal(Wrapper.IcuVersion, "52.1") < 0)
				Assert.Ignore("This test requires ICU 52 or higher");

			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.WORD, "zh-HK", "今晚、我會睡著。一隻狗");
			var expected = new[] { "今晚", "我會", "睡著", "一隻", "狗" };

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void Split_Line()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.LINE, "zh-HK", "今晚、我會睡著。");
			var expected = new[] { "今", "晚、", "我", "會", "睡", "著。" };

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void Split_Sentence()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.SENTENCE, "zh-HK", "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。");
			var expected = new[] {
				"供重呼車遊踏持図質腰大野明会掲歌? ",
				"方図強候準素能物第毎止田作昼野集。",
				"霊一起続時筑腺算掲断詳山住死示流投。"};

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void CreateWordInstanceTest()
		{
			if (string.CompareOrdinal(Wrapper.IcuVersion, "52.1") < 0)
				Assert.Ignore("This test requires ICU 52 or higher");

			var text = "你是中国人么？ 我喜欢你们的国家。";
			var locale = new Locale("zh");
			var expected = new[] {
				new Boundary(0, 2), new Boundary(2, 5), new Boundary(5, 6), // 你是中国人么
				new Boundary(6,7), new Boundary(7,8), new Boundary(8, 9), //？ 我
				new Boundary(9, 11), new Boundary(11, 13), new Boundary(13, 14), // 喜欢你们的
                new Boundary(14, 16), new Boundary(16, 17) // 国家。
            };

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: true))
			{
				Assert.AreEqual(text, bi.Text);
				Assert.AreEqual(locale, bi.Locale);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		public void CreateWordInstanceTest_IgnoreSpacesAndPunctuation()
		{
			if (string.CompareOrdinal(Wrapper.IcuVersion, "52.1") < 0)
				Assert.Ignore("This test requires ICU 52 or higher");

			var text = "你是中国人么？ 我喜欢你们的国家。";
			var locale = new Locale("zh");
			var expected = new[] {
				new Boundary(0, 2), new Boundary(2, 5), new Boundary(5, 6), // 你是中国人么
                new Boundary(8, 9), new Boundary(9, 11), new Boundary(11, 13), // 我喜欢你们
                new Boundary(13, 14), new Boundary(14, 16) // 的国家
            };

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: false))
			{
				Assert.AreEqual(text, bi.Text);
				Assert.AreEqual(locale, bi.Locale);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		public void CreateSentenceInstanceTest()
		{
			var text = "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。";
			var locale = new Locale("zh");
			var expected = new[] { new Boundary(0, 18), new Boundary(18, 35), new Boundary(35, 53) };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(locale, bi.Locale);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		public void CanIterateForwards()
		{
			var text = "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。";
			var locale = new Locale("zh");
			var expected = new[] { new Boundary(0, 18), new Boundary(18, 35), new Boundary(35, 53) };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
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
			if (string.CompareOrdinal(Wrapper.IcuVersion, "52.1") < 0)
				Assert.Ignore("This test requires ICU 52 or higher");

			var text = "你是中国人么？ 我喜欢你们的国家。";
			var locale = new Locale("zh");
			var expected = new[] {
				new Boundary(0, 2), new Boundary(2, 5), new Boundary(5, 6), // 你是中国人么
                new Boundary(8, 9), new Boundary(9, 11), new Boundary(11, 13), // 我喜欢你们
                new Boundary(13, 14), new Boundary(14, 16) // 的国家
            };

			using (var bi = BreakIterator.CreateWordInstance(locale, text, includeSpacesAndPunctuation: false))
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
				var last = expected.Last();
				Assert.AreEqual(last, bi.MoveLast());
				Assert.AreEqual(last, bi.Current);
			}
		}

		[Test]
		public void CanSetNewText()
		{
			var locale = new Locale("zh");
			var text = "Good-day, kind sir !  Can I have a glass of water?  I am very parched.";
			var expected = new[] { new Boundary(0, 22), new Boundary(22, 52), new Boundary(52, 70) };

			var secondText = "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。";
			var secondExpected = new[] { new Boundary(0, 18), new Boundary(18, 35), new Boundary(35, 53) };

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

	}
}
