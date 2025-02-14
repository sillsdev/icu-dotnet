// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Globalization;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class CharacterTests
	{
		private void SetUICulture(string culture)
		{
			var cultureInfo = new CultureInfo(culture);
#if NET40
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
#else
			CultureInfo.CurrentUICulture = cultureInfo;
#endif
		}

		// valid digit tests
		[TestCase('9', 10, ExpectedResult = 9)]
		[TestCase('A', 16, ExpectedResult = 10)]
		[TestCase('\u0c69', 10, ExpectedResult = 3, Description = "Telugu digit 3")]
		[TestCase('\u0033', 10, ExpectedResult = 3, Description = "Western digit 3")]
		[TestCase('\u0664', 10, ExpectedResult = 4, Description = "Arabic-indic digit 4")]
		[TestCase('\u096B', 10, ExpectedResult = 5, Description = "Devanagari '5'")]
		// invalid digit tests
		[TestCase('\u0Bf1', 10, ExpectedResult = -1, Description = "Tamil number one hundred (non-digit)")]
		[TestCase('\u0041', 10, ExpectedResult = -1, Description = "'A'")]
		[TestCase('a', 10, ExpectedResult = -1)]
		public int Digit(int c, byte radix)
		{
			return Character.Digit(c, radix);
		}

		[TestCase('a', ExpectedResult = true)]
		[TestCase('1', ExpectedResult = false)]
		public bool IsAlphabetic(char c)
		{
			return Character.IsAlphabetic(c);
		}

		[TestCase('\u4E10', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsIdeographic(char c)
		{
			return Character.IsIdeographic(c);
		}

		[TestCase('\u0300', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsDiacritic(char c)
		{
			return Character.IsDiacritic(c);
		}

		[TestCase('#', ExpectedResult = false)]
		[TestCase('a', ExpectedResult = false)]
		[TestCase('$', ExpectedResult = true)]
		[TestCase('+', ExpectedResult = true)]
		[TestCase('`', ExpectedResult = true)]
		[TestCase('\u0385', ExpectedResult = true)]
		[TestCase('\u0B70', ExpectedResult = true)]
		public bool IsSymbol(int c)
		{
			return Character.IsSymbol(c);
		}

		[TestCase('a', ExpectedResult = Character.UCharCategory.LOWERCASE_LETTER)]
		public Character.UCharCategory GetCharType(char c)
		{
			return Character.GetCharType(c);
		}

		[TestCase('4', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsNumeric(char c)
		{
			return Character.IsNumeric(c);
		}

		[TestCase('1', ExpectedResult = 1)]
		[TestCase('a', ExpectedResult = Character.NO_NUMERIC_VALUE)]
		public double GetNumericValue(char c)
		{
			return Character.GetNumericValue(c);
		}

		[TestCase('.', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsPunct(char c)
		{
			return Character.IsPunct(c);
		}

		[TestCase('a', ExpectedResult = false)]
		public bool IsMirrored(char c)
		{
			return Character.IsMirrored(c);
		}

		[TestCase('\u0001', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsControl(char c)
		{
			return Character.IsControl(c);
		}

		[TestCase("\u0001", ExpectedResult = true)]
		[TestCase("a", ExpectedResult = false)]
		[TestCase("\u0001\u0002", ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase(null, ExpectedResult = false)]
		public bool IsControl_String(string s)
		{
			return Character.IsControl(s);
		}

		[TestCase(' ', ExpectedResult = true)]
		[TestCase('a', ExpectedResult = false)]
		public bool IsSpace(char c)
		{
			return Character.IsSpace(c);
		}

		[TestCase(" ", ExpectedResult = true)]
		[TestCase("a", ExpectedResult = false)]
		[TestCase("  ", ExpectedResult = false)]
		[TestCase("", ExpectedResult = false)]
		[TestCase(null, ExpectedResult = false)]
		public bool IsSpace_String(string s)
		{
			return Character.IsSpace(s);
		}

		[Category("Full ICU")]
		[TestCase("a", ExpectedResult = "Latin Small Letter A")]
		[TestCase("ab", ExpectedResult = null)]
		[TestCase("", ExpectedResult = null)]
		[TestCase(null, ExpectedResult = null)]
		public string GetPrettyICUCharName(string s)
		{
			SetUICulture("en-US");
			return Character.GetPrettyICUCharName(s);
		}

		[Category("Full ICU")]
		[TestCase(65, ExpectedResult = "LATIN CAPITAL LETTER A", Description = "A")]
		[TestCase(-1, ExpectedResult = null)]
		public string GetCharName(int c)
		{
			return Character.GetCharName(c);
		}

		[Category("Full ICU")]
		[TestCase(null, ExpectedResult = Character.UCharDirection.BOUNDARY_NEUTRAL)]
		[TestCase('a', ExpectedResult = Character.UCharDirection.LEFT_TO_RIGHT)]
		[TestCase('1', ExpectedResult = Character.UCharDirection.EUROPEAN_NUMBER)]
		public Character.UCharDirection CharDirection(char c)
		{
			return Character.CharDirection(c);
		}

		[TestCase('A', ExpectedResult = 'a')]
		[TestCase('a', ExpectedResult = 'a')]
		[TestCase('/', ExpectedResult = '/')]
		public int ToLower(int c)
		{
			return Character.ToLower(c);
		}

		[TestCase('A', ExpectedResult = 'A')]
		[TestCase('a', ExpectedResult = 'A')]
		[TestCase('/', ExpectedResult = '/')]
		public int ToTitle(int c)
		{
			return Character.ToTitle(c);
		}

		[TestCase('A', ExpectedResult = 'A')]
		[TestCase('a', ExpectedResult = 'A')]
		[TestCase('/', ExpectedResult = '/')]
		public int ToUpper(int c)
		{
			return Character.ToUpper(c);
		}
	}
}
