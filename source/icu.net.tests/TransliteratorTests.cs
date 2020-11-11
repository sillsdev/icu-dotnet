// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class TransliteratorTests
	{
		Transliterator _trans = null;

		[TearDown]
		public void TearDown()
		{
			_trans?.Dispose();
			_trans = null;
		}

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

		[TestCase("Any-Latin", TestName = "OpenSingleId")]
		[TestCase("Any-Latin; Latin-ASCII", TestName = "OpenCompoundId")]
		public void CreateInstance(string id)
		{
			Assert.That(() => _trans = Transliterator.CreateInstance(id), Throws.Nothing);
			Assert.That(_trans, Is.Not.Null);
		}

		[Test]
		public void Transliterate_CompoundTransliterateSameLength()
		{
			const string source = @"Κοντογιαννάτος, Βασίλης";
			const string target = @"Kontogiannatos, Basiles";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(_trans.Transliterate(source), Is.EqualTo(target));
		}

		[Test]
		public void Transliterate_CompoundTransliterateLonger()
		{
			const string source = @"김, 국삼";
			const string target = @"gim, gugsam";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(_trans.Transliterate(source), Is.EqualTo(target));
		}

		[TestCase(-1)]
		[TestCase(0)]
		public void Transliterate_InvalidMultiplier(int multiplier)
		{
			const string source = @"김, 국삼";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(() => _trans.Transliterate(source, multiplier), Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void Transliterate_Overflow()
		{
			const string source = @"김, 국삼";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(() => _trans.Transliterate(source, 1), Throws.InstanceOf<OverflowException>());
		}
	}
}
