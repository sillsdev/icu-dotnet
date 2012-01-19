using System;
using System.Globalization;
using Icu.Collation;
using NUnit.Framework;

namespace icu.net.tests.Collation
{
	[TestFixture]
	public class CollatorTests
	{
		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the en locale.
		public void Create_Locale()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("en");
			Assert.IsNotNull(coll);
			Assert.AreEqual("en", coll.ValidId);
		}

		[Test]
		[Category("Full ICU")]

		//This fails when ICU's data DLL is built without rules for the en_GB locale.

		public void Create_Locale_en_GB()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("en_GB", Collator.Fallback.FallbackAllowed);
			Assert.IsNotNull(coll);
			Assert.AreEqual("en_GB", coll.ValidId);
		}

		[Test]
		[Category("Full ICU")]

		//This fails when ICU's data DLL is built without rules for the fr locale.

		public void Create_Locale_fr()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("fr");
			Assert.IsNotNull(coll);
			Assert.AreEqual("fr", coll.ValidId);
		}

		[Test]
		[Category("Full ICU")]

		//This fails when ICU's data DLL is built without rules for the es_ES locale.

		public void Create_Locale_es_ES()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("es_ES", Collator.Fallback.FallbackAllowed);
			Assert.IsNotNull(coll);
			Assert.AreEqual("es_ES", coll.ValidId);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocale()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("root");
			Assert.IsNotNull(coll);
			Assert.AreEqual("root", coll.ValidId);
		}

		[Test]
		[Category("Full ICU")]
		//This fails when ICU's data DLL is built without rules for the root locale.
		public void Create_RootLocaleAsEmpty()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create(string.Empty);
			Assert.IsNotNull(coll);
			Assert.AreEqual("root", coll.ValidId);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_nonexistent_throws()
		{
			Assert.IsNotNull(Collator.Create("non-existent"));
		}

		[Test]
		public void Create_nonexistentFallbackAllowed_fallsbackToUca()
		{
			RuleBasedCollator coll = (RuleBasedCollator) Collator.Create("non-existent", Collator.Fallback.FallbackAllowed);
			Assert.IsNotNull(coll);
			Assert.AreEqual(CultureInfo.CurrentCulture.Name.Replace('-','_'), coll.ValidId);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullString()
		{
			Collator.Create((string)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullCultureInfo()
		{
			Collator.Create((CultureInfo)null);
		}

	}

}
