// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class IcuDataFileNameTests
	{
		[TestCase("icudt70l.dat", 70)]
		[TestCase("icudt72b.dat", 72)]
		[TestCase("icudt44.dat", 44)]
		[TestCase("ICUDT70L.DAT", 70)]
		[TestCase(@"assets\icudt70l.dat", 70)]
		[TestCase("assets/icudt90l.dat", 90)]
		[TestCase("foo/bar/icudt70l.dat", 70)]
		public void TryParseIcuDataFileName_ParsesMajorVersion(string fileName, int expected)
		{
			Assert.That(NativeMethods.TryParseIcuDataFileName(fileName, out var major), Is.True);
			Assert.That(major, Is.EqualTo(expected));
		}

		[TestCase("")]
		[TestCase("libicuuc.so.70")]
		[TestCase("icudtl.dat")]
		[TestCase("icudt.dat")]
		[TestCase("readme.txt")]
		public void TryParseIcuDataFileName_RejectsInvalidNames(string fileName)
		{
			Assert.That(NativeMethods.TryParseIcuDataFileName(fileName, out _), Is.False);
		}

		[Test]
		public void TryParseIcuDataFileName_RejectsNull()
		{
			Assert.That(NativeMethods.TryParseIcuDataFileName(null, out _), Is.False);
		}
	}
}
