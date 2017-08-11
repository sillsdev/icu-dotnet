// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Text;
using Icu.Collation;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class NormalizerTests
	{
		[TestCase("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFC, Result = "X\u00C4bc")]
		[TestCase("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFD, Result = "XA\u0308bc")]
		[TestCase("tést", Normalizer.UNormalizationMode.UNORM_NFD, Result = "te\u0301st")]
		[TestCase("te\u0301st", Normalizer.UNormalizationMode.UNORM_NFC, Result = "tést")]
		[TestCase("te\u0301st", Normalizer.UNormalizationMode.UNORM_NFD, Result = "te\u0301st")]
		public string Normalize(string src, Normalizer.UNormalizationMode mode)
		{
			return Normalizer.Normalize(src, mode);
		}

		[TestCase("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFC, Result = true)]
		[TestCase("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFC, Result = false)]
		[TestCase("X\u00C4bc", Normalizer.UNormalizationMode.UNORM_NFD, Result = false)]
		[TestCase("XA\u0308bc", Normalizer.UNormalizationMode.UNORM_NFD, Result = true)]
		public bool IsNormalized(string src, Normalizer.UNormalizationMode expectNormalizationMode)
		{
			return Normalizer.IsNormalized(src, expectNormalizationMode);
		}

		// https://github.com/sillsdev/icu-dotnet/issues/47
		[TestCase("⒆⑵Ⓖ⒭⓫⒄⒱ⓞ")]
		[TestCase("㎞㌻㌵㍑㍑")]
		[TestCase("㌎㍊㌵㌇㌿")]
		public void Normalize_NoOverflowError(string input)
		{
			Assert.That(() => Normalizer.Normalize(input, Normalizer.UNormalizationMode.UNORM_NFKC),
				Throws.Nothing);
		}
	}
}
