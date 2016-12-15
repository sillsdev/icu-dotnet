// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;

namespace Icu
{
	/// <summary>
	/// The BreakIterator implements methods for finding the location of
	/// boundaries in text.
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

		private readonly bool _includeSpacesAndPunctuation;
		private readonly UBreakIteratorType _iteratorType;
		private readonly Locale _locale;

		private int _currentIndex = DONE;
		private bool _disposingValue = false; // To detect redundant calls
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
			: this(iteratorType, locale, text, includeSpacesAndPunctuation: false)
		{ }

		/// <summary>
		/// Creates a BreakIterator with the given BreakIteratorType, Locale
		/// and sets the initial text.
		/// </summary>
		/// <param name="iteratorType">Break type.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="text">Initial text.</param>
		/// <param name="includeSpacesAndPunctuation">
		/// ICU's UBreakIteratorType.WORD analysis considers spaces and
		/// punctuation as boundaries for words. Set parameter to true if all
		/// boundaries are desired; false otherwise.
		/// For more information: http://userguide.icu-project.org/boundaryanalysis#TOC-Count-the-words-in-a-document-C-only-:
		/// </param>
		protected BreakIterator(UBreakIteratorType iteratorType, Locale locale, string text, bool includeSpacesAndPunctuation)
		{
			_includeSpacesAndPunctuation = includeSpacesAndPunctuation;
			_iteratorType = iteratorType;
			_locale = locale;
			SetText(text);
		}

		/// <summary>
		/// Gets all of the Boundaries for the given text.
		/// Returns Boundary[0] if the text was empty or null.
		/// </summary>
		public virtual Boundary[] Boundaries { get; protected set; }

		/// <summary>
		/// Gets the text being examined by this BreakIterator.
		/// </summary>
		public virtual string Text { get { return _text; } }

		/// <summary>
		/// The current Boundary.
		/// Returns null if there are no boundaries left to return.
		/// </summary>
		public virtual Boundary Current
		{
			get
			{
				if (_currentIndex == DONE)
					return default(Boundary);

				return Boundaries[_currentIndex];
			}
		}

		/// <summary>
		/// Decrements the iterator and returns the previous Boundary.
		/// Returns null if the iterator moves past the first Boundary.
		/// </summary>
		public virtual Boundary MovePrevious()
		{
			if (_currentIndex == 0 || _currentIndex == DONE)
			{
				_currentIndex = DONE;
				return default(Boundary);
			}

			_currentIndex--;

			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Increments the iterator and returns the next Boundary.
		/// Returns null if there are no boundaries left to return.
		/// </summary>
		public virtual Boundary MoveNext()
		{
			if (_currentIndex == DONE)
				return default(Boundary);

			_currentIndex++;

			if (_currentIndex >= Boundaries.Length)
			{
				_currentIndex = DONE;
				return default(Boundary);
			}

			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Sets the iterator to the first Boundary and returns it.
		/// Returns null if there was no text set.
		/// </summary>
		public virtual Boundary MoveFirst()
		{
			if (Boundaries.Length == 0)
				return default(Boundary);

			_currentIndex = 0;
			return Boundaries[_currentIndex];
		}

		/// <summary>
		/// Sets the iterator to the last Boundary and returns it.
		/// Returns null if there was no text set.
		/// </summary>
		public virtual Boundary MoveLast()
		{
			if (Boundaries.Length == 0)
				return default(Boundary);

			_currentIndex = Boundaries.Length - 1;
			return Boundaries[_currentIndex];
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
				Boundaries = new Boundary[0];
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

			List<Boundary> boundaries = new List<Boundary>();

			int cur = NativeMethods.ubrk_first(_breakIterator);

			while (cur != DONE)
			{
				int next = NativeMethods.ubrk_next(_breakIterator);
				int status = NativeMethods.ubrk_getRuleStatus(_breakIterator);

				if (next == DONE)
				{
					break;
				}

				if (_includeSpacesAndPunctuation || AddToken(_iteratorType, status))
				{
					boundaries.Add(new Boundary(cur, next));
				}

				cur = next;
			}

			Boundaries = boundaries.ToArray();
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
		/// </summary>
		/// <param name="locale">The locale.</param>
		/// <param name="text">The initial text.</param>
		/// <param name="includeSpacesAndPunctuation">
		/// ICU's UBreakIteratorType.WORD analysis considers spaces and
		/// punctuation as boundaries for words. Set parameter to true if all
		/// boundaries are desired; false otherwise.
		/// For more information: http://userguide.icu-project.org/boundaryanalysis#TOC-Count-the-words-in-a-document-C-only-:
		/// </param>
		public static BreakIterator CreateWordInstance(Locale locale, string text, bool includeSpacesAndPunctuation)
		{
			return new BreakIterator(UBreakIteratorType.WORD, locale, text, includeSpacesAndPunctuation);
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
			using (var breakIterator = new BreakIterator(type, locale, text, includeSpacesAndPunctuation))
			{
				return breakIterator.Boundaries;
			}
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
