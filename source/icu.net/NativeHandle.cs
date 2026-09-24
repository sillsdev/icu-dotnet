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
		private IntPtr _handle;

		/// <param name="ownerName">Name of the class owning this handle; used in the
		/// <see cref="ObjectDisposedException"/> message.</param>
		public NativeHandle(string ownerName)
		{
			_ownerName = ownerName;
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
		/// Clears this handle and returns the pointer the caller has to close, or
		/// <see cref="IntPtr.Zero"/> if there is nothing to close because the handle was never
		/// opened, got closed already, or points into unloaded ICU libraries.
		/// </summary>
		public IntPtr Take()
		{
			var handle = _handle;
			_handle = IntPtr.Zero;
			return handle;
		}

		void IIcuHandleOwner.InvalidateHandle()
		{
			// A handle that isn't open doesn't point into the libraries that are about to go
			// away, so it can be opened again afterwards.
			if (!IsOpen)
				return;

			IsStale = true;
			_handle = IntPtr.Zero;
		}
	}
}
