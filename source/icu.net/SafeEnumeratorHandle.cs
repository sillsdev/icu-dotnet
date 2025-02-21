// Copyright (c) 2013-2025 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace Icu
{
	internal sealed class SafeEnumeratorHandle : SafeHandle
	{
		public SafeEnumeratorHandle() : base(IntPtr.Zero, true)
		{
		}

		///<summary>
		///When overridden in a derived class, executes the code required to free the handle.
		///</summary>
		///<returns>
		///true if the handle is released successfully; otherwise, in the event of a catastrophic
		/// failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging
		/// Assistant.
		///</returns>
#if NETFRAMEWORK
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		protected override bool ReleaseHandle()
		{
			NativeMethods.uenum_close(handle);
			handle = IntPtr.Zero;
			return true;
		}

		///<summary>
		///When overridden in a derived class, gets a value indicating whether the handle value
		/// is invalid.
		///</summary>
		///<returns>
		///true if the handle is valid; otherwise, false.
		///</returns>
		public override bool IsInvalid => handle == IntPtr.Zero;

		public string Next()
		{
			var str = NativeMethods.uenum_unext(this, out var length, out var e);
			ExceptionFromErrorCode.ThrowIfError(e);

			return str == IntPtr.Zero ? null : Marshal.PtrToStringUni(str, length);
		}
	}
}
