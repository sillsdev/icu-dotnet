// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class MessageFormatterTests
	{
		// Choice-format pattern. Works on ICU < 74; on ICU 74+ umsg_format
		// silently returns empty rather than an error (choice format was deprecated).
		private const string ChoiceMessageText =
			"The {1} \"{2}\" contains {0,choice,0#no files|1#one file|1<{0,number} files}.";

		// Plural-format pattern. umsg_open/umsg_toPattern work on all ICU versions;
		// umsg_format is affected by the Linux ICU 74+ varargs ABI issue.
		private const string PluralMessageText =
			"The {1} \"{2}\" contains {0,plural,=0{no files}=1{one file}other{{0,number} files}}.";

		// Skip when umsg_format produces wrong results due to the Linux ICU 74+ double-varargs
		// ABI mismatch, or when umsg_open silently mangles the choice format (also ICU 74+).
		// net461 only runs on Windows, so the check is unnecessary there.
		private static void SkipIfUnreliableOnThisPlatform()
		{
#if !NETFRAMEWORK
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
				string.CompareOrdinal(Wrapper.IcuVersion, "74") >= 0)
				Assert.Ignore("umsg_format not reliable on this platform/ICU version");
#endif
		}

		#region choice format tests

		[Test]
		public void ToPattern()
		{
			SkipIfUnreliableOnThisPlatform();
			using (var formatter = new MessageFormatter(ChoiceMessageText, "en_US"))
			{
				Assert.That(formatter.Pattern, Is.EqualTo(ChoiceMessageText));
			}
		}

		[Test]
		public void Format()
		{
			SkipIfUnreliableOnThisPlatform();
			using (var formatter = new MessageFormatter(ChoiceMessageText, "en_US"))
			{
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains 2 files."));
			}
		}

		[Test]
		public void StaticFormat()
		{
			SkipIfUnreliableOnThisPlatform();
			Assert.That(MessageFormatter.Format(ChoiceMessageText, "en_US", 1, "disk", "MyDisk"),
				Is.EqualTo("The disk \"MyDisk\" contains one file."));
		}

		#endregion

		#region plural format tests

		[Test]
		public void PluralFormat_ToPattern()
		{
			using (var formatter = new MessageFormatter(PluralMessageText, "en_US"))
			{
				Assert.That(formatter.Pattern, Is.EqualTo(PluralMessageText));
			}
		}

		[Test]
		public void PluralFormat_Format()
		{
			SkipIfUnreliableOnThisPlatform();
			using (var formatter = new MessageFormatter(PluralMessageText, "en_US"))
			{
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains 2 files."));
			}
		}

		[Test]
		public void PluralFormat_StaticFormat()
		{
			SkipIfUnreliableOnThisPlatform();
			Assert.That(MessageFormatter.Format(PluralMessageText, "en_US", 1, "disk", "MyDisk"),
				Is.EqualTo("The disk \"MyDisk\" contains one file."));
		}

		#endregion
	}
}
