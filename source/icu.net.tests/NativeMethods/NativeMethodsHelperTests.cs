// Copyright (c) 2018-2026 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Icu.Tests
{
#if !NET40
	[TestFixture]
	public class NativeMethodsHelperTests
	{
		private static bool IsMac => Platform.OperatingSystem == OperatingSystemType.MacOSX;

		private string _filenameWindows;
		private string _filenameLinux;
		private string _filenameMac;

		private int CallGetIcuVersionInfoForNetCoreOrWindows()
		{
			var icunetAssembly = typeof(Wrapper).Assembly;
			var nativeMethodsHelperType = icunetAssembly.GetType("Icu.NativeMethodsHelper");
			var method = nativeMethodsHelperType.GetMethod("GetIcuVersionInfoForNetCoreOrWindows",
				BindingFlags.Static | BindingFlags.Public);
			var result = method?.Invoke(null, null);
			var icuVersionInfoType = icunetAssembly.GetType("Icu.IcuVersionInfo");
			var icuVersionFieldInfo = icuVersionInfoType.GetField("IcuVersion");
			return (int)icuVersionFieldInfo.GetValue(result);
		}

		[SetUp]
		public void Setup()
		{
			// Trying to get the ICU version checks for filename, so we create a dummy file with
			// a version number.
			_filenameWindows = Path.Combine(NativeMethodsTests.OutputDirectory, $"icuuc{Wrapper.MaxSupportedIcuVersion}.dll");
			File.WriteAllText(_filenameWindows, "just a dummy file");
			_filenameLinux = Path.Combine(NativeMethodsTests.OutputDirectory, $"libicuuc.so.{Wrapper.MaxSupportedIcuVersion}.1");
			File.WriteAllText(_filenameLinux, "just a dummy file");
			_filenameMac = Path.Combine(NativeMethodsTests.OutputDirectory, $"libicuuc.{Wrapper.MaxSupportedIcuVersion}.dylib");
			File.WriteAllText(_filenameMac, "just a dummy file");
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_filenameWindows);
			File.Delete(_filenameLinux);
			File.Delete(_filenameMac);
			// Dummy files must be deleted BEFORE NativeMethodsHelper.Reset() and Wrapper.Cleanup().
			// Reset() clears the stale v90 version cache. If the dummy files are still present when
			// the cache is cleared, re-discovery would pick them up again, causing subsequent ICU
			// calls to look for versioned symbols like ucal_setDefaultTimeZone_90 in a library that
			// only exports _76. This crashes on Unix.
			//
			// On macOS, Wrapper.Cleanup skips u_cleanup() and NativeLibrary.Free (both omitted to
			// avoid dyld-destructor crashes), so the library stays resident.
			NativeMethodsHelper.Reset();
			Wrapper.Cleanup();
		}

		[Test]
		public void GetIcuVersionInfoForNetCoreOrWindows_DoesNotCrash()
		{
			if (!IsMac)
				Wrapper.Cleanup();
			else
				NativeMethodsHelper.Reset();
			var result = CallGetIcuVersionInfoForNetCoreOrWindows();
			Assert.That(result, Is.EqualTo(Wrapper.MaxSupportedIcuVersion));
		}
	}
#endif
}
