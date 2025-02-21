// Copyright (c) 2016-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Icu.Tests
{
	[Platform(Include = "Win",
		Reason = "These tests require ICU4C installed from NuGet packages which is only available on Windows")]
	[TestFixture]
	public class NativeMethodsTests
	{
		private      string _tmpDir;
		private      string _pathEnvironmentVariable;
		public const string FullIcuLibraryVersion      = "62.1";
		public const string FullIcuLibraryVersionMajor = "62";
		public const string MinIcuLibraryVersion       = "59.1";
		public const string MinIcuLibraryVersionMajor  = "59";

		internal static int MaxInstalledIcuLibraryVersion
		{
			get
			{
				var fullVersion = int.Parse(FullIcuLibraryVersionMajor);
				var minVersion = int.Parse(MinIcuLibraryVersionMajor);
				return Math.Max(fullVersion, minVersion);
			}
		}

		internal static void CopyFile(string srcPath, string dstDir)
		{
			var fileName = Path.GetFileName(srcPath);
			File.Copy(srcPath, Path.Combine(dstDir, fileName));
		}

		private static void CopyFilesFromDirectory(string srcPath, string dstDir)
		{
			var srcDirectory = new DirectoryInfo(srcPath);
			foreach (var file in srcDirectory.GetFiles())
			{
				CopyFile(file.FullName, dstDir);
			}
		}

		internal static string GetArchSubdir(string prefix = "")
		{
			var archSubdir = Environment.Is64BitOperatingSystem ? "x64" : "x86";
			return $"{prefix}{archSubdir}";
		}

		internal static string OutputDirectory => Path.GetDirectoryName(
			new Uri(
#if NET40
				typeof(NativeMethodsTests).Assembly.CodeBase
#elif NET5_0_OR_GREATER
				typeof(NativeMethodsTests).GetTypeInfo().Assembly.Location
#else
				typeof(NativeMethodsTests).GetTypeInfo().Assembly.CodeBase
#endif
				)
			.LocalPath);

		internal static string IcuDirectory
		{
			get
			{
				var directory = Path.Combine(OutputDirectory, "lib", GetArchSubdir("win-"));
				if (Directory.Exists(directory))
					return directory;
				directory = Path.Combine(OutputDirectory, "runtimes", GetArchSubdir("win7-"), "native");
				if (Directory.Exists(directory))
					return directory;

				throw new DirectoryNotFoundException("Can't find ICU directory");
			}
		}

		private string RunTestHelper(string workDir, string exeDir = null)
		{
			if (string.IsNullOrEmpty(exeDir))
				exeDir = _tmpDir;

			using var process = new Process();
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = workDir;
			var filename = Path.Combine(exeDir, "TestHelper.exe");
			if (File.Exists(filename))
			{
				process.StartInfo.Arguments = $"{Wrapper.MinSupportedIcuVersion} {MaxInstalledIcuLibraryVersion}";
			}
			else
			{
				// netcore
				process.StartInfo.Arguments = $"{Path.Combine(exeDir, "TestHelper.dll")} {Wrapper.MinSupportedIcuVersion} {MaxInstalledIcuLibraryVersion}";
				filename = "dotnet";
			}

			process.StartInfo.FileName = filename;

			process.Start();
			var output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				Console.WriteLine(process.StandardError.ReadToEnd());
			}
			return output.TrimEnd('\r', '\n');
		}

		private static void CopyMinimalIcuFiles(string targetDir)
		{
			CopyFile(Path.Combine(IcuDirectory, $"icudt{MinIcuLibraryVersionMajor}.dll"), targetDir);
			CopyFile(Path.Combine(IcuDirectory, $"icuin{MinIcuLibraryVersionMajor}.dll"), targetDir);
			CopyFile(Path.Combine(IcuDirectory, $"icuuc{MinIcuLibraryVersionMajor}.dll"), targetDir);
		}

		internal static void CopyTestFiles(string sourceDir, string targetDir)
		{
			// sourceDir is something like output/Debug/net461, TestHelper is in output/Debug/TestHelper/net461
			var framework = Path.GetFileName(sourceDir);
			sourceDir = Path.Combine(sourceDir, "..", "TestHelper", framework);
			CopyFilesFromDirectory(sourceDir, targetDir);
		}

		[SetUp]
		public void Setup()
		{
			_tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(_tmpDir);
			CopyTestFiles(OutputDirectory, _tmpDir);

			_pathEnvironmentVariable = Environment.GetEnvironmentVariable("PATH");
			var path = $"{IcuDirectory}{Path.PathSeparator}{_pathEnvironmentVariable}";
			Environment.SetEnvironmentVariable("PATH", path);
		}

		[TearDown]
		public void TearDown()
		{
			Wrapper.Cleanup();
			DeleteDirectory(_tmpDir);

			Environment.SetEnvironmentVariable("PATH", _pathEnvironmentVariable);
		}

		internal static void DeleteDirectory(string tmpDir)
		{
			if (string.IsNullOrEmpty(tmpDir))
				return;

			try
			{
				Directory.Delete(tmpDir, true);
			}
			catch (IOException e)
			{
				Console.WriteLine(
					$"IOException trying to delete temporary directory {tmpDir}: {e.Message}");
				foreach (var f in new DirectoryInfo(tmpDir).EnumerateFileSystemInfos())
				{
					try
					{
						if (f is DirectoryInfo directoryInfo)
							directoryInfo.Delete(true);
						else
							f.Delete();
					}
					catch (Exception)
					{
						// just ignore - not worth failing the test if we can't delete
						// a temporary file
						Console.WriteLine($"Can't delete {f.Name}");
					}
				}

				// Try again to delete the directory
				try
				{
					Directory.Delete(tmpDir, true);
				}
				catch (Exception)
				{
					// just ignore - not worth failing the test if we can't delete
					// a temporary directory
					Console.WriteLine($"Still can't delete {tmpDir}");
				}
			}
		}

		[Test]
		public void LoadIcuLibrary_GetFromPath()
		{
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo(FullIcuLibraryVersion));
		}

		[Test]
		public void LoadIcuLibrary_GetFromPathDifferentDir()
		{
			Assert.That(RunTestHelper(Path.GetTempPath()), Is.EqualTo(FullIcuLibraryVersion));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersion()
		{
			CopyMinimalIcuFiles(_tmpDir);
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo(MinIcuLibraryVersion));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersionDifferentWorkDir()
		{
			CopyMinimalIcuFiles(_tmpDir);
			Assert.That(RunTestHelper(Path.GetTempPath()), Is.EqualTo(MinIcuLibraryVersion));
		}

		[TestCase("")]
		[TestCase("win-")]
		public void LoadIcuLibrary_LoadLocalVersionFromArchSubDir(string prefix)
		{
			var targetDir = Path.Combine(_tmpDir, GetArchSubdir(prefix));
			Directory.CreateDirectory(targetDir);
			CopyMinimalIcuFiles(targetDir);
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo(MinIcuLibraryVersion));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersion_DirectoryWithSpaces()
		{
			var subdir = Path.Combine(_tmpDir, "Dir With Spaces");
			Directory.CreateDirectory(subdir);
			CopyTestFiles(OutputDirectory, subdir);
			CopyMinimalIcuFiles(subdir);
			Assert.That(RunTestHelper(subdir, subdir), Is.EqualTo(MinIcuLibraryVersion));
		}

	}
}
