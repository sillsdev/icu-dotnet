// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class UnicodeSetTests
	{
		[Test]
		public void EmptySetToPattern()
		{
			List<string> unicodeSet = new List<string>();
			Assert.That(UnicodeSet.ToPattern(unicodeSet), Is.EqualTo("[]"));
		}

		[Test]
		public void ToPattern()
		{
			List<string> unicodeSet = "A B C D E F G H I J K L M N O P Q R S TZ T U V W X Y Z".Split(' ').ToList();
			const string expected = "[A-Z{TZ}]";
			string result = UnicodeSet.ToPattern(unicodeSet);
			Assert.That(result, Is.EqualTo(expected));

		}

		[Test]
		public void NullToPattern()
		{
			Assert.Throws<ArgumentNullException>(
				() => UnicodeSet.ToPattern(null));
		}

		[Test]
		public void ToChar()
		{
			const string pattern = "[A-GH-NO-ST-Z{TZ}]";
			IEnumerable<string> unicodeSet = UnicodeSet.ToCharacters(pattern);
			IEnumerable<string> expected = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z TZ".Split(' ');
			Assert.That(unicodeSet, Is.EqualTo(expected));
		}

		[Test]
		public void SinglePatternToChar()
		{
			const string pattern = "[A]";

			IEnumerable<string> unicodeSet = UnicodeSet.ToCharacters(pattern);
			IEnumerable<string> expected = "A".Split(' ');
			Assert.That(unicodeSet, Is.EqualTo(expected));

		}

		[Test]
		public void NullToChar()
		{
			Assert.That(UnicodeSet.ToCharacters(null).Equals(Enumerable.Empty<string>()));
		}

	}
}
