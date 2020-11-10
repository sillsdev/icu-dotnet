// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

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

		[Test, Order(0)]
		public void GetIdsAndNames()
		{
			Assert.That(Transliterator.GetIdsAndNames(), Does.Contain(("Arabic-Latin", "Arabic to Latin")));
		}

		[Test, Order(1)]
		public void GetAvailableIds()
		{
			Assert.That(Transliterator.GetAvailableIds(), Does.Contain("Any-Accents"));
		}

		[Test, Order(2)]
		public void GetDisplayName()
		{
			Assert.That(Transliterator.GetDisplayName("Armenian-Latin", "de_DE"),
				Is.EqualTo("Armenian to Latin"));
		}

		[Test, Order(3)]
		public void OpenSingleId()
		{
			Assert.DoesNotThrow(() => _trans = Transliterator.CreateInstance("Any-Latin"));
			Assert.That(_trans, Is.Not.Null);
		}

		[Test, Order(4)]
		public void OpenCompoundId()
		{
			Assert.DoesNotThrow(() => _trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII"));
			Assert.That(_trans, Is.Not.Null);
		}

		private void TestTransliteration(string source, string target)
		{
			Assert.That(_trans?.Transliterate(source), Is.EqualTo(target));
		}

		[Test, Order(5)]
		public void CompoundTransliterateSameLength()
		{
			string source = @"Κοντογιαννάτος, Βασίλης";
			string target = @"Kontogiannatos, Basiles";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			TestTransliteration(source, target);
		}

		[Test, Order(6)]
		public void CompoundTransliterateLonger()
		{
			string source = @"김, 국삼";
			string target = @"gim, gugsam";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			TestTransliteration(source, target);
		}
	}
}
