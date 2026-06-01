// Copyright (c) 2017-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		private static bool IsWindows => Platform.OperatingSystem == OperatingSystemType.Windows;
		private static bool IsMac => Platform.OperatingSystem == OperatingSystemType.MacOSX;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			// On macOS the library is never explicitly loaded or unloaded (NativeLibrary.Free
			// is skipped to avoid dyld-destructor crashes); ICU loads lazily on first use.
			if (!IsMac)
			{
				Wrapper.Init();
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
			// Flush SafeHandle finalizers before Cleanup unloads ICU, so ReleaseHandle
			// calls don't fire against an already-unloaded library.
			GC.Collect(2, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();

			// On macOS the library stays resident (see Init comment above); skip Cleanup().
			if (!IsMac)
			{
				Wrapper.Cleanup();
			}
		}
	}
}
