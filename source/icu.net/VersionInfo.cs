using System;
using System.Runtime.InteropServices;

namespace Icu
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VersionInfo
	{
		#if ICU_VER_42
		internal const string ICU_I18N_LIB = "icuin42.dll";
		internal const string ICU_COMMON_LIB = "icuuc42.dll";
		internal const string ICU_VERSION_SUFFIX = "_4_2";
		#elif ICU_VER_48
		internal const string ICU_I18N_LIB = "icuin48.dll";
		internal const string ICU_COMMON_LIB = "icuuc48.dll";
		internal const string ICU_VERSION_SUFFIX = "_48";
		#else
		internal const string ICU_I18N_LIB = "icuin50.dll";
		internal const string ICU_COMMON_LIB = "icuuc50.dll";
		internal const string ICU_VERSION_SUFFIX = "_50";
		#endif

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
