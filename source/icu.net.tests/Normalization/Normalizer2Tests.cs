// Copyright (c) 2018 SIL International
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

		[TestCase("t\u00E9st", Normalizer2.Mode.COMPOSE, ExpectedResult = true)]
		[TestCase("t\u00E9st", Normalizer2.Mode.DECOMPOSE, ExpectedResult = false)]
		[TestCase("te\u0301st", Normalizer2.Mode.COMPOSE, ExpectedResult = false)]
		[TestCase("te\u0301st", Normalizer2.Mode.DECOMPOSE, ExpectedResult = true)]
		public bool IsNormalized(string str, Normalizer2.Mode mode)
		{
			var normalizer = Normalizer2.GetInstance(null, "nfc", mode);
			return normalizer.IsNormalized(str);
		}
	}
}
