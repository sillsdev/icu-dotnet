// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
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