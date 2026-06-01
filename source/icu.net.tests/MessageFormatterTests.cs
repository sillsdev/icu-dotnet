// Copyright (c) 2018-2026 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class MessageFormatterTests
	{
		// Choice-format pattern. Deprecated in ICU 49.
		// https://unicode-org.github.io/icu-docs/apidoc/released/icu4c/deprecated.html#_deprecated000322
		// On Linux ICU 74+ the double argument is read as 0 (varargs ABI mismatch), producing wrong output.
		private const string ChoiceMessageText =
			"The {1} \"{2}\" contains {0,choice,0#no files|1#one file|1<{0,number} files}.";

		// The actual TransliteratorNamePattern stored in ICUDATA-translit. Its 0# branch is
		// empty, so when the double arg is read as 0 on Linux ICU 74+, umsg_format returns "".
		// This is why Transliterator.GetDisplayName needs the IsNullOrEmpty fallback.
		private const string TransliteratorNamePattern = "{0,choice,0#|1#{1}|2#{1} to {2}}";

		// Plural-format pattern. umsg_open/umsg_toPattern work on all ICU versions;
		// umsg_format is affected by the Linux ICU 74+ varargs ABI issue.
		// https://github.com/dotnet/runtime/issues/48752
		private const string PluralMessageText =
			"The {1} \"{2}\" contains {0,plural,=0{no files}=1{one file}other{{0,number} files}}.";

		// Skip when umsg_format produces wrong results due to the Linux ICU 74+ double-varargs
		// ABI mismatch (https://github.com/dotnet/runtime/issues/48752), or when umsg_open
		// silently mangles the choice format (also ICU 74+).
		// net461 only runs on Windows, so the check is unnecessary there.
		private static void SkipIfUnreliableOnThisPlatform()
		{
#if !NETFRAMEWORK
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && IcuMajorVersionAtLeast(74))
				Assert.Ignore("umsg_format not reliable on this platform/ICU version");
#endif
		}

#if !NETFRAMEWORK
		private static bool IcuMajorVersionAtLeast(int n) =>
			int.Parse(Wrapper.IcuVersion.Split('.')[0]) >= n;
#endif

		#region choice format tests

		[Test]
		public void ChoiceFormat_ToPattern()
		{
			SkipIfUnreliableOnThisPlatform();
			using (var formatter = new MessageFormatter(ChoiceMessageText, "en_US"))
			{
				Assert.That(formatter.Pattern, Is.EqualTo(ChoiceMessageText));
			}
		}

		[Test]
		public void ChoiceFormat_Format()
		{
			SkipIfUnreliableOnThisPlatform();
			using (var formatter = new MessageFormatter(ChoiceMessageText, "en_US"))
			{
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains 2 files."));
			}
		}

		[Test]
		[Platform(Include = "Linux")]
		public void ChoiceFormat_Format_WrongOutputOnLinuxIcu74Plus()
		{
#if !NETFRAMEWORK
			if (!IcuMajorVersionAtLeast(74))
				Assert.Ignore("Behavior only occurs on Linux ICU 74+");
			using (var formatter = new MessageFormatter(ChoiceMessageText, "en_US"))
			{
				// Double arg is read as 0 due to varargs ABI mismatch; choice format picks "0#no files".
				Assert.That(formatter.Format(2, "disk", "MyDisk"),
					Is.EqualTo("The disk \"MyDisk\" contains no files."));
			}
#endif
		}

		[Test]
		[Platform(Include = "Linux")]
		public void ChoiceFormat_Format_TransliteratorPatternEmptyOnLinuxIcu74Plus()
		{
#if !NETFRAMEWORK
			if (!IcuMajorVersionAtLeast(74))
				Assert.Ignore("Behavior only occurs on Linux ICU 74+");
			using (var formatter = new MessageFormatter(TransliteratorNamePattern, "en_US"))
			{
				// Double arg is read as 0; the 0# branch is empty, so the result is "".
				// This is why Transliterator.GetDisplayName uses the IsNullOrEmpty fallback.
				Assert.That(formatter.Format(2, "Armenian", "Latin"), Is.Empty);
			}
#endif
		}

		[Test]
		public void ChoiceFormat_StaticFormat()
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
