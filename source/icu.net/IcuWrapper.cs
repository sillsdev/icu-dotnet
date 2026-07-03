// Copyright (c) 2013-2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("icu.net.tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f1a7e4dc5dedd55e54dbf599e2d82cf883c691e8bf81d0a8a993e2be9b7510ce6e7c2be8645e3b66d898f4f481b77bfcc57dfcbce28d744c06c3555d36afffaee59b1237e683cd9ea704d4529c5f48a9007a6408d6da069f991e4324c4ae804b0a6bff550ebf3cff44172b8df4bcb45841cf6fe23a2a34720d0ae059fa99a1d2")]

namespace Icu
{
	/// <summary>
	/// Helps fetch information about ICU library such as ICU version, data
	/// folder, etc.
	/// </summary>
	public static class Wrapper
	{
		public const int MinSupportedIcuVersion = 44;
		public const int MaxSupportedIcuVersion = 90;

		#region Public Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the currently supported Unicode version for the current version of ICU.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static string UnicodeVersion
		{
			get
			{
				NativeMethods.u_getUnicodeVersion(out var arg);
				return arg.ToString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the current version of ICU.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static string IcuVersion
		{
			get
			{
				NativeMethods.u_getVersion(out var arg);
				return arg.ToString();
			}
		}

		/// <summary>
		/// Set to <c>true</c> to output diagnostic trace messages
		/// </summary>
		[PublicAPI]
		public static bool Verbose
		{
			get => NativeMethods.Verbose;
			set => NativeMethods.Verbose = value;
		}
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Limits the ICU versions that are considered when trying to dynamically load ICU.
		/// </summary>
		/// <remarks>This method allows an application to select a specific ICU version. Otherwise
		/// the highest found supported ICU libraries will be used.</remarks>
		/// <param name="minIcuVersion">Minimum ICU version. Needs to be greater or equal to the
		/// minimum supported ICU version (currently 44).</param>
		/// <param name="maxIcuVersion">Maximum ICU version. Needs to be less or equal to the
		/// maximum supported ICU version (currently 60). Set to <c>-1</c> to use the same value
		/// as <paramref name="minIcuVersion"/>.</param>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static void ConfineIcuVersions(int minIcuVersion, int maxIcuVersion = -1)
		{
			if (maxIcuVersion == -1)
				maxIcuVersion = minIcuVersion;
			NativeMethods.SetMinMaxIcuVersions(minIcuVersion, maxIcuVersion);
		}

		/// <summary>
		/// Set directory where to look for unmanaged binaries first. This is helpful if the current
		/// directory contains a different version than should be used.
		/// </summary>
		/// <param name="directory">Path</param>
		[PublicAPI]
		public static void SetPreferredIcu4cDirectory(string directory)
		{
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				throw new DirectoryNotFoundException(directory);

			NativeMethods.PreferredDirectory = directory;
		}

		/// <summary>
		/// Android APKs require ICU native libraries to be named libicuuc.so (without a version suffix).
		/// Call this before <see cref="Init"/> with the major ICU version that was bundled.
		/// </summary>
		[PublicAPI]
		public static void SetAndroidBundledIcuMajorVersion(int majorVersion)
		{
			NativeMethods.AndroidBundledIcuMajorVersion = majorVersion;
		}

		/// <summary>
		/// Android hosts native libraries inside APK splits where dlopen needs a zip path.
		/// Provide a loader that returns a dlopen handle for a file name such as libicuuc.so.
		/// </summary>
		[PublicAPI]
		public static void SetAndroidNativeLibraryLoader(Func<string, IntPtr> loader)
		{
			NativeMethods.AndroidLoadNativeLibrary = loader;
		}

		/// <summary>
		/// Mono Android FastDev cannot resolve libc P/Invokes; provide dlsym lookup for ICU entry points.
		/// </summary>
		[PublicAPI]
		public static void SetAndroidSymbolResolver(Func<string, IntPtr> resolver)
		{
			NativeMethods.AndroidResolveSymbol = resolver;
		}

		/// <summary>
		/// Optional hook for Android device test hosts to configure bundled ICU before NUnit runs.
		/// </summary>
		[PublicAPI]
		public static Action AndroidTestConfigure { get; set; }

		#region Public wrappers around the ICU methods

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize ICU. In multi-threaded applications this should be the first ICU method
		/// that gets called, preferably before starting multiple threads.
		/// </summary>
		/// <seealso href="https://unicode-org.github.io/icu/userguide/icu/design.html#icu4c-initialization-and-termination"/>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static ErrorCode Init()
		{
			NativeMethods.u_init(out var errorCode);
			return errorCode;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Cleans up the ICU files that could be locked. This should be the last ICU method
		/// that gets called.
		/// </summary>
		/// <remarks>This method is not thread-safe! All other threads should stop using ICU
		/// before calling this function. </remarks>
		/// <seealso href="https://unicode-org.github.io/icu/userguide/icu/design.html#icu4c-initialization-and-termination"/>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static void Cleanup()
		{
			NativeMethods.Cleanup();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the current data directory.
		/// </summary>
		/// <returns>the pathname</returns>
		/// ------------------------------------------------------------------------------------
		[PublicAPI]
		public static string DataDirectory
		{
			get
			{
				var resPtr = NativeMethods.u_getDataDirectory();
				return Marshal.PtrToStringAnsi(resPtr);
			}
			set
			{
				// Remove a trailing backslash if it exists.
				if (value.EndsWith("\\") || value.EndsWith("/"))
					value = value.Substring(0, value.Length - 1);
				NativeMethods.u_setDataDirectory(value);
			}
		}
		#endregion

	}
}
