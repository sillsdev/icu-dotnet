// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	public static class Wrapper
	{
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
		#endregion

		#region Public wrappers around the ICU methods


		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Cleans up the ICU files that could be locked
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void Cleanup()
		{
			NativeMethods.u_Cleanup();
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
				IntPtr resPtr = NativeMethods.u_GetDataDirectory();
				return Marshal.PtrToStringAnsi(resPtr);
			}
			set
			{
				// Remove a trailing backslash if it exists.
				if (value.Length > 0 && value[value.Length - 1] == '\\')
					value = value.Substring(0, value.Length - 1);
				NativeMethods.u_SetDataDirectory(value);
			}
		}
		#endregion

	}
}
