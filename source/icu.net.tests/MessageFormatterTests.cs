// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System.Runtime.InteropServices;
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
			// umsg_format double varargs are broken on Linux with ICU 74+ (wrong value read)
			// and crash on macOS ARM64 (ABI mismatch) — skip in both cases.
			// net461 only runs on Windows so this check is unnecessary there.
#if !NETFRAMEWORK
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
				string.CompareOrdinal(Wrapper.IcuVersion, "74") >= 0)
				Assert.Ignore("umsg_format double varargs not reliable on this platform/ICU version");
#endif

			using (var formatter = new MessageFormatter(MessageText, "en_US"))
			{
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains 2 items."));
			}
		}

		[Test]
		public void StaticFormat()
		{
			// umsg_format double varargs are broken on Linux with ICU 74+ (wrong value read)
			// and crash on macOS ARM64 (ABI mismatch) — skip in both cases.
			// net461 only runs on Windows so this check is unnecessary there.
#if !NETFRAMEWORK
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
				string.CompareOrdinal(Wrapper.IcuVersion, "74") >= 0)
				Assert.Ignore("umsg_format double varargs not reliable on this platform/ICU version");
#endif

			Assert.That(MessageFormatter.Format(MessageText, "en_US", 1, "disk", "MyDisk"),
				Is.EqualTo("The disk \"MyDisk\" contains 1 items."));
		}
	}
}
