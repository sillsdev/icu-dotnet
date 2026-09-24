// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using Icu.Collation;
using Icu.Normalization;
using NUnit.Framework;

namespace Icu.Tests
{
	/// <summary>
	/// Tests that using an ICU object that was created before <see cref="Wrapper.Cleanup"/>
	/// reports an <see cref="ObjectDisposedException"/> instead of handing a dangling pointer to
	/// ICU, which killed the process with an AccessViolationException.
	/// </summary>
	[TestFixture]
	public class HandleInvalidationTests
	{
		[TearDown]
		public void TearDown()
		{
			// Every test unloads ICU, so load it again for whatever runs next.
			Wrapper.Init();
		}

		[Test]
		public void Collator_UsedAfterCleanup_Throws()
		{
			using (var collator = new RuleBasedCollator("&b < a"))
			{
				Assert.That(collator.Compare("a", "b"), Is.EqualTo(1));

				Wrapper.Cleanup();

				Assert.That(() => collator.Compare("a", "b"),
					Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		public void Transliterator_UsedAfterCleanup_Throws()
		{
			using (var transliterator = Transliterator.CreateInstance("Any-Latin"))
			{
				Assert.That(transliterator.Transliterate("Ελλάδα"), Is.EqualTo("Elláda"));

				Wrapper.Cleanup();

				Assert.That(() => transliterator.Transliterate("Ελλάδα"),
					Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		[Category("Full ICU")]
		public void BreakIterator_UsedAfterCleanup_Throws()
		{
			using (var breakIterator = new RuleBasedBreakIterator(
				BreakIterator.UBreakIteratorType.WORD, new Locale("en-US")))
			{
				breakIterator.SetText("hello there");
				Assert.That(breakIterator.Boundaries.Length, Is.GreaterThan(0));

				Wrapper.Cleanup();

				Assert.That(() => breakIterator.SetText("something else"),
					Throws.TypeOf<ObjectDisposedException>());
				Assert.That(() => breakIterator.Clone(),
					Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		[Category("Full ICU")]
		public void BiDi_UsedAfterCleanup_Throws()
		{
			using (var biDi = new BiDi())
			{
				biDi.SetPara("abc", 0, null);
				Assert.That(biDi.Direction, Is.EqualTo(BiDi.BiDiDirection.LTR));

				Wrapper.Cleanup();

				Assert.That(() => biDi.Direction, Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		public void RegexMatcher_UsedAfterCleanup_Throws()
		{
			using (var regexMatcher = new RegexMatcher("a+"))
			{
				Assert.That(regexMatcher.Matches("aaa"), Is.True);

				Wrapper.Cleanup();

				Assert.That(() => regexMatcher.Matches("aaa"),
					Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		public void MessageFormatter_UsedAfterCleanup_Throws()
		{
			using (var messageFormatter = new MessageFormatter("{0}", "en-US"))
			{
				Assert.That(messageFormatter.Pattern, Is.EqualTo("{0}"));

				Wrapper.Cleanup();

				Assert.That(() => messageFormatter.Pattern,
					Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		public void ResourceBundle_UsedAfterCleanup_Throws()
		{
			using (var resourceBundle = new ResourceBundle(null, "en"))
			{
				Assert.That(resourceBundle.Name, Is.EqualTo("en"));

				Wrapper.Cleanup();

				Assert.That(() => resourceBundle.Name, Throws.TypeOf<ObjectDisposedException>());
			}
		}

		[Test]
		public void Normalizer_UsedAfterCleanup_Throws()
		{
			var normalizer = Normalizer2.GetNFCInstance();
			Assert.That(normalizer.Normalize("é"), Is.EqualTo("é"));

			Wrapper.Cleanup();

			Assert.That(() => normalizer.Normalize("é"),
				Throws.TypeOf<ObjectDisposedException>());
		}

		[Test]
		public void NullResourceBundle_UsableAfterCleanup()
		{
			Wrapper.Cleanup();

			// A bundle that never pointed at anything can't have gone stale.
			Assert.That(ResourceBundle.Null.IsNull, Is.True);
		}

		[Test]
		public void Dispose_AfterCleanup_DoesNotThrow()
		{
			var collator = new RuleBasedCollator("&b < a");
			var biDi = new BiDi();
			biDi.SetPara("abc", 0, null);

			Wrapper.Cleanup();

			Assert.That(() =>
			{
				collator.Dispose();
				biDi.Dispose();
			}, Throws.Nothing);
		}

		[Test]
		public void ObjectCreatedAfterCleanup_Works()
		{
			Wrapper.Cleanup();
			Wrapper.Init();

			using (var collator = new RuleBasedCollator("&b < a"))
				Assert.That(collator.Compare("a", "b"), Is.EqualTo(1));
		}
	}
}
