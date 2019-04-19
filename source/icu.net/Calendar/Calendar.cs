using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

#if NETSTANDARD1_6
using Icu;
#else
using System.Globalization;
using System.Runtime.ConstrainedExecution;
#endif

namespace Icu
{
	public abstract class Calendar : IDisposable
	{
		public enum UCalendarDateFields
		{
			Era,
			Year,
			Month,
			WeekOfYear,
			WeekOfMonth,
			Date,
			DayOfYear,
			DayOfWeek,
			DayOfWeekInMonth,
			AmPm,
			Hour,
			HourOfDay,
			Minute,
			Second,
			Millisecond,
			ZoneOffset,
			DstOffset,
			YearWoy,
			DowLocal,
			ExtendedYear,
			JulianYear,
			MillisecondsInDay,
			IsLeapMonth,
			FieldCount,
			DayOfMonth = Date
		};

		public enum UCalendarType
		{
			Traditional,
			Default = Traditional,
			Gregorian
		}

		public enum UCalendarDisplayNameType
		{
			Standard,
			ShortStandard,
			Dst,
			ShortDst
		};

		public enum UCalendarAttribute
		{
			Lenient,
			FirstDayOfWeek,
			MinimalDaysInFirstWeek,
			RepeatedWallTime,
			SkippedWallTime
		};

		public enum UCalendarWallTimeOption
		{
			WalltimeLast,
			WalltimeFirst,
			WalltimeNextValid
		};

		public enum UCalendarDaysOfWeek
		{
			Sunday = 1,
			Monday,
			Tuesday,
			Wednesday,
			Thursday,
			Friday,
			Saturday
		};

		public enum UCalendarMonths
		{
			January,
			February,
			March,
			April,
			May,
			June,
			July,
			August,
			September,
			October,
			November,
			December,
			Undecimber
		};

		public enum UCalendarAMPMs
		{
			Am,
			Pm
		};

		internal protected sealed class SafeCalendarHandle : SafeHandle
		{
			public SafeCalendarHandle() :
				base(IntPtr.Zero, true)
			{ }

			///<summary>
			/// When overridden in a derived class, executes the code required to free the handle.
			///</summary>
			///<returns>
			/// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false.
			/// In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
			///</returns>
#if !NETSTANDARD1_6
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
			protected override bool ReleaseHandle()
			{
				if (handle != IntPtr.Zero)
					NativeMethods.ucal_close(handle);
				handle = IntPtr.Zero;
				return true;
			}

			///<summary>
			///When overridden in a derived class, gets a value indicating whether the handle value is invalid.
			///</summary>
			///<returns>
			///true if the handle is valid; otherwise, false.
			///</returns>
			public override bool IsInvalid
			{
				get { return handle == IntPtr.Zero; }
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				ReleaseHandle();
			}
		}

		private bool _isDisposed; // To detect redundant calls
		protected SafeCalendarHandle _calendarHandle = default(SafeCalendarHandle);
		protected Locale _locale;


		/// <summary>
		/// Gets this Calendar's time as milliseconds.
		/// </summary>
		/// <returns>The current time in UTC (GMT) time, or zero if the operation failed.</returns>
		public double GetTime()
		{
			var millis = NativeMethods.ucal_getMillis(_calendarHandle, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);

			return millis;
		}

		/// <summary>
		/// Create and return a polymorphic copy of this calendar.
		/// </summary>
		/// <returns>A polymorphic copy of this calendar. </returns>
		public abstract Calendar Clone();

		/// <summary>
		/// Sets this Calendar's current time with the given date.
		///The time specified should be in non-local UTC(GMT) time.
		/// </summary>
		/// <param name="date">The given date in UTC (GMT) time.</param>
		public void SetTime(double date)
		{
			NativeMethods.ucal_setMillis(_calendarHandle, date, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}

		/// <summary>
		/// Sets this Calendar's current time with the given date.
		/// Time will be set to matching time in calendar's timezone.
		/// </summary>
		/// <param name="dateTime">The given date</param>
		public void SetTime(DateTime dateTime)
		{
			var universalTime = dateTime.ToUniversalTime();

			Year = universalTime.Year;
			Month = (Calendar.UCalendarMonths)(universalTime.Month - 1);
			DayOfMonth = universalTime.Day;
			HourOfDay = universalTime.Hour;
			Minute = universalTime.Minute;
			Second = universalTime.Second;
			Millisecond = universalTime.Millisecond;

			// Offset to calendars local timezone
			Add(UCalendarDateFields.Millisecond, ZoneOffset+DstOffset);
		}

		/// <summary>
		/// Adds the specified(signed) amount of time to the given time field, based on the calendar's rules.
		/// 
		/// For example, to subtract 5 days from the current time of the calendar, call add(Calendar::DATE, -5).
		/// When adding on the month or Calendar::MONTH field, other fields like date might conflict and need to be changed.
		/// For instance, adding 1 month on the date 01/31/96 will result in 02/29/96. Adding a positive value always means
		/// moving forward in time, so for the Gregorian calendar, starting with 100 BC and adding +1 to year results in 99 BC
		/// (even though this actually reduces the numeric value of the field itself).
		/// </summary>
		/// <param name="field">Specifies which date field to modify. </param>
		/// <param name="amount">The amount of time to be added to the field, in the natural unit for that field (e.g., days for the day fields, hours for the hour field.) </param>
		public void Add(UCalendarDateFields field, int amount)
		{
			NativeMethods.ucal_add(_calendarHandle, field, amount, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}

		/// <summary>
		/// Time Field Rolling function.
		/// 
		/// The only difference between roll() and add() is that roll() does not change the value of more significant fields
		/// when it reaches the minimum or maximum of its range, whereas add() does.
		/// </summary>
		/// <param name="field">The time field.</param>
		/// <param name="amount">Indicates amount to roll.</param>
		public void Roll(UCalendarDateFields field, int amount)
		{
			NativeMethods.ucal_roll(_calendarHandle, field, amount, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}

		/// <summary>
		/// Return the difference between the given time and the time this calendar object is set to.
		///
		/// If this calendar is set before the given time, the returned value will be positive.
		/// If this calendar is set after the given time, the returned value will be negative.
		/// The field parameter specifies the units of the return value.
		///
		/// As a side effect of this call, this calendar is advanced toward when by the given amount.
		/// That is, calling this method has the side effect of calling add(field, n), where n is the return value.
		///
		/// Usage: To use this method, call it first with the largest field of interest, then with progressively smaller fields.
		/// </summary>
		/// <param name="when">the date to compare this calendar's time to </param>
		/// <param name="field">the field in which to compute the result </param>
		/// <returns></returns>
		public int FieldDifference(double when, UCalendarDateFields field)
		{
			var result = NativeMethods.ucal_getFieldDifference(_calendarHandle, when, field, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);

			return result;
		}

		/// <summary>
		/// Gets the value for a given time field.
		/// </summary>
		/// <param name="field">The given time field. </param>
		/// <returns>The value for the given time field, or zero if the field is unset, and set() has been called for any other field. </returns>
		public int Get(UCalendarDateFields field)
		{
			int val = NativeMethods.ucal_get(_calendarHandle, field, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			return val;
		}

		/// <summary>
		/// Sets the given time field with the given value. 
		/// </summary>
		/// <param name="field">The given time field.</param>
		/// <param name="value">The value to be set for the given time field.</param>
		public void Set(UCalendarDateFields field, int value)
		{
			NativeMethods.ucal_set(_calendarHandle, field, value);
		}

		/// <summary>
		/// Determines if the given time field has a value set. 
		/// </summary>
		/// <param name="field">The given time field.</param>
		/// <returns>True if the given time field has a value set; false otherwise.</returns>
		public bool IsSet(UCalendarDateFields field)
		{
			return NativeMethods.ucal_isSet(_calendarHandle, field);
		}

		/// <summary>
		/// Clears the values of all the time fields, making them both unset and assigning them a value of zero. 
		/// </summary>
		public void Clear()
		{
			NativeMethods.ucal_clear(_calendarHandle);
		}

		/// <summary>
		/// Clears the value in the given time field, both making it unset and assigning it a value of zero. 
		/// </summary>
		/// <param name="field">The time field to be cleared. </param>
		public void Clear(UCalendarDateFields field)
		{
			NativeMethods.ucal_clearField(_calendarHandle, field);
		}

		/// <summary>
		/// Sets the calendar's time zone to be the same as the one passed in. 
		/// </summary>
		/// <param name="timezone">The given time zone.</param>
		public void SetTimeZone(TimeZone timezone)
		{
			SetTimeZone(timezone.Id);
		}

		/// <summary>
		/// Sets the calendar's time zone to be equivalent to the one passed in. 
		/// </summary>
		/// <param name="timeZoneInfo">The given time zone info.</param>
		public void SetTimeZone(TimeZoneInfo timeZoneInfo)
		{
			var id = timeZoneInfo.Id;

			if(!TimeZone.GetTimeZones().Any(tz=>tz.Id == id))
				id = TimeZone.GetIdForWindowsId(id, _locale?.Country);

			SetTimeZone(id);
		}

		/// <summary>
		/// Returns time zone set for this calendar.
		/// </summary>
		/// <returns>Time zone set for this calendar.</returns>
		public TimeZone GetTimeZone()
		{
			string result = NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.ucal_getTimeZoneId(_calendarHandle, ptr, length, out ErrorCode status);
				return new Tuple<ErrorCode, int>(status, length);
			}, 255);
			return new TimeZone(result);
		}

		/// <summary>
		/// Returns time zone info for time zone set for this calendar.
		/// </summary>
		/// <returns>Time zone info for this calendar.</returns>
		public TimeZoneInfo GetTimeZoneInfo()
		{
			var id = GetTimeZone().Id;

			var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(zone => zone.Id == id);

			if (timeZoneInfo != null)
			{
				return timeZoneInfo;
			}

			string winid = TimeZone.GetWindowsId(id);

			if (!string.IsNullOrWhiteSpace(winid))
			{
				id = winid;
			}

			return TimeZoneInfo.FindSystemTimeZoneById(id);
		}

		public abstract bool InDaylightTime();

		/// <summary>
		/// Returns the current UTC (GMT) time measured in milliseconds since 0:00:00 on 1/1/70 (derived from the system time). 
		/// </summary>
		/// <returns>The current UTC time in milliseconds.</returns>
		public static double GetNow()
		{
			return NativeMethods.ucal_getNow();
		}

		public DateTime ToDateTime()
		{
			var dto = new DateTime(Year,
				1 + (int)Month,
				DayOfMonth,
				HourOfDay,
				Minute,
				Second,
				Millisecond);
			return dto;
		}

		/// <summary>
		/// Get the display name for a UCalendar's TimeZone.
		///
		/// A display name is suitable for presentation to a user.
		/// </summary>
		/// <param name="type">The desired display name format</param>
		/// <returns>Formatted time zone name</returns>
		public string GetTimeZoneDisplayName(UCalendarDisplayNameType type)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.ucal_getTimeZoneDisplayName(_calendarHandle, type, _locale.Name, ptr, length, out ErrorCode status);
				return new Tuple<ErrorCode, int>(status, length);
			}, 255);
		}

		#region Properties

		/// <summary>
		/// Gets or sets whether date/time interpretation is to be lenient.
		/// </summary>
		public bool Lenient
		{
			get
			{
				var result = NativeMethods.ucal_getAttribute(_calendarHandle, UCalendarAttribute.Lenient);
				return (result != 0);
			}
			set
			{
				NativeMethods.ucal_setAttribute(_calendarHandle, UCalendarAttribute.Lenient, value ? 1 : 0);
			}
		}

		/// <summary>
		/// Gets or sets first day of week.
		/// </summary>
		public UCalendarDaysOfWeek FirstDayOfWeek
		{
			get
			{
				return (UCalendarDaysOfWeek)NativeMethods.ucal_getAttribute(_calendarHandle, UCalendarAttribute.FirstDayOfWeek);
			}
			set
			{
				NativeMethods.ucal_setAttribute(_calendarHandle, UCalendarAttribute.FirstDayOfWeek, (int)value);
			}
		}

		/// <summary>
		/// Gets or sets minimal number of days in first week.
		/// </summary>
		public int MinimalDaysInFirstWeek
		{
			get
			{
				var result = NativeMethods.ucal_getAttribute(_calendarHandle, UCalendarAttribute.MinimalDaysInFirstWeek);
				return result;
			}
			set
			{
				NativeMethods.ucal_setAttribute(_calendarHandle, UCalendarAttribute.MinimalDaysInFirstWeek, value);
			}
		}

		/// <summary>
		/// Gets or sets option for handling ambiguous wall time at time zone offset transitions.
		/// </summary>
		public UCalendarWallTimeOption RepeatedWallTimeOption
		{
			get
			{
				var result = (UCalendarWallTimeOption)NativeMethods.ucal_getAttribute(_calendarHandle, UCalendarAttribute.RepeatedWallTime);

				return result;
			}
			set
			{
				NativeMethods.ucal_setAttribute(_calendarHandle, UCalendarAttribute.RepeatedWallTime, (int)value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public UCalendarWallTimeOption SkippedWallTimeOption
		{
			get
			{
				var result = (UCalendarWallTimeOption)NativeMethods.ucal_getAttribute(_calendarHandle, UCalendarAttribute.SkippedWallTime);
				
				return result;
			}
			set
			{
				NativeMethods.ucal_setAttribute(_calendarHandle, UCalendarAttribute.SkippedWallTime, (int)value);
			}
		}

		/// <summary>
		/// Gets or sets era.
		/// </summary>
		public int Era
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Era, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Era, value);
			}
		}

		/// <summary>
		/// Gets or sets year.
		/// </summary>
		public int Year
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Year, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Year, value);
			}
		}

		/// <summary>
		/// Gets or sets motnth.
		/// </summary>
		public UCalendarMonths Month
		{
			get
			{
				var result = (UCalendarMonths)NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Month, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Month, (int)value);
			}
		}

		/// <summary>
		/// Gets or sets week of year.
		/// </summary>
		public int WeekOfYear
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.WeekOfYear, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.WeekOfYear, value);
			}
		}

		/// <summary>
		/// Gets or sets week of month.
		/// </summary>
		public int WeekOfMonth
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.WeekOfMonth, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.WeekOfMonth, value);
			}
		}

		/// <summary>
		/// Gets or sets day of week.
		/// </summary>
		public int DayOfWeek
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.DayOfWeek, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.DayOfWeek, value);
			}
		}

		/// <summary>
		/// Gets or sets day of month.
		/// </summary>
		public int DayOfMonth
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.DayOfMonth, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.DayOfMonth, value);
			}
		}

		/// <summary>
		/// Gets or sets ordinal number of the day of the week within the current month.
		/// 
		/// Together with the DAY_OF_WEEK field, this uniquely specifies a day within a month.
		/// Unlike WEEK_OF_MONTH and WEEK_OF_YEAR, this field's value does not depend on getFirstDayOfWeek()
		/// or getMinimalDaysInFirstWeek(). DAY_OF_MONTH 1 through 7 always correspond to DAY_OF_WEEK_IN_MONTH 1;
		/// 8 through 15 correspond to DAY_OF_WEEK_IN_MONTH 2, and so on. DAY_OF_WEEK_IN_MONTH 0 indicates the week before DAY_OF_WEEK_IN_MONTH 1.
		/// Negative values count back from the end of the month, so the last Sunday of a month is specified as DAY_OF_WEEK = SUNDAY, DAY_OF_WEEK_IN_MONTH = -1.
		/// Because negative values count backward they will usually be aligned differently within the month than positive values. For example,
		/// if a month has 31 days, DAY_OF_WEEK_IN_MONTH -1 will overlap DAY_OF_WEEK_IN_MONTH 5 and the end of 4. 
		/// </summary>
		public int DayOfWeekInMonth
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.DayOfWeekInMonth, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.DayOfWeekInMonth, value);
			}
		}

		/// <summary>
		/// Gets or sets day of year.
		/// </summary>
		public int DayOfYear
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.DayOfYear, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.DayOfYear, value);
			}
		}

		/// <summary>
		/// Gets or seths whether the hour is before or after noon.
		/// </summary>
		public UCalendarAMPMs AmPm
		{
			get
			{
				var result = (UCalendarAMPMs)NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.AmPm, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.AmPm, (int)value);
			}
		}

		/// <summary>
		/// Gets or sets hour in 12 hour based format.
		/// </summary>
		public int Hour
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Hour, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Hour, value);
			}
		}

		/// <summary>
		/// Gets or sets hour in 24 hour based format.
		/// </summary>
		public int HourOfDay
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.HourOfDay, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.HourOfDay, value);
			}
		}

		/// <summary>
		/// Gets or sets minute.
		/// </summary>
		public int Minute
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Minute, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Minute, value);
			}
		}

		/// <summary>
		/// Gets or sets second.
		/// </summary>
		public int Second
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Second, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Second, value);
			}
		}

		/// <summary>
		/// Gets or sets millisecond.
		/// </summary>
		public int Millisecond
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.Millisecond, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.Millisecond, value);
			}
		}

		/// <summary>
		/// Gets or sets time zone ofset.
		/// </summary>
		public int ZoneOffset
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.ZoneOffset, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.ZoneOffset, value);
			}
		}

		/// <summary>
		/// Gets or sets daylight savings time offset.
		/// </summary>
		public int DstOffset
		{
			get
			{
				var result = NativeMethods.ucal_get(_calendarHandle, UCalendarDateFields.DstOffset, out ErrorCode errorCode);
				ExceptionFromErrorCode.ThrowIfError(errorCode);
				return result;
			}
			set
			{
				NativeMethods.ucal_set(_calendarHandle, UCalendarDateFields.DstOffset, value);
			}
		}

		/// <summary>
		/// Calendar's locale.
		/// </summary>
		public Locale Locale
		{
			get => _locale;
		}

		#endregion Properties


		private void SetTimeZone(string id)
		{
			NativeMethods.ucal_setTimeZone(_calendarHandle, id, id.Length, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}


		#region IDisposable support

		/// <summary>
		/// Dispose of managed/unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by Calendar.
		/// </summary>
		/// <param name="disposing">true to release managed and unmanaged
		/// resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_calendarHandle.Dispose();
				}

				_isDisposed = true;
			}
		}
		~Calendar()
		{
			Dispose(false);
		}

		#endregion
	}
}
