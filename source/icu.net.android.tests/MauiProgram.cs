using DeviceRunners.VisualRunners;
using DeviceRunners.VisualRunners.NUnit;
using Icu;
using Icu.Tests;
using Microsoft.Extensions.Logging;

namespace icu.net.android.tests;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Wrapper.AndroidTestConfigure = AndroidIcuSetup.Configure;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseVisualTestRunner(conf => conf
				.AddCliConfiguration()
				.AddConsoleResultChannel()
				.AddTestAssembly(typeof(MauiProgram).Assembly)
				.AddXunit()
				.AddTestAssembly(typeof(SetUpFixture).Assembly)
				.AddNUnit());

#if DEBUG
		builder.Logging.AddDebug();
#else
		builder.Logging.AddConsole();
#endif

		return builder.Build();
	}
}
