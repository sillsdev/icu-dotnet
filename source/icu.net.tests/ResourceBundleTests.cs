// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class ResourceBundleTests
	{
		[Test]
		public void IsNull()
		{
			using (var resourceBundle = ResourceBundle.Null)
			{
				Assert.That(resourceBundle.IsNull, Is.True);
			}
		}

		[Test]
		public void Name()
		{
			using (var resourceBundle = new ResourceBundle("ICUDATA-translit", "en_US"))
			{
				Assert.That(resourceBundle.Name, Is.EqualTo("en"));
			}
		}

		[Test]
		public void GetStringContents()
		{
			using (var resourceBundle = new ResourceBundle("ICUDATA-translit", "en_US"))
			{
				Assert.That(resourceBundle.GetStringContents(),
					Is.EquivalentTo(new [] { "Hex Escape", "Unicode Character", "Unicode Name", "{0,choice,0#|1#{1}|2#{1} to {2}}" }));
			}
		}

		[Test]
		public void GetStringContentsWithKeys()
		{
			using (var resourceBundle = new ResourceBundle("ICUDATA-translit", "en_US"))
			{
				Assert.That(resourceBundle.GetStringContentsWithKeys(), Is.EquivalentTo(new [] {
					("%Translit%Hex", "Hex Escape"),
					("%Translit%UnicodeChar", "Unicode Character"),
					("%Translit%UnicodeName", "Unicode Name"),
					("TransliteratorNamePattern", "{0,choice,0#|1#{1}|2#{1} to {2}}") }));
			}
		}

		[TestCase("en_US", ExpectedResult = "[abcdefghijklmnopqrstuvwxyz]")]
		[TestCase("de_DE", ExpectedResult = "[aäbcdefghijklmnoöpqrsßtuüvwxyz]")]
		[TestCase("fr_FR", ExpectedResult = "[aàâæbcçdeéèêëfghiîïjklmnoôœpqrstuùûüvwxyÿz]")]
		public string GetStringByKey(string localeId)
		{
			using (var resourceBundle = new ResourceBundle(null, localeId))
			{
				// Ideally this should be parsed by something that understands UnicodeSet structures
				// Since spaces aren't meaningful in UnicodeSets, we'll take a shortcut and remove them
				return resourceBundle.GetStringByKey("ExemplarCharacters").Replace(" ", "");
			}
		}
	}
}
