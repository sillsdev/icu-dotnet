// Copyright (c) 2016-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;

namespace Icu.Tests
{
	internal static class TestHelper
	{
		internal static int Main(string[] args)
		{
			// The only purpose of this TestHelper app is to output the ICU version
			// so that we can run unit tests that test loading of the unmanaged
			// ICU binaries

			if (args.Length < 2)
			{
				Console.Error.WriteLine("Missing min and max ICU versions");
				return 1;
			}

			if (args.Length >= 4)
			{
				if (!int.TryParse(args[2], out var icuVersion))
				{
					Console.Error.WriteLine($"Invalid ICU version: {args[2]}");
					return 2;
				}

				Wrapper.SetPreferredIcu4cDirectory(args[3]);
				Wrapper.ConfineIcuVersions(icuVersion);
			}
			else
			{
				if (!int.TryParse(args[0], out var minVersion) || !int.TryParse(args[1], out var maxVersion))
				{
					Console.Error.WriteLine($"Invalid ICU versions: ({args[0]}, {args[1]})");
					return 3;
				}
				Wrapper.ConfineIcuVersions(minVersion, maxVersion);
			}

			try
			{
				Console.WriteLine(Wrapper.IcuVersion);
				Wrapper.Cleanup();
			}
			catch (FileLoadException)
			{
				// Ignore - means we can't load ICU
			}

			return 0;
		}
	}
}
