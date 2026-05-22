// Copyright (c) 2018-2025 SIL Global
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
			return (int) icuVersionFieldInfo.GetValue(result);
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
			SetUpFixture.DiagLog("NativeMethodsHelperTests.TearDown start");
			File.Delete(_filenameWindows);
			File.Delete(_filenameLinux);
			File.Delete(_filenameMac);
			SetUpFixture.DiagLog("NativeMethodsHelperTests.TearDown before Wrapper.Cleanup");
			Wrapper.Cleanup();
			SetUpFixture.DiagLog("NativeMethodsHelperTests.TearDown after Wrapper.Cleanup");
		}

		[Test]
		public void GetIcuVersionInfoForNetCoreOrWindows_DoesNotCrash()
		{
			SetUpFixture.DiagLog("GetIcuVersionInfoForNetCoreOrWindows_DoesNotCrash: before Wrapper.Cleanup");
			Wrapper.Cleanup();
			SetUpFixture.DiagLog("GetIcuVersionInfoForNetCoreOrWindows_DoesNotCrash: before CallGetIcuVersionInfoForNetCoreOrWindows");
			var result = CallGetIcuVersionInfoForNetCoreOrWindows();
			SetUpFixture.DiagLog($"GetIcuVersionInfoForNetCoreOrWindows_DoesNotCrash: result={result}");
			Assert.That(result, Is.EqualTo(Wrapper.MaxSupportedIcuVersion));
		}
	}
#endif
}
