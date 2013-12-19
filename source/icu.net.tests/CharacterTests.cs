// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class CharacterTests
	{
		[Test]
		public void Digit()
		{
			// valid digit tests
			Assert.That(Character.Digit('9', 10), Is.EqualTo(9));
			Assert.That(Character.Digit('A', 16), Is.EqualTo(10));
			Assert.That(Character.Digit(0xc69, 10), Is.EqualTo(3));		// Telugu digit 3
			Assert.That(Character.Digit(0x033, 10), Is.EqualTo(3));		// Western digit 3
			Assert.That(Character.Digit(0x664, 10), Is.EqualTo(4));		// Arabic-indic digit 4
			Assert.That(Character.Digit('\u096B', 10), Is.EqualTo(5));	// Devanagari '5'

			// invalid digit tests
			Assert.That(Character.Digit(0xBf1, 10), Is.EqualTo(-1));	// Tamil number one hundred (non-digit)
			Assert.That(Character.Digit(0x041, 10), Is.EqualTo(-1));	// 'A'
			Assert.That(Character.Digit('A', 10), Is.EqualTo(-1));
		}

		[Test]
		public void IsAlphabetic()
		{
			Assert.That(Character.IsAlphabetic('a'), Is.True);
			Assert.That(Character.IsAlphabetic('1'), Is.False);
		}

		[Test]
		public void IsIdeographic()
		{
			Assert.That(Character.IsIdeographic('\u4E10'), Is.True);
			Assert.That(Character.IsIdeographic('a'), Is.False);
		}

		[Test]
		public void IsDiacritic()
		{
			Assert.That(Character.IsDiacritic('\u0300'), Is.True);
			Assert.That(Character.IsDiacritic('a'), Is.False);
		}

		[Test]
		public void IsSymbol()
		{
			Assert.That(Character.IsSymbol('#'), Is.False);
			Assert.That(Character.IsSymbol('a'), Is.False);
			Assert.That(Character.IsSymbol('$'), Is.True);
			Assert.That(Character.IsSymbol('+'), Is.True);
			Assert.That(Character.IsSymbol('`'), Is.True);
			Assert.That(Character.IsSymbol(0x0385), Is.True);
			Assert.That(Character.IsSymbol(0x0B70), Is.True);
		}

		[Test]
		public void GetCharType()
		{
			Assert.That(Character.GetCharType('a'), Is.EqualTo(Character.UCharCategory.LOWERCASE_LETTER));
		}

		[Test]
		public void IsNumeric()
		{
			Assert.That(Character.IsNumeric('4'), Is.True);
			Assert.That(Character.IsNumeric('a'), Is.False);
		}

		[Test]
		public void GetNumericValue()
		{
			Assert.That(Character.GetNumericValue('1'), Is.EqualTo(1));
			Assert.That(Character.GetNumericValue('a'), Is.EqualTo(Character.NO_NUMERIC_VALUE));
		}

		[Test]
		public void IsPunct()
		{
			Assert.That(Character.IsPunct('.'), Is.True);
			Assert.That(Character.IsPunct('a'), Is.False);
		}

		[Test]
		public void IsMirrored()
		{
			Assert.That(Character.IsMirrored('a'), Is.False);
		}

		[Test]
		public void IsControl()
		{
			Assert.That(Character.IsControl('\u0001'), Is.True);
			Assert.That(Character.IsControl('a'), Is.False);
		}

		[Test]
		public void IsControl_String()
		{
			Assert.That(Character.IsControl("\u0001"), Is.True);
			Assert.That(Character.IsControl("a"), Is.False);
			Assert.That(Character.IsControl("\u0001\u0002"), Is.False);
			Assert.That(Character.IsControl(string.Empty), Is.False);
			Assert.That(Character.IsControl(null), Is.False);
		}

		[Test]
		public void IsSpace()
		{
			Assert.That(Character.IsSpace(' '), Is.True);
			Assert.That(Character.IsSpace('a'), Is.False);
		}

		[Test]
		public void IsSpace_String()
		{
			Assert.That(Character.IsSpace(" "), Is.True);
			Assert.That(Character.IsSpace("a"), Is.False);
			Assert.That(Character.IsSpace("  "), Is.False);
			Assert.That(Character.IsSpace(string.Empty), Is.False);
			Assert.That(Character.IsSpace(null), Is.False);
		}

		[Test]
		[SetUICulture("en-US")]
		[Category("Full ICU")]
		public void GetPrettyICUCharName()
		{
			Assert.That(Character.GetPrettyICUCharName("a"), Is.EqualTo("Latin Small Letter A"));
			Assert.That(Character.GetPrettyICUCharName("ab"), Is.Null);
			Assert.That(Character.GetPrettyICUCharName(string.Empty), Is.Null);
			Assert.That(Character.GetPrettyICUCharName(null), Is.Null);
		}

		[Test]
		[Category("Full ICU")]
		public void GetCharName()
		{
			Assert.That(Character.GetCharName(65), Is.EqualTo("LATIN CAPITAL LETTER A"));
			Assert.That(Character.GetCharName(-1), Is.Null);
		}
	}
}
