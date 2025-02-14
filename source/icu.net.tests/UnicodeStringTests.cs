// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using Icu.Normalization;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class UnicodeStringTests
	{
		[TestCase("ABCI", "en", ExpectedResult = "abci")]
		[TestCase("ABCI", "tr", ExpectedResult = "abc\u0131")] // dotless i
		[TestCase("ABC\u0130", "en", ExpectedResult = "abci\u0307")] // 0130: I with dot above, 0307: Combining dot above
		[TestCase("ABC\u0130", "tr", ExpectedResult = "abci")] // I with dot above
		[TestCase("abci", "en", ExpectedResult = "abci")]
		[TestCase("Abci", "en", ExpectedResult = "abci")]
		[TestCase(";,.?", "en", ExpectedResult = ";,.?")]
		[TestCase("ABc", "en-US", ExpectedResult = "abc")]
		public string ToLower(string src, string locale)
		{
			return UnicodeString.ToLower(src, locale);
		}

		[Test]
		public void ToLower_Locale()
		{
			Assert.That(UnicodeString.ToLower("ABc", new Locale("en-US")), Is.EqualTo("abc"));
		}

		[TestCase("a", "en", ExpectedResult = "A")]
		[TestCase("ABC", "en", ExpectedResult = "ABC")]
		[TestCase("abci", "en", ExpectedResult = "ABCI")]
		[TestCase("abci", "tr", ExpectedResult = "ABC\u0130")] // I with dot above
		[TestCase("abc\u0131", "en", ExpectedResult = "ABCI")] // dotless i
		[TestCase("abc\u0131", "tr", ExpectedResult = "ABCI")] // dotless i
		[TestCase("aBc", "en", ExpectedResult = "ABC")]
		[TestCase(";,.", "en", ExpectedResult = ";,.")]
		[TestCase("ABc", "en-US", ExpectedResult = "ABC")]
		public string ToUpper(string src, string locale)
		{
			return UnicodeString.ToUpper(src, locale);
		}

		[Test]
		public void ToUpper_Locale()
		{
			Assert.That(UnicodeString.ToUpper("ABc", new Locale("en-US")), Is.EqualTo("ABC"));
		}

		[Category("Full ICU")]
		[TestCase("a", "en", ExpectedResult = "A")]
		[TestCase("Abc", "en", ExpectedResult = "Abc")]
		[TestCase("abc", "en", ExpectedResult = "Abc")]
		[TestCase("ABC", "en", ExpectedResult = "Abc")]
		[TestCase(";,.", "en", ExpectedResult = ";,.")]
		[TestCase("ABc", "en-US", ExpectedResult = "Abc")]
		public string ToTitle(string src, string locale)
		{
			return UnicodeString.ToTitle(src, locale);
		}

		[Test]
		[Category("Full ICU")]
		public void ToTitle_Locale()
		{
			Assert.That(UnicodeString.ToTitle("ABc", new Locale("en-US")), Is.EqualTo("Abc"));
		}

		[Test]
		[Category("Full ICU")]
		public void ToTitleWithNormalization_NFC_LetterIWithDotAbove()
		{
			var result = UnicodeString.ToTitle("ibc", "tr", Normalizer.UNormalizationMode.UNORM_NFC);
			Assert.That(result, Is.EqualTo("\u0130bc"));
			Assert.That(Normalizer.IsNormalized(result, Normalizer.UNormalizationMode.UNORM_NFC));
		}

		[Test]
		[Category("Full ICU")]
		public void ToTitleWithNormalization_NFD_LetterIWithDotAbove()
		{
			var result = UnicodeString.ToTitle("\u0131\u0307bc", "tr", Normalizer.UNormalizationMode.UNORM_NFD);
			Assert.That(result, Is.EqualTo("I\u0307bc"));
			Assert.That(Normalizer.IsNormalized(result, Normalizer.UNormalizationMode.UNORM_NFD));
		}
	}
}
