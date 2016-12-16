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
	public class BreakIterator : IDisposable
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

		private readonly UBreakIteratorType _iteratorType;
		private readonly Locale _locale;

		private int _currentIndex = DONE;
		private bool _disposingValue = false; // To detect redundant calls
		private int[] _ruleBreakStatuses = new int[0];
		private string _text;

		protected internal IntPtr _breakIterator = IntPtr.Zero;

		/// <summary>
		/// Creates a BreakIterator with the given BreakIteratorType, Locale
		/// and sets the initial text.
		/// </summary>
		/// <param name="iteratorType">Break type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">Initial text.</param>
		/// <remarks>
		/// If iterator type is UBreakIteratorType.WORD, it will NOT include
		/// spaces and punctuation as boundaries for words.  If this is 
		/// desired <see cref="BreakIterator(UBreakIteratorType, Locale, string, bool)"/>.
		/// </remarks>
		protected BreakIterator(UBreakIteratorType iteratorType, Locale locale, string text)
		{
			_iteratorType = iteratorType;
			_locale = locale;
			SetText(text);
		}

		/// <summary>
		/// Gets all of the boundaries for the given text.
		/// </summary>
		public virtual int[] Boundaries { get; protected set; }

		/// <summary>
		/// Gets the text being examined by this BreakIterator.
		/// </summary>
		public virtual string Text { get { return _text; } }

		/// <summary>
		/// The current Boundary.
		/// Returns <see cref="DONE"/> if there are no boundaries left to return.
		/// </summary>
		public virtual int Current
		{
			get
			{
				if (_currentIndex == DONE)
					return DONE;

				return Boundaries[_currentIndex];
			}
		}

		/// <summary>
		/// Decrements the iterator and returns the previous Boundary.
		/// Returns null if the iterator moves past the first Boundary.
		/// </summary>
		public virtual int MovePrevious()
		{
			if (_currentIndex == 0)
				_currentIndex = DONE;

			if (_currentIndex == DONE)
				return DONE;

			_currentIndex--;

			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Increments the iterator and returns the next Boundary.
		/// Returns null if there are no boundaries left to return.
		/// </summary>
		public virtual int MoveNext()
		{
			if (_currentIndex == DONE)
				return DONE;

			_currentIndex++;

			if (_currentIndex >= Boundaries.Length)
			{
				_currentIndex = DONE;
				return DONE;
			}

			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Sets the iterator to the first Boundary and returns it.
		/// Returns null if there was no text set.
		/// </summary>
		public virtual int MoveFirst()
		{
			if (Boundaries.Length == 0)
				return DONE;

			_currentIndex = 0;
			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Sets the iterator to the last Boundary and returns it.
		/// Returns null if there was no text set.
		/// </summary>
		public virtual int MoveLast()
		{
			if (Boundaries.Length == 0)
				return DONE;

			_currentIndex = Boundaries.Length - 1;
			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Returns the status tag from the break rule that determined the
		/// most recently returned break position.  For break iterator types
		/// that do not support a rule status, a default value of 0 is returned.
		/// </summary>
		public virtual int GetRuleStatus()
		{
			if (_currentIndex == DONE)
				return 0;

			return _ruleBreakStatuses[_currentIndex];
		}

		/// <summary>
		/// Sets the current text being examined to the given text.
		/// Sets the current index back to the first element.
		/// </summary>
		/// <param name="text">New text.</param>
		public virtual void SetText(string text)
		{
			_text = text;

			if (string.IsNullOrEmpty(Text))
			{
				Boundaries = new int[0];
				_currentIndex = DONE;
				return;
			}

			ErrorCode err;

			if (_breakIterator == IntPtr.Zero)
			{
				_breakIterator = NativeMethods.ubrk_open(_iteratorType, _locale.Id, Text, Text.Length, out err);
			}
			else
			{
				NativeMethods.ubrk_setText(_breakIterator, Text, Text.Length, out err);
			}

			if (err.IsFailure())
				throw new Exception("BreakIterator.Split() failed with code " + err);

			//List<Boundary> boundaries = new List<Boundary>();
			List<int> boundaries = new List<int>();
			List<int> ruleBreakStatuses = new List<int>();

			int cur = NativeMethods.ubrk_first(_breakIterator);
			int status = NativeMethods.ubrk_getRuleStatus(_breakIterator);

			if (cur != DONE)
			{
				boundaries.Add(cur);
				ruleBreakStatuses.Add(status);
			}

			while (cur != DONE)
			{
				int next = NativeMethods.ubrk_next(_breakIterator);
				status = NativeMethods.ubrk_getRuleStatus(_breakIterator);

				if (next == DONE)
				{
					break;
				}

				boundaries.Add(next);
				ruleBreakStatuses.Add(status);
				
				cur = next;
			}

			Boundaries = boundaries.ToArray();
			_ruleBreakStatuses = ruleBreakStatuses.ToArray();
			_currentIndex = 0;
		}

		/// <summary>
		/// Gets the locale for this BreakIterator.
		/// </summary>
		public Locale Locale { get { return _locale; } }

		/// <summary>
		/// Creates a BreakIterator that splits on characters for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The initial text.</param>
		public static BreakIterator CreateCharacterInstance(Locale locale, string text)
		{
			return new BreakIterator(UBreakIteratorType.CHARACTER, locale, text);
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
		/// <param name="text">The initial text.</param>
		public static BreakIterator CreateWordInstance(Locale locale, string text)
		{
			return new BreakIterator(UBreakIteratorType.WORD, locale, text);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on lines for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The initial text.</param>
		public static BreakIterator CreateLineInstance(Locale locale, string text)
		{
			return new BreakIterator(UBreakIteratorType.LINE, locale, text);
		}

		/// <summary>
		/// Creates a BreakIterator that splits on sentences for the given locale.
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The initial text.</param>
		public static BreakIterator CreateSentenceInstance(Locale locale, string text)
		{
			return new BreakIterator(UBreakIteratorType.SENTENCE, locale, text);
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

			using (var breakIterator = new BreakIterator(type, locale, text))
			{
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

		#region IDisposable Support

		/// <summary>
		/// Implementing IDisposable pattern to properly release unmanaged resources. 
		/// See https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.110).aspx
		/// and https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.100).aspx
		/// for more information.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposingValue)
			{
				if (disposing)
				{
					// Dispose managed state (managed objects), if any.
				}

				if (_breakIterator != IntPtr.Zero)
				{
					NativeMethods.ubrk_close(_breakIterator);
					_breakIterator = IntPtr.Zero;
				}

				_disposingValue = true;
			}
		}

		~BreakIterator()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
