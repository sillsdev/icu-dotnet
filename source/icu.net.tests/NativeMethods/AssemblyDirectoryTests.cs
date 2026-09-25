// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.IO;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	public class AssemblyDirectoryTests
	{
		[Test]
		[Platform(Include = "Win")]
		public void GetAssemblyDirectory_LongPathWithExtendedPrefix_ReturnsDirectory()
		{
			var directory = @"\?\C:\" + new string('a', 250);
			var location = directory + @"\icu.net.dll";

			Assert.That(NativeMethods.GetAssemblyDirectory(null, location, null),
				Is.EqualTo(directory));
		}

		[Test]
		public void GetAssemblyDirectory_Location_ReturnsDirectory()
		{
			var directory = Path.Combine(Path.GetTempPath(), "some dir");

			Assert.That(NativeMethods.GetAssemblyDirectory(null,
				Path.Combine(directory, "icu.net.dll"), null), Is.EqualTo(directory));
		}

		[Test]
		public void GetAssemblyDirectory_LocationWithUriSpecialCharacters_ReturnsDirectoryUnchanged()
		{
			var directory = Path.Combine(Path.GetTempPath(), "a%20b#c");

			Assert.That(NativeMethods.GetAssemblyDirectory(null,
				Path.Combine(directory, "icu.net.dll"), null), Is.EqualTo(directory));
		}

		[Test]
		public void GetAssemblyDirectory_FileUriCodeBase_PreferredOverLocation()
		{
			var directory = Path.Combine(Path.GetTempPath(), "codebase dir");
			var codeBase = new System.Uri(Path.Combine(directory, "icu.net.dll")).AbsoluteUri;

			Assert.That(NativeMethods.GetAssemblyDirectory(codeBase,
				Path.Combine(Path.GetTempPath(), "shadow", "icu.net.dll"), null),
				Is.EqualTo(directory));
		}

		[Test]
		public void GetAssemblyDirectory_EmptyLocation_UsesFallback()
		{
			var directory = Path.Combine(Path.GetTempPath(), "app");

			Assert.That(NativeMethods.GetAssemblyDirectory(null, "",
				directory + Path.DirectorySeparatorChar), Is.EqualTo(directory));
		}
	}
}
