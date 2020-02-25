using System;
using NUnit.Framework;
using Icu;
using System.Globalization;
using System.Linq;


namespace Icu.Tests
{
	[TestFixture]
	class CalendarTests
	{

		[Test]
		public void GetTimeZoneDisplayNameTest()
		{
			var timezone = new TimeZone("AST");
			using (var cal = new GregorianCalendar(timezone))
			{
				var displayName = cal.GetTimeZoneDisplayName(Calendar.UCalendarDisplayNameType.Standard);

				Assert.AreEqual("Alaska Standard Time", displayName);
			}
		}

		[Test]
		public void ClearTest()
		{
			using (var cal = new GregorianCalendar())
			{
				cal.Month = Calendar.UCalendarMonths.September;
				cal.DayOfMonth = 4;

				cal.Clear();

				Assert.AreEqual(Calendar.UCalendarMonths.January, cal.Month);
				Assert.AreEqual(1, cal.DayOfMonth);
			}
		}

		[Test]
		public void ClearFieldTest()
		{
			using (var cal = new GregorianCalendar())
			{
				cal.Month = Calendar.UCalendarMonths.September;

				cal.Clear(Calendar.UCalendarDateFields.Month);

				Assert.AreEqual(Calendar.UCalendarMonths.January, cal.Month);
			}
		}

		[Test]
		public void CloneTest()
		{
			using (var cal1 = new GregorianCalendar())
			{
				cal1.DayOfMonth = 5;
				using (var cal2 = cal1.Clone())
				{
					cal2.DayOfMonth = 10;

					Assert.AreEqual(5, cal1.DayOfMonth);
					Assert.AreEqual(10, cal2.DayOfMonth);
				}
			}
		}

		[Test]
		public void RollTest()
		{
			using (var cal = new GregorianCalendar())
			{
				var startMonth = cal.Month;

				cal.Roll(Calendar.UCalendarDateFields.DayOfMonth, 100);

				Assert.AreEqual(startMonth, cal.Month);
			}
		}

		[Test]
		public void SetTest()
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.Clear();

				cal.Set(Calendar.UCalendarDateFields.DayOfYear, 2);

				Assert.AreEqual(24 * 60 * 60 * 1000, cal.GetTime());
			}
		}

		[Test]
		public void RollAddDifferenceTest()
		{
			var month = Calendar.UCalendarMonths.April;
			int day = 30;

			using (Calendar cal1 = new GregorianCalendar(new TimeZone("UTC")),
							cal2 = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal1.Month = month;
				cal1.DayOfMonth = day;

				cal2.Month = month;
				cal2.DayOfMonth = day;

				cal1.Add(Calendar.UCalendarDateFields.DayOfMonth, 1);
				cal2.Roll(Calendar.UCalendarDateFields.DayOfMonth, 1);

				Assert.AreEqual(Calendar.UCalendarMonths.May, cal1.Month);
				Assert.AreEqual(1, cal1.DayOfMonth);

				Assert.AreEqual(Calendar.UCalendarMonths.April, cal2.Month);
				Assert.AreEqual(1, cal2.DayOfMonth);
			}
		}

		[Test]
		public void GetTest()
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				var expected = 15;
				cal.Set(Calendar.UCalendarDateFields.DayOfYear, expected);

				var result = cal.Get(Calendar.UCalendarDateFields.DayOfYear);

				Assert.AreEqual(expected, result);
			}
		}

		[Test]
		public void FieldDifferenceTest()
		{
			using (var cal = new GregorianCalendar())
			{
				var time = cal.GetTime();

				cal.Add(Calendar.UCalendarDateFields.Hour, 2);
				cal.Add(Calendar.UCalendarDateFields.Minute, 2);

				var difference = cal.FieldDifference(time, Calendar.UCalendarDateFields.Minute);

				Assert.AreEqual(time, cal.GetTime());
				Assert.AreEqual(-122, difference);
			}
		}

		[Test]
		public void IsSetTest()
		{
			using (var cal = new GregorianCalendar())
			{
				cal.Month = Calendar.UCalendarMonths.September;
				cal.DayOfMonth = 4;

				var setBefore = cal.IsSet(Calendar.UCalendarDateFields.Month);
				cal.Clear();
				var setAfter = cal.IsSet(Calendar.UCalendarDateFields.Month);

				Assert.AreEqual(true, setBefore);
				Assert.AreEqual(false, setAfter);
			}
		}

		[Test]
		public void InDaylightTime()
		{
			using (var cal = new GregorianCalendar(new TimeZone("Europe/Warsaw")))
			{
				cal.Month = Calendar.UCalendarMonths.September;
				cal.DayOfMonth = 1;

				Assert.AreEqual(true, cal.InDaylightTime());
			}
		}

		[Test]
		public void SetTimeTest()
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.SetTime(0);

				Assert.AreEqual(1970, cal.Year);
				Assert.AreEqual(0, cal.Hour);
				Assert.AreEqual(0, cal.Minute);
				Assert.AreEqual(0, cal.Millisecond);
			}
		}

		[Test]
		public void SetTimeDateTimeTest()
		{
			using (var cal = new GregorianCalendar())
			{
				var dateTime = DateTime.Now;
				cal.SetTime(dateTime);

				var result = cal.ToDateTime();

				// Truncate submillisecond part
				dateTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerMillisecond));

				Assert.AreEqual(dateTime, result);
			}
		}

		[Test]
		public void SetTimeDateTimeTest2()
		{
			var timezones = TimeZone.GetTimeZones();
			using (var cal = new GregorianCalendar(new TimeZone("Etc/GMT-7")))
			{
				var dateTime = DateTime.Now.ToUniversalTime();
				cal.SetTime(dateTime);

				var result = cal.ToDateTime();

				// Truncate submillisecond part
				dateTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerMillisecond));

				dateTime = dateTime.AddHours(7);

				Assert.AreEqual(dateTime, result);
			}
		}

		[Test]
		public void ToDateTimeTest()
		{
			using (var cal = new GregorianCalendar())
			{
				var datetime = cal.ToDateTime();

				Assert.AreEqual(cal.Year, datetime.Year);
				Assert.AreEqual((int)cal.Month + 1, datetime.Month);
				Assert.AreEqual(cal.DayOfMonth, datetime.Day);
				Assert.AreEqual(cal.HourOfDay, datetime.Hour);
				Assert.AreEqual(cal.Minute, datetime.Minute);
				Assert.AreEqual(cal.Second, datetime.Second);
				Assert.AreEqual(cal.Millisecond, datetime.Millisecond);
			}
		}

		[Test]
		public void SetTimeZoneTest()
		{
			var expected = new TimeZone("AST");

			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.SetTimeZone(expected);
				var result = cal.GetTimeZone();

				Assert.AreEqual(expected.Id, result.Id);
			}
		}

		[TestCase(Calendar.UCalendarDaysOfWeek.Sunday, ExpectedResult = Calendar.UCalendarDaysOfWeek.Sunday)]
		[TestCase(Calendar.UCalendarDaysOfWeek.Thursday, ExpectedResult = Calendar.UCalendarDaysOfWeek.Thursday)]
		public Calendar.UCalendarDaysOfWeek FirstDayOfWeekTest(Calendar.UCalendarDaysOfWeek firstDayOfWeek)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.FirstDayOfWeek = firstDayOfWeek;
				return cal.FirstDayOfWeek;
			}
		}

		[TestCase(Calendar.UCalendarDaysOfWeek.Sunday, ExpectedResult = 2)]
		[TestCase(Calendar.UCalendarDaysOfWeek.Thursday, ExpectedResult = 1)]
		[Test]
		public int WeekOfYearTest(Calendar.UCalendarDaysOfWeek firstDayOfWeek)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.Clear();
				cal.DayOfMonth = 4;

				cal.FirstDayOfWeek = firstDayOfWeek;
				return cal.WeekOfYear;
			}
		}

		[TestCase(5, ExpectedResult = 1)]
		[TestCase(1, ExpectedResult = 2)]
		public int MinimalDaysInFirstWeekTest(int minimumDaysInFirstWeek)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.Clear();
				cal.DayOfMonth = 4;

				cal.MinimalDaysInFirstWeek = minimumDaysInFirstWeek;
				return cal.WeekOfYear;
			}
		}

		[Test]
		public void SkippedWallTimeTest()
		{
			using (var cal = new GregorianCalendar(new TimeZone("America/New_York")))
			{
				cal.Year = 2011;
				cal.Month = Calendar.UCalendarMonths.March;
				cal.DayOfMonth = 13;
				cal.HourOfDay = 0;
				cal.Minute = 0;

				cal.SkippedWallTimeOption = Calendar.UCalendarWallTimeOption.WalltimeFirst;
				cal.HourOfDay = 2;
				cal.Minute = 30;
				var hour = cal.HourOfDay;
				var minute = cal.Minute;

				Assert.AreEqual(1, hour);
				Assert.AreEqual(30, minute);
			}
		}

		[Test]
		public void RepeatedWallTimeTest()
		{
			using (var cal = new GregorianCalendar(new TimeZone("America/New_York")))
			{
				cal.Year = 2011;
				cal.Month = Calendar.UCalendarMonths.November;
				cal.DayOfMonth = 6;
				cal.HourOfDay = 0;
				cal.Minute = 0;

				cal.RepeatedWallTimeOption = Calendar.UCalendarWallTimeOption.WalltimeFirst;
				cal.HourOfDay = 1;
				cal.Minute = 30;
				cal.Add(Calendar.UCalendarDateFields.Minute, 60);
				var hour = cal.HourOfDay;
				var minute = cal.Minute;

				Assert.AreEqual(1, hour);
				Assert.AreEqual(30, minute);
			}
		}

		[Test]
		public void LenientTest_ThrowsArgumentException()
		{
			using (var cal = new GregorianCalendar(new TimeZone("America/New_York")))
			{
				cal.Year = 2011;
				cal.Month = Calendar.UCalendarMonths.March;
				cal.DayOfMonth = 13;
				cal.HourOfDay = 0;
				cal.Minute = 0;

				cal.Lenient = false;
				cal.SkippedWallTimeOption = Calendar.UCalendarWallTimeOption.WalltimeFirst;
				cal.Minute = 30;
				cal.HourOfDay = 2;

				Assert.Throws<ArgumentException>(() => cal.GetTime());
			}
		}

		[TestCase(5, ExpectedResult = 1)]
		[TestCase(1, ExpectedResult = 2)]
		public int WeekOfMonthTest(int minimumDaysInFirstWeek)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.Clear();
				cal.DayOfMonth = 4;

				cal.MinimalDaysInFirstWeek = minimumDaysInFirstWeek;
				return cal.WeekOfMonth;
			}
		}

		[TestCase(2000, ExpectedResult = 1)]
		[TestCase(-1, ExpectedResult = 0)]
		public int EraTest(int year)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.Year = year;
				return cal.Era;
			}
		}

		[Test]
		public void ZoneOffsetTest()
		{
			var expected = 60 * 60 * 1000;
			var zone = new TimeZone("Europe/Paris");

			using (var cal = new GregorianCalendar(zone))
			{
				var offset = cal.ZoneOffset;

				Assert.AreEqual(expected, offset);
			}
		}

		[TestCase(Calendar.UCalendarMonths.July, ExpectedResult = 3600000 /* 60min * 60s * 1000ms */)]
		[TestCase(Calendar.UCalendarMonths.January, ExpectedResult = 0)]
		public int DstOffsetTest(Calendar.UCalendarMonths month)
		{
			var zone = new TimeZone("Europe/Paris");

			using (var cal = new GregorianCalendar(zone))
			{
				cal.Month = month;
				cal.DayOfMonth = 20;

				return cal.DstOffset;
			}
		}

		[TestCase(3, ExpectedResult = Calendar.UCalendarAMPMs.Am)]
		[TestCase(14, ExpectedResult = Calendar.UCalendarAMPMs.Pm)]
		public Calendar.UCalendarAMPMs AmPmTest(int hourOfDay)
		{
			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.HourOfDay = hourOfDay;
				return cal.AmPm;
			}
		}

		[Test]
		[Platform(Include = "win")]
		public void SetTimeZoneTestWin()
		{
			var timezones = TimeZoneInfo.GetSystemTimeZones();
			var timezone = timezones.First(tzi => tzi.Id == "Romance Standard Time");

			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.SetTimeZone(timezone);

				var tz = cal.GetTimeZone();

				Assert.AreEqual("Europe/Paris", tz.Id);
			}
		}

		[Test]
		[Platform(Exclude = "win")]
		public void SetTimeZoneTestLinux()
		{
			var tzdbId = "Europe/Paris";

			var timezones = TimeZoneInfo.GetSystemTimeZones();

			var timezone = timezones.First(tzi => tzi.Id == tzdbId);

			using (var cal = new GregorianCalendar(new TimeZone("UTC")))
			{
				cal.SetTimeZone(timezone);

				var tz = cal.GetTimeZone();

				Assert.AreEqual(tzdbId, tz.Id);
			}
		}

		[Test]
		[Platform(Include = "win")]
		public void GetTilmeZoneInfoTestWin()
		{
			var timezone = new TimeZone("Europe/Zagreb");

			using (var cal = new GregorianCalendar(timezone))
			{
				var result = cal.GetTimeZoneInfo();

				Assert.IsTrue(result.Id == "Central European Standard Time");
			}
		}

		[Test]
		[Platform(Exclude = "win")]
		public void GetTimeZoneInfoTestLinux()
		{
			var tzdbId = "Europe/Zagreb";

			var timezone = new TimeZone(tzdbId);

			using (var cal = new GregorianCalendar(timezone))
			{
				var result = cal.GetTimeZoneInfo();

				Assert.IsTrue(result.Id == tzdbId);
			}
		}
	}
}
