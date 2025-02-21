// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	class VersionInfoTests
	{
		[Test]
		public void ToStringMajorMinorMilliMicro()
		{
			var ver = new VersionInfo{Major = 12, Minor = 34, Milli = 56, Micro = 78};
			Assert.That(ver.ToString(), Is.EqualTo("12.34.56.78") );
		}

		[Test]
		public void ToStringMajorMinorMilli()
		{
			var ver = new VersionInfo{ Major = 12, Minor = 34, Milli = 56, Micro = 0 };
			Assert.That(ver.ToString(), Is.EqualTo("12.34.56"));
		}

		[Test]
		public void ToStringMajorMinor()
		{
			var ver = new VersionInfo{ Major = 12, Minor = 34, Milli = 0, Micro = 0 };
			Assert.That(ver.ToString(), Is.EqualTo("12.34"));
		}

		[Test]
		public void ToStringMajorMinorMicro()
		{
			var ver = new VersionInfo{ Major = 12, Minor = 34, Milli = 0, Micro = 78 };
			Assert.That(ver.ToString(), Is.EqualTo("12.34.0.78"));
		}
	}
}
