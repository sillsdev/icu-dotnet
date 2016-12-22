using System;
using System.Collections.Generic;

namespace Icu
{
	/// <summary>
	/// A subclass of BreakIterator whose behavior is specified using a list of rules.
	/// Instances of this class are most commonly created by the factory methods of
	/// <see cref="BreakIterator.CreateWordInstance(Locale, string)"/>, 
	/// <see cref="BreakIterator.CreateLineInstance(Locale, string)"/> etc.,
	/// and then used via the abstract API in class BreakIterator
	/// </summary>
	public class RuleBasedBreakIterator : BreakIterator
	{
		private readonly UBreakIteratorType _iteratorType;
		private readonly string _rules = null;

		private bool _disposingValue = false; // To detect redundant calls
		private IntPtr _breakIterator = IntPtr.Zero;

		/// <summary>
		/// Creates a BreakIterator with the given BreakIteratorType, Locale.
		/// </summary>
		/// <param name="iteratorType">Break type.</param>
		/// <param name="locale">The locale.</param>
		/// <remarks>
		/// If iterator type is UBreakIteratorType.WORD, it will include
		/// spaces and punctuation as boundaries for words.  If this is 
		/// not desired <see cref="BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType, Locale, string, bool)"/>.
		/// </remarks>
		public RuleBasedBreakIterator(UBreakIteratorType iteratorType, Locale locale)
			: base(locale)
		{
			_iteratorType = iteratorType;
		}

		/// <summary>
		/// Creates a RuleBasedBreakIterator with the given rules.
		/// </summary>
		public RuleBasedBreakIterator(string rules)
			: base(DefaultLocale)
		{
			_rules = rules;
		}

		public override void SetText(string text)
		{
			base.SetText(text);

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
			else
			{
				ErrorCode err;

				NativeMethods.ubrk_setText(_breakIterator, Text, Text.Length, out err);

				if (err.IsFailure())
					throw new Exception("BreakIterator.ubrk_setText() failed with code " + err);
			}

			List<TextBoundary> textBoundaries = new List<TextBoundary>();

			// Function that checks if the offset is valid, gets the RuleStatus
			// and RuleStatusVector for the offset and then adds it to
			// textBoundaries.  Returns true if the boundary was not DONE.
			Func<int, bool> checkOffsetAndAddRuleStatus = (int offset) => {

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

				textBoundaries.Add(new TextBoundary(offset, ruleStatuses));

				return true;
			};

			// Start at the the beginning of the text and iterate until all
			// of the boundaries are consumed.
			int cur = NativeMethods.ubrk_first(_breakIterator);

			if (!checkOffsetAndAddRuleStatus(cur))
				return;

			while (cur != DONE)
			{
				int next = NativeMethods.ubrk_next(_breakIterator);

				if (!checkOffsetAndAddRuleStatus(next))
					break;

				cur = next;
			}

			_textBoundaries = textBoundaries.ToArray();
			_currentIndex = 0;
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

			if (_rules != null)
			{
				ErrorCode errorCode;
				ParseError parseError;

				_breakIterator = NativeMethods.ubrk_openRules(_rules, _rules.Length, Text, Text.Length, out parseError, out errorCode);

				if (errorCode.IsFailure())
				{
					throw new InvalidOperationException(
						string.Format("Could not create a BreakIterator with the given rules. Parse Error: {0}" + Environment.NewLine + "Rules: {1}", parseError.ToString(), _rules));
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
			if (_rules == null)
			{
				return string.Format("Locale: {0}, BreakIteratorType: {1}", _locale, _iteratorType);
			}
			else
			{
				return _rules;
			}
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

		~RuleBasedBreakIterator()
		{
			Dispose(false);
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
