// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Base class for the safe handles that wrap a native ICU object. It closes the native
	/// object and makes the handle unusable when <see cref="Wrapper.Cleanup"/> unloads the ICU
	/// libraries.
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

		/// <summary>Closes the native ICU object.</summary>
		protected abstract bool ReleaseIcuHandle();

#if NETFRAMEWORK
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		protected sealed override bool ReleaseHandle()
		{
			lock (NativeMethods.CleanupLock)
			{
				// A release that was already waiting for the lock when Cleanup() ran finds the
				// libraries gone.
				return IsStale || ReleaseIcuHandle();
			}
		}

		void IIcuHandleOwner.InvalidateHandle()
		{
			// A handle that isn't open doesn't point into the libraries that are about to go
			// away.
			if (handle == IntPtr.Zero)
				return;

			// Cleanup() holds the lock, so this closes the native object now, unless a call is
			// still using the handle or its finalizer got here first. In those cases
			// ReleaseHandle() will find IsStale and skip the close.
			Dispose();
			IsStale = true;
		}
	}
}
