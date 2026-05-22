// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using NUnit.Framework;

namespace Icu.Tests
{
	// Excluded on macOS: MessageFormatter.Format calls the variadic C function umsg_format,
	// which crashes on ARM64 due to ABI mismatch (AAPCS64 varargs vs .NET fixed-slot marshaling).
	[Platform(Exclude = "MacOsX")]
	[TestFixture]
	public class MessageFormatterTests
	{
		private const string MessageText = "The {1} \"{2}\" contains {0,choice,0#no files|1#one file|1<{0,number} files}.";

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
					Is.EqualTo("The disk \"MyDisk\" contains 2 files."));
			}
		}

		[Test]
		public void StaticFormat()
		{
			Assert.That(MessageFormatter.Format(MessageText, "en_US", 1, "disk", "MyDisk"),
				Is.EqualTo("The disk \"MyDisk\" contains one file."));
		}
	}
}
