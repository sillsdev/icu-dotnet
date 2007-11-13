#define MINIMAL_ICU
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
#if MINIMAL_ICU
		[Ignore("This fails when ICU's data DLL is built without rules for the en locale.")]
#endif
		public void Create_Locale()
		{
			Assert.IsNotNull(Collator.Create("en"));
		}

		[Test]
#if MINIMAL_ICU
		[Ignore("This fails when ICU's data DLL is built without rules for the root locale.")]
#endif
		public void Create_RootLocale()
		{
			Assert.IsNotNull(Collator.Create("root"));
		}

		[Test]
#if MINIMAL_ICU
		[Ignore("This fails when ICU's data DLL is built without rules for the root locale.")]
#endif
		public void Create_RootLocaleAsEmpty()
		{
			Assert.IsNotNull(Collator.Create(string.Empty));
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
			Assert.IsNotNull(Collator.Create("non-existent", Collator.Fallback.FallbackAllowed));
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