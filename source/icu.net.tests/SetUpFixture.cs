// Copyright (c) 2017-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
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

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			AppDomain.CurrentDomain.ProcessExit += (_, _) =>
			{
				Console.Error.WriteLine("[icu.net diag] AppDomain.ProcessExit fired");
				Console.Error.Flush();
			};

			Wrapper.Init();

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
			Console.Error.WriteLine("[icu.net diag] RunAfterAnyTests: before GC.Collect");
			Console.Error.Flush();
			GC.Collect(2, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();
			Console.Error.WriteLine("[icu.net diag] RunAfterAnyTests: after GC, before Wrapper.Cleanup");
			Console.Error.Flush();
			Wrapper.Cleanup();
			Console.Error.WriteLine("[icu.net diag] RunAfterAnyTests: after Wrapper.Cleanup");
			Console.Error.Flush();
		}
	}
}
