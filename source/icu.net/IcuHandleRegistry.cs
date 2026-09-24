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
		/// Marks the native handle as unusable. Called before the ICU libraries get unloaded,
		/// so implementations must not call into ICU.
		/// </summary>
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

				_owners.Add(new WeakReference(owner));
			}
		}

		/// <summary>
		/// Invalidates the handle of every registered object that is still alive. Afterwards
		/// using one of them throws <see cref="ObjectDisposedException"/> instead of handing a
		/// dangling pointer to ICU, which would take down the process with an
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

			// Invalidate outside the lock: invalidating one object might create another one.
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
