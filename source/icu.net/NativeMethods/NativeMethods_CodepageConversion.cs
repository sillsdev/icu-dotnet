// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;

namespace Icu
{
	internal static partial class NativeMethods
	{
		private class CodepageConversionMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate SafeEnumeratorHandle ucnv_openAllNamesDelegate(out ErrorCode err);
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ucnv_getStandardNameDelegate(string name, string standard, 
				out ErrorCode errorCode);

			internal ucnv_openAllNamesDelegate ucnv_openAllNames;
			internal ucnv_getStandardNameDelegate ucnv_getStandardName;
		}

		private static CodepageConversionMethodsContainer _CodepageConversionMethods;

		private static CodepageConversionMethodsContainer CodepageConversionMethods =>
			_CodepageConversionMethods ??
			(_CodepageConversionMethods = new CodepageConversionMethodsContainer());

		public static SafeEnumeratorHandle ucnv_openAllNames(out ErrorCode err)
		{
			if (CodepageConversionMethods.ucnv_openAllNames == null)
				CodepageConversionMethods.ucnv_openAllNames = GetMethod<CodepageConversionMethodsContainer.ucnv_openAllNamesDelegate>(IcuCommonLibHandle, nameof(ucnv_openAllNames));
			return CodepageConversionMethods.ucnv_openAllNames(out err);
		}

		public static IntPtr ucnv_getStandardName(string name, string standard, out ErrorCode err)
		{
			if (CodepageConversionMethods.ucnv_getStandardName == null)
				CodepageConversionMethods.ucnv_getStandardName = GetMethod<CodepageConversionMethodsContainer.ucnv_getStandardNameDelegate>(IcuCommonLibHandle, nameof(ucnv_getStandardName));
			return CodepageConversionMethods.ucnv_getStandardName(name, standard, out err);
		}
	}
}
