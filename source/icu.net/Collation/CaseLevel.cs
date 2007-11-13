namespace Icu.Collation
{

	/// <summary>
	/// Controls whether an extra case level (positioned before the third
	/// level) is generated or not. Contents of the case level are affected by
	/// the value of CaseFirst enum. A simple way to ignore
	/// accent differences in a string is to set the strength to Primary
	/// and enable case level. */
	/// </summary>
	public enum CaseLevel
	{
		Default = -1,
		/// <summary>
		/// case level is not generated
		/// </summary>
		Off = 16,
		/// <summary>
		/// causes the case level to be generated.
		/// </summary>
		On = 17
	}
}
