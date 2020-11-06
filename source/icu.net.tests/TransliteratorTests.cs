// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using NUnit.Framework;
using System;

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

		[Test]
		public void OpenSingleId()
		{
			Transliterator.CreateInstance(@"Any-Latin");
		}

		[Test]
		public void OpenCompoundId()
		{
			Transliterator.CreateInstance(@"Any-Latin; Latin-ASCII");
		}

		[Test]
		public void CloseSingleId()
		{
			Transliterator trans = Transliterator.CreateInstance("Any-Latin");
			trans.Dispose();
		}

		[Test]
		public void Transliterate()
		{
			Transliterator trans = Transliterator.CreateInstance(@"Any-Latin; Latin-ASCII");
			string source = @"Κοντογιαννάτος, Βασίλης";
			string target = @"Kontogiannatos, Basiles";

			Assert.That(trans.Transliterate(source), Is.EqualTo(target));
		}
	}
}
