// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Base class for the safe handles that wrap a native ICU object. It makes the handle
	/// unusable when <see cref="Wrapper.Cleanup"/> unloads the ICU libraries.
	/// </summary>
	/// <remarks>Derived classes must not declare a constructor other than a public default one:
	/// the interop marshaler creates the handles that ICU methods return.</remarks>
	internal abstract class SafeIcuHandle : SafeHandle, IIcuHandleOwner
	{
		protected SafeIcuHandle() : base(IntPtr.Zero, true)
		{
			IcuHandleRegistry.Register(this);
		}

		/// <summary><c>true</c> if the ICU libraries got unloaded while this handle was open,
		/// which makes the native object it pointed to unusable.</summary>
		public bool IsStale { get; private set; }

		void IIcuHandleOwner.InvalidateHandle()
		{
			// A handle that isn't open doesn't point into the libraries that are about to go
			// away.
			if (handle == IntPtr.Zero)
				return;

			IsStale = true;

			// Closing the handle makes the interop marshaler throw ObjectDisposedException
			// rather than hand the dangling pointer to ICU, and keeps ReleaseHandle from
			// closing an object that no longer exists.
			SetHandleAsInvalid();
		}
	}
}
