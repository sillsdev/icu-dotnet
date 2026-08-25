// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Text;
using Icu;
using NUnit.Framework;

namespace icu.net.android.tests.Tests;

[TestFixture]
public class IcuLoadDiagnostics
{
	[Test]
	public void ReportIcuLoadEnvironment()
	{
		var log = new StringBuilder();
		log.AppendLine("=== ICU load diagnostics ===");

#if __ANDROID__
		var context = Android.App.Application.Context;
		var nativeDir = context?.ApplicationInfo?.NativeLibraryDir;
		log.AppendLine($"NativeLibraryDir: {nativeDir ?? "(null)"}");
		if (!string.IsNullOrEmpty(nativeDir) && Directory.Exists(nativeDir))
		{
			foreach (var file in Directory.GetFiles(nativeDir, "libicu*").OrderBy(f => f))
				log.AppendLine($"  {Path.GetFileName(file)}");
		}
#else
		log.AppendLine("Not running on Android (__ANDROID__ not defined).");
#endif

		Wrapper.Verbose = true;

		var initResult = Wrapper.Init();
		log.AppendLine($"Wrapper.Init(): {initResult}");
		log.AppendLine($"Wrapper.IcuVersion: {Wrapper.IcuVersion}");
		log.AppendLine($"Wrapper.UnicodeVersion: {Wrapper.UnicodeVersion}");

		TestContext.WriteLine(log.ToString());

		Assert.That(initResult, Is.EqualTo(ErrorCode.ZERO_ERROR));
	}
}
