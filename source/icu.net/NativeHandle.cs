// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;

namespace Icu
{
	/// <summary>
	/// A pointer to a native ICU object that gets invalidated when <see cref="Wrapper.Cleanup"/>
	/// unloads the ICU libraries. This is used by the classes that deal with a plain
	/// <see cref="IntPtr"/> instead of a <see cref="System.Runtime.InteropServices.SafeHandle"/>.
	/// </summary>
	internal sealed class NativeHandle : IIcuHandleOwner
	{
		private readonly string _ownerName;
		private readonly Action<IntPtr> _close;
		private IntPtr _handle;

		/// <param name="ownerName">Name of the class owning this handle; used in the
		/// <see cref="ObjectDisposedException"/> message.</param>
		/// <param name="close">The ICU method that closes the native object, or <c>null</c> if
		/// ICU owns it.</param>
		public NativeHandle(string ownerName, Action<IntPtr> close)
		{
			_ownerName = ownerName;
			_close = close;
			IcuHandleRegistry.Register(this);
		}

		/// <summary><c>true</c> if this handle points to a native object.</summary>
		public bool IsOpen => _handle != IntPtr.Zero;

		/// <summary><c>true</c> if the ICU libraries got unloaded while this handle was open,
		/// which makes the native object it pointed to unusable.</summary>
		public bool IsStale { get; private set; }

		/// <summary>The pointer to pass to ICU.</summary>
		/// <exception cref="ObjectDisposedException">The ICU libraries were unloaded by
		/// <see cref="Wrapper.Cleanup"/> after this handle got opened.</exception>
		public IntPtr Pointer
		{
			get
			{
				ThrowIfStale();
				return _handle;
			}
		}

		/// <exception cref="ObjectDisposedException">The ICU libraries were unloaded by
		/// <see cref="Wrapper.Cleanup"/> after this handle got opened.</exception>
		public void ThrowIfStale()
		{
			if (IsStale)
				throw IcuHandleRegistry.UnloadedException(_ownerName);
		}

		public void Set(IntPtr handle)
		{
			_handle = handle;
		}

		/// <summary>
		/// Closes the native object, if this handle is open. Safe to call from a finalizer.
		/// </summary>
		public void Close()
		{
			lock (NativeMethods.CleanupLock)
				CloseCore();
		}

		private void CloseCore()
		{
			var handle = _handle;
			_handle = IntPtr.Zero;
			if (handle != IntPtr.Zero)
				_close?.Invoke(handle);
		}

		void IIcuHandleOwner.InvalidateHandle()
		{
			// A handle that isn't open doesn't point into the libraries that are about to go
			// away. Its owner stays usable and might open it later, so keep tracking it.
			if (!IsOpen)
			{
				IcuHandleRegistry.Register(this);
				return;
			}

			CloseCore();
			IsStale = true;
		}
	}
}
