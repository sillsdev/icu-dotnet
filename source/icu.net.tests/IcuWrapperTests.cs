// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class IcuWrapperTests
	{
		[Test]
		public void UnicodeVersion()
		{
			var result = Wrapper.UnicodeVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(3));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			int major;
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out major), Is.True);
		}
	}
}
