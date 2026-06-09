// Copyright (c) 2018-2026 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class TransliteratorTests
	{
		private class TestableTraceListener : TraceListener
		{
			public StringBuilder _output = new StringBuilder();

			public override void Write(string message)
			{
				_output.Append(message);
			}

			public override void WriteLine(string message)
			{
				_output.AppendLine(message);
			}
		}

		private Transliterator _trans;

		[TearDown]
		public void TearDown()
		{
			_trans?.Dispose();
			_trans = null;
		}

		[Test]
		public void GetIdsAndNames()
		{
			Assert.That(Transliterator.GetIdsAndNames(), Does.Contain(("Arabic-Latin", "Arabic to Latin")));
		}

		[Test]
		public void GetAvailableIds()
		{
			Assert.That(Transliterator.GetAvailableIds(), Does.Contain("Any-Accents"));
		}

		[Test]
		public void GetDisplayName()
		{
			Assert.That(Transliterator.GetDisplayName("Armenian-Latin", "de_DE"),
				Is.EqualTo("Armenian to Latin"));
		}

		[TestCase("Any-Latin", TestName = "OpenSingleId")]
		[TestCase("Any-Latin; Latin-ASCII", TestName = "OpenCompoundId")]
		public void CreateInstance(string id)
		{
			Assert.That(() => _trans = Transliterator.CreateInstance(id), Throws.Nothing);
			Assert.That(_trans, Is.Not.Null);
		}

		[TestCase(1)]
		[TestCase(3)]
		public void Transliterate_CompoundTransliterateSameLength(int multiplier)
		{
			const string source = @"Κοντογιαννάτος, Βασίλης";
			const string target = @"Kontogiannatos, Basiles";

			using (var traceListener = new TestableTraceListener())
			{
				Trace.Listeners.Add(traceListener);
				_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
				Assert.That(_trans.Transliterate(source, multiplier), Is.EqualTo(target));
				Assert.That(traceListener._output.ToString(), Is.EqualTo(""));
			}
		}

		[Test]
		public void Transliterate_CompoundTransliterateLonger()
		{
			const string source = @"김, 국삼";
			const string target = @"gim, gugsam";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(_trans.Transliterate(source), Is.EqualTo(target));
		}

		[TestCase(-1)]
		[TestCase(0)]
		public void Transliterate_InvalidMultiplier(int multiplier)
		{
			const string source = @"김, 국삼";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(() => _trans.Transliterate(source, multiplier), Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void Transliterate_Overflow()
		{
			const string source = @"김, 국삼";
			const string target = @"gim, gugsam";

			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(_trans.Transliterate(source, 1), Is.EqualTo(target));
		}

		[Test]
		public void Transliterate_HighExpansionChar_DefaultMultiplier()
		{
			// U+FDFA (ﷺ) expands to many Latin chars (e.g., "ṣly̱ ạllh ʿlyh wslm", 19 chars in
			// ICU 62.1). A multiplier of 3 gives only 3 UChars, so the retry path must kick in.
			// Exact output is ICU-version-specific.
			_trans = Transliterator.CreateInstance("Any-Latn");
			var result = _trans.Transliterate("ﷺ");
			Assert.That(result.Length, Is.GreaterThan(15));
		}

		[Test]
		public void Transliterate_MultipleHighExpansionChars_SmallMultiplier()
		{
			// "ﷺﷺ" = U+FDFA U+FDFA; each expands to many Latin chars (see above test comment).
			// Setting multiplier=1 forces the retry path.
			_trans = Transliterator.CreateInstance("Any-Latn");
			var result = _trans.Transliterate("ﷺﷺ", 1);
			Assert.That(result.Length, Is.GreaterThan(30));
		}

		[Test]
		public void Transliterate_EmptyString()
		{
			_trans = Transliterator.CreateInstance("Any-Latin; Latin-ASCII");
			Assert.That(_trans.Transliterate(string.Empty), Is.EqualTo(string.Empty));
		}
	}
}
