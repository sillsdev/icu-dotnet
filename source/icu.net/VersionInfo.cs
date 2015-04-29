// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VersionInfo
	{
		public Byte Major;
		public Byte Minor;
		public Byte Milli;
		public Byte Micro;
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
