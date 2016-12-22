// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Linq;

namespace Icu
{
	/// <summary>
	/// The BreakIterator implements methods for finding the location of
	/// boundaries in text.
	/// When using BreakIterator class, it will iterate over the boundaries
	/// as described here: http://userguide.icu-project.org/boundaryanalysis
	/// for all UBreakIteratorTypes (including UBreakIteratorType.Word).
	/// </summary>
	public abstract class BreakIterator : IDisposable
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
			/// <summary>
			/// Tag value for words that appear to be numbers, lower limit.
			/// </summary>
			NUMBER         = 100,
			/// <summary>
			/// Tag value for words that appear to be numbers, upper limit.
			/// </summary>
			NUMBER_LIMIT   = 200,
			/// <summary>
			/// Tag value for words that contain letters, excluding hiragana,
			/// katakana or ideographic characters, lower limit.
			/// </summary>
			LETTER         = 200,
			/// <summary>
			/// Tag value for words containing letters, upper limit.
			/// </summary>
			LETTER_LIMIT   = 300,
			/// <summary>
			/// Tag value for words containing kana characters, lower limit.
			/// </summary>
			KANA           = 300,
			/// <summary>
			/// Tag value for words containing kana characters, upper limit.
			/// </summary>
			KANA_LIMIT     = 400,
			/// <summary>
			/// Tag value for words containing ideographic characters, lower limit.
			/// </summary>
			IDEO           = 400,
			/// <summary>
			/// Tag value for words containing ideographic characters, upper limit.
			/// </summary>
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
		/// Default RuleStatus vector returns 0.
		/// </summary>
		protected readonly static int[] EmptyRuleStatusVector = new int[] { 0 };
		protected readonly static Locale DefaultLocale = new Locale();

		protected readonly Locale _locale;

		protected int _currentIndex = 0;
		protected TextBoundary[] _textBoundaries = new TextBoundary[0];
		protected string _text;

		/// <summary>
		/// Creates a BreakIterator
		/// </summary>
		/// <param name="locale">The locale to use.</param>
		protected BreakIterator(Locale locale)
		{
			_locale = locale;
		}

		/// <summary>
		/// Gets all of the boundaries for the given text.
		/// </summary>
		public virtual int[] Boundaries
		{
			get { return _textBoundaries.Select(x => x.Offset).ToArray(); }
		}

		/// <summary>
		/// Gets the text being examined by this BreakIterator.
		/// </summary>
		public virtual string Text { get { return _text; } }

		/// <summary>
		/// Determine the most recently-returned text boundary.
		/// Returns <see cref="DONE"/> if there are no boundaries left to return.
		/// </summary>
		public virtual int Current
		{
			get
			{
				if (string.IsNullOrEmpty(Text))
					return 0;

				return _textBoundaries[_currentIndex].Offset;
			}
		}

		/// <summary>
		/// Decrements the iterator and returns the previous boundary.
		/// Returns DONE if the iterator moves past the first boundary.
		/// </summary>
		public virtual int MovePrevious()
		{
			// If we are trying to go into negative indices, return DONE.
			if (_currentIndex == 0)
			{
				return DONE;
			}

			_currentIndex--;

			return _textBoundaries[_currentIndex].Offset;
		}

		/// <summary>
		/// Increments the iterator and returns the next boundary.
		/// Returns DONE if there are no boundaries left to return.
		/// </summary>
		public virtual int MoveNext()
		{
			int nextIndex = _currentIndex + 1;

			// If the next index is going to move this out of boundaries, do
			// not incremement the index.
			if (nextIndex >= Boundaries.Length)
			{
				return DONE;
			}
			else
			{
				_currentIndex = nextIndex;
			}

			return _textBoundaries[_currentIndex].Offset;
		}

		/// <summary>
		/// Sets the iterator to the first boundary and returns it.
		/// Returns DONE if there was no text set.
		/// </summary>
		public virtual int MoveFirst()
		{
			if (string.IsNullOrEmpty(Text))
				return 0;

			_currentIndex = 0;
			return _textBoundaries[_currentIndex].Offset;
		}

		/// <summary>
		/// Sets the iterator to the last boundary and returns the offset into
		/// the text.
		/// Returns DONE if there was no text set.
		/// </summary>
		public virtual int MoveLast()
		{
			if (Text == null)
			{
				return DONE;
			}
			else if (Text.Equals(string.Empty))
			{
				return 0;
			}

			_currentIndex = Boundaries.Length - 1;
			return _textBoundaries[_currentIndex].Offset;
		}

		/// <summary>
		/// Sets the iterator to refer to the first boundary position following
		/// the specified position.
		/// </summary>
		/// <param name="offset">The position from which to begin searching for
		/// a break position.</param>
		/// <returns>The position of the first break after the current position.
		/// The value returned is always greater than offset, or <see cref="BreakIterator.DONE"/>
		/// </returns>
		public virtual int MoveFollowing(int offset)
		{
			// if the offset passed in is already past the end of the text,
			// just return DONE; if it's before the beginning, return the
			// text's starting offset
			if (Text == null || offset >= Text.Length)
			{
				MoveLast();
				return MoveNext();
			}
			else if (offset < 0)
			{
				return MoveFirst();
			}

			if (offset == _textBoundaries[0].Offset)
			{
				_currentIndex = 0;
				return MoveNext();
			}
			else if (offset == _textBoundaries[_textBoundaries.Length - 1].Offset)
			{
				_currentIndex = _textBoundaries.Length - 1;
				return MoveNext();
			}
			else
			{
				int index = 1;
				// We are guaranteed not to leave the array due to range test above
				while (offset >= _textBoundaries[index].Offset)
				{
					index++;
				}

				_currentIndex = index;
				return _textBoundaries[_currentIndex].Offset;
			}
		}

		/// <summary>
		/// Sets the iterator to refer to the last boundary position before the
		/// specified position.
		/// </summary>
		/// <param name="offset">The position to begin searching for a break from.</param>
		/// <returns>The position of the last boundary before the starting
		/// position. The value returned is always smaller than the offset
		/// or the value <see cref="BreakIterator.DONE"/></returns>
		public virtual int MovePreceding(int offset)
		{
			if (Text == null || offset > Text.Length)
			{
				return MoveLast();
			}
			else if (offset < 0)
			{
				return MoveFirst();
			}

			// If the offset is less than the first boundary, return the first
			// boundary. Else if, the offset is greater than the last boundary,
			// return the last boundary.
			// Otherwise, the offset is somewhere in the middle. So we start
			// iterating through the boundaries until we get to a point where
			// the current boundary we are at has a greater offset than the given
			// one. So we return that.
			if (_textBoundaries.Length == 0 || offset == _textBoundaries[0].Offset)
			{
				_currentIndex = 0;
				return DONE;
			}
			else if (offset < _textBoundaries[0].Offset)
			{
				_currentIndex = 0;
			}
			else if (offset > _textBoundaries[_textBoundaries.Length - 1].Offset)
			{
				_currentIndex = _textBoundaries.Length - 1;
			}
			else
			{
				int index = 0;

				while (index < _textBoundaries.Length
					  && offset > _textBoundaries[index].Offset)
				{
					index++;
				}

				index--;

				_currentIndex = index;
			}

			return _textBoundaries[_currentIndex].Offset;
		}

		/// <summary>
		/// Returns true if the specified position is a boundary position and
		/// false, otherwise. In addition, it leaves the iterator pointing to
		/// the first boundary position at or after "offset".
		/// </summary>
		public virtual bool IsBoundary(int offset)
		{
			// When the text is null or empty, there are no boundaries
			// The current offset is already at BreakIterator.DONE.
			if (_textBoundaries.Length == 0)
			{
				return false;
			}

			// the beginning index of the iterator is always a boundary position by definition
			if (offset == 0)
			{
				MoveFirst(); // For side effects on current position, tag values.
				return true;
			}

			int lastOffset = _textBoundaries.Last().Offset;

			if (offset == lastOffset)
			{
				MoveLast(); 
				return true;
			}

			// out-of-range indexes are never boundary positions
			if (offset < 0)
			{
				MoveFirst();
				return false;
			}

			if (offset > lastOffset)
			{
				MoveLast();
				return false;
			}

			var current = MoveFirst();

			while (current != DONE)
			{
				if (current == offset)
					return true;

				if (current > offset)
					return false;

				current = MoveNext();
			}

			return false;
		}

		/// <summary>
		/// Returns the status tag from the break rule that determined the
		/// current position.  For break iterator types that do not support a
		/// rule status, a default value of 0 is returned.
		/// </summary>
		/// <remarks>
		/// For more information, see
		/// http://userguide.icu-project.org/boundaryanalysis#TOC-Rule-Status-Values
		/// </remarks>
		public virtual int GetRuleStatus()
		{
			if (string.IsNullOrEmpty(Text))
				return 0;

			return _textBoundaries[_currentIndex].RuleStatus;
		}

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
		public virtual int[] GetRuleStatusVector()
		{
			if (string.IsNullOrEmpty(Text))
				return EmptyRuleStatusVector;

			return _textBoundaries[_currentIndex].RuleStatusVector;
		}

		/// <summary>
		/// Sets the current text being examined to the given text.
		/// Sets the current index back to the first element.
		/// </summary>
		/// <param name="text">New text.</param>
		/// <exception cref="ArgumentNullException">Thrown when given text is
		/// null. </exception>
		public virtual void SetText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			_text = text;
		}

		/// <summary>
		/// Gets the locale for this BreakIterator.
		/// </summary>
		public Locale Locale { get { return _locale; } }

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
		/// <see cref="BreakIterator.Split(UBreakIteratorType, Locale, string)"/>
		/// or <see cref="BreakIterator.GetWordBoundaries(Locale, string, bool)"/>,
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
		/// <param name="text">The initial text.</param>
		public static BreakIterator CreateLineInstance(Locale locale)
		{
			return new RuleBasedBreakIterator(UBreakIteratorType.LINE, locale);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on sentences for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The initial text.</param>
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
		public static IEnumerable<string> Split(UBreakIteratorType type, Locale locale, string text)
		{
			if (string.IsNullOrEmpty(text))
				yield break;

			foreach (var boundary in GetBoundaries(type, locale.Id, text, includeSpacesAndPunctuation: false))
			{
				yield return text.Substring(boundary.Start, boundary.End - boundary.Start);
			}
		}

		/// <summary>
		/// Gets word boundaries for given text.
		/// </summary>
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
		/// Keeps track of each text boundary by containing its offset,
		/// and the set of rules used to obtain that boundary.
		/// </summary>
		protected struct TextBoundary
		{
			public readonly int Offset;
			public readonly int RuleStatus;
			public readonly int[] RuleStatusVector;

			public TextBoundary(int index, int[] ruleStatusVector)
			{
				Offset = index;
				RuleStatusVector = ruleStatusVector;

				// According to the documentation, in the event that there are
				// multiple values in ubrk_getRuleStatusVec(), a call to
				// ubrk_getRuleStatus() will return the numerically largest
				// from the vector.  We are saving a PInvoke by finding the max
				// value.
				// http://userguide.icu-project.org/boundaryanalysis#TOC-Rule-Status-Values 
				//int status = NativeMethods.ubrk_getRuleStatus(_breakIterator);
				RuleStatus = ruleStatusVector.Max();
			}
		}

		/// <summary>
		/// Dispose of managed/unmanaged resources.
		/// Allow any inheriting classes to dispose of manage
		/// </summary>
		public virtual void Dispose() { }
	}
}
