namespace Icu
{
	/// <summary>
	/// Specifies the type of system time zones to enumerate, used as a filter in
	/// <see cref="TimeZone.GetTimeZones(USystemTimeZoneType, string)"/>.
	/// </summary>
	/// <seealso href="https://unicode-org.github.io/icu-docs/apidoc/released/icu4c/ucal_8h.html#a246d867677ec1a02775072aa0b5b018a"/>
	public enum USystemTimeZoneType
	{
		/// <summary>Any system zones.</summary>
		Any,
		/// <summary>Canonical system zones.</summary>
		Canonical,
		/// <summary>Canonical system zones associated with actual locations.</summary>
		CanonicalLocation,
	}
}
