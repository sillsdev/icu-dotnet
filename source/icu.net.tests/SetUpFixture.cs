// Copyright (c) 2017-2022 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using NUnit.Framework;

namespace Icu.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
#if NUNIT2
		[SetUp]
#else
		[OneTimeSetUp]
#endif
		public void RunBeforeAnyTests()
		{
			Wrapper.Init();

			// Limit maximum version to the version we install, otherwise some tests might fail
			// if we find a higher version on the PATH.
			Wrapper.ConfineIcuVersions(Wrapper.MinSupportedIcuVersion,
				NativeMethodsTests.MaxInstalledIcuLibraryVersion);
		}

#if NUNIT2
		[TearDown]
#else
		[OneTimeTearDown]
#endif
		public void RunAfterAnyTests()
		{
			Wrapper.Cleanup();
		}
	}
}
