// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

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

		public static string ProcessArchitecture
		{
			get {

#if NETSTANDARD1_6
				// Workaround described here since the API does not exist:
				// https://github.com/dotnet/corefx/issues/999#issuecomment-75907756
				return IntPtr.Size == 4 ? x86 : x64;
#else
				return Environment.Is64BitProcess ? x64 : x86;
#endif
			}
		}

		public static OperatingSystemType OperatingSystem
		{
			get
			{
#if NET40
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
#else
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					return OperatingSystemType.Windows;
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					return OperatingSystemType.Unix;
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					return OperatingSystemType.MacOSX;
				else
					throw new NotSupportedException("Cannot get OperatingSystemType from: " + RuntimeInformation.OSDescription);
#endif
			}
		}
	}
}
