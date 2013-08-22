// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class UnicodeStringTests
	{
		[Test]
		public void ToLower()
		{
			Assert.That(UnicodeString.ToLower("ABc", "en-US"), Is.EqualTo("abc"));
			Assert.That(UnicodeString.ToLower("ABc", new Locale("en-US")), Is.EqualTo("abc"));
			Assert.That(UnicodeString.ToLower("ABC", "en"), Is.EqualTo("abc"));
			Assert.That(UnicodeString.ToLower("abc", "en"), Is.EqualTo("abc"));
			Assert.That(UnicodeString.ToLower("Abc", "en"), Is.EqualTo("abc"));
			Assert.That(UnicodeString.ToLower(";,.", "en"), Is.EqualTo(";,."));
		}

		[Test]
		public void ToUpper()
		{
			Assert.That(UnicodeString.ToUpper("ABc", "en-US"), Is.EqualTo("ABC"));
			Assert.That(UnicodeString.ToUpper("ABc", new Locale("en-US")), Is.EqualTo("ABC"));
			Assert.That(UnicodeString.ToUpper("ABC", "en"), Is.EqualTo("ABC"));
			Assert.That(UnicodeString.ToUpper("abc", "en"), Is.EqualTo("ABC"));
			Assert.That(UnicodeString.ToUpper("aBc", "en"), Is.EqualTo("ABC"));
			Assert.That(UnicodeString.ToUpper("a", "en"), Is.EqualTo("A"));
			Assert.That(UnicodeString.ToUpper(";,.", "en"), Is.EqualTo(";,."));
		}

		[Test]
		[Category("Full ICU")]
		public void ToTitle()
		{
			Assert.That(UnicodeString.ToTitle("ABc", "en-US"), Is.EqualTo("Abc"));
			Assert.That(UnicodeString.ToTitle("ABc", new Locale("en-US")), Is.EqualTo("Abc"));
			Assert.That(UnicodeString.ToTitle("a", "en"), Is.EqualTo("A"));
			Assert.That(UnicodeString.ToTitle("Abc", "en"), Is.EqualTo("Abc"));
			Assert.That(UnicodeString.ToTitle("abc", "en"), Is.EqualTo("Abc"));
			Assert.That(UnicodeString.ToTitle("ABC", "en"), Is.EqualTo("Abc"));
			Assert.That(UnicodeString.ToTitle(";,.", "en"), Is.EqualTo(";,."));
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
