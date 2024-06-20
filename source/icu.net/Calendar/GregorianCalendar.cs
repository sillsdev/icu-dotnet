using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Icu
{
	public class GregorianCalendar : Calendar
	{
		public GregorianCalendar()
			: this(new Locale())
		{
		}

		public GregorianCalendar(Locale locale)
			:this(TimeZone.GetDefault(),locale)
		{
		}

		public GregorianCalendar(TimeZone timezone)
			:this(timezone, new Locale())
		{
		}

		public GregorianCalendar(TimeZone timezone, Locale locale)
		{
			_locale = locale;
			_calendarHandle = NativeMethods.ucal_open(timezone.Id,
									 locale.Name, UCalendarType.Gregorian, out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
		}

		private GregorianCalendar(SafeCalendarHandle handle)
		{
			_calendarHandle = handle;
		}

		public override Calendar Clone()
		{
			var handle = NativeMethods.ucal_clone(_calendarHandle, out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
			var calendar = new GregorianCalendar(handle);
			calendar._locale = _locale;
			return calendar;
		}
				
		public override bool InDaylightTime()
		{
			bool isDaylightTime = NativeMethods.ucal_inDaylightTime(_calendarHandle, out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
			return isDaylightTime;
		}
	}
}
