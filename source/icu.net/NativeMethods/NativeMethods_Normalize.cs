// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Icu.Normalization;

// ReSharper disable once CheckNamespace
namespace Icu
{
	internal static partial class NativeMethods
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class NormalizeMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm_normalizeDelegate(string source, int sourceLength,
				Normalizer.UNormalizationMode mode, int options,
				IntPtr result, int resultLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate byte unorm_isNormalizedDelegate(string source, int sourceLength,
				Normalizer.UNormalizationMode mode, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr unorm2_getInstanceDelegate(
				[MarshalAs(UnmanagedType.LPStr)] string packageName,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				Normalizer2.Mode mode, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm2_normalizeDelegate(IntPtr norm2, string source,
				int sourceLength, IntPtr dest, int capacity, ref ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool unorm2_isNormalizedDelegate(IntPtr norm2, string source,
				int sourceLength, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool unorm2_hasBoundaryAfterDelegate(IntPtr norm2, int c);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool unorm2_hasBoundaryBeforeDelegate(IntPtr norm2, int c);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm2_getDecompositionDelegate(IntPtr norm2, int c,
				IntPtr decomposition, int capacity, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm2_getRawDecompositionDelegate(IntPtr norm2, int c,
				IntPtr decomposition, int capacity, out ErrorCode errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int unorm2_getCombiningClassDelegate(IntPtr norm2, int c);

			internal unorm_normalizeDelegate unorm_normalize;
			internal unorm_isNormalizedDelegate unorm_isNormalized;
			internal unorm2_getInstanceDelegate unorm2_getInstance;
			internal unorm2_normalizeDelegate unorm2_normalize;
			internal unorm2_isNormalizedDelegate unorm2_isNormalized;
			internal unorm2_hasBoundaryAfterDelegate unorm2_hasBoundaryAfter;
			internal unorm2_hasBoundaryBeforeDelegate unorm2_hasBoundaryBefore;
			internal unorm2_getDecompositionDelegate unorm2_getDecomposition;
			internal unorm2_getRawDecompositionDelegate unorm2_getRawDecomposition;
			internal unorm2_getCombiningClassDelegate unorm2_getCombiningClass;
		}

		// ReSharper disable once InconsistentNaming
		private static NormalizeMethodsContainer NormalizeMethods = new NormalizeMethodsContainer();

		#region normalize

		/// <summary>
		/// Normalize a string according to the given mode and options.
		/// </summary>
		public static int unorm_normalize(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, int options,
			IntPtr result, int resultLength, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm_normalize == null)
				NormalizeMethods.unorm_normalize = GetMethod<NormalizeMethodsContainer.unorm_normalizeDelegate>(IcuCommonLibHandle, "unorm_normalize");
			return NormalizeMethods.unorm_normalize(source, sourceLength, mode, options, result,
				resultLength, out errorCode);
		}

		// Note that ICU's UBool type is typedef to an 8-bit integer.

		/// <summary>
		/// Check whether a string is normalized according to the given mode and options.
		/// </summary>
		public static byte unorm_isNormalized(string source, int sourceLength,
			Normalizer.UNormalizationMode mode, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm_isNormalized == null)
				NormalizeMethods.unorm_isNormalized = GetMethod<NormalizeMethodsContainer.unorm_isNormalizedDelegate>(IcuCommonLibHandle, "unorm_isNormalized");
			return NormalizeMethods.unorm_isNormalized(source, sourceLength, mode, out errorCode);
		}

		#endregion normalize

		#region normalize2

		/// <summary>
		/// Returns a UNormalizer2 instance which uses the specified data file (packageName/name
		/// similar to ucnv_openPackage() and ures_open()/ResourceBundle) and which composes or
		/// decomposes text according to the specified mode.
		/// </summary>
		public static IntPtr unorm2_getInstance(string packageName, string name,
			Normalizer2.Mode mode, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm2_getInstance == null)
			{
				NormalizeMethods.unorm2_getInstance = GetMethod<NormalizeMethodsContainer.unorm2_getInstanceDelegate>(
					IcuCommonLibHandle, "unorm2_getInstance");
			}
			return NormalizeMethods.unorm2_getInstance(packageName, name, mode, out errorCode);
		}

		/// <summary>
		/// Normalize a string according to the given mode and options.
		/// </summary>
		public static int unorm2_normalize(IntPtr norm2, string source, int sourceLength,
			IntPtr result, int resultLength, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm2_normalize == null)
			{
				NormalizeMethods.unorm2_normalize = GetMethod<NormalizeMethodsContainer.unorm2_normalizeDelegate>(
					IcuCommonLibHandle, "unorm2_normalize");
			}
			// Theoretically it should be unnecessary to initialize errorCode here. Instead we
			// should be able to use `out` instead. However, there seems to be a bug somewhere:
			// if this method gets called twice and errorCode != NoErrors, the error code won't
			// get reset, even though the method seems to work without errors.
			errorCode = ErrorCode.NoErrors;
			return NormalizeMethods.unorm2_normalize(norm2, source, sourceLength, result,
				resultLength, ref errorCode);
		}

		// Note that ICU's UBool type is typedef to an 8-bit integer.

		/// <summary>
		/// Check whether a string is normalized according to the given mode and options.
		/// </summary>
		public static bool unorm2_isNormalized(IntPtr norm2, string source, int sourceLength,
			out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm2_isNormalized == null)
			{
				NormalizeMethods.unorm2_isNormalized = GetMethod<NormalizeMethodsContainer.unorm2_isNormalizedDelegate>(
					IcuCommonLibHandle, "unorm2_isNormalized");
			}
			return NormalizeMethods.unorm2_isNormalized(norm2, source, sourceLength, out errorCode);
		}

		/// <summary>Tests if the character always has a normalization boundary after it,
		/// regardless of context.</summary>
		public static bool unorm2_hasBoundaryAfter(IntPtr norm2, int codePoint)
		{
			if (NormalizeMethods.unorm2_hasBoundaryAfter == null)
			{
				NormalizeMethods.unorm2_hasBoundaryAfter = GetMethod<NormalizeMethodsContainer.unorm2_hasBoundaryAfterDelegate>(
					IcuCommonLibHandle, nameof(unorm2_hasBoundaryAfter));
			}
			return NormalizeMethods.unorm2_hasBoundaryAfter(norm2, codePoint);
		}

		/// <summary>Tests if the character always has a normalization boundary before it,
		/// regardless of context.</summary>
		public static bool unorm2_hasBoundaryBefore(IntPtr norm2, int codePoint)
		{
			if (NormalizeMethods.unorm2_hasBoundaryBefore == null)
			{
				NormalizeMethods.unorm2_hasBoundaryBefore = GetMethod<NormalizeMethodsContainer.unorm2_hasBoundaryBeforeDelegate>(
					IcuCommonLibHandle, nameof(unorm2_hasBoundaryBefore));
			}
			return NormalizeMethods.unorm2_hasBoundaryBefore(norm2, codePoint);
		}

		/// <summary>Gets the decomposition mapping of c.</summary>
		public static int unorm2_getDecomposition(IntPtr norm2, int c, IntPtr decomposition,
			int capacity, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm2_getDecomposition == null)
			{
				NormalizeMethods.unorm2_getDecomposition = GetMethod<NormalizeMethodsContainer.unorm2_getDecompositionDelegate>(
					IcuCommonLibHandle, nameof(unorm2_getDecomposition));
			}
			return NormalizeMethods.unorm2_getDecomposition(norm2, c, decomposition, capacity,
				out errorCode);
		}

		/// <summary>Gets the raw decomposition mapping of c.</summary>
		public static int unorm2_getRawDecomposition(IntPtr norm2, int c, IntPtr decomposition,
			int capacity, out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (NormalizeMethods.unorm2_getRawDecomposition == null)
			{
				NormalizeMethods.unorm2_getRawDecomposition = GetMethod<NormalizeMethodsContainer.unorm2_getRawDecompositionDelegate>(
					IcuCommonLibHandle, nameof(unorm2_getRawDecomposition));
			}
			return NormalizeMethods.unorm2_getRawDecomposition(norm2, c, decomposition, capacity,
				out errorCode);
		}

		/// <summary>Gets the combining class of c.</summary>
		public static int unorm2_getCombiningClass(IntPtr norm2, int c)
		{
			if (NormalizeMethods.unorm2_getCombiningClass == null)
			{
				NormalizeMethods.unorm2_getCombiningClass = GetMethod<NormalizeMethodsContainer.unorm2_getCombiningClassDelegate>(
					IcuCommonLibHandle, nameof(unorm2_getCombiningClass));
			}
			return NormalizeMethods.unorm2_getCombiningClass(norm2, c);
		}

		#endregion normalize2

	}
}
