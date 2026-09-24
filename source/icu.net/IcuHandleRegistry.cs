// Copyright (c) 2026 SIL Global
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;

namespace Icu
{
	/// <summary>
	/// Implemented by objects that own a native ICU handle and therefore become unusable once
	/// <see cref="Wrapper.Cleanup"/> unloads the ICU libraries.
	/// </summary>
	internal interface IIcuHandleOwner
	{
		/// <summary>
		/// Closes the native object and marks the handle as unusable.
		/// </summary>
		/// <remarks>Runs while the ICU libraries are still loaded and with
		/// <see cref="NativeMethods.CleanupLock"/> held.</remarks>
		void InvalidateHandle();
	}

	/// <summary>
	/// Keeps track of the ICU objects that hold a native handle so that
	/// <see cref="Wrapper.Cleanup"/> can invalidate them before it unloads the ICU libraries.
	/// </summary>
	/// <remarks>The registry holds weak references, so registering doesn't keep an object
	/// alive.</remarks>
	internal static class IcuHandleRegistry
	{
		private const int MinCompactThreshold = 64;

		private static readonly object _lock = new object();
		private static readonly List<WeakReference> _owners = new List<WeakReference>();
		private static int _compactThreshold = MinCompactThreshold;

		internal static void Register(IIcuHandleOwner owner)
		{
			lock (_lock)
			{
				if (_owners.Count >= _compactThreshold)
				{
					_owners.RemoveAll(weakReference => !weakReference.IsAlive);
					_compactThreshold = Math.Max(MinCompactThreshold, _owners.Count * 2);
				}

				// A short weak reference is cleared before finalization, so InvalidateAll() would
				// miss undisposed owners that are still waiting for their finalizer.
				_owners.Add(new WeakReference(owner, trackResurrection: true));
			}
		}

		/// <summary>
		/// Closes and invalidates the handle of every registered object that is still alive.
		/// Afterwards using one of them throws <see cref="ObjectDisposedException"/> instead of
		/// handing a dangling pointer to ICU, which would take down the process with an
		/// <c>AccessViolationException</c>.
		/// </summary>
		internal static void InvalidateAll()
		{
			List<IIcuHandleOwner> owners;
			lock (_lock)
			{
				owners = new List<IIcuHandleOwner>(_owners.Count);
				foreach (var weakReference in _owners)
				{
					if (weakReference.Target is IIcuHandleOwner owner)
						owners.Add(owner);
				}

				_owners.Clear();
				_compactThreshold = MinCompactThreshold;
			}

			// Invalidate outside the lock: closing a handle calls into ICU.
			foreach (var owner in owners)
				owner.InvalidateHandle();
		}

		internal static ObjectDisposedException UnloadedException(string objectName)
		{
			return new ObjectDisposedException(objectName,
				$"The ICU libraries were unloaded by {nameof(Wrapper)}.{nameof(Wrapper.Cleanup)}() after this {objectName} was created.");
		}
	}
}
