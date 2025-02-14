// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Icu
{
	/// <summary>
	/// Represents an instance of an ICU ResourceBundle object.
	///
	/// <h3>When to use</h3>
	/// To retrieve information from ICU resources; this is useful for things like enumerating
	/// the countries and locales known to the system.
	///
	/// <h3>About "Null" resource bundles</h3>
	/// If a resource bundle is asked for a subsection that doesn't exist, instead of throwing
	/// an exception or returning null, it will return a "Null" ResourceBundle: an instance of
	/// ResourceBundle that will never throw an exception but always returns an "empty" value
	/// whenever queried. This will make client code much cleaner to write.
	/// </summary>
	public class ResourceBundle: IDisposable
	{
		private IntPtr _ResourceBundle { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="packageName">The packageName and locale together point to an ICU udata
		/// object, as defined by udata_open( packageName, "res", locale, err) or equivalent.
		/// Typically, packageName will refer to a (.dat) file, or to a package registered with
		/// udata_setAppData(). Using a full file or directory pathname for packageName is
		/// deprecated.</param>
		/// <param name="locale">This is the locale this resource bundle is for.</param>
		public ResourceBundle(string packageName, string locale)
		{
			_ResourceBundle = NativeMethods.ures_open(packageName, locale, out var errorCode);
			ExceptionFromErrorCode.ThrowIfError(errorCode);
		}

		private ResourceBundle(IntPtr resourceBundle)
		{
			_ResourceBundle = resourceBundle;
		}

		#region Dispose pattern
		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				// do nothing
			}

			if (_ResourceBundle != IntPtr.Zero)
				NativeMethods.ures_close(_ResourceBundle);
			_ResourceBundle = IntPtr.Zero;
		}
		#endregion

		/// <summary>
		/// Returns <c>true</c> if this is a Null resource bundle
		/// </summary>
		public bool IsNull => _ResourceBundle == IntPtr.Zero;

		/// <summary>
		/// Gets the Null resource bundle
		/// </summary>
		public static ResourceBundle Null { get; } = new ResourceBundle(IntPtr.Zero);

		/// <summary>
		/// Returns the key associated with this resource.
		/// Not all the resources have a key - only those that are members of a table.
		/// </summary>
		/// <returns>a key associated to this resource, or <c>string.Empty</c> if it doesn't have
		/// a key.</returns>
		public string Key
		{
			get
			{
				if (IsNull)
					return string.Empty;

				var keyPtr = NativeMethods.ures_getKey(_ResourceBundle);
				return keyPtr == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(keyPtr);
			}
		}

		/// <summary>
		/// Returns a string from a string resource type
		/// </summary>
		/// <returns>The string that this bundle contained, or <c>string.Empty</c> if this bundle
		/// was not just a single string. May also throw an Exception in error situations.</returns>
		public string String
		{
			get
			{
				if (IsNull)
					return string.Empty;

				var resultPtr = NativeMethods.ures_getString(_ResourceBundle, out var len,
					out var status);
				ExceptionFromErrorCode.ThrowIfError(status);
				if (status.IsFailure() || resultPtr == IntPtr.Zero)
					return string.Empty;
				return Marshal.PtrToStringUni(resultPtr, len);
			}
		}

		/// <summary>
		/// Return the locale ID associated with this ResourceBundle.
		/// </summary>
		/// <returns>A string with the locale ID, or <c>string.Empty</c> if the resource doesn't
		/// have a locale associated. May also throw an Exception in error situations.</returns>
		/// <remarks>Note that Key and String of the bundle are often more useful.</remarks>
		public string Name
		{
			get
			{
				if (IsNull)
					return string.Empty;

				var resultPtr = NativeMethods.ures_getLocale(_ResourceBundle, out var status);
				ExceptionFromErrorCode.ThrowIfError(status);
				if (status.IsFailure() || resultPtr == IntPtr.Zero)
					return string.Empty;
				return Marshal.PtrToStringAnsi(resultPtr);
			}
		}

		/// <summary>
		/// Returns a resource in a resource that has a given <paramref name="key"/>.
		/// </summary>
		/// <param name="key">a key associated with the wanted resource </param>
		/// <returns>ResourceBundle object, or <c>ResourceBundle.Null</c> if there is an
		/// error</returns>
		/// <remarks>This procedure works only with table resources.</remarks>
		public ResourceBundle this[string key]
		{
			get
			{
				if (IsNull)
					return Null;

				var bundle = NativeMethods.ures_getByKey(_ResourceBundle, key, IntPtr.Zero,
					out var status);
				if (status.IsFailure() || bundle == IntPtr.Zero)
					return Null;

				return new ResourceBundle(bundle);
			}
		}

		/// <summary>
		/// Returns a string in a resource that has a given <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The name of the string to retrieve</param>
		/// <returns>A string containing the requested string, or <c>string.Empty</c> if the
		/// string was not found (or if that key represented a different type of data, such as a
		/// number or a resource bundle subsection). May also throw an Exception in exceptional
		/// error situations.</returns>
		/// <remarks>This procedure works only with table resources.</remarks>
		public string GetStringByKey(string key)
		{
			if (IsNull)
				return string.Empty;

			var resultPtr = NativeMethods.ures_getStringByKey(_ResourceBundle, key,
				out var len, out var status);
			if (status.IsFailure())
			{
				if (status == ErrorCode.MISSING_RESOURCE_ERROR || status == ErrorCode.INVALID_FORMAT_ERROR)
					return string.Empty;
				ExceptionFromErrorCode.ThrowIfError(status, $"- GetStringByKey failed for key {key}");
			}
			return resultPtr == IntPtr.Zero ? string.Empty : Marshal.PtrToStringUni(resultPtr, len);
		}

		/// <summary>
		/// Reset a resource bundle's internal iterator to the start of its items.
		/// </summary>
		/// <remarks>
		/// NOTE: this is not thread-safe: if two threads share a ResourceBundle, they will
		/// interfere with each others iterations.
		/// </remarks>
		public void ResetIterator()
		{
			if (IsNull)
				return;

			NativeMethods.ures_resetIterator(_ResourceBundle);
		}

		/// <summary>
		/// Checks whether the resource has another element to iterate over.
		/// </summary>
		/// <returns><c>true></c> if there are more elements, <c>false</c> if there are no more
		/// elements</returns>
		public bool HasNext()
		{
			return !IsNull && NativeMethods.ures_hasNext(_ResourceBundle);
		}

		/// <summary>
		/// Returns the next resource in a given resource or <c>null</c> if there are no more
		/// resources.
		/// </summary>
		/// <returns>The next ResourceBundle, or <c>ResourceBundle.Null</c> if there are no more
		/// resources.</returns>
		/// <remarks>Note that this is not thread-safe: if two threads share a ResourceBundle,
		/// they will interfere with each others iterations.</remarks>
		public ResourceBundle GetNext()
		{
			if (IsNull)
				return Null;

			var resultPtr = NativeMethods.ures_getNextResource(_ResourceBundle, IntPtr.Zero,
				out var status);
			if (status.IsFailure() || resultPtr == IntPtr.Zero)
				return Null;

			return new ResourceBundle(resultPtr);
		}

		/// <summary>
		/// Get next string in a resource bundle, or <c>null</c> if its internal iterator has
		/// reached the end of the bundle (or if an error occurred).
		/// </summary>
		/// <param name="key">Out parameter: the key associated with the result string, or
		/// <c>null</c> if the result string had no key in this resource bundle.</param>
		/// <returns>The next string contained in the resource bundle, or <c>null</c> if there
		/// are no more strings to return.</returns>
		/// <remarks>Note that this is not thread-safe: if two threads share a ResourceBundle,
		/// they will interfere with each others iterations.</remarks>
		public string GetNextString(out string key)
		{
			key = string.Empty;
			if (IsNull)
				return string.Empty;

			var resultPtr = NativeMethods.ures_getNextString(_ResourceBundle, out var ignoreLen,
				out var keyPtr, out var status);
			if (status.IsFailure() || resultPtr == IntPtr.Zero)
				return null;

			key = keyPtr == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(keyPtr);
			return Marshal.PtrToStringUni(resultPtr);
		}

		/// <summary>
		/// Get next string in a resource bundle, or <c>null</c> if its internal iterator has
		/// reached the end of the bundle (or if an error occurred).
		/// This overload should be used if you don't care about the keys in the bundle, but only
		/// about its contents.
		/// </summary>
		/// <returns>The next string contained in the resource bundle, or <c>null</c> if there
		/// are no more strings to return.</returns>
		/// <remarks>Note that this is not thread-safe: if two threads share a ResourceBundle,
		/// they will interfere with each others iterations.</remarks>
		public string GetNextString()
		{
			return GetNextString(out var ignoredKey);
		}

		/// <summary>
		/// Get all the strings this resource bundle contains.
		///
		/// Any resources inside this bundle that were *not* strings (say, other bundles) will be
		/// skipped by this function. It will not recurse into "child" bundles, but only provide
		/// the strings that are direct children of this bundle.
		/// </summary>
		/// <returns>An IEnumerable providing all the string contents of this bundle.</returns>
		public IEnumerable<string> GetStringContents()
		{
			if (IsNull)
				yield break;

			ResetIterator();
			string result;
			while ((result = GetNextString()) != null)
			{
				yield return result;
			}
		}

		/// <summary>
		/// Get all the keyed strings this resource bundle contains.
		///
		/// Any resources inside this bundle that were *not* strings (say, other bundles) will be
		/// skipped by this function, and any string contents that do *not* have keys will also be
		/// skipped. (If the bundle is a "table" -- a dictionary in C# terms -- then this function
		/// is appropriate. If the bundle is an array, then you want
		/// <see cref="GetStringContents"/>instead.) It will not recurse into "child" bundles, but
		/// only provide the strings that are direct children of this bundle.
		/// </summary>
		/// <returns>An IEnumerable providing all the string contents of this bundle, along with
		/// the keys associated with those strings in the bundle.</returns>
		public IEnumerable<(string key, string value)> GetStringContentsWithKeys()
		{
			if (IsNull)
				yield break;

			ResetIterator();
			string strValue;
			while ((strValue = GetNextString(out var key)) != null)
			{
				if (key != null)
					yield return (key, strValue);
			}
		}
	}
}
