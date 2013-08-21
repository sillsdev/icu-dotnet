// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;
using Icu;

namespace Icu.Tests
{
	[TestFixture]
	[SetCulture("es-ES")]
	[SetUICulture("en-US")]
	public class LocaleTests
	{
		[Test]
		[SetUICulture("de-DE")]
		public void ConstructDefault()
		{
			Locale locale = new Locale();
			Assert.That(locale.Id, Is.EqualTo("de_DE"));
		}

		[Test]
		public void ConstructFromLocale_DotNetVersion()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.ToString(), Is.EqualTo("en_US"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromLocale_IcuVersion()
		{
			Locale locale = new Locale("en_US");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromLocale_Lowercase()
		{
			Locale locale = new Locale("en_us");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromLocale_PosixLocale()
		{
			Locale locale = new Locale("en_US.UTF-8");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromLanguage()
		{
			Locale locale = new Locale("en");
			Assert.That(locale.Id, Is.EqualTo("en"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.Empty);
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromParts_LanguageAndCountry()
		{
			Locale locale = new Locale("en", "US");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromParts_LanguageCountryAndVariant()
		{
			Locale locale = new Locale("en", "US", "WIN");
			Assert.That(locale.Id, Is.EqualTo("en_US_WIN"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.EqualTo("US"));
			Assert.That(locale.Variant, Is.EqualTo("WIN"));
		}

		[Test]
		public void ConstructFromParts_LanguageAndVariant()
		{
			Locale locale = new Locale("en", string.Empty, "WIN");
			Assert.That(locale.Id, Is.EqualTo("en__WIN"));
			Assert.That(locale.Language, Is.EqualTo("en"));
			Assert.That(locale.Script, Is.Empty);
			Assert.That(locale.Country, Is.Empty);
			Assert.That(locale.Variant, Is.EqualTo("WIN"));
		}

		[Test]
		public void ConstructFromLocale_Script()
		{
			Locale locale = new Locale("ru-Cyrl-AZ");
			Assert.That(locale.Id, Is.EqualTo("ru_Cyrl_AZ"));
			Assert.That(locale.Language, Is.EqualTo("ru"));
			Assert.That(locale.Script, Is.EqualTo("Cyrl"));
			Assert.That(locale.Country, Is.EqualTo("AZ"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromPartsWithScript()
		{
			Locale locale = new Locale("ru", "Cyrl", "AZ");
			Assert.That(locale.Id, Is.EqualTo("ru_Cyrl_AZ"));
			Assert.That(locale.Language, Is.EqualTo("ru"));
			Assert.That(locale.Script, Is.EqualTo("Cyrl"));
			Assert.That(locale.Country, Is.EqualTo("AZ"));
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ConstructFromLocale_ScriptAndVariantAndKeywords()
		{
			Locale locale = new Locale("sr_Latn_RS_REVISED@currency=USD");
			Assert.That(locale.Id, Is.EqualTo("sr_Latn_RS_REVISED@currency=USD"));
			Assert.That(locale.Language, Is.EqualTo("sr"));
			Assert.That(locale.Script, Is.EqualTo("Latn"));
			Assert.That(locale.Country, Is.EqualTo("RS"));
			Assert.That(locale.Variant, Is.EqualTo("REVISED"));
		}

		[Test]
		public void ConstructFromParts_ScriptAndVariantAndKeywords()
		{
			Locale locale = new Locale("sr", "Latn", "RS_REVISED", "currency=USD");
			Assert.That(locale.Id, Is.EqualTo("sr_Latn_RS_REVISED@currency=USD"));
			Assert.That(locale.Language, Is.EqualTo("sr"));
			Assert.That(locale.Script, Is.EqualTo("Latn"));
			Assert.That(locale.Country, Is.EqualTo("RS"));
			Assert.That(locale.Variant, Is.EqualTo("REVISED"));
		}

		[Test]
		public void ConstructFromParts_ScriptAndKeywords()
		{
			Locale locale = new Locale("sr", "Latn", string.Empty, "currency=USD");
			Assert.That(locale.Id, Is.EqualTo("sr_Latn@currency=USD"));
			Assert.That(locale.Language, Is.EqualTo("sr"));
			Assert.That(locale.Script, Is.EqualTo("Latn"));
			Assert.That(locale.Country, Is.Empty);
			Assert.That(locale.Variant, Is.Empty);
		}

		[Test]
		public void ToString_SameAsId()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Id, Is.EqualTo("en_US"));
			Assert.That(locale.ToString(), Is.EqualTo("en_US"));
		}

		[Test]
		public void Lcid()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Lcid, Is.EqualTo(1033));
		}

		[Test]
		public void Iso3Country()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Iso3Country, Is.EqualTo("USA"));
			Assert.That(locale.Country, Is.EqualTo("US"));
		}

		[Test]
		public void Iso3Language()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Iso3Language, Is.EqualTo("eng"));
			Assert.That(locale.Language, Is.EqualTo("en"));
		}

		[Test]
		public void DisplayLanguage()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.DisplayLanguage, Is.EqualTo("English"));
		}

		[Test]
		[SetUICulture("de-DE")]
		public void DisplayLanguage_DifferentDefaultLocale()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.DisplayLanguage, Is.EqualTo("Englisch"));
		}

		[Test]
		public void GetDisplayLanguage()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.GetDisplayLanguage(new Locale("fr_FR")), Is.EqualTo("anglais"));
		}

		[Test]
		public void GetDisplayName()
		{
			Locale locale = new Locale("en_US_POSIX");
			Assert.That(locale.GetDisplayName(new Locale("fr")), Is.EqualTo("anglais (États-Unis, informatique)"));
		}

		[Test]
		public void Name()
		{
			Locale locale = new Locale("en-US");
			Assert.That(locale.Name, Is.EqualTo("en_US"));
		}

		[Test]
		public void AvailableLocales()
		{
			var locales = Locale.AvailableLocales;
			Assert.That(locales.Count, Is.GreaterThan(0));
		}
	}
}
