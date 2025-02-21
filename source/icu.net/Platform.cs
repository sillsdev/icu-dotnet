// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
#if NET || NETSTANDARD
using System.Runtime.InteropServices;
#endif

namespace Icu
{
	/// <summary>
	/// Operating systems.
	/// Taken from:
	/// https://github.com/libgit2/libgit2sharp/blob/master/LibGit2Sharp/Core/Platform.cs
	/// </summary>
	internal enum OperatingSystemType
	{
		Windows,
		Unix,
		MacOSX
	}

	/// <summary>
	/// Class that fetches the current process bitness and architecture.
	/// Adapted from:
	/// https://github.com/libgit2/libgit2sharp/blob/master/LibGit2Sharp/Core/Platform.cs
	/// </summary>
	internal static class Platform
	{
		public const string x64 = nameof(x64);
		public const string x86 = nameof(x86);

		public static string ProcessArchitecture => Environment.Is64BitProcess ? x64 : x86;

		public static OperatingSystemType OperatingSystem
		{
			get
			{
#if NET || NETSTANDARD
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					return OperatingSystemType.Windows;
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					return OperatingSystemType.Unix;
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					return OperatingSystemType.MacOSX;
				else
					throw new NotSupportedException("Cannot get OperatingSystemType from: " + RuntimeInformation.OSDescription);
#else
				// See http://www.mono-project.com/docs/faq/technical/#how-to-detect-the-execution-platform
				switch ((int)Environment.OSVersion.Platform)
				{
					case 4:
					case 128:
						return OperatingSystemType.Unix;
					case 6:
						return OperatingSystemType.MacOSX;
					default:
						return OperatingSystemType.Windows;
				}
#endif
			}
		}
	}
}
