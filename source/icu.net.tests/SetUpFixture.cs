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
			Wrapper.Cleanup();
		}
	}
}
