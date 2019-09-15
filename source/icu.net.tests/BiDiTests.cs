// Copyright (c) 2019 Jeff Skaistis
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	[Category("Full ICU")]
	public class BiDiTests
	{
		private const string ENGLISH_LONG = "A wonderful serenity has taken possession of my entire soul, like these sweet mornings of spring which I enjoy with my whole heart.";
		private const string HEBREW_SHORT = "עִבְרִית";
		private const string HEBREW_LONG = "סעיף א. כל בני אדם נולדו בני חורין ושווים בערכם ובזכויותיהם. כולם חוננו בתבונה ובמצפון, לפיכך חובה עליהם לנהוג איש ברעהו ברוח של אחוה.";
		private const string ENGLISH_PART_1 = "Primary direction LTR English, ";
		private const string ENGLISH_PART_2 = " ending in English";
		private const string ENGLISH_HEBREW_MIXED = ENGLISH_PART_1 + HEBREW_SHORT + ENGLISH_PART_2;
		private const string HEBREW_ENGLISH_MIXED = "סעיף א. כל בני אדם נולדו בני חורין ושווים בערכם ובזכויותיהם. כולם Some English in the text עליהם לנהוג איש ברעהו ברוח של אחוה.";
		private const string PARAGRAPHS = ENGLISH_LONG + "\r\n" + HEBREW_ENGLISH_MIXED + "\r\n" + HEBREW_LONG + "\r\n\r\n" + ENGLISH_HEBREW_MIXED;

		// Static method tests

		[Test]
		public void StaticReverseString()
		{
			var reversed = BiDi.ReverseString(HEBREW_SHORT, BiDi.CallReorderingOptions.DEFAULT);
			Assert.AreNotEqual(null, reversed);
			Assert.AreEqual(HEBREW_SHORT.Length, reversed.Length);

			var reverseHebrew = "תיִרְבִע";
			Assert.AreEqual(reverseHebrew, reversed);

			reversed = BiDi.ReverseString(HEBREW_SHORT, BiDi.CallReorderingOptions.KEEP_BASE_COMBINING);
			Assert.AreNotEqual(null, reversed);
			Assert.AreEqual(HEBREW_SHORT.Length, reversed.Length);

			var reverseHebrewCombiningOrder = "תירִבְעִ";
			Assert.AreEqual(reverseHebrewCombiningOrder, reversed);
		}

		[Test]
		public void StaticBaseDirection()
		{
			Assert.AreEqual(BiDi.BiDiDirection.LTR, BiDi.GetBaseDirection(ENGLISH_LONG));
			Assert.AreEqual(BiDi.BiDiDirection.RTL, BiDi.GetBaseDirection(HEBREW_LONG));
			Assert.AreEqual(BiDi.BiDiDirection.LTR, BiDi.GetBaseDirection(ENGLISH_HEBREW_MIXED));
			Assert.AreEqual(BiDi.BiDiDirection.RTL, BiDi.GetBaseDirection(HEBREW_ENGLISH_MIXED));
			Assert.AreEqual(BiDi.BiDiDirection.NEUTRAL, BiDi.GetBaseDirection("102039"));
		}


		// Instance method tests

		[Test]
		public void BasicProperties()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_HEBREW_MIXED, 0, null);

				Assert.AreEqual(bidi.Text, ENGLISH_HEBREW_MIXED);
				Assert.AreEqual(bidi.Length, ENGLISH_HEBREW_MIXED.Length);
				Assert.AreEqual(bidi.ProcessedLength, ENGLISH_HEBREW_MIXED.Length);
				Assert.AreEqual(bidi.ResultLength, ENGLISH_HEBREW_MIXED.Length);
				Assert.AreEqual(bidi.Direction, BiDi.BiDiDirection.MIXED);
			}
		}

		[Test]
		public void Levels()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_LONG, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(BiDi.BiDiDirection.LTR, bidi.Direction);
				Assert.AreEqual(0, bidi.GetParaLevel());
				Assert.AreEqual(0, bidi.GetLevelAt(ENGLISH_LONG.Length / 2));
				var levels = bidi.GetLevels().ToArray();
				Assert.AreEqual(ENGLISH_LONG.Length, levels.Length);
				Assert.That(levels.Where(ll => ll != 0).Count() == 0);
			}

			using (var bidi = new BiDi())
			{
				bidi.SetPara(HEBREW_LONG, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(BiDi.BiDiDirection.RTL, bidi.Direction);
				Assert.AreEqual(1, bidi.GetParaLevel());
				Assert.AreEqual(1, bidi.GetLevelAt(HEBREW_LONG.Length / 2));
				var levels = bidi.GetLevels().ToArray();
				Assert.AreEqual(HEBREW_LONG.Length, levels.Length);
				Assert.That(levels.Where(ll => ll != 1).Count() == 0);
			}

			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_HEBREW_MIXED, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(BiDi.BiDiDirection.MIXED, bidi.Direction);
				Assert.AreEqual(0, bidi.GetParaLevel());
				Assert.AreEqual(0, bidi.GetLevelAt(ENGLISH_HEBREW_MIXED.Length - 1));
				var levels = bidi.GetLevels().ToArray();
				Assert.AreEqual(ENGLISH_HEBREW_MIXED.Length, levels.Length);
				Assert.AreEqual(HEBREW_SHORT.Length, levels.Where(ll => ll != 0).Count());
			}

			using (var bidi = new BiDi())
			{
				bidi.SetPara(HEBREW_ENGLISH_MIXED, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(BiDi.BiDiDirection.MIXED, bidi.Direction);
				Assert.AreEqual(1, bidi.GetParaLevel());
				Assert.AreEqual(1, bidi.GetLevelAt(HEBREW_ENGLISH_MIXED.Length - 1));
				var levels = bidi.GetLevels().ToArray();
				Assert.AreEqual(HEBREW_ENGLISH_MIXED.Length, levels.Length);
				Assert.That(levels.Where(ll => ll == 2).Count() > 0);
			}
		}

		[Test]
		public void Paragraphs()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(PARAGRAPHS, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(BiDi.BiDiDirection.MIXED, bidi.Direction);
				Assert.AreEqual(5, bidi.CountParagraphs());

				var index = bidi.GetParagraph(0, out int paraStart, out int paraLimit, out byte paraLevel);
				Assert.AreEqual(0, index);
				Assert.AreEqual(0, paraStart);
				Assert.AreEqual(ENGLISH_LONG.Length + 2, paraLimit);    // +2 for crlf
				Assert.AreEqual(0, paraLevel);

				index = bidi.GetParagraph(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + 2 + 14, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(2, index);
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + 2, paraStart);  // +2 for crlf
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + 2 + HEBREW_LONG.Length + 2, paraLimit);    // +2 for crlf
				Assert.AreEqual(1, paraLevel);

				index = bidi.GetParagraph(PARAGRAPHS.Length - 2, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(4, index);
				Assert.AreEqual(PARAGRAPHS.Length - ENGLISH_HEBREW_MIXED.Length, paraStart);
				Assert.AreEqual(PARAGRAPHS.Length, paraLimit);
				Assert.AreEqual(0, paraLevel);

				bidi.GetParagraphByIndex(0, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(0, paraStart);
				Assert.AreEqual(ENGLISH_LONG.Length + 2, paraLimit);    // +2 for crlf
				Assert.AreEqual(0, paraLevel);

				bidi.GetParagraphByIndex(2, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + 2, paraStart);  // +2 for crlf
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + 2 + HEBREW_LONG.Length + 2, paraLimit);    // +2 for crlf
				Assert.AreEqual(1, paraLevel);

				bidi.GetParagraphByIndex(3, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + HEBREW_LONG.Length + 2 + 2, paraStart);     // +2 for crlf
				Assert.AreEqual(ENGLISH_LONG.Length + 2 + HEBREW_ENGLISH_MIXED.Length + HEBREW_LONG.Length + 2 + 2 + 2, paraLimit);     // +2 for crlf
				Assert.AreEqual(0, paraLevel);

				bidi.GetParagraphByIndex(4, out paraStart, out paraLimit, out paraLevel);
				Assert.AreEqual(PARAGRAPHS.Length - ENGLISH_HEBREW_MIXED.Length, paraStart);
				Assert.AreEqual(PARAGRAPHS.Length, paraLimit);
				Assert.AreEqual(0, paraLevel);
			}
		}

		[Test]
		public void LogicalRuns()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_HEBREW_MIXED, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(3, bidi.CountRuns());

				var logicalMap = bidi.GetLogicalMap();
				Assert.AreEqual(ENGLISH_HEBREW_MIXED.Length, logicalMap.Length);
				Assert.AreEqual(HEBREW_SHORT.Length, logicalMap.Select((m, i) => (m, i)).Count(mi => mi.Item1 != mi.Item2));

				Assert.That(Enumerable.Range(0, ENGLISH_PART_1.Length).All(i => bidi.GetLogicalIndex(i) == i));
				Assert.That(Enumerable.Range(ENGLISH_PART_1.Length, HEBREW_SHORT.Length).All(i => bidi.GetLogicalIndex(i) == ENGLISH_PART_1.Length + HEBREW_SHORT.Length - i + ENGLISH_PART_1.Length - 1));
				Assert.That(Enumerable.Range(ENGLISH_PART_1.Length + HEBREW_SHORT.Length, ENGLISH_PART_2.Length).All(i => bidi.GetLogicalIndex(i) == i));

				var limit = bidi.GetLogicalRun(3, out byte runlevel);
				Assert.AreEqual(ENGLISH_PART_1.Length, limit);
				Assert.AreEqual(0, runlevel);

				limit = bidi.GetLogicalRun(ENGLISH_PART_1.Length + 3, out runlevel);
				Assert.AreEqual(ENGLISH_PART_1.Length + HEBREW_SHORT.Length, limit);
				Assert.AreEqual(1, runlevel);

				limit = bidi.GetLogicalRun(ENGLISH_PART_1.Length + HEBREW_SHORT.Length + 3, out runlevel);
				Assert.AreEqual(ENGLISH_HEBREW_MIXED.Length, limit);
				Assert.AreEqual(0, runlevel);

				bidi.SetPara(ENGLISH_LONG, BiDi.DEFAULT_LTR, null);
				Assert.AreEqual(1, bidi.CountRuns());

				bidi.SetPara(HEBREW_LONG, BiDi.DEFAULT_LTR, null);
				Assert.AreEqual(1, bidi.CountRuns());
			}
		}

		[Test]
		public void VisualRuns()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_HEBREW_MIXED, BiDi.DEFAULT_LTR, null);

				Assert.AreEqual(3, bidi.CountRuns());

				var visualMap = bidi.GetVisualMap();
				Assert.AreEqual(ENGLISH_HEBREW_MIXED.Length, visualMap.Length);
				Assert.AreEqual(HEBREW_SHORT.Length, visualMap.Select((m, i) => (m, i)).Count(mi => mi.Item1 != mi.Item2));

				Assert.That(Enumerable.Range(0, ENGLISH_PART_1.Length).All(i => bidi.GetVisualIndex(i) == i));
				Assert.That(Enumerable.Range(ENGLISH_PART_1.Length, HEBREW_SHORT.Length).All(i => bidi.GetVisualIndex(i) == ENGLISH_PART_1.Length + HEBREW_SHORT.Length - i + ENGLISH_PART_1.Length - 1));
				Assert.That(Enumerable.Range(ENGLISH_PART_1.Length + HEBREW_SHORT.Length, ENGLISH_PART_2.Length).All(i => bidi.GetVisualIndex(i) == i));

				var direction = bidi.GetVisualRun(0, out int start, out int len);
				Assert.AreEqual(0, start);
				Assert.AreEqual(ENGLISH_PART_1.Length, len);
				Assert.AreEqual(direction, BiDi.BiDiDirection.LTR);

				direction = bidi.GetVisualRun(1, out start, out len);
				Assert.AreEqual(ENGLISH_PART_1.Length, start);
				Assert.AreEqual(HEBREW_SHORT.Length, len);
				Assert.AreEqual(direction, BiDi.BiDiDirection.RTL);

				direction = bidi.GetVisualRun(2, out start, out len);
				Assert.AreEqual(ENGLISH_PART_1.Length + HEBREW_SHORT.Length, start);
				Assert.AreEqual(ENGLISH_PART_2.Length, len);
				Assert.AreEqual(direction, BiDi.BiDiDirection.LTR);
			}
		}

		[Test]
		public void Reordering()
		{
			using (var bidi = new BiDi())
			{
				bidi.SetPara(ENGLISH_HEBREW_MIXED, BiDi.DEFAULT_LTR, null);
				var reordered = bidi.GetReordered(BiDi.CallReorderingOptions.DEFAULT);
				Assert.AreEqual(ENGLISH_PART_1, reordered.Substring(0, ENGLISH_PART_1.Length));
				Assert.AreEqual(string.Join("", HEBREW_SHORT.Reverse()), reordered.Substring(ENGLISH_PART_1.Length, HEBREW_SHORT.Length));
				Assert.AreEqual(ENGLISH_PART_2, reordered.Substring(ENGLISH_PART_1.Length + HEBREW_SHORT.Length));

				reordered = bidi.GetReordered(BiDi.CallReorderingOptions.KEEP_BASE_COMBINING);
				Assert.AreEqual(ENGLISH_PART_1, reordered.Substring(0, ENGLISH_PART_1.Length));
				Assert.AreNotEqual(string.Join("", HEBREW_SHORT.Reverse()), reordered.Substring(ENGLISH_PART_1.Length, HEBREW_SHORT.Length));
				Assert.AreEqual(ENGLISH_PART_2, reordered.Substring(ENGLISH_PART_1.Length + HEBREW_SHORT.Length));

				bidi.SetPara(HEBREW_LONG, BiDi.DEFAULT_RTL, null);
				var reorderedHebrew = bidi.GetReordered(BiDi.CallReorderingOptions.DEFAULT);
				Assert.AreEqual(string.Join("", HEBREW_LONG.Reverse()), reorderedHebrew);

				bidi.IsInverse = true;
				bidi.SetPara(reorderedHebrew, BiDi.DEFAULT_LTR, null);
				Assert.AreEqual(HEBREW_LONG, bidi.GetReordered(BiDi.CallReorderingOptions.DEFAULT));
			}
		}
	}
}
