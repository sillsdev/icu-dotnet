// Copyright (c) 2017-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Icu.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		private static bool IsWindows
		{
			get
			{
				// See Icu.Platform. Unfortunately that's internal, so we can't use it.

#if NETFRAMEWORK
				// See http://www.mono-project.com/docs/faq/technical/#how-to-detect-the-execution-platform
				switch ((int)Environment.OSVersion.Platform)
				{
					case 4:
					case 128:
					case 6:
						return false;
					default:
						return true;
				}
#else
				return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
			}
		}

		private static bool IsMac
		{
			get
			{
#if NETFRAMEWORK
				// See http://www.mono-project.com/docs/faq/technical/#how-to-detect-the-execution-platform
				return (int)Environment.OSVersion.Platform == 6;
#else
				return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
			}
		}

		// Use RUNNER_TEMP (set by GitHub Actions) when available so the CI step can find it.
		private static readonly string DiagFile = Path.Combine(
			Environment.GetEnvironmentVariable("RUNNER_TEMP") ?? Path.GetTempPath(),
			"icu-dotnet-diag.txt");

		internal static void DiagLog(string message)
		{
			var line = $"[{DateTime.UtcNow:HH:mm:ss.fff}] {message}{Environment.NewLine}";
			File.AppendAllText(DiagFile, line);
		}

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			File.WriteAllText(DiagFile, "");  // reset log
			DiagLog("RunBeforeAnyTests start");

			if (IsMac)
			{
				DiagLog("RunBeforeAnyTests: macOS detected, skipping explicit Wrapper.Init");
			}
			else
			{
				Wrapper.Init();
				DiagLog("Wrapper.Init complete");
			}

			if (IsWindows)
			{
				// Limit maximum version to the version we install, otherwise some tests might
				// fail if we find a higher version on the PATH.
				Wrapper.ConfineIcuVersions(Wrapper.MinSupportedIcuVersion,
					NativeMethodsTests.MaxInstalledIcuLibraryVersion);
			}
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			DiagLog("RunAfterAnyTests: before GC.Collect");
			GC.Collect(2, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();
			if (IsMac)
			{
				DiagLog("RunAfterAnyTests: macOS detected, skipping Wrapper.Cleanup");
				return;
			}

			DiagLog("RunAfterAnyTests: after GC, before Wrapper.Cleanup");
			Wrapper.Cleanup();
			DiagLog("RunAfterAnyTests: after Wrapper.Cleanup");
		}
	}
}
