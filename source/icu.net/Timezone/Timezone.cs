using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	public class TimeZone
	{
		private readonly string zoneID;

		public string ID => zoneID;

		public TimeZone(string id)
		{
			zoneID = id;
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones with the given filter conditions. 
		/// </summary>
		/// <param name="zoneType">The system time zone type.</param>
		/// <param name="region">The ISO 3166 two-letter country code or UN M.49 three-digit area code. When NULL, no filtering done by region.</param>
		public static IEnumerable<TimeZone> GetTimeZones(USystemTimeZoneType zoneType, string region)
		{
			List<TimeZone> timeZones = new List<TimeZone>();

			SafeEnumeratorHandle en = NativeMethods.
				ucal_openTimeZoneIDEnumeration(zoneType,
											   region,
											   out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			try
			{
				string str = en.Next();
				while (str != null)
				{
					timeZones.Add(new TimeZone(str));
					str = en.Next();
				}
			}
			finally
			{
				en.Dispose();
			}
			return timeZones;
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones with the given filter conditions. 
		/// </summary>
		/// <param name="zoneType">The system time zone type.</param>
		/// <param name="region">The ISO 3166 two-letter country code or UN M.49 three-digit area code. When NULL, no filtering done by region.</param>
		/// <param name="zoneOffset">An offset from GMT in milliseconds, ignoring the effect of daylight savings time, if any. When NULL, no filtering done by zone offset.</param>
		public static IEnumerable<TimeZone> GetTimeZones(USystemTimeZoneType zoneType, string region, int zoneOffset)
		{
			List<TimeZone> timeZones = new List<TimeZone>();
			
			SafeEnumeratorHandle en = NativeMethods.
				ucal_openTimeZoneIDEnumeration(zoneType,
											   region,
											   ref zoneOffset,
											   out ErrorCode ec);
			

			ExceptionFromErrorCode.ThrowIfError(ec);
			try
			{
				string str = en.Next();
				while (str != null)
				{
					timeZones.Add(new TimeZone(str));
					str = en.Next();
				}
			}
			finally
			{
				en.Dispose();
			}
			return timeZones;
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones. 
		/// </summary>
		public static IEnumerable<TimeZone> GetTimeZones()
		{
			List<TimeZone> timeZones = new List<TimeZone>();

			SafeEnumeratorHandle en = NativeMethods.ucal_openTimeZones(out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			try
			{
				string str = en.Next();
				while (str != null)
				{
					timeZones.Add(new TimeZone(str));
					str = en.Next();
				}
			}
			finally
			{
				en.Dispose();
			}
			return timeZones;
		}

		/// <summary>
		/// Returns an enumeration over TimeZones associated with the given country.
		/// </summary>
		/// <param name="country">The ISO 3166 two-letter country code, or NULL to retrieve zones not affiliated with any country.</param>
		public static IEnumerable<TimeZone> GetCountryTimeZones(string country)
		{
			List<TimeZone> timeZones = new List<TimeZone>();

			SafeEnumeratorHandle en = NativeMethods.ucal_openCountryTimeZones(country, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			try
			{
				string str = en.Next();
				while (str != null)
				{
					timeZones.Add(new TimeZone(str));
					str = en.Next();
				}
			}
			finally
			{
				en.Dispose();
			}
			return timeZones;
		}

		/// <summary>
		/// Return the default time zone. 
		/// </summary>
		/// <returns>Default timezone.</returns>
		public static TimeZone GetDefault()
		{
			var timeZoneName = NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.ucal_getDefaultTimeZone(ptr, length, out ErrorCode err);
				return new Tuple<ErrorCode, int>(err, length);
			});
			return new TimeZone(timeZoneName);
		}

		/// <summary>
		/// Sets the default time zone to be the specified time zone. 
		/// </summary>
		/// <param name="timezone">The given timezone. </param>
		public static void SetDefault(TimeZone timezone)
		{
			NativeMethods.ucal_setDefaultTimeZone(timezone.ID, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
		}

		/// <summary>
		/// Returns the amount of time to be added to local standard time to get local wall clock time.
		/// </summary>
		/// <returns>the amount of saving time in milliseconds</returns>
		public int GetDstSavings()
		{
			int result = NativeMethods.ucal_getDSTSavings(zoneID, out ErrorCode ec);
			ExceptionFromErrorCode.ThrowIfError(ec);
			return result;
		}

		/// <summary>
		/// Converts a system time zone ID to an equivalent Windows time zone ID.
		///
		/// For example, Windows time zone ID "Pacific Standard Time" is returned for input "America/Los_Angeles".
		/// </summary>
		/// <param name="id">A system time zone ID. </param>
		/// <returns>A Windows time zone ID.</returns>
		public static string GetWindowsId(string id)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.ucal_getWindowsTimeZoneId(id, ptr, length, out ErrorCode err);
				return new Tuple<ErrorCode, int>(err, length);
			});
		}

		/// <summary>
		/// Converts a Windows time zone ID to an equivalent system time zone ID for a region.
		///
		/// For example, system time zone ID "America/Los_Angeles" is returned for input Windows ID "Pacific Standard Time" and region "US" (or null), "America/Vancouver" is returned for the same Windows ID "Pacific Standard Time" and region "CA".
		/// </summary>
		/// <param name="winId">A Windows time zone ID.</param>
		/// <param name="region">region code, or NULL if no regional preference.</param>
		/// <returns>A system time zone ID</returns>
		public static string GetIdForWindowsId(string winId, string region)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = NativeMethods.ucal_getTimeZoneIDForWindowsId(winId, region, ptr, length, out ErrorCode err);
				return new Tuple<ErrorCode, int>(err, length);
			});
		}
	}
}
