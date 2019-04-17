using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	internal static partial class NativeMethods
	{
		private class CalendarMehodsContainer
		{


			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setTimeZoneDelegate(
				Calendar.SafeCalendarHandle cal,
				[MarshalAs(UnmanagedType.LPWStr)] string zoneID,
				int len,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getTimeZoneIdDelegate(
				Calendar.SafeCalendarHandle cal,
				IntPtr result,
				int resultLength,
				out ErrorCode ec);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Calendar.SafeCalendarHandle ucal_openDelegate(
				[MarshalAs(UnmanagedType.LPWStr)] string zoneID,
				int len,
				[MarshalAs(UnmanagedType.LPStr)] string locale,
				Calendar.UCalendarType type,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_closeDelegate(IntPtr cal);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_addDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				int amount,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_rollDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				int amount,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_clearDelegate(
				Calendar.SafeCalendarHandle cal);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_clearFieldDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Calendar.SafeCalendarHandle ucal_cloneDelegate(
				Calendar.SafeCalendarHandle cal,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getAttributeDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarAttribute attribute);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getFieldDifferenceDelegate(
				Calendar.SafeCalendarHandle cal,
				double target,
				Calendar.UCalendarDateFields field,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate double ucal_getMillisDelegate(
				Calendar.SafeCalendarHandle cal,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate double ucal_getNowDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int ucal_getTimeZoneDisplayNameDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDisplayNameType type,
				[MarshalAs(UnmanagedType.LPStr)] string locale,
				IntPtr result,
				int resultLength,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate bool ucal_inDaylightTimeDelegate(
				Calendar.SafeCalendarHandle cal,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				int value);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate bool ucal_isSetDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setAttributeDelegate(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarAttribute attr,
				int newValue);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setDateTimeDelegate(
				Calendar.SafeCalendarHandle cal,
				int year, int month, int date,
				out ErrorCode status);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void ucal_setMillisDelegate(
				Calendar.SafeCalendarHandle cal,
				double dateTime,
				out ErrorCode status);


			internal ucal_openDelegate ucal_open;
			internal ucal_closeDelegate ucal_close;
			internal ucal_setTimeZoneDelegate ucal_setTimeZone;
			internal ucal_getTimeZoneIdDelegate ucal_getTimeZoneId;
			internal ucal_addDelegate ucal_add;
			internal ucal_rollDelegate ucal_roll;
			internal ucal_getDelegate ucal_get;
			internal ucal_clearDelegate ucal_clear;
			internal ucal_clearFieldDelegate ucal_clearField;
			internal ucal_cloneDelegate ucal_clone;
			internal ucal_getAttributeDelegate ucal_getAttribute;
			internal ucal_getFieldDifferenceDelegate ucal_getFieldDifference;
			internal ucal_getMillisDelegate ucal_getMillis;
			internal ucal_getNowDelegate ucal_getNow;
			internal ucal_getTimeZoneDisplayNameDelegate ucal_getTimeZoneDisplayName;
			internal ucal_inDaylightTimeDelegate ucal_inDaylightTime;
			internal ucal_setDelegate ucal_set;
			internal ucal_isSetDelegate ucal_isSet;
			internal ucal_setAttributeDelegate ucal_setAttribute;
			internal ucal_setDateTimeDelegate ucal_setDateTime;
			internal ucal_setMillisDelegate ucal_setMillis;
		}

		private static CalendarMehodsContainer _CalendarMethods;

		private static CalendarMehodsContainer CalendarMethods =>
			_CalendarMethods ??
			(_CalendarMethods = new CalendarMehodsContainer());

		#region Calendar


		public static void ucal_setTimeZone(
				Calendar.SafeCalendarHandle cal,
				string zoneID,
				int len,
				out ErrorCode ec)
		{
			if (CalendarMethods.ucal_setTimeZone == null)
				CalendarMethods.ucal_setTimeZone = GetMethod<CalendarMehodsContainer.ucal_setTimeZoneDelegate>(IcuI18NLibHandle, "ucal_setTimeZone");
			CalendarMethods.ucal_setTimeZone(cal, zoneID, len, out ec);
		}

		public static int ucal_getTimeZoneId(
			Calendar.SafeCalendarHandle cal,
			out string result,
			int resultLength,
			out ErrorCode ec)
		{
			if (CalendarMethods.ucal_getTimeZoneId == null)
				CalendarMethods.ucal_getTimeZoneId = GetMethod<CalendarMehodsContainer.ucal_getTimeZoneIdDelegate>(IcuI18NLibHandle, "ucal_getTimeZoneID");

			IntPtr outBuf = Marshal.AllocHGlobal(resultLength * sizeof(char));
			try
			{
				int length = CalendarMethods.ucal_getTimeZoneId(cal, outBuf, resultLength, out ec);
				char[] buf = new char[Math.Min(resultLength, length)];
				Marshal.Copy(outBuf, buf, 0, buf.Length);
				result = new string(buf);
				return length;
			}
			finally
			{
				Marshal.FreeHGlobal(outBuf);
			}
		}

		public static Calendar.SafeCalendarHandle ucal_open(
			   string zoneID,
			   string locale,
			   Calendar.UCalendarType type,
			   out ErrorCode status)
		{
			if (CalendarMethods.ucal_open == null)
				CalendarMethods.ucal_open = GetMethod<CalendarMehodsContainer.ucal_openDelegate>(IcuI18NLibHandle, "ucal_open");
			return CalendarMethods.ucal_open(zoneID, zoneID.Length, locale, type, out status);
		}

		public static void ucal_close(IntPtr cal)
		{
			if (CalendarMethods.ucal_close == null)
				CalendarMethods.ucal_close = GetMethod<CalendarMehodsContainer.ucal_closeDelegate>(IcuI18NLibHandle, "ucal_close");
			CalendarMethods.ucal_close(cal);
		}


		public static void ucal_add(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				int amount,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_add == null)
				CalendarMethods.ucal_add = GetMethod<CalendarMehodsContainer.ucal_addDelegate>(IcuI18NLibHandle, "ucal_add");
			CalendarMethods.ucal_add(cal, field, amount, out status);
		}
		public static void ucal_roll(
		Calendar.SafeCalendarHandle cal,
		Calendar.UCalendarDateFields field,
		int amount,
		out ErrorCode status)
		{
			if (CalendarMethods.ucal_roll == null)
				CalendarMethods.ucal_roll = GetMethod<CalendarMehodsContainer.ucal_rollDelegate>(IcuI18NLibHandle, "ucal_roll");
			CalendarMethods.ucal_roll(cal, field, amount, out status);
		}

		public static int ucal_get(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_get == null)
				CalendarMethods.ucal_get = GetMethod<CalendarMehodsContainer.ucal_getDelegate>(IcuI18NLibHandle, "ucal_get");
			return CalendarMethods.ucal_get(cal, field, out status);
		}

		public static int ucal_getAttribute(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarAttribute attribute)
		{
			if (CalendarMethods.ucal_getAttribute == null)
				CalendarMethods.ucal_getAttribute = GetMethod<CalendarMehodsContainer.ucal_getAttributeDelegate>(IcuI18NLibHandle, "ucal_getAttribute");
			return CalendarMethods.ucal_getAttribute(cal, attribute);
		}

		public static void ucal_clear(
		Calendar.SafeCalendarHandle cal)
		{
			if (CalendarMethods.ucal_clear == null)
				CalendarMethods.ucal_clear = GetMethod<CalendarMehodsContainer.ucal_clearDelegate>(IcuI18NLibHandle, "ucal_clear");
			CalendarMethods.ucal_clear(cal);
		}

		public static void ucal_clearField(
		Calendar.SafeCalendarHandle cal,
		Calendar.UCalendarDateFields field)
		{
			if (CalendarMethods.ucal_clearField == null)
				CalendarMethods.ucal_clearField = GetMethod<CalendarMehodsContainer.ucal_clearFieldDelegate>(IcuI18NLibHandle, "ucal_clearField");
			CalendarMethods.ucal_clearField(cal, field);
		}

		public static Calendar.SafeCalendarHandle ucal_clone(Calendar.SafeCalendarHandle cal, out ErrorCode status)
		{
			if (CalendarMethods.ucal_clone == null)
				CalendarMethods.ucal_clone = GetMethod<CalendarMehodsContainer.ucal_cloneDelegate>(IcuI18NLibHandle, "ucal_clone");
			return CalendarMethods.ucal_clone(cal, out status);
		}

		public static int ucal_getFieldDifference(
				Calendar.SafeCalendarHandle cal,
				double target,
				Calendar.UCalendarDateFields field,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_getFieldDifference == null)
				CalendarMethods.ucal_getFieldDifference = GetMethod<CalendarMehodsContainer.ucal_getFieldDifferenceDelegate>(IcuI18NLibHandle, "ucal_getFieldDifference");
			return CalendarMethods.ucal_getFieldDifference(cal, target, field, out status);
		}

		public static double ucal_getMillis(
				Calendar.SafeCalendarHandle cal,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_getMillis == null)
				CalendarMethods.ucal_getMillis = GetMethod<CalendarMehodsContainer.ucal_getMillisDelegate>(IcuI18NLibHandle, "ucal_getMillis");
			return CalendarMethods.ucal_getMillis(cal, out status);
		}

		public static double ucal_getNow()
		{
			if (CalendarMethods.ucal_getNow == null)
				CalendarMethods.ucal_getNow = GetMethod<CalendarMehodsContainer.ucal_getNowDelegate>(IcuI18NLibHandle, "ucal_getNow");
			return CalendarMethods.ucal_getNow();
		}

		public static int ucal_getTimeZoneDisplayName(
					Calendar.SafeCalendarHandle cal,
					Calendar.UCalendarDisplayNameType type,
					string locale,
					IntPtr result,
					int resultLength,
					out ErrorCode status)
		{
			if (CalendarMethods.ucal_getTimeZoneDisplayName == null)
				CalendarMethods.ucal_getTimeZoneDisplayName = GetMethod<CalendarMehodsContainer.ucal_getTimeZoneDisplayNameDelegate>(IcuI18NLibHandle, "ucal_getTimeZoneDisplayName");
			return CalendarMethods.ucal_getTimeZoneDisplayName(cal, type, locale, result, resultLength, out status);
		}

		public static bool ucal_inDaylightTime(
				Calendar.SafeCalendarHandle cal,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_inDaylightTime == null)
				CalendarMethods.ucal_inDaylightTime = GetMethod<CalendarMehodsContainer.ucal_inDaylightTimeDelegate>(IcuI18NLibHandle, "ucal_inDaylightTime");
			return CalendarMethods.ucal_inDaylightTime(cal, out status);
		}


		public static void ucal_set(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field,
				int value)
		{
			if (CalendarMethods.ucal_set == null)
				CalendarMethods.ucal_set = GetMethod<CalendarMehodsContainer.ucal_setDelegate>(IcuI18NLibHandle, "ucal_set");
			CalendarMethods.ucal_set(cal, field, value);
		}
		public static bool ucal_isSet(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarDateFields field)
		{
			if (CalendarMethods.ucal_isSet == null)
				CalendarMethods.ucal_isSet = GetMethod<CalendarMehodsContainer.ucal_isSetDelegate>(IcuI18NLibHandle, "ucal_isSet");
			return CalendarMethods.ucal_isSet(cal, field);
		}

		public static void ucal_setAttribute(
				Calendar.SafeCalendarHandle cal,
				Calendar.UCalendarAttribute attr,
				int newValue)
		{
			if (CalendarMethods.ucal_setAttribute == null)
				CalendarMethods.ucal_setAttribute = GetMethod<CalendarMehodsContainer.ucal_setAttributeDelegate>(IcuI18NLibHandle, "ucal_setAttribute");
			CalendarMethods.ucal_setAttribute(cal, attr, newValue);
		}

		public static void ucal_setDateTime(
				Calendar.SafeCalendarHandle cal,
				int year, int month, int date,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_setDateTime == null)
				CalendarMethods.ucal_setDateTime = GetMethod<CalendarMehodsContainer.ucal_setDateTimeDelegate>(IcuI18NLibHandle, "ucal_setDateTime");
			CalendarMethods.ucal_setDateTime(cal, year, month, date, out status);
		}

		public static void ucal_setMillis(
				Calendar.SafeCalendarHandle cal,
				double dateTime,
				out ErrorCode status)
		{
			if (CalendarMethods.ucal_setMillis == null)
				CalendarMethods.ucal_setMillis = GetMethod<CalendarMehodsContainer.ucal_setMillisDelegate>(IcuI18NLibHandle, "ucal_setMillis");
			CalendarMethods.ucal_setMillis(cal, dateTime, out status);
		}

		#endregion
	}
}
