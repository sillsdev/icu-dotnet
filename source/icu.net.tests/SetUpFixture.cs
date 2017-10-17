// Copyright (c) 2017 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using NUnit.Framework;

namespace Icu.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		[SetUp]
		public void RunBeforeAnyTests()
		{
			Wrapper.Init();
		}

		[TearDown]
		public void RunAfterAnyTests()
		{
			Wrapper.Cleanup();
		}
	}
}
