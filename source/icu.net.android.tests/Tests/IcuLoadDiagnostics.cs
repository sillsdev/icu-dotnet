// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Text;
using Icu;
using Icu.Collation;
using Xunit.Abstractions;

namespace icu.net.android.tests.Tests;

public class IcuLoadDiagnostics
{
	public IcuLoadDiagnostics(ITestOutputHelper output)
	{
		_output = output;
	}

	private readonly ITestOutputHelper _output;

	[Fact]
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

		AndroidIcuSetup.Prepare();
		Wrapper.Verbose = true;
		AndroidIcuSetup.ApplyDataDirectory();

		var initResult = Wrapper.Init();
		log.AppendLine($"Wrapper.Init(): {initResult}");

		try
		{
			log.AppendLine($"Wrapper.IcuVersion: {Wrapper.IcuVersion}");
			log.AppendLine($"Wrapper.UnicodeVersion: {Wrapper.UnicodeVersion}");
			using var collator = new RuleBasedCollator("");
			log.AppendLine($"RuleBasedCollator.Compare('a','b'): {collator.Compare("a", "b")}");
		}
		catch (Exception ex)
		{
			log.AppendLine($"Collation probe failed: {ex.GetType().Name}: {ex.Message}");
			_output.WriteLine(log.ToString());
			throw;
		}

		_output.WriteLine(log.ToString());
	}
}
