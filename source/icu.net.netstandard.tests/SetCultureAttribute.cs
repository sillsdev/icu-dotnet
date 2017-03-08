using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Icu.Tests
{
	/// <summary>
	/// TODO: Remove this when NUnit adds this to its .NETStandard packages.
	/// Taken from:
	/// https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Attributes/SetCultureAttribute.cs
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class SetCultureAttribute : PropertyAttribute, IApplyToContext
	{
		private string _culture;

		/// <summary>
		/// Construct given the name of a culture
		/// </summary>
		/// <param name="culture"></param>
		public SetCultureAttribute(string culture) : base("SetCulture", culture)
		{
			_culture = culture;
		}

		#region IApplyToContext Members

		void IApplyToContext.ApplyToContext(TestExecutionContext context)
		{
			context.CurrentCulture = new System.Globalization.CultureInfo(_culture);
		}

		#endregion
	}
}
