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
		private class ResourceBundleMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_openDelegate(string packageName, string locale,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ures_closeDelegate(IntPtr resourceBundle);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getKeyDelegate(IntPtr resourceBundle);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ures_getStringDelegate(IntPtr resourceBundle, out int len,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getLocaleDelegate(IntPtr resourceBundle,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getByKeyDelegate(IntPtr resourceBundle,
				string key, IntPtr fillIn, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getStringByKeyDelegate(IntPtr resourceBundle,
				string key, out int len, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate void ures_resetIteratorDelegate(IntPtr resourceBundle);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getNextStringDelegate(IntPtr resourceBundle,
				out int len, out IntPtr key, out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			[return: MarshalAs(UnmanagedType.I1)]
			internal delegate bool ures_hasNextDelegate(IntPtr resourceBundle);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr ures_getNextResourceDelegate(IntPtr resourceBundle,
				IntPtr fillIn, out ErrorCode status);

			internal ures_openDelegate ures_open;
			internal ures_closeDelegate ures_close;
			internal ures_getKeyDelegate ures_getKey;
			internal ures_getStringDelegate ures_getString;
			internal ures_getLocaleDelegate ures_getLocale;
			internal ures_getByKeyDelegate ures_getByKey;
			internal ures_getStringByKeyDelegate ures_getStringByKey;
			internal ures_resetIteratorDelegate ures_resetIterator;
			internal ures_getNextStringDelegate ures_getNextString;
			internal ures_hasNextDelegate ures_hasNext;
			internal ures_getNextResourceDelegate ures_getNextResource;
		}

		// ReSharper disable once InconsistentNaming
		private static ResourceBundleMethodsContainer ResourceBundleMethods = new ResourceBundleMethodsContainer();

		/// <summary/>
		public static IntPtr ures_open(string packageName, string locale, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_open == null)
			{
				ResourceBundleMethods.ures_open = GetMethod<ResourceBundleMethodsContainer.ures_openDelegate>(
					IcuCommonLibHandle, nameof(ures_open));
			}

			return ResourceBundleMethods.ures_open(packageName, locale, out status);
		}

		/// <summary/>
		public static void ures_close(IntPtr resourceBundle)
		{
			if (ResourceBundleMethods.ures_close == null)
			{
				ResourceBundleMethods.ures_close = GetMethod<ResourceBundleMethodsContainer.ures_closeDelegate>(
					IcuCommonLibHandle, nameof(ures_close));
			}

			ResourceBundleMethods.ures_close(resourceBundle);
		}

		/// <summary/>
		public static IntPtr ures_getKey(IntPtr resourceBundle)
		{
			if (ResourceBundleMethods.ures_getKey == null)
			{
				ResourceBundleMethods.ures_getKey = GetMethod<ResourceBundleMethodsContainer.ures_getKeyDelegate>(
					IcuCommonLibHandle, nameof(ures_getKey));
			}

			return ResourceBundleMethods.ures_getKey(resourceBundle);
		}

		/// <summary/>
		public static IntPtr ures_getString(IntPtr resourceBundle, out int len, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getString == null)
			{
				ResourceBundleMethods.ures_getString = GetMethod<ResourceBundleMethodsContainer.ures_getStringDelegate>(
					IcuCommonLibHandle, nameof(ures_getString));
			}

			return ResourceBundleMethods.ures_getString(resourceBundle, out len, out status);
		}

		/// <summary/>
		public static IntPtr ures_getLocale(IntPtr resourceBundle, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getLocale == null)
			{
				ResourceBundleMethods.ures_getLocale = GetMethod<ResourceBundleMethodsContainer.ures_getLocaleDelegate>(
					IcuCommonLibHandle, nameof(ures_getLocale));
			}

			return ResourceBundleMethods.ures_getLocale(resourceBundle, out status);
		}

		/// <summary/>
		public static IntPtr ures_getByKey(IntPtr resourceBundle, string key, IntPtr fillIn,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getByKey == null)
			{
				ResourceBundleMethods.ures_getByKey = GetMethod<ResourceBundleMethodsContainer.ures_getByKeyDelegate>(
					IcuCommonLibHandle, nameof(ures_getByKey));
			}

			return ResourceBundleMethods.ures_getByKey(resourceBundle, key, fillIn, out status);
		}

		/// <summary/>
		public static IntPtr ures_getStringByKey(IntPtr resourceBundle, string key, out int len,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getStringByKey == null)
			{
				ResourceBundleMethods.ures_getStringByKey = GetMethod<ResourceBundleMethodsContainer.ures_getStringByKeyDelegate>(
					IcuCommonLibHandle, nameof(ures_getStringByKey));
			}

			return ResourceBundleMethods.ures_getStringByKey(resourceBundle, key, out len, out status);
		}

		/// <summary/>
		public static void ures_resetIterator(IntPtr resourceBundle)
		{
			if (ResourceBundleMethods.ures_resetIterator == null)
			{
				ResourceBundleMethods.ures_resetIterator = GetMethod<ResourceBundleMethodsContainer.ures_resetIteratorDelegate>(
					IcuCommonLibHandle, nameof(ures_resetIterator));
			}

			ResourceBundleMethods.ures_resetIterator(resourceBundle);
		}

		/// <summary/>
		public static IntPtr ures_getNextString(IntPtr resourceBundle, out int len,
			out IntPtr key, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getNextString == null)
			{
				ResourceBundleMethods.ures_getNextString = GetMethod<ResourceBundleMethodsContainer.ures_getNextStringDelegate>(
					IcuCommonLibHandle, nameof(ures_getNextString));
			}

			return ResourceBundleMethods.ures_getNextString(resourceBundle, out len, out key, out status);
		}

		/// <summary/>
		public static IntPtr ures_getNextResource(IntPtr resourceBundle, IntPtr fillIn,
			out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (ResourceBundleMethods.ures_getNextResource == null)
			{
				ResourceBundleMethods.ures_getNextResource = GetMethod<ResourceBundleMethodsContainer.ures_getNextResourceDelegate>(
					IcuCommonLibHandle, nameof(ures_getNextResource));
			}

			return ResourceBundleMethods.ures_getNextResource(resourceBundle, fillIn, out status);
		}

		/// <summary/>
		public static bool ures_hasNext(IntPtr resourceBundle)
		{
			if (ResourceBundleMethods.ures_hasNext == null)
			{
				ResourceBundleMethods.ures_hasNext = GetMethod<ResourceBundleMethodsContainer.ures_hasNextDelegate>(
					IcuCommonLibHandle, nameof(ures_hasNext));
			}

			return ResourceBundleMethods.ures_hasNext(resourceBundle);
		}

	}
}
