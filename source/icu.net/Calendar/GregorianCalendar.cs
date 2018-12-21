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
									 locale.Name, UCalendarType.Gregorian, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}

		private GregorianCalendar(SafeCalendarHandle handle)
		{
			_calendarHandle = handle;
		}

		public override Calendar Clone()
		{
			var handle = NativeMethods.ucal_clone(_calendarHandle, out ErrorCode status);
			return new GregorianCalendar(handle);
		}
				
		public override bool InDaylightTime()
		{
			return NativeMethods.ucal_inDaylightTime(_calendarHandle, out _);
		}
	}
}
