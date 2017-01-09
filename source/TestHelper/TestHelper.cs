// Copyright (c) 2016 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;

namespace Icu.Tests
{
	class TestHelper
	{
		static void Main(string[] args)
		{
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
