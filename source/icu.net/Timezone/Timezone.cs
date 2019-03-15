using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Icu
{
	public class TimeZone
	{
		private readonly string zoneId;

		/// <summary>
		/// Returns TimeZone's Id.
		/// </summary>
		public string Id => zoneId;

		/// <summary>
		/// Creates a TimeZone for the given <paramref name="id"/>.
		/// </summary>
		/// <param name="id">The Id for a TimeZone, such as "America/Los_Angeles"</param>
		public TimeZone(string id)
		{
			zoneId = id;
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones with the given filter conditions.
		/// </summary>
		/// <param name="zoneType">The system time zone type.</param>
		/// <param name="region">The ISO 3166 two-letter country code or UN M.49 three-digit area code. When NULL, no filtering done by region.</param>
		public static IEnumerable<TimeZone> GetTimeZones(USystemTimeZoneType zoneType, string region)
		{
			return CreateTimeZoneList(() =>
			{
				var en = NativeMethods.ucal_openTimeZoneIDEnumeration(zoneType, region, out ErrorCode errorCode);
				return new Tuple<SafeEnumeratorHandle, ErrorCode>(en, errorCode);
			});
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones with the given filter conditions.
		/// </summary>
		/// <param name="zoneType">The system time zone type.</param>
		/// <param name="region">The ISO 3166 two-letter country code or UN M.49 three-digit area code. When NULL, no filtering done by region.</param>
		/// <param name="zoneOffset">An offset from GMT in milliseconds, ignoring the effect of daylight savings time, if any. When NULL, no filtering done by zone offset.</param>
		public static IEnumerable<TimeZone> GetTimeZones(USystemTimeZoneType zoneType, string region, int zoneOffset)
		{
			return CreateTimeZoneList(() =>
			{
				var en = NativeMethods.ucal_openTimeZoneIDEnumeration(zoneType, region, ref zoneOffset, out ErrorCode errorCode);
				return new Tuple<SafeEnumeratorHandle, ErrorCode>(en, errorCode);
			});
		}

		/// <summary>
		/// Returns an enumeration over system TimeZones.
		/// </summary>
		public static IEnumerable<TimeZone> GetTimeZones()
		{
			return CreateTimeZoneList(() =>
			{
				var en = NativeMethods.ucal_openTimeZones(out ErrorCode errorCode);
				return new Tuple<SafeEnumeratorHandle, ErrorCode>(en, errorCode);
			});
		}

		/// <summary>
		/// Returns an enumeration over TimeZones associated with the given country.
		/// </summary>
		/// <param name="country">The ISO 3166 two-letter country code, or NULL to retrieve zones not affiliated with any country.</param>
		public static IEnumerable<TimeZone> GetCountryTimeZones(string country)
		{
			return CreateTimeZoneList(() =>
			{
				var en = NativeMethods.ucal_openCountryTimeZones(country, out ErrorCode errorCode);
				return new Tuple<SafeEnumeratorHandle, ErrorCode>(en, errorCode);
			});
		}

		private static IEnumerable<TimeZone> CreateTimeZoneList(Func<Tuple<SafeEnumeratorHandle, ErrorCode>> enumeratorSource)
		{
			List<TimeZone> timeZones = new List<TimeZone>();

			(SafeEnumeratorHandle enumerator, ErrorCode errorCode) = enumeratorSource();
			ExceptionFromErrorCode.ThrowIfError(errorCode);
			try
			{
				string timezoneId = enumerator.Next();
				while (timezoneId != null)
				{
					timeZones.Add(new TimeZone(timezoneId));
					timezoneId = enumerator.Next();
				}
			}
			finally
			{
				enumerator.Dispose();
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
				length = NativeMethods.ucal_getDefaultTimeZone(ptr, length, out ErrorCode errorCode);
				return new Tuple<ErrorCode, int>(errorCode, length);
			});
			return new TimeZone(timeZoneName);
		}

		/// <summary>
		/// Sets the default time zone to be the specified time zone.
		/// </summary>
		/// <param name="timezone">The given timezone. </param>
		public static void SetDefault(TimeZone timezone)
		{
			NativeMethods.ucal_setDefaultTimeZone(timezone.Id, out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
		}

		/// <summary>
		/// Returns the amount of time to be added to local standard time to get local wall clock time.
		/// </summary>
		/// <returns>the amount of saving time in milliseconds</returns>
		public int GetDstSavings()
		{
			int result = NativeMethods.ucal_getDSTSavings(zoneId, out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
			return result;
		}

		/// <summary>
		/// Returns the timezone data version currently used by ICU.
		/// </summary>
		/// <returns>the version string, such as "2007f"</returns>
		public static string GetTZDataVersion()
		{
			var ptr = NativeMethods.ucal_getTZDataVersion(out ErrorCode errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);

			return Marshal.PtrToStringAnsi(ptr);
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
				length = NativeMethods.ucal_getWindowsTimeZoneId(id, ptr, length, out ErrorCode errorCode);
				return new Tuple<ErrorCode, int>(errorCode, length);
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
				length = NativeMethods.ucal_getTimeZoneIDForWindowsId(winId, region, ptr, length, out ErrorCode errorCode);
				return new Tuple<ErrorCode, int>(errorCode, length);
			});
		}
	}
}
