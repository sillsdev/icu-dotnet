using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	class TimeZoneTests
	{
		[Test]
		public void DefaultTimeZoneTest()
		{
			var toSet = new TimeZone("AST");

			TimeZone.SetDefault(toSet);
			var def = TimeZone.GetDefault();

			Assert.AreEqual(toSet.ID, def.ID);
		}

		[Test]
		public void GetDstSavingsTest()
		{
			var timezone = new TimeZone("Europe/Warsaw");

			var savings = timezone.GetDstSavings();

			Assert.AreEqual(3600 * 1000, savings);
		}

		[Test]
		public void GetTimeZonesTest()
		{
			var timezones = TimeZone.GetTimeZones();

			Assert.AreEqual(632, timezones.Count());
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Asia/Seoul"));
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Europe/London"));
		}

		[Test]
		public void GetTimeZoneIDsTest()
		{
			var timezones = TimeZone.GetTimeZones(USystemTimeZoneType.Any, "PL");

			Assert.AreEqual(2, timezones.Count());
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Poland"));
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Europe/Warsaw"));
		}

		[Test]
		public void GetCountryTimeZonesTest()
		{
			var timezones = TimeZone.GetCountryTimeZones("PL");

			Assert.AreEqual(2, timezones.Count());
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Poland"));
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Europe/Warsaw"));
		}

		[Test]
		public void GetTimeZoneIDsTest2()
		{
			var timezones = TimeZone.GetTimeZones(USystemTimeZoneType.Any, null, -1 * 3600 * 1000);

			Assert.AreEqual(4, timezones.Count());
			Assert.AreEqual(1, timezones.Count(x => x.ID == "Atlantic/Azores"));
			Assert.AreEqual(1, timezones.Count(x => x.ID == "America/Scoresbysund"));
		}

		[Test]
		public void GetDefaultTimeZoneTest()
		{
			var expected = new TimeZone("AST");
			TimeZone.SetDefault(expected);

			var result = TimeZone.GetDefault();

			Assert.AreEqual(expected.ID, result.ID);
		}

		[Test]
		public void TimeZoneIdFromWinIdTest()
		{
			var winId = "Central European Standard Time";
			var expected = "Europe/Zagreb";

			var id = TimeZone.GetIdForWindowsId(winId, "HR");

			Assert.AreEqual(expected, id);
		}

		[Test]
		public void TimeZoneWinIdFromIdTest()
		{
			var id = "Europe/Zagreb";
			var expected = "Central European Standard Time";

			var winId = TimeZone.GetWindowsId(id);

			Assert.AreEqual(expected, winId);
		}
	}
}
