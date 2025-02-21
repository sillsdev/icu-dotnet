// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu
{
	/// <summary>
	/// Specifies the start and end indexes of a word, line, sentence or character.
	/// </summary>
	public class Boundary
	{
		/// <summary>
		/// Starting index of a boundary
		/// </summary>
		public readonly int Start;

		/// <summary>
		/// End index of a boundary
		/// </summary>
		public readonly int End;

		/// <summary>
		/// Creates a boundary with the specified start and end. The word would
		/// lie between indices x, Start &lt;= x &lt; End
		/// </summary>
		public Boundary(int start, int end)
		{
			if (start > end)
			{
				throw new ArgumentException("start index cannot be greater than the end index.");
			}

			Start = start;
			End = end;
		}

		/// <summary>
		/// Checks to see whether the given object is a Boundary with the same
		/// start and end positions.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if the obj is equal, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			var compared = obj as Boundary;

			return compared != null
				&& Start == compared.Start
				&& End == compared.End;
		}

		/// <summary>
		/// Serves the default hash function.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns a string with the Start and End positions of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Start: [{0}], End: [{1}]", Start, End);
		}
	}
}
