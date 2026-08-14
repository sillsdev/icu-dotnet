// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using Icu;
using Microsoft.Maui.Devices;
using NUnit.Framework;

namespace icu.net.android.tests.Tests;

[TestFixture]
public class AndroidSmokeTests
{
	[Test]
	public void IsRunningOnAndroid()
	{
		Assert.That(OperatingSystem.IsAndroid(), Is.True);
	}

	[Test]
	public void DeviceInfo_IsAndroid()
	{
		Assert.That(DeviceInfo.Platform, Is.EqualTo(DevicePlatform.Android));
	}

	[Test]
	public void Init_DiscoversBundledIcuDataVersion()
	{
		var result = Wrapper.Init();
		Assert.That(result, Is.EqualTo(ErrorCode.ZERO_ERROR));

		var major = int.Parse(Wrapper.IcuVersion.Split('.')[0]);
		Assert.That(major, Is.GreaterThanOrEqualTo(Wrapper.MinSupportedIcuVersion));
		Assert.That(major, Is.LessThanOrEqualTo(Wrapper.MaxSupportedIcuVersion));

		using var stream = Android.App.Application.Context.Assets!.Open($"icudt{major}l.dat");
		Assert.That(stream, Is.Not.Null);
	}
}
