
namespace Icu.Collation
{
	/// <summary>
	/// When turned on, this attribute generates a collation key
	/// for the numeric value of substrings of digits.
	/// This is a way to get '100' to sort AFTER '2'.
	/// </summary>
	public enum NumericCollation
	{
		Default = -1,
		Off = 16,
		On = 17
	}
}
