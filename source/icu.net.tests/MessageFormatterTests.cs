// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class MessageFormatterTests
	{
		private const string MessageText = "The {1} \"{2}\" contains {0,number} items.";

		[Test]
		public void ToPattern()
		{
			using (var formatter = new MessageFormatter(MessageText, "en_US"))
			{
				Assert.That(formatter.Pattern, Is.EqualTo(MessageText));
			}
		}

		[Test]
		public void Format()
		{
			using (var formatter = new MessageFormatter(MessageText, "en_US"))
			{
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains 2 items."));
			}
		}

		[Test]
		public void StaticFormat()
		{
			Assert.That(MessageFormatter.Format(MessageText, "en_US", 1, "disk", "MyDisk"),
				Is.EqualTo("The disk \"MyDisk\" contains 1 items."));
		}
	}
}
