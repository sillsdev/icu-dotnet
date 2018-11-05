// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class TransliteratorTests
	{
		[Test]
		public void GetIdsAndNames()
		{
			Assert.That(Transliterator.GetIdsAndNames(), Does.Contain(("Arabic-Latin", "Arabic to Latin")));
		}

		[Test]
		public void GetAvailableIds()
		{
			Assert.That(Transliterator.GetAvailableIds(), Does.Contain("Any-Accents"));
		}

		[Test]
		public void GetDisplayName()
		{
			Assert.That(Transliterator.GetDisplayName("Armenian-Latin", "de_DE"),
				Is.EqualTo("Armenian to Latin"));
		}
	}
}
