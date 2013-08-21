// --------------------------------------------------------------------------------------------
// <copyright from='2013' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Win32;
using Icu.Collation;

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
				var arg = new byte[4];
				NativeMethods.u_getUnicodeVersion(arg);
				return string.Format("{0}.{1}", arg[0], arg[1]);
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
