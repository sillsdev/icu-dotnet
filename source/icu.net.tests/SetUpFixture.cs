// Copyright (c) 2017 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using NUnit.Framework;

namespace Icu.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Wrapper.Init();
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			Wrapper.Cleanup();
		}
	}
}
