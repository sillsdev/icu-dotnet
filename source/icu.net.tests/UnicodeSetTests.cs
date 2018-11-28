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
		[TestCase(null, // new[] {}
			ExpectedResult = "[]")]
		[TestCase("", // new[] { "" }
			ExpectedResult = "[]")]
		[TestCase("A B C D E F G H I J K L M N O P Q R S TZ T U V W X Y Z",
			ExpectedResult = "[A-Z{TZ}]")]
		[TestCase("α Α ά ὰ ᾷ ἀ Ἀ ἁ Ἁ ἄ Ἄ ἂ ἅ Ἅ ἃ Ἃ ᾶ ᾳ ᾴ ἆ Ἆ ᾄ ᾅ β Β γ Γ δ Δ ε Ε έ ὲ ἐ Ἐ ἑ Ἑ ἔ Ἔ ἕ Ἕ ἓ Ἓ ζ Ζ η Η ή ὴ ῇ ἠ Ἠ ἡ Ἡ ἤ Ἤ ἢ ἥ Ἥ Ἢ ἣ ᾗ ῆ ῃ ῄ ἦ Ἦ ᾖ ἧ ᾐ ᾑ ᾔ θ Θ ι ί ὶ ϊ ΐ ῒ ἰ Ἰ ἱ Ἱ ἴ Ἴ ἵ Ἵ ἳ ῖ ἶ ἷ κ Κ λ Λ μ Μ ν Ν ξ Ξ ο Ο ό ὸ ὀ Ὀ ὁ Ὁ ὄ Ὄ ὅ ὂ Ὅ ὃ Ὃ π Π ρ Ρ ῥ Ῥ σ ς Σ τ Τ υ Υ ύ ὺ ϋ ΰ ῢ ὐ ὑ Ὑ ὔ ὕ ὒ Ὕ ὓ ῦ ὖ ὗ Ὗ φ Φ χ Χ ψ Ψ ω ώ ὼ ῷ ὠ ὡ Ὡ ὤ Ὤ ὢ ὥ Ὥ ᾧ ῶ ῳ ῴ ὦ Ὦ ὧ Ὧ ᾠ",
			ExpectedResult = @"[\u0391-\u0398\u039A-\u03A1\u03A3-\u03A8\u03B1-\u03C9{\u0391\u0313}{\u0391\u0313\u0301}{\u0391\u0313\u0342}{\u0391\u0314}{\u0391\u0314\u0300}{\u0391\u0314\u0301}{\u0395\u0313}{\u0395\u0313\u0301}{\u0395\u0314}{\u0395\u0314\u0300}{\u0395\u0314\u0301}{\u0397\u0313}{\u0397\u0313\u0300}{\u0397\u0313\u0301}{\u0397\u0313\u0342}{\u0397\u0314}{\u0397\u0314\u0301}{\u0399\u0313}{\u0399\u0313\u0301}{\u0399\u0314}{\u0399\u0314\u0301}{\u039F\u0313}{\u039F\u0313\u0301}{\u039F\u0314}{\u039F\u0314\u0300}{\u039F\u0314\u0301}{\u03A1\u0314}{\u03A5\u0314}{\u03A5\u0314\u0301}{\u03A5\u0314\u0342}{\u03A9\u0313\u0301}{\u03A9\u0313\u0342}{\u03A9\u0314}{\u03A9\u0314\u0301}{\u03A9\u0314\u0342}{\u03B1\u0300}{\u03B1\u0301}{\u03B1\u0301\u0345}{\u03B1\u0313}{\u03B1\u0313\u0300}{\u03B1\u0313\u0301}{\u03B1\u0313\u0301\u0345}{\u03B1\u0313\u0342}{\u03B1\u0314}{\u03B1\u0314\u0300}{\u03B1\u0314\u0301}{\u03B1\u0314\u0301\u0345}{\u03B1\u0342}{\u03B1\u0342\u0345}{\u03B1\u0345}{\u03B5\u0300}{\u03B5\u0301}{\u03B5\u0313}{\u03B5\u0313\u0301}{\u03B5\u0314}{\u03B5\u0314\u0300}{\u03B5\u0314\u0301}{\u03B7\u0300}{\u03B7\u0301}{\u03B7\u0301\u0345}{\u03B7\u0313}{\u03B7\u0313\u0300}{\u03B7\u0313\u0301}{\u03B7\u0313\u0301\u0345}{\u03B7\u0313\u0342}{\u03B7\u0313\u0342\u0345}{\u03B7\u0313\u0345}{\u03B7\u0314}{\u03B7\u0314\u0300}{\u03B7\u0314\u0301}{\u03B7\u0314\u0342}{\u03B7\u0314\u0342\u0345}{\u03B7\u0314\u0345}{\u03B7\u0342}{\u03B7\u0342\u0345}{\u03B7\u0345}{\u03B9\u0300}{\u03B9\u0301}{\u03B9\u0308}{\u03B9\u0308\u0300}{\u03B9\u0308\u0301}{\u03B9\u0313}{\u03B9\u0313\u0301}{\u03B9\u0313\u0342}{\u03B9\u0314}{\u03B9\u0314\u0300}{\u03B9\u0314\u0301}{\u03B9\u0314\u0342}{\u03B9\u0342}{\u03BF\u0300}{\u03BF\u0301}{\u03BF\u0313}{\u03BF\u0313\u0300}{\u03BF\u0313\u0301}{\u03BF\u0314}{\u03BF\u0314\u0300}{\u03BF\u0314\u0301}{\u03C1\u0314}{\u03C5\u0300}{\u03C5\u0301}{\u03C5\u0308}{\u03C5\u0308\u0300}{\u03C5\u0308\u0301}{\u03C5\u0313}{\u03C5\u0313\u0300}{\u03C5\u0313\u0301}{\u03C5\u0313\u0342}{\u03C5\u0314}{\u03C5\u0314\u0300}{\u03C5\u0314\u0301}{\u03C5\u0314\u0342}{\u03C5\u0342}{\u03C9\u0300}{\u03C9\u0301}{\u03C9\u0301\u0345}{\u03C9\u0313}{\u03C9\u0313\u0300}{\u03C9\u0313\u0301}{\u03C9\u0313\u0342}{\u03C9\u0313\u0345}{\u03C9\u0314}{\u03C9\u0314\u0301}{\u03C9\u0314\u0342}{\u03C9\u0314\u0342\u0345}{\u03C9\u0342}{\u03C9\u0342\u0345}{\u03C9\u0345}]")]
		public string ToPattern(string input)
		{
			return UnicodeSet.ToPattern(input?.Split(' ') ?? new string[]{});
		}

		[Test]
		public void NullToPattern()
		{
			Assert.That(() => UnicodeSet.ToPattern(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void ToChar()
		{
			const string pattern = "[A-GH-NO-ST-Z{TZ}]";
			IEnumerable<string> expected = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z TZ".Split(' ');
			Assert.That(UnicodeSet.ToCharacters(pattern), Is.EqualTo(expected));
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
