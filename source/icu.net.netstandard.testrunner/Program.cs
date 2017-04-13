using Icu.Tests;
using NUnitLite;
using System.Reflection;

namespace icu.net.netstandard.testrunner
{
	class Program
	{
		public static int Main(string[] args)
		{
			var assembly = typeof(BreakIteratorTests).GetTypeInfo().Assembly;
			return new AutoRun(assembly).Execute(args);
		}
	}
}