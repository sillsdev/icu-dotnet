// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections;
using System.Collections.Generic;
using Icu.BreakIterators;

namespace Icu
{
	/// <summary>
	/// The BreakIterator implements methods for finding the location of
	/// boundaries in text.
	/// When using BreakIterator class, it will iterate over the boundaries
	/// as described here: http://userguide.icu-project.org/boundaryanalysis
	/// for all UBreakIteratorTypes (including UBreakIteratorType.Word).
	/// </summary>
	public abstract class BreakIterator : IDisposable, IEnumerable<string>
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

		/// <summary>
		/// Enum constants for the word break tags returned by <see cref="GetRuleStatus"/>
		/// and <see cref="GetRuleStatusVector"/>.
		/// A range of values is defined for each category of word, to allow for
		/// further subdivisions of a category in future releases.
		/// Applications should check for tag values falling within the range,
		/// rather than for single individual values.
		/// </summary>
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
			NONE_LIMIT     = NUMBER,
			/// <summary>
			/// Tag value for words that appear to be numbers, lower limit.
			/// </summary>
			NUMBER         = 100,
			/// <summary>
			/// Tag value for words that appear to be numbers, upper limit.
			/// </summary>
			NUMBER_LIMIT   = LETTER,
			/// <summary>
			/// Tag value for words that contain letters, excluding hiragana,
			/// katakana or ideographic characters, lower limit.
			/// </summary>
			LETTER         = 200,
			/// <summary>
			/// Tag value for words containing letters, upper limit.
			/// </summary>
			LETTER_LIMIT   = KANA,
			/// <summary>
			/// Tag value for words containing kana characters, lower limit.
			/// </summary>
			KANA           = 300,
			/// <summary>
			/// Tag value for words containing kana characters, upper limit.
			/// </summary>
			KANA_LIMIT     = IDEO,
			/// <summary>
			/// Tag value for words containing ideographic characters, lower limit.
			/// </summary>
			IDEO           = 400,
			/// <summary>
			/// Tag value for words containing ideographic characters, upper limit.
			/// </summary>
			IDEO_LIMIT     = 500,
		}

		/// <summary>
		/// Enum constants for the line break tags returned by <see cref="GetRuleStatus"/>
		/// and <see cref="GetRuleStatusVector"/>.
		/// A range of values is defined for each category of word, to allow for
		/// further subdivisions of a category in future releases.
		/// Applications should check for tag values falling within the range,
		/// rather than for single individual values.
		/// </summary>
		public enum ULineBreakTag
		{
			/// <summary>
			/// Tag value for soft line breaks, positions at which a line break
			/// is acceptable but not required
			/// </summary>
			SOFT            = 0,
			/// <summary>
			/// Upper bound for soft line breaks.
			/// </summary>
			SOFT_LIMIT      = HARD,
			/// <summary>
			/// Tag value for a hard, or mandatory line break
			/// </summary>
			HARD            = 100,
			/// <summary>
			/// Upper bound for hard line breaks.
			/// </summary>
			HARD_LIMIT      = 200,
		}

		/// <summary>
		/// Enum constants for the sentence break tags returned by <see cref="GetRuleStatus"/>
		/// and <see cref="GetRuleStatusVector"/>.
		/// A range of values is defined for each category of sentence, to allow
		/// for further subdivisions of a category in future releases.
		/// Applications should check for tag values falling within the range,
		/// rather than for single individual values.
		/// </summary>
		public enum USentenceBreakTag
		{
			/// <summary>
			/// Tag value for sentences ending with a sentence terminator
			/// ('.', '?', '!', etc.) character, possibly followed by a
			/// hard separator (CR, LF, PS, etc.)
			/// </summary>
			TERM       = 0,
			/// <summary>
			/// Upper bound for tags for sentences ended by sentence terminators.
			/// </summary>
			TERM_LIMIT = SEP,
			/// <summary>
			/// Tag value for sentences that do not contain an ending
			/// sentence terminator ('.', '?', '!', etc.) character, but
			/// are ended only by a hard separator (CR, LF, PS, etc.) or end of input.
			/// </summary>
			SEP        = 100,
			/// <summary>
			/// Upper bound for tags for sentences ended by a separator.
			/// </summary>
			SEP_LIMIT  = 200,
		}

		/// <summary>
		/// Value indicating all text boundaries have been returned.
		/// </summary>
		public const int DONE = -1;

		/// <summary>
		/// Gets all of the boundaries for the given text.
		/// </summary>
		public abstract int[] Boundaries { get; }

		/// <summary>
		/// Gets the text being examined by this BreakIterator.
		/// </summary>
		public abstract string Text { get; }

		/// <summary>
		/// Determine the most recently-returned text boundary.
		/// Returns <see cref="DONE"/> if there are no boundaries left to return.
		/// </summary>
		public abstract int Current { get; }

		/// <summary>
		/// Decrements the iterator and returns the previous boundary.
		/// Returns DONE if the iterator moves past the first boundary.
		/// </summary>
		public abstract int MovePrevious();

		/// <summary>
		/// Increments the iterator and returns the next boundary.
		/// Returns DONE if there are no boundaries left to return.
		/// </summary>
		public abstract int MoveNext();

		/// <summary>
		/// Sets the iterator to the first boundary and returns it.
		/// Returns DONE if there was no text set.
		/// </summary>
		public abstract int MoveFirst();

		/// <summary>
		/// Sets the iterator to the last boundary and returns the offset into
		/// the text.
		/// Returns DONE if there was no text set.
		/// </summary>
		public abstract int MoveLast();

		/// <summary>
		/// Sets the iterator to refer to the first boundary position following
		/// the specified position.
		/// </summary>
		/// <param name="offset">The position from which to begin searching for
		/// a break position.</param>
		/// <returns>The position of the first break after the current position.
		/// The value returned is always greater than offset, or <see cref="BreakIterator.DONE"/>
		/// </returns>
		public abstract int MoveFollowing(int offset);

		/// <summary>
		/// Sets the iterator to refer to the last boundary position before the
		/// specified position.
		/// </summary>
		/// <param name="offset">The position to begin searching for a break from.</param>
		/// <returns>The position of the last boundary before the starting
		/// position. The value returned is always smaller than the offset
		/// or the value <see cref="BreakIterator.DONE"/></returns>
		public abstract int MovePreceding(int offset);

		/// <summary>
		/// Returns true if the specified position is a boundary position and
		/// false, otherwise. In addition, it leaves the iterator pointing to
		/// the first boundary position at or after "offset".
		/// </summary>
		public abstract bool IsBoundary(int offset);

		/// <summary>
		/// Returns the status tag from the break rule that determined the
		/// current position.  For break iterator types that do not support a
		/// rule status, a default value of 0 is returned.
		/// </summary>
		/// <remarks>
		/// For more information, see
		/// http://userguide.icu-project.org/boundaryanalysis#TOC-Rule-Status-Values
		/// </remarks>
		public abstract int GetRuleStatus();

		/// <summary>
		/// Get the statuses from the break rules that determined the most
		/// recently returned break position.
		///
		/// The values appear in the rule source within brackets, {123}, for
		/// example.  The default status value for rules that do not explicitly
		/// provide one is zero.
		///
		/// For word break iterators, the possible values are defined in enum
		/// <see cref="BreakIterator.UWordBreak"/>.
		/// </summary>
		/// <remarks>
		/// For more information, see
		/// http://userguide.icu-project.org/boundaryanalysis#TOC-Rule-Status-Values
		/// </remarks>
		public abstract int[] GetRuleStatusVector();

		/// <summary>
		/// Thread safe cloning operation
		/// </summary>
		/// <typeparam name="T">The specific subclass</typeparam>
		/// <returns>A new clone</returns>
		public T Clone<T>() where T : BreakIterator, new()
		{
			return Clone() as T;
		}

		/// <summary>
		/// Thread safe cloning operation
		/// </summary>
		/// <returns>A new clone</returns>
		public abstract BreakIterator Clone();

		/// <summary>
		/// Sets the current text being examined to the given text.
		/// Sets the current index back to the first element.
		/// </summary>
		/// <param name="text">New text.</param>
		/// <exception cref="ArgumentNullException">Thrown when given text is
		/// null. </exception>
		public abstract void SetText(string text);

		/// <summary>
		/// Gets the locale for this BreakIterator.
		/// </summary>
		public abstract Locale Locale { get; }

		/// <summary>
		/// Creates a BreakIterator that splits on characters for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		public static BreakIterator CreateCharacterInstance(Locale locale)
		{
			return new RuleBasedBreakIterator(UBreakIteratorType.CHARACTER, locale);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on words for the given locale.
		/// It iterates over boundaries as described in the "Word Boundary"
		/// section http://userguide.icu-project.org/boundaryanalysis.
		/// If you want to ignore spaces and punctuation, consider using:
		/// <see cref="BreakIterator.Split(UBreakIteratorType, Icu.Locale, string)"/>
		/// or <see cref="BreakIterator.GetWordBoundaries(Icu.Locale, string, bool)"/>,
		/// </summary>
		/// <param name="locale">The locale.</param>
		public static BreakIterator CreateWordInstance(Locale locale)
		{
			return new RuleBasedBreakIterator(UBreakIteratorType.WORD, locale);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on lines for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		public static BreakIterator CreateLineInstance(Locale locale)
		{
			return new RuleBasedBreakIterator(UBreakIteratorType.LINE, locale);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on sentences for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		public static BreakIterator CreateSentenceInstance(Locale locale)
		{
			return new RuleBasedBreakIterator(UBreakIteratorType.SENTENCE, locale);
		}

		/// <summary>
		/// Splits the specified text along the specified type of boundaries.
		/// Spaces and punctuations are not returned.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The text.</param>
		/// <returns>The tokens.</returns>
		/// <remarks>
		/// If you want to get tokens for spaces and punctuations (as described
		/// in http://userguide.icu-project.org/boundaryanalysis), consider using
		/// <see cref="BreakIterator.GetEnumerator()"/>.
		/// </remarks>
		public static IEnumerable<string> Split(UBreakIteratorType type, string locale, string text)
		{
			return Split(type, new Locale(locale), text);
		}

		/// <summary>
		/// Splits the specified text along the specified type of boundaries.
		/// Spaces and punctuations are not returned.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The text.</param>
		/// <returns>The tokens.</returns>
		/// <remarks>
		/// If you want to get tokens for spaces and punctuations (as described
		/// in http://userguide.icu-project.org/boundaryanalysis), consider using
		/// <see cref="BreakIterator.GetEnumerator()"/>.
		/// </remarks>
		public static IEnumerable<string> Split(UBreakIteratorType type, Locale locale, string text)
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
		/// <param name="locale">The locale to use</param>
		/// <param name="text">The input text</param>
		/// <param name="includeSpacesAndPunctuation">
		/// ICU's UBreakIteratorType.WORD analysis considers spaces and
		/// punctuation as boundaries for words. Set parameter to true if all
		/// boundaries are desired; false otherwise.
		/// For more information: http://userguide.icu-project.org/boundaryanalysis#TOC-Count-the-words-in-a-document-C-only-:
		/// </param>
		public static IEnumerable<Boundary> GetWordBoundaries(string locale, string text, bool includeSpacesAndPunctuation)
		{
			return GetWordBoundaries(new Locale(locale), text, includeSpacesAndPunctuation);
		}

		/// <summary>
		/// Gets word boundaries for given text.
		/// </summary>
		/// <param name="locale">The locale to use</param>
		/// <param name="text">The input text</param>
		/// <param name="includeSpacesAndPunctuation">
		/// ICU's UBreakIteratorType.WORD analysis considers spaces and
		/// punctuation as boundaries for words. Set parameter to true if all
		/// boundaries are desired; false otherwise.
		/// For more information: http://userguide.icu-project.org/boundaryanalysis#TOC-Count-the-words-in-a-document-C-only-:
		/// </param>
		public static IEnumerable<Boundary> GetWordBoundaries(Locale locale, string text, bool includeSpacesAndPunctuation)
		{
			return GetBoundaries(UBreakIteratorType.WORD, locale, text, includeSpacesAndPunctuation);
		}

		/// <summary>
		/// Gets the sentence/line/word/character boundaries for the text. Spaces and punctuations
		/// are not returned for UBreakIteratorType.WORD.
		/// </summary>
		public static IEnumerable<Boundary> GetBoundaries(UBreakIteratorType type, Locale locale, string text)
		{
			return GetBoundaries(type, locale, text, false);
		}

		private static IEnumerable<Boundary> GetBoundaries(UBreakIteratorType type, Locale locale, string text, bool includeSpacesAndPunctuation)
		{
			List<Boundary> boundaries = new List<Boundary>();

			using (var breakIterator = new RuleBasedBreakIterator(type, locale))
			{
				breakIterator.SetText(text);

				int current = breakIterator.Current;

				while (current != DONE)
				{
					int next = breakIterator.MoveNext();
					int status = breakIterator.GetRuleStatus();

					if (next == DONE)
					{
						break;
					}

					if (includeSpacesAndPunctuation || AddToken(type, status))
					{
						boundaries.Add(new Boundary(current, next));
					}

					current = next;
				}
			}

			return boundaries;
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

		/// <summary>
		/// Dispose of managed/unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by BreakIterator.
		/// </summary>
		/// <param name="disposing">true to release managed and unmanaged
		/// resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing) { }

		/// <summary>
		/// Retrieves an object that can iterate through the individual segments of
		/// the text of this BreakIterator.
		/// It iterates over boundaries as described in
		/// http://userguide.icu-project.org/boundaryanalysis.
		/// If you want to ignore spaces and punctuation, consider using:
		/// <see cref="BreakIterator.Split(UBreakIteratorType, Icu.Locale, string)"/>
		/// or <see cref="BreakIterator.GetWordBoundaries(Icu.Locale, string, bool)"/>,
		/// </summary>
		public IEnumerator<string> GetEnumerator()
		{
			return new BreakEnumerator(this);
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
