// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class IcuWrapperTests
	{
		[Test]
		public void UnicodeVersion()
		{
			string result = Wrapper.UnicodeVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(3));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			int major;
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out major), Is.True);
		}

		[Test]
		public void IcuVersion()
		{
			string result = Wrapper.IcuVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(4));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			int major;
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out major), Is.True);
		}
	}
}
