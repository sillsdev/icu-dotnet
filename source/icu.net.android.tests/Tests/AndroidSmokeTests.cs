// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
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
}
