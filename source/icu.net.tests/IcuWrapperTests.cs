// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class IcuWrapperTests
	{
		[TearDown]
		public void TearDown()
		{
			Wrapper.ConfineIcuVersions(Wrapper.MinSupportedIcuVersion, Wrapper.MaxSupportedIcuVersion);
		}

		[Test]
		public void UnicodeVersion()
		{
			string result = Wrapper.UnicodeVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(3));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out var major), Is.True);
		}

		[Test]
		public void IcuVersion()
		{
			string result = Wrapper.IcuVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(4));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out var major), Is.True);
		}

#if NETCOREAPP2_1
		[Ignore("Platform is not supported in NUnit for .NET Core 2")]
#else
		[Platform(Exclude = "Linux",
			Reason = "These tests require ICU4C installed from NuGet packages which isn't available on Linux")]
#endif
		[Test]
		public void ConfineVersions_WorksAfterInit()
		{
			Wrapper.Init();
			var version = int.Parse(NativeMethodsTests.MinIcuLibraryVersionMajor);
			Wrapper.ConfineIcuVersions(version);

			Assert.That(Wrapper.IcuVersion, Is.EqualTo(NativeMethodsTests.MinIcuLibraryVersion));
		}
	}
}
