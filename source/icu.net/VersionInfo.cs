using System;
using System.Runtime.InteropServices;

namespace Icu
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VersionInfo
	{
		public Byte Major;
		public Byte Minor;
		public Byte Micro;
		public Byte Milli;
		public override string ToString()
		{
			return Major + "." + Minor + "." + Micro + "." + Milli;
		}
	}
}
