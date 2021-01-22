// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Helps fetch information about ICU library such as ICU version, data
	/// folder, etc.
	/// </summary>
	public static class Wrapper
	{
		public const int MinSupportedIcuVersion = 44;
		public const int MaxSupportedIcuVersion = 70;

		#region Public Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the currently supported Unicode version for the current version of ICU.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string UnicodeVersion
		{
			get
			{
				VersionInfo arg;
				NativeMethods.u_getUnicodeVersion(out arg);
				return arg.ToString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the current version of ICU.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string IcuVersion
		{
			get
			{
				VersionInfo arg;
				NativeMethods.u_getVersion(out arg);
				return arg.ToString();
			}
		}

		/// <summary>
		/// Set to <c>true</c> to output diagnostic trace messages
		/// </summary>
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
		public static void ConfineIcuVersions(int minIcuVersion, int maxIcuVersion = -1)
		{
			if (maxIcuVersion == -1)
				maxIcuVersion = minIcuVersion;
			NativeMethods.SetMinMaxIcuVersions(minIcuVersion, maxIcuVersion);
		}

		#region Public wrappers around the ICU methods

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize ICU. In multi-threaded applications this should be the first ICU method
		/// that gets called, preferrably before starting multiple threads.
		/// </summary>
		/// <seealso href="http://userguide.icu-project.org/design#TOC-ICU-Initialization-and-Termination"/>
		/// ------------------------------------------------------------------------------------
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
		/// <seealso href="http://userguide.icu-project.org/design#TOC-ICU-Initialization-and-Termination"/>
		/// ------------------------------------------------------------------------------------
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
		public static string DataDirectory
		{
			get
			{
				IntPtr resPtr = NativeMethods.u_getDataDirectory();
				return Marshal.PtrToStringAnsi(resPtr);
			}
			set
			{
				// Remove a trailing backslash if it exists.
				if (value.Length > 0 && value[value.Length - 1] == '\\')
					value = value.Substring(0, value.Length - 1);
				NativeMethods.u_setDataDirectory(value);
			}
		}
		#endregion

	}
}
