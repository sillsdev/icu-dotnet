// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Linq;

namespace Icu
{
	/// <summary>
	/// A subclass of BreakIterator whose behavior is specified using a list of rules.
	/// Instances of this class are most commonly created by the factory methods
	/// <see cref="BreakIterator.CreateWordInstance(Icu.Locale)"/>,
	/// <see cref="BreakIterator.CreateLineInstance(Icu.Locale)"/>, etc.
	/// and then used via the abstract API in class BreakIterator
	/// </summary>
	public class RuleBasedBreakIterator : BreakIterator
	{
		private readonly UBreakIteratorType _iteratorType;
		/// <summary>
		/// Sets the rules this break iterator uses
		/// </summary>
		protected string Rules;
		private readonly Locale _locale = DefaultLocale;

		private bool _disposingValue; // To detect redundant calls
		private IntPtr _breakIterator = IntPtr.Zero;
		private string _text;
		private int _currentIndex;
		private TextBoundary[] _textBoundaries = new TextBoundary[0];

		/// <summary>
		/// Default RuleStatus vector returns 0.
		/// </summary>
		protected static readonly int[] EmptyRuleStatusVector = new int[] { 0 };
		/// <summary>
		/// The default locale.
		/// </summary>
		protected static readonly Locale DefaultLocale = new Locale();

		/// <summary>
		/// Creates a BreakIterator with the given BreakIteratorType and Locale.
		/// </summary>
		/// <param name="iteratorType">Break type.</param>
		/// <param name="locale">The locale.</param>
		/// <remarks>
		/// If iterator type is UBreakIteratorType.WORD, it will include
		/// spaces and punctuation as boundaries for words.  If this is
		/// not desired <see cref="BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType, Icu.Locale, string, bool)"/>.
		/// </remarks>
		public RuleBasedBreakIterator(UBreakIteratorType iteratorType, Locale locale)
		{
			_locale = locale;
			_iteratorType = iteratorType;
		}

		/// <summary>
		/// Creates a RuleBasedBreakIterator with the given rules.
		/// </summary>
		public RuleBasedBreakIterator(string rules)
		{
			Rules = rules;
		}

		/// <summary>
		/// Creates a copy of the given RuleBasedBreakIterator
		/// </summary>
		/// <param name="bi">break iterator</param>
		/// <exception cref="Exception">Throws an exception if we get an error cloning the native
		/// break iterator</exception>
		private RuleBasedBreakIterator(RuleBasedBreakIterator bi)
		{
			_iteratorType = bi._iteratorType;
			Rules = bi.Rules;
			_locale = bi._locale;
			_text = bi._text;
			_currentIndex = bi._currentIndex;
			_textBoundaries = new TextBoundary[bi._textBoundaries.Length];
			bi._textBoundaries.CopyTo(_textBoundaries, 0);

			if (bi._breakIterator == IntPtr.Zero)
				return;

			ErrorCode errorCode;
			_breakIterator = NativeMethods.ubrk_safeClone(bi._breakIterator, IntPtr.Zero, IntPtr.Zero, out errorCode);

			if (errorCode.IsFailure())
				throw new Exception($"BreakIterator.ubrk_safeClone() failed with code {errorCode}");
		}

		/// <summary>
		/// Clones this RuleBasedBreakIterator
		/// </summary>
		public override BreakIterator Clone()
		{
			return new RuleBasedBreakIterator(this);
		}

		/// <summary>
		/// Gets all of the boundaries for the given text.
		/// </summary>
		public override int[] Boundaries
		{
			get { return _textBoundaries.Select(x => x.Offset).ToArray(); }
		}

		/// <summary>
		/// Gets the locale for this BreakIterator.
		/// </summary>
		public override Locale Locale => _locale;

		/// <summary>
		/// Gets the text being examined by this BreakIterator.
		/// </summary>
		public override string Text => _text;

		/// <summary>
		/// Determine the most recently-returned text boundary.
		/// Returns <see cref="BreakIterator.DONE"/> if there are no boundaries left to return.
		/// </summary>
		public override int Current
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
		public override int MovePrevious()
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
		public override int MoveNext()
		{
			int nextIndex = _currentIndex + 1;

			// If the next index is going to move this out of boundaries, do
			// not increment the index.
			if (nextIndex >= _textBoundaries.Length)
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
		public override int MoveFirst()
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
		public override int MoveLast()
		{
			if (Text == null)
			{
				return DONE;
			}
			else if (Text.Equals(string.Empty))
			{
				return 0;
			}

			_currentIndex = _textBoundaries.Length - 1;
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
		public override int MoveFollowing(int offset)
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
		public override int MovePreceding(int offset)
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
		public override bool IsBoundary(int offset)
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
		public override int GetRuleStatus()
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
		public override int[] GetRuleStatusVector()
		{
			if (string.IsNullOrEmpty(Text))
				return EmptyRuleStatusVector;

			return _textBoundaries[_currentIndex].RuleStatusVector;
		}

		/// <summary>
		/// Sets an existing iterator to point to a new piece of text.
		/// </summary>
		/// <param name="text">New text</param>
		public override void SetText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			_text = text;

			if (string.IsNullOrEmpty(Text))
			{
				_textBoundaries = new TextBoundary[0];
				_currentIndex = 0;
				return;
			}

			if (_breakIterator == IntPtr.Zero)
			{
				InitializeBreakIterator();
			}

			unsafe
			{
				// Need to lock down the pointer to _text between calls to the unmanaged ICU
				// code in case the GC kicks in and moves it.
				fixed (char* p = _text)
				{
					ErrorCode err;

					NativeMethods.ubrk_setText(_breakIterator, Text, Text.Length, out err);

					if (err.IsFailure())
						throw new Exception(
							"BreakIterator.ubrk_setText() failed with code " + err);

					List<TextBoundary> textBoundaries = new List<TextBoundary>();

					// Start at the the beginning of the text and iterate until all
					// of the boundaries are consumed.
					int cur = NativeMethods.ubrk_first(_breakIterator);

					TextBoundary textBoundary;

					if (!TryGetTextBoundaryFromOffset(cur, out textBoundary))
						return;

					textBoundaries.Add(textBoundary);

					while (cur != DONE)
					{
						int next = NativeMethods.ubrk_next(_breakIterator);

						if (!TryGetTextBoundaryFromOffset(next, out textBoundary))
							break;

						textBoundaries.Add(textBoundary);
						cur = next;
					}

					_textBoundaries = textBoundaries.ToArray();
					_currentIndex = 0;
				}
			}
		}

		/// <summary>
		/// Checks if the offset is valid, gets the RuleStatus and
		/// RuleStatusVector for the offset and returns that TextBoundary.
		/// </summary>
		/// <param name="offset">Offset to check in text.</param>
		/// <param name="textBoundary">TextBoundary for the given offset.</param>
		/// <returns>true if the offset is valid; false otherwise.</returns>
		private bool TryGetTextBoundaryFromOffset(int offset, out TextBoundary textBoundary)
		{
			textBoundary = default(TextBoundary);

			if (offset == DONE)
				return false;

			const int length = 128;

			int[] vector = new int[length];

			ErrorCode errorCode;
			int actualLen = NativeMethods.ubrk_getRuleStatusVec(_breakIterator, vector, length, out errorCode);

			if (errorCode.IsFailure())
				throw new Exception("BreakIterator.GetRuleStatusVector failed! " + errorCode);

			if (actualLen > length)
			{
				vector = new int[actualLen];
				NativeMethods.ubrk_getRuleStatusVec(_breakIterator, vector, vector.Length, out errorCode);

				if (errorCode.IsFailure())
					throw new Exception("BreakIterator.GetRuleStatusVector failed! " + errorCode);
			}

			int[] ruleStatuses;

			// Constrain the size of the array to actual number of elements
			// that were returned.
			if (actualLen < vector.Length)
			{
				ruleStatuses = new int[actualLen];
				Array.Copy(vector, ruleStatuses, actualLen);
			}
			else
			{
				ruleStatuses = vector;
			}

			textBoundary = new TextBoundary(offset, ruleStatuses);

			return true;
		}

		/// <summary>
		/// Initialises or returns unmanaged pointer to BreakIterator
		/// </summary>
		/// <returns></returns>
		private void InitializeBreakIterator()
		{
			if (_breakIterator != IntPtr.Zero)
			{
				return;
			}

			if (Rules != null)
			{
				ErrorCode errorCode;
				ParseError parseError;

				_breakIterator = NativeMethods.ubrk_openRules(Rules, Rules.Length, Text, Text.Length, out parseError, out errorCode);

				if (errorCode.IsFailure())
				{
					throw new ParseErrorException("Couldn't create RuleBasedBreakIterator with the given rules!", parseError, Rules);
				}
			}
			else
			{
				ErrorCode errorCode;
				_breakIterator = NativeMethods.ubrk_open(_iteratorType, _locale.Id, Text, Text.Length, out errorCode);
				if (errorCode.IsFailure())
				{
					throw new InvalidOperationException(
						string.Format("Could not create a BreakIterator with locale [{0}], BreakIteratorType [{1}] and Text [{2}]", _locale.Id, _iteratorType, Text));
				}
			}
		}

		/// <summary>
		/// If the RuleBasedBreakIterator was created using custom rules,
		/// returns those rules as its string interpretation, otherwise,
		/// returns the locale and BreakIteratorType.
		/// </summary>
		public override string ToString()
		{
			return Rules ?? $"Locale: {_locale}, BreakIteratorType: {_iteratorType}";
		}

		#region IDisposable Support

		/// <summary>
		/// Implementing IDisposable pattern to properly release unmanaged resources.
		/// See https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.110).aspx
		/// and https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.100).aspx
		/// for more information.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
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

		/// <summary>
		/// Disposes of all unmanaged resources used by RulesBasedBreakIterator
		/// </summary>
		~RuleBasedBreakIterator()
		{
			Dispose(false);
		}

		#endregion

		/// <summary>
		/// Keeps track of each text boundary by containing its offset,
		/// and the set of rules used to obtain that boundary.
		/// </summary>
		protected struct TextBoundary
		{
			/// <summary>
			/// Gets the offset within the text of the boundary.
			/// </summary>
			public readonly int Offset;
			/// <summary>
			/// Gets the rule that was used to determine the boundary. Possible
			/// values are from: <see cref="BreakIterator.ULineBreakTag"/>,
			/// <see cref="BreakIterator.UWordBreak"/>, or <see cref="BreakIterator.USentenceBreakTag"/>
			/// </summary>
			/// <remarks>
			/// If there are more than one rule that determined the boundary,
			/// returns the numerically largest value from <see cref="RuleStatusVector"/>.
			/// More information: http://userguide.icu-project.org/boundaryanalysis#TOC-Rule-Status-Values
			/// </remarks>
			public readonly int RuleStatus;
			/// <summary>
			/// Gets the set of rules used to determine the boundary. Possible
			/// values are from: <see cref="BreakIterator.ULineBreakTag"/>,
			/// <see cref="BreakIterator.UWordBreak"/>, or <see cref="BreakIterator.USentenceBreakTag"/>
			/// </summary>
			public readonly int[] RuleStatusVector;

			/// <summary>
			/// Creates a text boundary with the given offset and rule status vector.
			/// </summary>
			/// <param name="index">Offset in text of the boundary.</param>
			/// <param name="ruleStatusVector">Rule status vector of boundary.</param>
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
	}
}
