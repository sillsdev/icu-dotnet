// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;

namespace Icu.Collation
{
	/// <summary>
	/// The sort key bound mode
	/// </summary>
	public enum UColBoundMode
	{
		/// <summary>
		/// lower bound
		/// </summary>
		UCOL_BOUND_LOWER = 0,
		/// <summary>
		/// upper bound that will match strings of exact size
		/// </summary>
		UCOL_BOUND_UPPER,
		/// <summary>
		/// upper bound that will match all strings that have the same initial substring as the given string
		/// </summary>
		UCOL_BOUND_UPPER_LONG
	}
}
