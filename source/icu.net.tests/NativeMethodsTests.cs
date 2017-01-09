// Copyright (c) 2016 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	[Platform(Exclude = "Linux",
		Reason = "These tests require ICU4C installed from NuGet packages which isn't available on Linux")]
	public class NativeMethodsTests
	{
		private string _tmpDir;
		private string _pathEnvironmentVariable;

		private static void CopyFile(string srcPath, string dstDir)
		{
			var fileName = Path.GetFileName(srcPath);
			File.Copy(srcPath, Path.Combine(dstDir, fileName));
		}

		private static string OutputDirectory
		{
			get { return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath); }
		}

		private string RunTestHelper(string dir, string exeDir = null)
		{
			if (string.IsNullOrEmpty(exeDir))
				exeDir = _tmpDir;

			using (var process = new Process())
			{
				process.StartInfo.RedirectStandardError = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = dir;
				process.StartInfo.FileName = Path.Combine(exeDir, "TestHelper.exe");

				process.Start();
				var output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return output.TrimEnd('\r', '\n');
			}
		}

		[SetUp]
		public void Setup()
		{
			_tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(_tmpDir);
			var sourceDir = OutputDirectory;
			CopyFile(Path.Combine(sourceDir, "TestHelper.exe"), _tmpDir);
			CopyFile(Path.Combine(sourceDir, "icu.net.dll"), _tmpDir);

			_pathEnvironmentVariable = Environment.GetEnvironmentVariable("PATH");
			var path = string.Format("{0}{1}{2}", OutputDirectory, Path.PathSeparator,
				_pathEnvironmentVariable);
			Environment.SetEnvironmentVariable("PATH", path);
		}

		[TearDown]
		public void TearDown()
		{
			Wrapper.Cleanup();
			new DirectoryInfo(_tmpDir).Delete(true);
			Environment.SetEnvironmentVariable("PATH", _pathEnvironmentVariable);
		}

		[Test]
		public void LoadIcuLibrary_GetFromPath()
		{
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo("56.1"));
		}

		[Test]
		public void LoadIcuLibrary_GetFromPathDifferentDir()
		{
			Assert.That(RunTestHelper(Path.GetTempPath()), Is.EqualTo("56.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersion()
		{
			CopyFile(Path.Combine(OutputDirectory, "icudt54.dll"), _tmpDir);
			CopyFile(Path.Combine(OutputDirectory, "icuin54.dll"), _tmpDir);
			CopyFile(Path.Combine(OutputDirectory, "icuuc54.dll"), _tmpDir);
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo("54.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersionDifferentPath()
		{
			CopyFile(Path.Combine(OutputDirectory, "icudt54.dll"), _tmpDir);
			CopyFile(Path.Combine(OutputDirectory, "icuin54.dll"), _tmpDir);
			CopyFile(Path.Combine(OutputDirectory, "icuuc54.dll"), _tmpDir);
			Assert.That(RunTestHelper(Path.GetTempPath()), Is.EqualTo("54.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersion_DirectoryWithSpaces()
		{
			var subdir = Path.Combine(_tmpDir, "Dir With Spaces");
			Directory.CreateDirectory(subdir);
			CopyFile(Path.Combine(_tmpDir, "TestHelper.exe"), subdir);
			CopyFile(Path.Combine(_tmpDir, "icu.net.dll"), subdir);
			CopyFile(Path.Combine(OutputDirectory, "icudt54.dll"), subdir);
			CopyFile(Path.Combine(OutputDirectory, "icuin54.dll"), subdir);
			CopyFile(Path.Combine(OutputDirectory, "icuuc54.dll"), subdir);
			Assert.That(RunTestHelper(subdir, subdir), Is.EqualTo("54.1"));
		}

	}
}
