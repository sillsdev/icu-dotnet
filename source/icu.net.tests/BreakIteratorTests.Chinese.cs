using System;
using System.Linq;
using NUnit.Framework;

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
				0, 2, 5, 6,		//你是中国人么
				7, 8, 9, 11,	//？ 我喜欢
				13, 14, 16, 17	//的国家。
			};
			var none = (int)BreakIterator.UWordBreak.NONE;
			var ideographic = (int)BreakIterator.UWordBreak.IDEO; //ideographic character
			var ruleStatus = new[] {
				none, ideographic, ideographic, ideographic,
				none, none, ideographic, ideographic,
				ideographic, ideographic, ideographic, none
			};

			using (var bi = BreakIterator.CreateWordInstance(locale, text))
			{
				Assert.AreEqual(text, bi.Text);
				Assert.AreEqual(locale, bi.Locale);
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				// Verify each boundary and rule status.
				for (int i = 0; i < expected.Length; i++)
				{
					int current = bi.Current;
					int status = bi.GetRuleStatus();

					Assert.AreEqual(expected[i], current);
					Assert.AreEqual(ruleStatus[i], status);

					bi.MoveNext();
				}

				// Verify that the BreakIterator is exhausted because we've
				// moved past every item.
				Assert.AreEqual(BreakIterator.DONE, bi.Current);
			}
		}

		[Test]
		public void CreateSentenceInstanceTest()
		{
			var text = "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。";
			var locale = new Locale("zh");
			var expected = new[] { 0, 18, 35, 53 };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(locale, bi.Locale);
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);
			}
		}

		[Test]
		[TestCase(BreakIterator.UBreakIteratorType.SENTENCE, 
			"供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。",
			new[] { 0, 18, 35, 53 }, 
			new[] { 0, 0, 0, 0 })]
		[TestCase(BreakIterator.UBreakIteratorType.WORD,
			"你是中国人么？ 我喜欢你们的国家。",
			new[] { 0, 2, 5, 6,	7, 8, 9, 11, 13, 14, 16, 17	}, 
			new[] { 0, 400, 400, 400, 0, 0, 400, 400, 400, 400, 400, 0 })]
		public void CanIterateForwards(BreakIterator.UBreakIteratorType type, string text, int[] expected, BreakIterator.UWordBreak[] ruleStatus)
		{
			var locale = new Locale("zh");

			BreakIterator bi = default(BreakIterator);

			try
			{
				switch (type)
				{
					case BreakIterator.UBreakIteratorType.SENTENCE:
						bi = BreakIterator.CreateSentenceInstance(locale, text);
						break;
					case BreakIterator.UBreakIteratorType.WORD:
						bi = BreakIterator.CreateWordInstance(locale, text);
						break;
					default:
						throw new NotSupportedException("This iterator type is not supported in this test yet. [" + type + "]");
				}

				CollectionAssert.AreEqual(expected, bi.Boundaries);

				// Verify each boundary for the sentences
				for (int i = 0; i < expected.Length; i++)
				{
					int current = bi.Current;
					int status = bi.GetRuleStatus();

					int expectedStatus = (int)ruleStatus[i];

					Assert.AreEqual(expected[i], current);
					Assert.AreEqual(expectedStatus, status);
					CollectionAssert.AreEqual(new[] { expectedStatus }, bi.GetRuleStatusVector());

					bi.MoveNext();
				}

				// Verify that the BreakIterator is exhausted because we've
				// moved past every item.
				Assert.AreEqual(BreakIterator.DONE, bi.Current);

				// And if we try to move again, it'll return DONE.
				Assert.AreEqual(BreakIterator.DONE, bi.MoveNext());
				Assert.AreEqual(BreakIterator.DONE, bi.Current);

				// Verify that the first element is correct now that we've moved to the end.
				Assert.AreEqual(expected[0], bi.MoveFirst());
				Assert.AreEqual(expected[0], bi.Current);
			}
			finally
			{
				if (bi != default(BreakIterator))
					bi.Dispose();
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
				0, 2, 5, 6,		//你是中国人么
				7, 8, 9, 11,	//？ 我喜欢
				13, 14, 16, 17	//的国家。
			};
			var none = (int)BreakIterator.UWordBreak.NONE;
			var ideographic = (int)BreakIterator.UWordBreak.IDEO; //ideographic character
			var ruleStatus = new[] {
				none, ideographic, ideographic, ideographic,
				none, none, ideographic, ideographic,
				ideographic, ideographic, ideographic, none
			};

			using (var bi = BreakIterator.CreateWordInstance(locale, text))
			{
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				int current = 0;
				var currentBoundary = expected[current];
				var currentStatus = ruleStatus[current];
				Assert.AreEqual(currentBoundary, bi.Current);
				Assert.AreEqual(currentStatus, bi.GetRuleStatus());
				// For these, we only expect one rule to be applied in order to find the text boundary.
				CollectionAssert.AreEqual(new[] { currentStatus }, bi.GetRuleStatusVector());

				// Increment the index and verify that the next Boundary is correct.
				current++;
				currentBoundary = expected[current];
				currentStatus = ruleStatus[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);
				Assert.AreEqual(currentStatus, bi.GetRuleStatus());
				CollectionAssert.AreEqual(new[] { currentStatus }, bi.GetRuleStatusVector());

				current++;
				currentBoundary = expected[current];
				currentStatus = ruleStatus[current];
				Assert.AreEqual(currentBoundary, bi.MoveNext());
				Assert.AreEqual(currentBoundary, bi.Current);
				Assert.AreEqual(currentStatus, bi.GetRuleStatus());
				CollectionAssert.AreEqual(new[] { currentStatus }, bi.GetRuleStatusVector());

				current--;
				currentBoundary = expected[current];
				currentStatus = ruleStatus[current];
				Assert.AreEqual(currentBoundary, bi.MovePrevious());
				Assert.AreEqual(currentBoundary, bi.Current);
				Assert.AreEqual(currentStatus, bi.GetRuleStatus());
				CollectionAssert.AreEqual(new[] { currentStatus }, bi.GetRuleStatusVector());

				current--;
				currentBoundary = expected[current];
				currentStatus = ruleStatus[current];
				Assert.AreEqual(currentBoundary, bi.MovePrevious());
				Assert.AreEqual(currentBoundary, bi.Current);
				Assert.AreEqual(currentStatus, bi.GetRuleStatus());
				CollectionAssert.AreEqual(new[] { currentStatus }, bi.GetRuleStatusVector());

				// We've moved past the first word, it should return null.
				Assert.AreEqual(BreakIterator.DONE, bi.MovePrevious());
				Assert.AreEqual(BreakIterator.DONE, bi.Current);
				Assert.AreEqual(0, bi.GetRuleStatus()); // this by default returns 0.
				CollectionAssert.AreEqual(new[] { 0 }, bi.GetRuleStatusVector()); // default returns 0 in the status vector

				// Verify that the element is correct now that we've moved to the end.
				var last = expected.Last();
				var lastStatus = ruleStatus.Last();

				Assert.AreEqual(last, bi.MoveLast());
				Assert.AreEqual(last, bi.Current);
				Assert.AreEqual(lastStatus, bi.GetRuleStatus());
				CollectionAssert.AreEqual(new[] { lastStatus }, bi.GetRuleStatusVector());
			}
		}

		[Test]
		public void CanSetNewText()
		{
			var locale = new Locale("zh");
			var text = "Good-day, kind sir !  Can I have a glass of water?  I am very parched.";
			var expected = new[] { 0, 22, 52, 70 };

			var secondText = "供重呼車遊踏持図質腰大野明会掲歌? 方図強候準素能物第毎止田作昼野集。霊一起続時筑腺算掲断詳山住死示流投。";
			var secondExpected = new[] { 0, 18, 35, 53 };

			using (var bi = BreakIterator.CreateSentenceInstance(locale, text))
			{
				Assert.AreEqual(text, bi.Text);
				CollectionAssert.AreEqual(expected, bi.Boundaries);

				// Move the iterator to the next boundary
				Assert.AreEqual(expected[1], bi.MoveNext());
				Assert.AreEqual(expected[1], bi.Current);
				Assert.AreEqual((int)BreakIterator.UWordBreak.NONE, bi.GetRuleStatus());

				bi.SetText(secondText);
				Assert.AreEqual(secondText, bi.Text);

				// Assert that the iterator was reset back to the first element
				// when we set new text.
				Assert.AreEqual(secondExpected[0], bi.Current);
				Assert.AreEqual((int)BreakIterator.UWordBreak.NONE, bi.GetRuleStatus());

				CollectionAssert.AreEqual(secondExpected, bi.Boundaries);
			}
		}
	}
}
