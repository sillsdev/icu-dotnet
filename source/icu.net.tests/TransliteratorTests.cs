// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class TransliteratorTests
	{
		Transliterator _transSingle = null, _transCompound = null;

		[Test, Order(1)]
		public void GetIdsAndNames()
		{
			Assert.That(Transliterator.GetIdsAndNames(), Does.Contain(("Arabic-Latin", "Arabic to Latin")));
		}

		[Test, Order(1)]
		public void GetAvailableIds()
		{
			Assert.That(Transliterator.GetAvailableIds(), Does.Contain("Any-Accents"));
		}

		[Test, Order(1)]
		public void GetDisplayName()
		{
			Assert.That(Transliterator.GetDisplayName("Armenian-Latin", "de_DE"),
				Is.EqualTo("Armenian to Latin"));
		}

		private void MaybeOpenSingle(bool force = false)
		{
			if (force || _transSingle == null)
			{
				_transSingle?.Dispose();
				_transSingle = null;
				_transSingle = Transliterator.CreateInstance("Any-Latin");
			}
		}

		[Test, Order(2)]
		public void OpenSingleId()
		{
			Assert.DoesNotThrow(() => MaybeOpenSingle(true));

			Assert.That(
				_transSingle,
				Is.Not.Null);
		}

		private void MaybeOpenCompound(bool force = false)
		{
			if (force || _transCompound == null)
			{
				_transCompound?.Dispose();
				_transCompound = null;
				_transCompound = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			}
		}

		[Test, Order(2)]
		public void OpenCompoundId()
		{
			Assert.DoesNotThrow(() => MaybeOpenCompound(true));

			Assert.That(
				_transCompound,
				Is.Not.Null);
		}

		[Test, Order(3)]
		public void TransliterateSameLength()
		{
			string source = @"Κοντογιαννάτος, Βασίλης";
			string target = @"Kontogiannatos, Basiles";

			MaybeOpenCompound();

			Assert.That(_transCompound.Transliterate(source), Is.EqualTo(target));
		}

		[Test, Order(3)]
		public void TransliterateLonger()
		{
			string source = @"김, 국삼";
			string target = @"gim, gugsam";

			MaybeOpenCompound();

			Assert.That(_transCompound.Transliterate(source), Is.EqualTo(target));
		}

		[TearDown]
		public void TearDown()
		{
			_transSingle?.Dispose();
			_transSingle = null;

			_transCompound?.Dispose();
			_transCompound = null;
		}
	}
}
