// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using Icu.Normalization;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class Normalizer2Tests
	{
		[TestCase('é', ExpectedResult = "e\u0301")]
		[TestCase('a', ExpectedResult = null)]
		[TestCase('~', ExpectedResult = null)]
		public string GetDecomposition(int codePoint)
		{
			var normalizer = Normalizer2.GetNFCInstance();
			return normalizer.GetDecomposition(codePoint);
		}

		[TestCase("XA\u0308bc", "nfc", Normalizer2.Mode.COMPOSE, ExpectedResult = "X\u00C4bc")]
		[TestCase("X\u00C4bc", "nfc", Normalizer2.Mode.DECOMPOSE, ExpectedResult = "XA\u0308bc")]
		[TestCase("tést", "nfc", Normalizer2.Mode.DECOMPOSE, ExpectedResult = "te\u0301st")]
		[TestCase("te\u0301st", "nfc", Normalizer2.Mode.COMPOSE, ExpectedResult = "tést")]
		[TestCase("te\u0301st", "nfc", Normalizer2.Mode.DECOMPOSE, ExpectedResult = "te\u0301st")]
		[TestCase("Te\u0301st", "nfkc", Normalizer2.Mode.COMPOSE, ExpectedResult = "Tést")]
		[TestCase("Te\u0301st", "nfkc_cf", Normalizer2.Mode.COMPOSE, ExpectedResult = "tést")]
		[TestCase("éééééééééé", "nfc", Normalizer2.Mode.DECOMPOSE, ExpectedResult = "e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301")]
		[TestCase("ééééééééééé", "nfc", Normalizer2.Mode.DECOMPOSE, ExpectedResult = "e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301e\u0301")]
		public string Normalize(string src, string name, Normalizer2.Mode mode)
		{
			var normalizer = Normalizer2.GetInstance(null, name, mode);
			return normalizer.Normalize(src);
		}

		[TestCase("t\u00E9st", Normalizer2.Mode.COMPOSE, ExpectedResult = true)]
		[TestCase("t\u00E9st", Normalizer2.Mode.DECOMPOSE, ExpectedResult = false)]
		[TestCase("te\u0301st", Normalizer2.Mode.COMPOSE, ExpectedResult = false)]
		[TestCase("te\u0301st", Normalizer2.Mode.DECOMPOSE, ExpectedResult = true)]
		public bool IsNormalized(string str, Normalizer2.Mode mode)
		{
			var normalizer = Normalizer2.GetInstance(null, "nfc", mode);
			return normalizer.IsNormalized(str);
		}

		[TestCase('a', ExpectedResult = 0)]
		[TestCase(769, ExpectedResult = 0xE6)]
		public int GetCombiningClass(int characterCode)
		{
			var normalizer = Normalizer2.GetInstance(null, "nfc", Normalizer2.Mode.COMPOSE);
			return normalizer.GetCombiningClass(characterCode);
		}
	}
}
