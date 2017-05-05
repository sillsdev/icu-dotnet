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

		private static bool IsRunning64Bit
		{
			get { return IntPtr.Size == 8; }
		}

		private static string ArchSubdir
		{
			get { return IsRunning64Bit ? "x64" : "x86"; }
		}

		private static string OutputDirectory
		{
			get
			{
				return Path.GetDirectoryName(
					new Uri(typeof(NativeMethodsTests).Assembly.CodeBase).LocalPath);
			}
		}

		private static string IcuDirectory
		{
			get { return Path.Combine(OutputDirectory, "lib", ArchSubdir); }
		}

		private string RunTestHelper(string workDir, string exeDir = null)
		{
			if (string.IsNullOrEmpty(exeDir))
				exeDir = _tmpDir;

			using (var process = new Process())
			{
				process.StartInfo.RedirectStandardError = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WorkingDirectory = workDir;
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
			var path = string.Format("{0}{1}{2}", IcuDirectory, Path.PathSeparator,
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
			CopyFile(Path.Combine(IcuDirectory, "icudt54.dll"), _tmpDir);
			CopyFile(Path.Combine(IcuDirectory, "icuin54.dll"), _tmpDir);
			CopyFile(Path.Combine(IcuDirectory, "icuuc54.dll"), _tmpDir);
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo("54.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersionDifferentWorkDir()
		{
			CopyFile(Path.Combine(IcuDirectory, "icudt54.dll"), _tmpDir);
			CopyFile(Path.Combine(IcuDirectory, "icuin54.dll"), _tmpDir);
			CopyFile(Path.Combine(IcuDirectory, "icuuc54.dll"), _tmpDir);
			Assert.That(RunTestHelper(Path.GetTempPath()), Is.EqualTo("54.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersionFromArchSubDir()
		{
			var targetDir = Path.Combine(_tmpDir, ArchSubdir);
			Directory.CreateDirectory(targetDir);
			CopyFile(Path.Combine(IcuDirectory, "icudt54.dll"), targetDir);
			CopyFile(Path.Combine(IcuDirectory, "icuin54.dll"), targetDir);
			CopyFile(Path.Combine(IcuDirectory, "icuuc54.dll"), targetDir);
			Assert.That(RunTestHelper(_tmpDir), Is.EqualTo("54.1"));
		}

		[Test]
		public void LoadIcuLibrary_LoadLocalVersion_DirectoryWithSpaces()
		{
			var subdir = Path.Combine(_tmpDir, "Dir With Spaces");
			Directory.CreateDirectory(subdir);
			CopyFile(Path.Combine(_tmpDir, "TestHelper.exe"), subdir);
			CopyFile(Path.Combine(_tmpDir, "icu.net.dll"), subdir);
			CopyFile(Path.Combine(IcuDirectory, "icudt54.dll"), subdir);
			CopyFile(Path.Combine(IcuDirectory, "icuin54.dll"), subdir);
			CopyFile(Path.Combine(IcuDirectory, "icuuc54.dll"), subdir);
			Assert.That(RunTestHelper(subdir, subdir), Is.EqualTo("54.1"));
		}

	}
}
