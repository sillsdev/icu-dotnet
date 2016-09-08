// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Linq;

namespace Icu
{
	public static class BreakIterator
	{
		/// <summary>
		/// The possible types of text boundaries.
		/// </summary>
		public enum UBreakIteratorType
		{
			/// <summary>Character breaks.</summary>
			CHARACTER = 0,
			/// <summary>Word breaks.</summary>
			WORD,
			/// <summary>Line breaks.</summary>
			LINE,
			/// <summary>Sentence breaks.</summary>
			SENTENCE,
			// <summary>Title Case breaks.</summary>
			// obsolete. Use WORD instead.
			//TITLE
		}

		public enum UWordBreak
		{
			/// <summary>
			/// Tag value for "words" that do not fit into any of other categories.
			/// Includes spaces and most punctuation.
			/// </summary>
			NONE           = 0,
			/// <summary>
			/// Upper bound for tags for uncategorized words.
			/// </summary>
			NONE_LIMIT     = 100,
			NUMBER         = 100,
			NUMBER_LIMIT   = 200,
			LETTER         = 200,
			LETTER_LIMIT   = 300,
			KANA           = 300,
			KANA_LIMIT     = 400,
			IDEO           = 400,
			IDEO_LIMIT     = 500,
		}

		public enum ULineBreakTag
		{
			SOFT            = 0,
			SOFT_LIMIT      = 100,
			HARD            = 100,
			HARD_LIMIT      = 200,
		}

		public enum USentenceBreakTag
		{
			TERM       = 0,
			TERM_LIMIT = 100,
			SEP        = 100,
			SEP_LIMIT  = 200,
		}

		/// <summary>
		/// Value indicating all text boundaries have been returned.
		/// </summary>
		public const int DONE = -1;

		/// <summary>
		/// Splits the specified text along the specified type of boundaries. Spaces and punctuations
		/// are not returned.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The text.</param>
		/// <returns>The tokens.</returns>
		public static IEnumerable<string> Split(UBreakIteratorType type, Locale locale, string text)
		{
			return Split(type, locale.Id, text);
		}

		public static IEnumerable<string> Split(UBreakIteratorType type, string locale, string text)
		{
			if (string.IsNullOrEmpty(text))
				yield break;

			foreach (var boundary in GetBoundaries(type, locale, text, includeSpacesAndPunctuation: false))
			{
				yield return text.Substring(boundary.Start, boundary.End - boundary.Start);
			}
		}

		/// <summary>
		/// Gets word boundaries for given text.
		/// </summary>
		/// <param name="includeSpacesAndPunctuation">ICU's UBreakIteratorType.WORD analysis considers
		/// spaces and punctuation as boundaries for words. Set parameter to true if all boundaries
		/// are desired; false otherwise.
		/// For more information: http://userguide.icu-project.org/boundaryanalysis#TOC-Count-the-words-in-a-document-C-only-:
		/// </param>
		public static IEnumerable<Boundary> GetWordBoundaries(Locale locale, string text, bool includeSpacesAndPunctuation)
		{
			return GetWordBoundaries(locale.Id, text, includeSpacesAndPunctuation);
		}

		public static IEnumerable<Boundary> GetWordBoundaries(string locale, string text, bool includeSpacesAndPunctuation)
		{
			return GetBoundaries(UBreakIteratorType.WORD, locale, text, includeSpacesAndPunctuation);
		}

		/// <summary>
		/// Gets the sentence/line/word/character boundaries for the text. Spaces and punctuations
		/// are not returned for UBreakIteratorType.WORD.
		/// </summary>
		public static IEnumerable<Boundary> GetBoundaries(UBreakIteratorType type, Locale locale, string text)
		{
			return GetBoundaries(type, locale.Id, text, false);
		}

		private static IEnumerable<Boundary> GetBoundaries(UBreakIteratorType type, string locale, string text, bool includeSpacesAndPunctuation)
		{
			if (string.IsNullOrEmpty(text))
				yield break;

			ErrorCode err;
			IntPtr bi = NativeMethods.ubrk_open(type, locale, text, text.Length, out err);
			if (err.IsFailure())
				throw new Exception("BreakIterator.Split() failed with code " + err);

			int cur = NativeMethods.ubrk_first(bi);

			while (cur != DONE)
			{
				int next = NativeMethods.ubrk_next(bi);
				int status = NativeMethods.ubrk_getRuleStatus(bi);
				
				if (next == DONE)
				{
					break;
				}

				if (includeSpacesAndPunctuation || AddToken(type, status))
				{
					yield return new Boundary(cur, next);
				}

				cur = next;
			}

			NativeMethods.ubrk_close(bi);
		}

		private static bool AddToken(UBreakIteratorType type, int status)
		{
			switch (type)
			{
				case UBreakIteratorType.CHARACTER:
					return true;
				case UBreakIteratorType.LINE:
				case UBreakIteratorType.SENTENCE:
					return true;
				case UBreakIteratorType.WORD:
					return status < (int)UWordBreak.NONE || status >= (int)UWordBreak.NONE_LIMIT;
			}
			return false;
		}
	}
}
