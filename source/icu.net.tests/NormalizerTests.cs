// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using System.Text;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class NormalizerTests
	{
		[Test]
		public void Normalize_NFC()
		{
			Assert.That(Normalizer.Normalize("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFC),
				Is.EqualTo("X\u00C4bc"));
		}

		[Test]
		public void Normalize_NFD()
		{
			Assert.That(Normalizer.Normalize("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFD),
				Is.EqualTo("XA\u0308bc"));
		}

		[Test]
		public void Normalize_NFC2NFC()
		{
			var normalizedString = Normalizer.Normalize("tést", Normalizer.UNormalizationMode.UNORM_NFC);
			Assert.That(normalizedString, Is.EqualTo("tést"));
			Assert.That(normalizedString.IsNormalized(NormalizationForm.FormC), Is.True);
		}

		[Test]
		public void Normalize_NFC2NFD()
		{
			var normalizedString = Normalizer.Normalize("tést", Normalizer.UNormalizationMode.UNORM_NFD);
			Assert.That(normalizedString[2], Is.EqualTo('\u0301'));
			Assert.That(normalizedString, Is.EqualTo("te\u0301st"));
			Assert.That(normalizedString.IsNormalized(NormalizationForm.FormD), Is.True);
		}

		[Test]
		public void Normalize_NFD2NFC()
		{
			var normalizedString = Normalizer.Normalize("te\u0301st", Normalizer.UNormalizationMode.UNORM_NFC);
			Assert.That(normalizedString, Is.EqualTo("tést"));
			Assert.That(normalizedString.IsNormalized(NormalizationForm.FormC), Is.True);
		}

		[Test]
		public void Normalize_NFD2NFD()
		{
			var normalizedString = Normalizer.Normalize("te\u0301st", Normalizer.UNormalizationMode.UNORM_NFD);
			Assert.That(normalizedString, Is.EqualTo("te\u0301st"));
			Assert.That(normalizedString.IsNormalized(NormalizationForm.FormD), Is.True);
		}

		[Test]
		public void IsNormalized_NFC()
		{
			Assert.That(Normalizer.IsNormalized("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFC),
				Is.True);
			Assert.That(Normalizer.IsNormalized("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFC),
				Is.False);
		}

		[Test]
		public void IsNormalized_NFD()
		{
			Assert.That(Normalizer.IsNormalized("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFD),
				Is.True);
			Assert.That(Normalizer.IsNormalized("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFD),
				Is.False);
		}
	}
}
