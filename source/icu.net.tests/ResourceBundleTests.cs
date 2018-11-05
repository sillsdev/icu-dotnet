// Copyright (c) 2018 SIL International
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

		[TestCase("en_US", ExpectedResult = "[a b c d e f g h i j k l m n o p q r s t u v w x y z]")]
		[TestCase("de_DE", ExpectedResult = "[a ä b c d e f g h i j k l m n o ö p q r s ß t u ü v w x y z]")]
		[TestCase("fr_FR", ExpectedResult = "[a à â æ b c ç d e é è ê ë f g h i î ï j k l m n o ô œ p q r s t u ù û ü v w x y ÿ z]")]
		public string GetStringByKey(string localeId)
		{
			using (var resourceBundle = new ResourceBundle(null, localeId))
			{
				return resourceBundle.GetStringByKey("ExemplarCharacters");
			}
		}
	}
}
