// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Icu
{
	internal static partial class NativeMethods
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
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

		// ReSharper disable once InconsistentNaming
		private static CodepageConversionMethodsContainer CodepageConversionMethods = new CodepageConversionMethodsContainer();

		public static SafeEnumeratorHandle ucnv_openAllNames(out ErrorCode err)
		{
			err = ErrorCode.NoErrors;
			if (CodepageConversionMethods.ucnv_openAllNames == null)
				CodepageConversionMethods.ucnv_openAllNames = GetMethod<CodepageConversionMethodsContainer.ucnv_openAllNamesDelegate>(IcuCommonLibHandle, nameof(ucnv_openAllNames));
			return CodepageConversionMethods.ucnv_openAllNames(out err);
		}

		public static IntPtr ucnv_getStandardName(string name, string standard, out ErrorCode err)
		{
			err = ErrorCode.NoErrors;
			if (CodepageConversionMethods.ucnv_getStandardName == null)
				CodepageConversionMethods.ucnv_getStandardName = GetMethod<CodepageConversionMethodsContainer.ucnv_getStandardNameDelegate>(IcuCommonLibHandle, nameof(ucnv_getStandardName));
			return CodepageConversionMethods.ucnv_getStandardName(name, standard, out err);
		}
	}
}
