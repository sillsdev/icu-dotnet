using Microsoft.Maui.Devices;

namespace icu.net.android.tests.Tests;

public class AndroidSmokeTests
{
	[Fact]
	public void IsRunningOnAndroid()
	{
		Assert.True(OperatingSystem.IsAndroid());
	}

	[Fact]
	public void DeviceInfo_IsAndroid()
	{
		Assert.Equal(DevicePlatform.Android, DeviceInfo.Platform);
	}
}
