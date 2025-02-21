// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	/// <summary>
	/// A Locale object represents a specific geographical, political, or cultural region.
	/// </summary>
	public class Locale	: ICloneable
	{
		/// <summary>
		/// Construct a default locale object, a Locale for the default locale ID
		/// </summary>
		public Locale()
		{
			Id = Canonicalize(CultureInfo.CurrentUICulture.Name);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Icu.Locale"/> class.
		/// <paramref name="localeId"/> can either be a full locale like "en-US", or a language.
		/// </summary>
		/// <param name="localeId">Locale.</param>
		public Locale(string localeId)
		{
			Id = Canonicalize(localeId);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Icu.Locale"/> class.
		/// </summary>
		/// <param name="language">Language.</param>
		/// <param name="country">Country or script.</param>
		public Locale(string language, string country):
			this (language, country, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Locale"/> class.
		/// </summary>
		/// <param name="language">Lowercase two-letter or three-letter ISO-639 code.</param>
		/// <param name="country">Uppercase two-letter ISO-3166 code.</param>
		/// <param name="variant">Uppercase vendor and browser specific code.</param>
		public Locale(string language, string country, string variant):
			this (language, country, variant, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Locale"/> class.
		/// </summary>
		/// <param name="language">Lowercase two-letter or three-letter ISO-639 code.</param>
		/// <param name="country">Uppercase two-letter ISO-3166 code.</param>
		/// <param name="variant">Uppercase vendor and browser specific code.</param>
		/// <param name="keywordsAndValues">A string consisting of keyword/values pairs, such as "collation=phonebook;currency=euro"</param>
		public Locale(string language, string country, string variant, string keywordsAndValues)
		{
			var bldr = new StringBuilder(language);
			if (!string.IsNullOrEmpty(country) || !string.IsNullOrEmpty(variant))
			{
				bldr.AppendFormat("_{0}", country);
			}
			if (!string.IsNullOrEmpty(variant))
			{
				bldr.AppendFormat("_{0}", variant);
			}
			if (!string.IsNullOrEmpty(keywordsAndValues))
			{
				bldr.AppendFormat("@{0}", keywordsAndValues);
			}
			Id = Canonicalize(bldr.ToString());
		}

		/// <summary>
		/// Clone this object. (Not Implemented.)
		/// </summary>
		public object Clone()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets or sets the identifier of the ICU locale.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Gets the two-letter language code as defined by ISO-639.
		/// </summary>
		public string Language
		{
			get { return GetString(NativeMethods.uloc_getLanguage, Id); }
		}

		/// <summary>
		/// Gets the script code as defined by ISO-15924.
		/// </summary>
		public string Script
		{
			get { return GetString(NativeMethods.uloc_getScript, Id); }
		}

		/// <summary>
		/// Gets the two-letter country code as defined by ISO-3166.
		/// </summary>
		public string Country
		{
			get { return GetString(NativeMethods.uloc_getCountry, Id); }
		}

		/// <summary>
		/// Gets the variant code for the Locale.
		/// </summary>
		public string Variant
		{
			get { return GetString(NativeMethods.uloc_getVariant, Id); }
		}

		/// <summary>
		/// Gets the full name for the specified locale.
		/// </summary>
		public string Name
		{
			get { return GetString(NativeMethods.uloc_getName, Id); }
		}

		/// <summary>
		/// Gets the full name for the specified locale, like <see cref="Name"/>,
		/// but without keywords.
		/// </summary>
		public string BaseName
		{
			get { return GetString(NativeMethods.uloc_getBaseName, Id); }
		}

		/// <summary>
		/// Gets the 3-letter ISO 639-3 language code
		/// </summary>
		public string Iso3Language
		{
			get { return Marshal.PtrToStringAnsi(NativeMethods.uloc_getISO3Language(Id)); }
		}

		/// <summary>
		/// Gets the 3-letter ISO country code
		/// </summary>
		public string Iso3Country
		{
			get { return Marshal.PtrToStringAnsi(NativeMethods.uloc_getISO3Country(Id)); }
		}

		/// <summary>
		/// Gets the Win32 LCID value for the specified locale.
		/// </summary>
		public int Lcid
		{
			get { return NativeMethods.uloc_getLCID(Id); }
		}

		/// <summary>
		/// Gets the language for the UI's locale.
		/// </summary>
		public string DisplayLanguage
		{
			get { return GetDisplayLanguage(new Locale().Id); }
		}

		/// <summary>
		/// Gets the language name suitable for display for the specified locale
		/// </summary>
		/// <param name="displayLocale">The display locale</param>
		public string GetDisplayLanguage(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayLanguage, Id, displayLocale.Id);
		}

		/// <summary>
		/// Gets the country for the UI's locale.
		/// </summary>
		public string DisplayCountry
		{
			get { return GetDisplayCountry(new Locale().Id); }
		}

		/// <summary>
		/// Gets the country name suitable for display for the specified locale.
		/// </summary>
		/// <param name="displayLocale">The display locale</param>
		public string GetDisplayCountry(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayCountry, Id, displayLocale.Id);
		}

		/// <summary>
		/// Gets the full name for the UI's locale.
		/// </summary>
		public string DisplayName
		{
			get { return GetDisplayName(new Locale().Id); }
		}

		/// <summary>
		/// Gets the full name suitable for display for the specified locale.
		/// </summary>
		/// <param name="displayLocale">The display locale</param>
		public string GetDisplayName(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayName, Id, displayLocale.Id);
		}

		/// <summary>
		/// Gets the Locale's <see cref="Id"/> as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}

		/// <summary>
		/// Gets a list of all available locales.
		/// </summary>
		public static List<Locale> AvailableLocales
		{
			get
			{
				var locales = new List<Locale>();
				for (int i = 0; i < NativeMethods.uloc_countAvailable(); i++)
					locales.Add(new Locale(Marshal.PtrToStringAnsi(NativeMethods.uloc_getAvailable(i))));
				return locales;
			}
		}

		/// <summary>
		/// Creates a Locale with the given Locale id.
		/// </summary>
		/// <param name="localeId">Locale id</param>
		public static implicit operator Locale(string localeId)
		{
			return new Locale(localeId);
		}

		/// <summary>
		/// Gets the locale for the specified Win32 LCID value
		/// </summary>
		/// <param name="lcid">the Win32 LCID to translate </param>
		/// <returns>The locale for the specified Win32 LCID value, or <c>null</c> in the
		/// case of an error.</returns>
		public static Locale GetLocaleForLCID(int lcid)
		{
			var localeId = GetString(NativeMethods.uloc_getLocaleForLCID, lcid);
			return !string.IsNullOrEmpty(localeId) ? new Locale(localeId) : null;
		}

		#region ICU wrapper methods

		private delegate int GetStringMethod<in T>(T localeId, IntPtr name,
			int nameCapacity, out ErrorCode err);
		private delegate int GetStringDisplayMethod(string localeID, string displayLocaleID, IntPtr name,
			int nameCapacity, out ErrorCode err);

		private static string GetString<T>(GetStringMethod<T> method, T localeId)
		{
			return NativeMethods.GetAnsiString((ptr, length) =>
			{
				length = method(localeId, ptr, length, out var err);
				return new Tuple<ErrorCode, int>(err, length);
			});
		}

		private static string GetString(GetStringDisplayMethod method, string localeId, string displayLocaleId)
		{
			return NativeMethods.GetUnicodeString((ptr, length) =>
			{
				length = method(localeId, displayLocaleId, ptr, length, out var err);
				return new Tuple<ErrorCode, int>(err, length);
			});
		}

		/// <summary>
		/// Gets the full name for the specified locale.
		/// </summary>
		private static string Canonicalize(string localeID)
		{
			return GetString(NativeMethods.uloc_canonicalize, localeID);
		}
		#endregion
	}
}
