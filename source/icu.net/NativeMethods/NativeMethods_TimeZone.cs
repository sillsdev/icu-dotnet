using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Icu
{
	internal static partial class NativeMethods
	{
		private class TimeZoneMethodsContainer
		{
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate SafeEnumeratorHandle ucal_openTimeZoneIDEnumerationDelegate(
				USystemTimeZoneType zoneType,
				[MarshalAs(UnmanagedType.LPStr)] string region,
				ref int rawOffset,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate SafeEnumeratorHandle ucal_openTimeZoneIDEnumerationPtrDelegate(
				USystemTimeZoneType zoneType,
				[MarshalAs(UnmanagedType.LPStr)] string region,
				IntPtr rawOffset,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate SafeEnumeratorHandle ucal_openTimeZonesDelegate(
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate SafeEnumeratorHandle ucal_openCountryTimeZonesDelegate(
				[MarshalAs(UnmanagedType.LPStr)] string country,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getDefaultTimeZoneDelegate(
				IntPtr result,
				int resultCapacity,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setDefaultTimeZoneDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string zoneID,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getDSTSavingsDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string zoneID,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr ucal_getTZDataVersionDelegate(
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getWindowsTimeZoneIdDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string id,
				int len,
				IntPtr winid,
				int winidCapacity,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getTimeZoneIDForWindowsIdDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string winid,
				int len,
				[MarshalAs(UnmanagedType.LPStr)] string region,
				IntPtr id,
				int idCapacity,
				out ErrorCode status);

			internal ucal_openTimeZoneIDEnumerationDelegate ucal_openTimeZoneIDEnumeration;
			internal ucal_openTimeZoneIDEnumerationPtrDelegate ucal_openTimeZoneIDEnumerationPtr;
			internal ucal_openTimeZonesDelegate ucal_openTimeZones;
			internal ucal_openCountryTimeZonesDelegate ucal_openCountryTimeZones;
			internal ucal_getDefaultTimeZoneDelegate ucal_getDefaultTimeZone;
			internal ucal_setDefaultTimeZoneDelegate ucal_setDefaultTimeZone;
			internal ucal_getDSTSavingsDelegate ucal_getDSTSavings;
			internal ucal_getTZDataVersionDelegate ucal_getTZDataVersion;
			internal ucal_getWindowsTimeZoneIdDelegate ucal_getWindowsTimeZoneId;
			internal ucal_getTimeZoneIDForWindowsIdDelegate ucal_getTimeZoneIDForWindowsId;
		}


		private static TimeZoneMethodsContainer _TimeZoneMethods;

		private static TimeZoneMethodsContainer TimeZoneMethods =>
			_TimeZoneMethods ??
			(_TimeZoneMethods = new TimeZoneMethodsContainer());

		public static SafeEnumeratorHandle ucal_openTimeZoneIDEnumeration(
			USystemTimeZoneType zoneType,
			string region,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_openTimeZoneIDEnumerationPtr == null)
				TimeZoneMethods.ucal_openTimeZoneIDEnumerationPtr = GetMethod<TimeZoneMethodsContainer.ucal_openTimeZoneIDEnumerationPtrDelegate>(IcuI18NLibHandle, "ucal_openTimeZoneIDEnumeration");
			return TimeZoneMethods.ucal_openTimeZoneIDEnumerationPtr(zoneType, region, IntPtr.Zero, out ec);
		}

		public static SafeEnumeratorHandle ucal_openTimeZoneIDEnumeration(
			USystemTimeZoneType zoneType,
			string region,
			ref int rawOffset,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_openTimeZoneIDEnumeration == null)
				TimeZoneMethods.ucal_openTimeZoneIDEnumeration = GetMethod<TimeZoneMethodsContainer.ucal_openTimeZoneIDEnumerationDelegate>(IcuI18NLibHandle, "ucal_openTimeZoneIDEnumeration");
			return TimeZoneMethods.ucal_openTimeZoneIDEnumeration(zoneType, region, ref rawOffset, out ec);
		}

		public static SafeEnumeratorHandle ucal_openTimeZones(out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_openTimeZones == null)
				TimeZoneMethods.ucal_openTimeZones = GetMethod<TimeZoneMethodsContainer.ucal_openTimeZonesDelegate>(IcuI18NLibHandle, "ucal_openTimeZones");
			return TimeZoneMethods.ucal_openTimeZones(out ec);
		}

		public static SafeEnumeratorHandle ucal_openCountryTimeZones(
			string country,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_openCountryTimeZones == null)
				TimeZoneMethods.ucal_openCountryTimeZones = GetMethod<TimeZoneMethodsContainer.ucal_openCountryTimeZonesDelegate>(IcuI18NLibHandle, "ucal_openCountryTimeZones");
			return TimeZoneMethods.ucal_openCountryTimeZones(country, out ec);
		}

		public static int ucal_getDefaultTimeZone(
			IntPtr result,
			int resultCapacity,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_getDefaultTimeZone == null)
				TimeZoneMethods.ucal_getDefaultTimeZone = GetMethod<TimeZoneMethodsContainer.ucal_getDefaultTimeZoneDelegate>(IcuI18NLibHandle, "ucal_getDefaultTimeZone");
			return TimeZoneMethods.ucal_getDefaultTimeZone(result, resultCapacity, out ec);
		}

		public static void ucal_setDefaultTimeZone(
			string zoneID,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_setDefaultTimeZone == null)
				TimeZoneMethods.ucal_setDefaultTimeZone = GetMethod<TimeZoneMethodsContainer.ucal_setDefaultTimeZoneDelegate>(IcuI18NLibHandle, "ucal_setDefaultTimeZone");
			TimeZoneMethods.ucal_setDefaultTimeZone(zoneID, out ec);
		}

		public static int ucal_getDSTSavings(
			string zoneID,
			out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_getDSTSavings == null)
				TimeZoneMethods.ucal_getDSTSavings = GetMethod<TimeZoneMethodsContainer.ucal_getDSTSavingsDelegate>(IcuI18NLibHandle, "ucal_getDSTSavings");
			return TimeZoneMethods.ucal_getDSTSavings(zoneID, out ec);
		}

		public static IntPtr ucal_getTZDataVersion(out ErrorCode ec)
		{
			if (TimeZoneMethods.ucal_getTZDataVersion == null)
				TimeZoneMethods.ucal_getTZDataVersion = GetMethod<TimeZoneMethodsContainer.ucal_getTZDataVersionDelegate>(IcuI18NLibHandle, "ucal_getTZDataVersion");
			return TimeZoneMethods.ucal_getTZDataVersion(out ec);
		}

		public static int ucal_getWindowsTimeZoneId(
			string id,
			IntPtr winid,
			int winidCapacity,
			out ErrorCode status)
		{
			if (TimeZoneMethods.ucal_getWindowsTimeZoneId == null)
				TimeZoneMethods.ucal_getWindowsTimeZoneId = GetMethod<TimeZoneMethodsContainer.ucal_getWindowsTimeZoneIdDelegate>(IcuI18NLibHandle, "ucal_getWindowsTimeZoneID");
			return TimeZoneMethods.ucal_getWindowsTimeZoneId(id, id.Length, winid, winidCapacity, out status);
		}

		public static int ucal_getTimeZoneIDForWindowsId(
			string winid,
			string region,
			IntPtr id,
			int idCapacity,
			out ErrorCode status)
		{
			if (TimeZoneMethods.ucal_getTimeZoneIDForWindowsId == null)
				TimeZoneMethods.ucal_getTimeZoneIDForWindowsId = GetMethod<TimeZoneMethodsContainer.ucal_getTimeZoneIDForWindowsIdDelegate>(IcuI18NLibHandle, "ucal_getTimeZoneIDForWindowsID");
			return TimeZoneMethods.ucal_getTimeZoneIDForWindowsId(winid, winid.Length, region, id, idCapacity, out status);
		}
	}
}
