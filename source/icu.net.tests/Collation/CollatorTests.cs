// Copyright (c) 2013-2025 SIL Global
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
			using (var collator = Collator.Create("en"))
			{
				Assert.That(collator, Is.Not.Null);
			}
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en-US locale.
		public void Create_LocaleWithCultureInfo()
		{
			var cultureInfo = new CultureInfo("en-US");
			using (var collator = Collator.Create(cultureInfo.Name))
			{
				Assert.That(collator, Is.Not.Null);
			}
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en-US locale.
		public void Create_LocaleWithICUCultureId()
		{
			using (var collator = Collator.Create("en_US"))
			{
				Assert.That(collator, Is.Not.Null);
			}
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocale()
		{
			using (var collator = Collator.Create("root"))
			{
				Assert.That(collator, Is.Not.Null);
			}
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocaleAsEmpty()
		{
			using (var collator = Collator.Create(string.Empty))
			{
				Assert.That(collator, Is.Not.Null);
			}
		}

		[Test]
		public void Create_nonexistent_throws()
		{
			Assert.That(() => Collator.Create("non-existent"), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void Create_nonexistentFallbackAllowed_fallsBackToUca()
		{
			using (var collator = Collator.Create("non-existent", Collator.Fallback.FallbackAllowed))
			{
				Assert.That(collator, Is.Not.Null);
			}
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

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_NoPredefinedCollator_Throws()
		{
			Assert.That(() =>
			{
				using (Collator.Create("my-MM")) {}
			}, Throws.ArgumentException);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_NoPredefinedCollator_NoFallback_Throws()
		{
			Assert.That(() =>
				{
					using (Collator.Create("my-MM", Collator.Fallback.NoFallback)) {}
				},
				Throws.ArgumentException);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_NoPredefinedCollator_FallbackAllowed_Works()
		{
			Assert.That(() =>
				{
					using (Collator.Create("my-MM", Collator.Fallback.FallbackAllowed)) {}
				},
				Throws.Nothing);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_PredefinedCollator_Works()
		{
			Assert.That(() =>
				{
					using (Collator.Create("my")) {}
				},
				Throws.Nothing);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_PredefinedCollator_NoFallback_Works()
		{
			Assert.That(() =>
				{
					using (Collator.Create("my", Collator.Fallback.NoFallback)) {}
				},
				Throws.Nothing);
		}
	}

}
