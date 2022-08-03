// Copyright (c) 2022 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

#if NET
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Icu
{
	// Mono has the concept of DllMaps in the .dll.config file that allows to map a dll name to
	// the platform specific implementation.
	// .NET Core 3.1 and .NET (>= 5.0) don't support this, but offer a different mechanism which
	// this class implements.
	// https://github.com/dotnet/runtime/blob/main/docs/design/features/dllmap.md
	internal class DllResolver
	{
		public DllResolver(Assembly assembly)
		{
			NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
		}

		// https://github.com/dotnet/samples/blob/main/core/extensions/DllMapDemo/Map.cs

		// The callback: which loads the mapped library in place of the original
		private static IntPtr MapAndLoad(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
		{
			var mappedName = MapLibraryName(assembly.Location, libraryName);
			return NativeLibrary.Load(mappedName, assembly, dllImportSearchPath);
		}

		// Parse the assembly.config file, and map the old name to the new name of a library.
		internal static string MapLibraryName(string assemblyLocation, string originalLibName)
		{
			var configFile = Path.Combine(Path.GetDirectoryName(assemblyLocation),
				Path.GetFileName(assemblyLocation) + ".config");

			if (!File.Exists(configFile))
				return originalLibName;

			var mappedLibName = originalLibName;
			var root = XElement.Load(configFile);
			var map =
				(from el in root.Elements("dllmap")
				where (string)el.Attribute("dll") == originalLibName && ConditionApplies((string)el.Attribute("os"))
				select el).SingleOrDefault();
			return map != null ? map.Attribute("target").Value : originalLibName;
		}

		internal static bool ConditionApplies(string condition)
		{
			// <dllmap os="!windows,osx" dll="libdl.so" target="libdl.so.2" />
			// <dllmap os="osx" dll="libdl.so" target="libdl.dylib"/>

			var negate = condition.StartsWith("!");
			if (negate)
				condition = condition.Substring(1);
			var retVal = negate;
			foreach (var platform in condition.Split(','))
			{
				var osPlatform = OSPlatform.Create(platform.Trim());
				if (negate)
					retVal &= !RuntimeInformation.IsOSPlatform(osPlatform);
				else
					retVal |= RuntimeInformation.IsOSPlatform(osPlatform);
			}
			return retVal;
		}
	}
}
#endif
