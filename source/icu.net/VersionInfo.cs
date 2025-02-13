// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Struct representing Version information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct VersionInfo
	{
		/// <summary>
		/// Major version number.
		/// </summary>
		public Byte Major;
		/// <summary>
		/// Minor version number.
		/// </summary>
		public Byte Minor;
		/// <summary>
		/// Milli portion of version number
		/// </summary>
		public Byte Milli;
		/// <summary>
		/// Micro portion of version number.
		/// </summary>
		public Byte Micro;
		/// <summary>
		/// Gets string representation of Version as: Major.Minor.Milli
		/// or Major.Minor.Milli.Micro
		/// </summary>
		public override string ToString()
		{
			// Only include Milli and Micro portions if they're non-zero
			var milliMicro = string.Empty;
			if (Micro > 0)
				milliMicro = "." + Milli + "." + Micro;
			else if (Milli > 0)
				milliMicro = "." + Milli;

			return string.Format("{0}.{1}{2}", Major, Minor, milliMicro);
		}
	}
}
