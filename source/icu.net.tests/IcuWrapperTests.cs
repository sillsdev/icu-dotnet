// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class IcuWrapperTests
	{
		private string _tmpDir;

		private string CopyIcuDllsToTempDirectory(string directory, string icuVersion)
		{
			if (string.IsNullOrEmpty(_tmpDir))
			{
				_tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				Directory.CreateDirectory(_tmpDir);
			}

			var targetDir = Path.Combine(_tmpDir, directory, "lib", NativeMethodsTests.GetArchSubdir("win-"));
			Directory.CreateDirectory(targetDir);

			NativeMethodsTests.CopyFile(Path.Combine(NativeMethodsTests.IcuDirectory, $"icudt{icuVersion}.dll"),
				targetDir);
			NativeMethodsTests.CopyFile(Path.Combine(NativeMethodsTests.IcuDirectory, $"icuin{icuVersion}.dll"), targetDir);
			NativeMethodsTests.CopyFile(Path.Combine(NativeMethodsTests.IcuDirectory, $"icuuc{icuVersion}.dll"), targetDir);

			return targetDir;
		}

		private static string RunTestHelper(string exeDir, string desiredIcuDirectory, string icuVersion)
		{
			using var process = new Process();
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = exeDir;
			var filename = Path.Combine(exeDir, "TestHelper.exe");
			if (File.Exists(filename))
			{
				process.StartInfo.Arguments = $"{Wrapper.MinSupportedIcuVersion} {NativeMethodsTests.MaxInstalledIcuLibraryVersion} {icuVersion} {desiredIcuDirectory}";
			}
			else
			{
				// netcore
				process.StartInfo.Arguments = $"{Path.Combine(exeDir, "TestHelper.dll")} {Wrapper.MinSupportedIcuVersion} {NativeMethodsTests.MaxInstalledIcuLibraryVersion} {icuVersion} {desiredIcuDirectory}";
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

		[TearDown]
		public void TearDown()
		{
			Wrapper.ConfineIcuVersions(Wrapper.MinSupportedIcuVersion, Wrapper.MaxSupportedIcuVersion);
			NativeMethodsTests.DeleteDirectory(_tmpDir);
			_tmpDir = null;
			Wrapper.SetPreferredIcu4cDirectory(null);
		}

		[Test]
		public void UnicodeVersion()
		{
			var result = Wrapper.UnicodeVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(3));
			Assert.That(result.IndexOf("."), Is.GreaterThan(0));
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".")), out var major), Is.True);
		}

		[Test]
		public void IcuVersion()
		{
			var result = Wrapper.IcuVersion;
			Assert.That(result.Length, Is.GreaterThanOrEqualTo(4));
			Assert.That(result.IndexOf(".", StringComparison.Ordinal), Is.GreaterThan(0));
			Assert.That(int.TryParse(result.Substring(0, result.IndexOf(".", StringComparison.Ordinal)), out var major), Is.True);
		}

		[Platform(Include = "Win",
			Reason = "These tests require ICU4C installed from NuGet packages which is only available on Windows")]
		[Test]
		public void ConfineVersions_WorksAfterInit()
		{
			Wrapper.Init();
			var version = int.Parse(NativeMethodsTests.MinIcuLibraryVersionMajor);
			Wrapper.DataDirectory = "Test";
			Wrapper.ConfineIcuVersions(version);

			Assert.That(Wrapper.IcuVersion, Is.EqualTo(NativeMethodsTests.MinIcuLibraryVersion));
			Assert.That(Wrapper.DataDirectory, Is.EqualTo("Test"));
		}

		[Platform(Include = "Win",
			Reason = "These tests require ICU4C installed from NuGet packages which is only available on Windows")]
		[Test]
		public void ConfineVersions_LoadFromDifferentDirectory_LowerVersion()
		{
			// Sanity check
			Assert.That(NativeMethodsTests.MinIcuLibraryVersionMajor,
				Is.LessThan(NativeMethodsTests.FullIcuLibraryVersionMajor));

			// Setup
			var minIcuVersionDirectory = CopyIcuDllsToTempDirectory(Path.GetRandomFileName(),
				NativeMethodsTests.MinIcuLibraryVersionMajor);
			var testDirectory = Path.GetRandomFileName();
			CopyIcuDllsToTempDirectory(testDirectory, NativeMethodsTests.FullIcuLibraryVersionMajor);
			var fullTestDirectory = Path.Combine(_tmpDir, testDirectory);
			NativeMethodsTests.CopyTestFiles(NativeMethodsTests.OutputDirectory, fullTestDirectory);

			// Execute
			var result = RunTestHelper(fullTestDirectory, minIcuVersionDirectory,
				NativeMethodsTests.MinIcuLibraryVersionMajor);

			// Verify
			Assert.That(result, Is.EqualTo(NativeMethodsTests.MinIcuLibraryVersion));
		}

		[Platform(Include = "Win",
			Reason = "These tests require ICU4C installed from NuGet packages which is only available on Windows")]
		[Test]
		public void ConfineVersions_LoadFromDifferentDirectory_HigherVersion()
		{
			// Sanity check
			Assert.That(NativeMethodsTests.MinIcuLibraryVersionMajor,
				Is.LessThan(NativeMethodsTests.FullIcuLibraryVersionMajor));

			// Setup
			var fullIcuVersionDirectory = CopyIcuDllsToTempDirectory(Path.GetRandomFileName(),
				NativeMethodsTests.FullIcuLibraryVersionMajor);
			var testDirectory = Path.GetRandomFileName();
			CopyIcuDllsToTempDirectory(testDirectory, NativeMethodsTests.MinIcuLibraryVersionMajor);
			var fullTestDirectory = Path.Combine(_tmpDir, testDirectory);
			NativeMethodsTests.CopyTestFiles(NativeMethodsTests.OutputDirectory, fullTestDirectory);

			// Execute
			var result = RunTestHelper(fullTestDirectory, fullIcuVersionDirectory,
				NativeMethodsTests.FullIcuLibraryVersionMajor);

			// Verify
			Assert.That(result, Is.EqualTo(NativeMethodsTests.FullIcuLibraryVersion));
		}

		[Platform(Include = "Win",
			Reason = "These tests require ICU4C installed from NuGet packages which is only available on Windows")]
		[Test]
		public void ConfineVersions_LoadFromDifferentDirectory_NotInPreferredDir()
		{
			// Sanity check
			Assert.That(NativeMethodsTests.MinIcuLibraryVersionMajor,
				Is.LessThan(NativeMethodsTests.FullIcuLibraryVersionMajor));

			// Setup
			var fullIcuVersionDirectory = CopyIcuDllsToTempDirectory(Path.GetRandomFileName(),
				NativeMethodsTests.FullIcuLibraryVersionMajor);
			var testDirectory = Path.GetRandomFileName();
			CopyIcuDllsToTempDirectory(testDirectory, NativeMethodsTests.MinIcuLibraryVersionMajor);
			var fullTestDirectory = Path.Combine(_tmpDir, testDirectory);
			NativeMethodsTests.CopyTestFiles(NativeMethodsTests.OutputDirectory, fullTestDirectory);

			// Execute
			var result = RunTestHelper(fullTestDirectory, fullIcuVersionDirectory,
				NativeMethodsTests.MinIcuLibraryVersionMajor);

			// Verify
			Assert.That(result, Is.EqualTo(NativeMethodsTests.MinIcuLibraryVersion));
		}
	}
}
