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
	}
}
