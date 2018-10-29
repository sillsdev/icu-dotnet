// Copyright (c) 2013-2018 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if !NET40
using Microsoft.Extensions.DependencyModel;
#endif

namespace Icu
{
	/// <summary>
	/// Helper class to try and get path to icu native binaries when running on
	/// Windows or .NET Core.
	/// </summary>
	internal static class NativeMethodsHelper
	{
		private const string Icu4c = nameof(Icu4c);
		private const string IcuRegexLinux = @"libicu\w+.so\.(?<version>[0-9]{2,})(\.[0-9])*";
		private const string IcuRegexWindows = @"icu\w+(?<version>[0-9]{2,})(\.[0-9])*\.dll";

		private static readonly Regex IcuBinaryRegex = new Regex($"{IcuRegexWindows}|{IcuRegexLinux}$", RegexOptions.Compiled);
		private static readonly string IcuSearchPattern = Platform.OperatingSystem == OperatingSystemType.Windows ? "icu*.dll" : "libicu*.so.*";
		private static readonly string NugetPackageDirectory = GetDefaultPackageDirectory(Platform.OperatingSystem);

		private static IcuVersionInfo IcuVersion;

		/// <summary>
		/// Reset the member variables
		/// </summary>
		public static void Reset()
		{
			IcuVersion = null;
		}

		/// <summary>
		/// Tries to get path and version to Icu when running on .NET Core or Windows.
		/// </summary>
		/// <returns>The path and version of icu binaries if found. Check
		/// <see cref="IcuVersionInfo.Success"/> to see if the values were set.</returns>
		public static IcuVersionInfo GetIcuVersionInfoForNetCoreOrWindows()
		{
			// We've already tried to set the path to native assets. Don't try again.
			if (IcuVersion != null)
			{
				return IcuVersion;
			}

			// Set the default to IcuVersion with an empty path and no version set.
			IcuVersion = new IcuVersionInfo();

			if (TryGetPathFromAssemblyDirectory())
			{
				return IcuVersion;
			}

			// That's odd.. I guess we use normal search paths from %PATH% then.
			// One possibility is that it is not a dev machine and the application
			// is a published app... but then it should have returned true in
			// TryGetPathFromAssemblyDirectory.
			if (string.IsNullOrEmpty(NugetPackageDirectory))
			{
				Trace.TraceWarning($"{nameof(NugetPackageDirectory)} is empty and application was unable to set path from current assembly directory.");
				return IcuVersion;
			}

#if !NET40
			var context = DependencyContext.Default;
			// If this is false, something went wrong.  These files should have
			// either been found above or we should have been able to locate the
			// asset paths (for .NET Core and NuGet v3+ projects).
			if (!TryGetNativeAssetPaths(context, out string[] nativeAssetPaths))
			{
				Trace.WriteLine("Could not locate icu native assets from DependencyModel.");
				return IcuVersion;
			}

			var icuLib = context.CompileLibraries
				.Where(x => x.Name.StartsWith(Icu4c, StringComparison.OrdinalIgnoreCase))
				.FirstOrDefault();

			if (icuLib == default(CompilationLibrary))
			{
				Trace.TraceWarning("Could not find Icu4c compilation library. Possible that the library writer did not include the Icu4c.Win.Full.Lib or Icu4c.Win.Full.Lib NuGet package.");
				return IcuVersion;
			}

			if (!TryResolvePackagePath(icuLib, NugetPackageDirectory, out string packagePath))
			{
				Trace.WriteLine("Could not resolve nuget package directory....");
				return IcuVersion;
			}

			TrySetIcuPathFromDirectory(new DirectoryInfo(packagePath), nativeAssetPaths);
#endif

			return IcuVersion;
		}

		/// <summary>
		/// Tries to set the native library using the <see cref="NativeMethods.DirectoryOfThisAssembly"/>
		/// as the root directory.  The following scenarios could happen:
		/// 1. {directoryOfAssembly}/icu*.dll
		///     Occurs when the project is published using the .NET Core CLI
		///     against the full .NET framework.
		/// 2. {directoryOfAssembly}/lib/{arch}/icu*.dll
		///     Occurs when the project is using NuGet v2, the traditional
		///    .NET Framework csproj or the new VS2017 projects.
		/// 3. {directoryOfAssembly}/runtimes/{runtimeId}/native/icu*.dll
		///     Occurs when the project is published using the .NET Core CLI.
		/// If one of the scenarios match, sets the <see cref="IcuVersion"/>.
		/// </summary>
		/// <returns>True if it was able to find the icu binaries within
		/// <see cref="NativeMethods.DirectoryOfThisAssembly"/>, false otherwise.
		/// </returns>
		private static bool TryGetPathFromAssemblyDirectory()
		{
			var assemblyDirectory = new DirectoryInfo(NativeMethods.DirectoryOfThisAssembly);
			int version;

			// 1. Check in {assemblyDirectory}/
			if (TryGetIcuVersionNumber(assemblyDirectory, out version))
			{
				IcuVersion = new IcuVersionInfo(assemblyDirectory, version);
				return true;
			}

			// 2. Check in {assemblyDirectory}/lib/*{architecture}*/
			var libDirectory = Path.Combine(assemblyDirectory.FullName, "lib");
			if (Directory.Exists(libDirectory))
			{
				var candidateDirectories = Directory
					.EnumerateDirectories(libDirectory, $"*{Platform.ProcessArchitecture}*")
					.Select(x => new DirectoryInfo(x));

				foreach (var directory in candidateDirectories)
				{
					if (TryGetIcuVersionNumber(directory, out version))
					{
						IcuVersion = new IcuVersionInfo(directory, version);
						return true;
					}
				}
			}

			string[] nativeAssetPaths = null;
#if !NET40
			// 3. Check in {directoryOfAssembly}/runtimes/{runtimeId}/native/
			if (!TryGetNativeAssetPaths(DependencyContext.Default, out nativeAssetPaths))
			{
				Trace.WriteLine("Could not locate icu native assets from DependencyModel.");
				return false;
			}
#endif
			// If we found the icu*.dll files under {directoryOfAssembly}/runtimes/{rid}/native/,
			// they should ALL be there... or else something went wrong in publishing the app or
			// restoring the files, or packaging the NuGet package.
			return TrySetIcuPathFromDirectory(assemblyDirectory, nativeAssetPaths);
		}

		/// <summary>
		/// Iterates through the directory for files with the path icu*.dll and
		/// tries to fetch the icu version number from them.
		/// </summary>
		/// <returns>Returns the version number if the search was successful and
		/// null, otherwise.</returns>
		private static bool TryGetIcuVersionNumber(DirectoryInfo directory, out int icuVersion)
		{
			icuVersion = int.MinValue;

			if (directory == null || !directory.Exists)
				return false;

			var version = directory.GetFiles(IcuSearchPattern)
				.Select(x =>
				{
					var match = IcuBinaryRegex.Match(x.Name);
					if (match.Success && int.TryParse(match.Groups["version"].Value, out var retVal))
						return retVal;

					return new int?();
				})
				.OrderByDescending(x => x)
				.FirstOrDefault();

			if (version.HasValue)
				icuVersion = version.Value;

			return version.HasValue;
		}

		/// <summary>
		/// Given a root path and a set of native asset paths, tries to see if
		/// the files exist and if they do, sets <see cref="IcuVersion"/>.
		/// </summary>
		/// <param name="baseDirectory">Root path to append asset paths to.</param>
		/// <param name="nativeAssetPaths">Set of native asset paths to check.</param>
		/// <returns>true if it was able to find the directory for all the asset
		/// paths given; false otherwise.</returns>
		private static bool TrySetIcuPathFromDirectory(DirectoryInfo baseDirectory, string[] nativeAssetPaths)
		{
			if (nativeAssetPaths == null || nativeAssetPaths.Length == 0)
				return false;

			Trace.WriteLine("Assets: " + Environment.NewLine + string.Join(Environment.NewLine + "\t-", nativeAssetPaths));

			var assetPaths = nativeAssetPaths
				.Select(asset => new FileInfo(Path.Combine(baseDirectory.FullName, asset)));

			var doAllAssetsExistInDirectory = assetPaths.All(x => x.Exists);

			if (doAllAssetsExistInDirectory)
			{
				var directories = assetPaths.Select(file => file.Directory).ToArray();

				if (directories.Length > 1)
					Trace.TraceWarning($"There are multiple directories for these runtime assets: {string.Join(Path.PathSeparator.ToString(), directories.Select(x => x.FullName))}.  There should only be one... Using first directory.");

				var icuDirectory = directories.First();

				if (TryGetIcuVersionNumber(icuDirectory, out int version))
				{
					IcuVersion = new IcuVersionInfo(icuDirectory, version);
				}
			}

			return doAllAssetsExistInDirectory;
		}

#if !NET40
		/// <summary>
		/// Tries to get the icu native binaries by searching the Runtime
		/// ID graph to find the first set of paths that have those binaries.
		/// </summary>
		/// <returns>Unique relative paths to the native assets; empty if none
		/// could be found.</returns>
		private static bool TryGetNativeAssetPaths(DependencyContext context, out string[] nativeAssetPaths)
		{
			var assetPaths = Enumerable.Empty<string>();

			if (context == null)
			{
				nativeAssetPaths = assetPaths.ToArray();
				return false;
			}

			var defaultNativeAssets = context.GetDefaultNativeAssets().ToArray();

			// This goes through the runtime graph and tries to find icu native
			// asset paths matching that runtime.
			foreach (var runtime in context.RuntimeGraph)
			{
				var nativeAssets = context.GetRuntimeNativeAssets(runtime.Runtime);
				assetPaths = nativeAssets.Except(defaultNativeAssets).Where(assetPath => IcuBinaryRegex.IsMatch(assetPath));

				if (assetPaths.Any())
					break;
			}

			nativeAssetPaths = assetPaths.ToArray();

			return nativeAssetPaths.Length > 0;
		}

		/// <summary>
		/// Given a CompilationLibrary and a base path, tries to construct the
		/// nuget package location and returns true if it exists.
		///
		/// Taken from: https://github.com/dotnet/core-setup/blob/master/src/Microsoft.Extensions.DependencyModel/Resolution/ResolverUtils.cs#L12
		/// </summary>
		/// <param name="library">Compilation library to try to get the rooted
		/// path from.</param>
		/// <param name="basePath">Rooted base path to try and get library from.</param>
		/// <param name="packagePath">The path for the library if it exists;
		/// null otherwise.</param>
		/// <returns></returns>
		private static bool TryResolvePackagePath(CompilationLibrary library, string basePath, out string packagePath)
		{
			var path = library.Path;
			if (string.IsNullOrEmpty(path))
			{
				path = Path.Combine(library.Name, library.Version);
			}

			packagePath = Path.Combine(basePath, path);

			return Directory.Exists(packagePath);
		}
#endif

		/// <summary>
		/// Tries to fetch the default package directory for NuGet packages.
		/// Taken from:
		/// https://github.com/dotnet/core-setup/blob/master/src/Microsoft.Extensions.DependencyModel/Resolution/PackageCompilationAssemblyResolver.cs#L41-L64
		/// </summary>
		/// <param name="osPlatform">OS Platform to fetch default package
		/// directory for.</param>
		/// <returns>The path to the default package directory; null if none
		/// could be set.</returns>
		private static string GetDefaultPackageDirectory(OperatingSystemType osPlatform)
		{
			var packageDirectory = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

			if (!string.IsNullOrEmpty(packageDirectory))
			{
				return packageDirectory;
			}

			string basePath;
			if (osPlatform == OperatingSystemType.Windows)
			{
				basePath = Environment.GetEnvironmentVariable("USERPROFILE");
			}
			else
			{
				basePath = Environment.GetEnvironmentVariable("HOME");
			}
			if (string.IsNullOrEmpty(basePath))
			{
				return null;
			}
			return Path.Combine(basePath, ".nuget", "packages");
		}
	}
}
