using System.Linq;
using System.Text.RegularExpressions;
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

			Assert.GreaterOrEqual(timezones.Count(), 400);
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Asia/Seoul"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Europe/London"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Etc/GMT-10"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Etc/GMT+3"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Etc/UTC"));
		}

		[Test]
		public void GetRegionTimeZonesTest()
		{
			var timezones = TimeZone.GetTimeZones(USystemTimeZoneType.Any, "PL");

			Assert.AreEqual(2, timezones.Count());
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Poland"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Europe/Warsaw"));
		}

		[Test]
		public void GetCountryTimeZonesTest()
		{
			var timezones = TimeZone.GetCountryTimeZones("PL");

			Assert.AreEqual(2, timezones.Count());
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Poland"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Europe/Warsaw"));
		}

		[Test]
		public void GetOffsetTimeZonesTest()
		{
			var timezones = TimeZone.GetTimeZones(USystemTimeZoneType.Any, null, -1 * 3600 * 1000);

			Assert.GreaterOrEqual(timezones.Count(), 4);
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "Atlantic/Azores"));
			Assert.AreEqual(1, timezones.Count(tz => tz.ID == "America/Scoresbysund"));
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
		public void GetTZVersionTest()
		{
			var version = TimeZone.GetTZDataVersion();

			Assert.True(Regex.IsMatch(version, "[0-9]{4}[a-z]"));
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
