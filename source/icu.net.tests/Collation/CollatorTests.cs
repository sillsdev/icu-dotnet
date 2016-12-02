// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;
using Icu.Collation;
using NUnit.Framework;

namespace Icu.Tests.Collation
{
	[TestFixture]
	public class CollatorTests
	{
		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en locale.
		public void Create_Locale()
		{
			Assert.That(Collator.Create("en"), Is.Not.Null);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en-US locale.
		public void Create_LocaleWithCultureInfo()
		{
			var cultureInfo = new CultureInfo("en-US");
			Assert.That(Collator.Create(cultureInfo.Name), Is.Not.Null);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en-US locale.
		public void Create_LocaleWithICUCultureId()
		{
			Assert.That(Collator.Create("en_US"), Is.Not.Null);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocale()
		{
			Assert.That(Collator.Create("root"), Is.Not.Null);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocaleAsEmpty()
		{
			Assert.That(Collator.Create(string.Empty), Is.Not.Null);
		}

		[Test]
		public void Create_nonexistent_throws()
		{
			Assert.That(() => Collator.Create("non-existent"), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void Create_nonexistentFallbackAllowed_fallsbackToUca()
		{
			Assert.That(Collator.Create("non-existent", Collator.Fallback.FallbackAllowed), Is.Not.Null);
		}

		[Test]
		public void Create_NullString()
		{
			Assert.That(() => Collator.Create((string)null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void Create_NullCultureInfo()
		{
			Assert.That(() => Collator.Create((CultureInfo)null), Throws.TypeOf<ArgumentNullException>());
		}

	}

}
