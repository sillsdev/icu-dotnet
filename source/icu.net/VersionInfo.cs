// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VersionInfo
	{
		#if ICU_VER_40
		internal const string ICU_I18N_LIB = "icuin40.dll";
		internal const string ICU_COMMON_LIB = "icuuc40.dll";
		internal const string ICU_VERSION_SUFFIX = "_4_0";
		#else
		internal const string ICU_I18N_LIB = "icuin48.dll";
		internal const string ICU_COMMON_LIB = "icuuc48.dll";
		internal const string ICU_VERSION_SUFFIX = "_48";
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
