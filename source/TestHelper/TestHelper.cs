// Copyright (c) 2016 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;

namespace Icu.Tests
{
	static class TestHelper
	{
		static void Main(string[] args)
		{
			// The only purpose of this TestHelper app is to output the ICU version
			// so that we can run unit tests that test loading of the unmanaged
			// ICU binaries
			try
			{
				Console.WriteLine(Wrapper.IcuVersion);
				Wrapper.Cleanup();
			}
			catch (FileLoadException)
			{
				// Ignore - means we can't load ICU
			}
		}
	}
}
