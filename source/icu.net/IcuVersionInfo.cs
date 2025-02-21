// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.IO;

namespace Icu
{
	/// <summary>
	/// Returns the result of trying to resolve the icu binary paths.
	/// </summary>
	internal class IcuVersionInfo
	{
		public IcuVersionInfo()
		{
			Success = false;
		}

		public IcuVersionInfo(DirectoryInfo icuPath, int icuVersion)
		{
			if (icuVersion <= 0)
				throw new ArgumentOutOfRangeException(nameof(icuVersion), "IcuVersion should be greater than 0");

			IcuPath = icuPath;
			IcuVersion = icuVersion;
			Success = true;
		}

		public bool Success { get; }

		public readonly DirectoryInfo IcuPath;
		public readonly int IcuVersion;
	}
}