// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Icu.BreakIterators
{
	/// <summary>
	/// Supports iterating over a BreakIterator object and reading its individual text segments.
	/// </summary>
	public sealed class BreakEnumerator: IEnumerator<string>
	{
		private BreakIterator _breakIterator;
		private int _currentStart;
		private int _currentLimit;

		internal BreakEnumerator(BreakIterator iterator)
		{
			_breakIterator = iterator.Clone();
		}

		#region Disposable
		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
				_breakIterator.Dispose();
			_breakIterator = null;
		}

		~BreakEnumerator()
		{
			Debug.WriteLineIf(_breakIterator != null, $"Missing Dispose() for {GetType()}");
			Dispose(false);
		}
		#endregion

		#region IEnumerator implementation

		/// <inheritdoc/>
		public bool MoveNext()
		{
			_currentStart = _currentLimit;
			_currentLimit = _breakIterator.MoveNext();
			return _currentLimit != BreakIterator.DONE;
		}

		/// <inheritdoc/>
		public void Reset()
		{
			_currentLimit = _breakIterator.MoveFirst();
		}

		/// <inheritdoc/>
		public string Current => _breakIterator.Text.Substring(_currentStart, _currentLimit - _currentStart);

		/// <inheritdoc/>
		object IEnumerator.Current => Current;
		#endregion
	}
}
