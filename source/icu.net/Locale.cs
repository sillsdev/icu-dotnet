using System;

namespace Icu
{
	public class Locale:ICloneable
	{
		/// <summary>
		/// Construct a default locale object, a Locale for the default locale ID
		/// </summary>
		public Locale()
		{

		}

		public Locale(string language) :
		  this (language, null, null, null){}

		public Locale(string language, string country):
		  this (language, country, null, null){}

		public Locale(string language, string country, string variant):
		  this (language, country, variant, null){}

		public Locale(string language, string country, string variant, string keywordsAndValues)
		{

		}

		~Locale() {}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		public string Language
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Script
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Country
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Variant
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string BaseName
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Iso3Language
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Iso3Country
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string Lcid
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string DisplayLanguage
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public string GetDisplayLanguage(Locale displayLocale)
		{
			throw new NotImplementedException();
		}

	}
}
