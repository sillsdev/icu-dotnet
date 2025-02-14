// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	public class Transliterator : IDisposable
	{
		public enum UTransDirection
		{
			Forward,
			Reverse
		}

		// ReSharper disable once ClassNeverInstantiated.Global
		internal sealed class SafeTransliteratorHandle : SafeHandle
		{
			public SafeTransliteratorHandle() :
				base(IntPtr.Zero, true) {}

			public override bool IsInvalid => handle == IntPtr.Zero;

			protected override bool ReleaseHandle()
			{
				NativeMethods.utrans_close(handle);
				return true;
			}
		}

		private readonly SafeTransliteratorHandle _transliteratorHandle;

		#region Static Methods
		/// <summary>
		/// Get an ICU Transliterator.
		/// </summary>
		/// <param name="id">a valid transliterator ID</param>
		/// <param name="dir">the desired direction</param>
		/// <param name="rules">the transliterator rules. If <c>null</c> then a system transliterator
		/// matching the ID is returned.</param>
		/// <returns>
		/// A Transliterator class instance. Be sure to call the instance's `Dispose` method to clean up.
		/// </returns>
		public static Transliterator CreateInstance(string id, UTransDirection dir = UTransDirection.Forward, string rules = null)
		{
			var handle = NativeMethods.utrans_openU(id, dir, rules, out _, out var status);
			ExceptionFromErrorCode.ThrowIfError(status);

			return new Transliterator(handle);
		}

		/// <summary>
		/// Get an ICU UEnumeration pointer that will enumerate all transliterator IDs.
		/// </summary>
		/// <returns>The opaque UEnumeration pointer. Closing it properly is the responsibility of the caller.</returns>
		private static SafeEnumeratorHandle GetEnumerator()
		{
			var result = NativeMethods.utrans_openIDs(out var status);
			ExceptionFromErrorCode.ThrowIfError(status);
			return result;
		}

		/// <summary>
		/// Get the IDs available at the time of the call, including user-registered IDs.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetAvailableIds()
		{
			using (var icuEnumerator = GetEnumerator())
			{
				for (var id = icuEnumerator.Next(); !string.IsNullOrEmpty(id); id = icuEnumerator.Next())
				{
					yield return id;
				}
			}
		}

		/// <summary>
		/// Get the IDs and display names of all transliterators registered with ICU.
		/// Display names will be in the locale specified by the displayLocale parameter; omit it or pass in null to use the default locale.
		/// </summary>
		public static IEnumerable<(string id, string name)> GetIdsAndNames(string displayLocale = null)
		{
			using (var icuEnumerator = GetEnumerator())
			{
				for (var id = icuEnumerator.Next(); !string.IsNullOrEmpty(id); id = icuEnumerator.Next())
				{
					var name = GetDisplayName(id, displayLocale);
					if (name != null)
						yield return (id, name);
				}
			}
		}

		/// <summary>
		/// Reimplementation of TransliteratorIDParser::IDtoSTV from the ICU C++ API. Parses a
		/// transliterator ID in one of several formats and returns the source, target and variant
		/// components of the ID. Valid formats are T, T/V, S-T, S-T/V, or S/V-T. If source is
		/// missing, it will be set to "Any". If target or variant is missing, they will be the
		/// empty string. (If target is missing, the ID is not well-formed, but this function will
		/// not throw an exception). If variant is present, the slash will be included in.
		/// </summary>
		/// <param name="transId">Transliterator ID to parse</param>
		/// <param name="source">"Any" if source was missing, otherwise source component of the
		/// ID</param>
		/// <param name="target">Empty string if no target, otherwise target component of the ID
		/// (should always be present in a well-formed ID)</param>
		/// <param name="variant">Empty string if no variant, otherwise variant component of the
		/// ID, *with a '/' as its first character*.</param>
		/// <returns>True if source was present, false if source was missing.</returns>
		private static bool ParseTransliteratorID(string transId, out string source,
			out string target, out string variant)
		{
			// This is a straight port of the TransliteratorIDParser::IDtoSTV logic, with basically no changes
			source = "Any";
			var tgtSep = transId.IndexOf("-");
			var varSep = transId.IndexOf("/");
			if (varSep < 0)
				varSep = transId.Length;

			var isSourcePresent = false;
			if (tgtSep < 0)
			{
				// Form: T/V or T (or /V)
				target = transId.Substring(0, varSep);
				variant = transId.Substring(varSep);
			}
			else if (tgtSep < varSep)
			{
				// Form: S-T/V or S-T (or -T/V or -T)
				if (tgtSep > 0)
				{
					source = transId.Substring(0, tgtSep);
					isSourcePresent = true;
				}
				target = transId.Substring(tgtSep + 1, varSep - tgtSep - 1);
				variant = transId.Substring(varSep);
			}
			else
			{
				// Form: S/V-T or /V-T
				if (varSep > 0)
				{
					source = transId.Substring(0, varSep);
					isSourcePresent = true;
				}
				variant = transId.Substring(varSep, tgtSep - varSep);
				target = transId.Substring(tgtSep + 1);
			}

			// The above Substring calls have all left the variant either empty or looking like "/V". In the original C++
			// implementation, we removed the leading "/". But here, we keep it because the only code that needs to call
			// this is GetTransliteratorDisplayName, which wants the leading "/" on variant names.
			//if (variant.Length > 0)
			//	variant = variant.Substring(1);

			return isSourcePresent; // This is currently not used, but we return it anyway for compatibility with original C++ implementation
		}

		/// <summary>
		/// Get a display name for a transliterator. This reimplements the logic from the C++
		/// Transliterator::getDisplayName method, since the ICU C API doesn't expose a
		/// utrans_getDisplayName() call. (Unfortunately).
		/// Note that if no text is found for the given locale, ICU will (by default) fallback to
		/// the root locale. However, the root locale's strings for transliterator display names
		/// are ugly and not suitable for displaying to the user. Therefore, if we have to
		/// fallback, we fallback to the "en" locale instead of the root locale.
		/// </summary>
		/// <param name="transId">The translator's system ID in ICU.</param>
		/// <param name="localeName">The ICU name of the locale in which to calculate the display
		/// name.</param>
		/// <returns>A name suitable for displaying to the user in the given locale, or in English
		/// if no translated text is present in the given locale.</returns>
		public static string GetDisplayName(string transId, string localeName)
		{
			const string translitDisplayNameRBKeyPrefix = "%Translit%%";  // See RB_DISPLAY_NAME_PREFIX in translit.cpp in ICU source code
			const string scriptDisplayNameRBKeyPrefix = "%Translit%";  // See RB_SCRIPT_DISPLAY_NAME_PREFIX in translit.cpp in ICU source code
			const string translitResourceBundleName = "ICUDATA-translit";
			const string translitDisplayNamePatternKey = "TransliteratorNamePattern";

			ParseTransliteratorID(transId, out var source, out var target, out var variant);
			if (target.Length < 1)
				return transId;  // Malformed ID? Give up

			using (var bundle = new ResourceBundle(translitResourceBundleName, localeName))
			using (var bundleFallback = new ResourceBundle(translitResourceBundleName, "en"))
			{
				var pattern = bundle.GetStringByKey(translitDisplayNamePatternKey);
				// If we don't find a MessageFormat pattern in our locale, try the English fallback
				if (string.IsNullOrEmpty(pattern))
					pattern = bundleFallback.GetStringByKey(translitDisplayNamePatternKey);
				// Still can't find a pattern? Then we won't be able to format the ID, so just return it
				if (string.IsNullOrEmpty(pattern))
					return transId;

				// First check if there is a specific localized name for this transliterator, and if so, just return it.
				// Note that we need to check whether the string we got still starts with the "%Translit%%" prefix, because
				// if so, it means that we got a value from the root locale's bundle, which isn't actually localized.
				var translitLocalizedName = bundle.GetStringByKey(translitDisplayNameRBKeyPrefix + transId);

				if (!string.IsNullOrEmpty(translitLocalizedName) && !translitLocalizedName.StartsWith(translitDisplayNameRBKeyPrefix))
					return translitLocalizedName;

				// There was no specific localized name for this transliterator (which will be true of most cases). Build one.

				// Try getting localized display names for the source and target, if possible.
				var localizedSource = bundle.GetStringByKey(scriptDisplayNameRBKeyPrefix + source);
				if (string.IsNullOrEmpty(localizedSource))
				{
					localizedSource = source; // Can't localize
				}
				else
				{
					// As with the transliterator name, we need to check that the string we got didn't come from the root bundle
					// (which just returns a string that still contains the ugly %Translit% prefix). If it did, fall back to English.
					if (localizedSource.StartsWith(scriptDisplayNameRBKeyPrefix))
						localizedSource = bundleFallback.GetStringByKey(scriptDisplayNameRBKeyPrefix + source);
					if (string.IsNullOrEmpty(localizedSource) || localizedSource.StartsWith(scriptDisplayNameRBKeyPrefix))
						localizedSource = source;
				}

				// Same thing for target
				var localizedTarget = bundle.GetStringByKey(scriptDisplayNameRBKeyPrefix + target);

				if (string.IsNullOrEmpty(localizedTarget))
				{
					localizedTarget = target; // Can't localize
				}
				else
				{
					if (localizedTarget.StartsWith(scriptDisplayNameRBKeyPrefix))
						localizedTarget = bundleFallback.GetStringByKey(scriptDisplayNameRBKeyPrefix + target);
					if (string.IsNullOrEmpty(localizedTarget) || localizedTarget.StartsWith(scriptDisplayNameRBKeyPrefix))
						localizedTarget = target;
				}

				var displayName = MessageFormatter.Format(pattern, localeName, out var status,
					2.0, localizedSource, localizedTarget);
				if (status.IsSuccess())
					return displayName + variant; // Variant is either empty string or starts with "/"
				return transId; // If formatting fails, the transliterator's ID is still our final fallback
			}
		}
		#endregion

		#region Instance Methods

		private Transliterator(SafeTransliteratorHandle handle)
		{
			_transliteratorHandle = handle;
		}

		/// <summary>
		/// Transliterate <paramref name="text"/>.
		/// </summary>
		/// <param name="text">The text to transliterate</param>
		/// <<param name="textCapacityMultiplier">The capacity for the buffer that holds the
		/// transliterated text, expressed as a multiplier of the text length.</param>
		/// <returns>
		/// The transliterated text, truncated to a maximum of `text.Length * textCapacityMultiplier` characters.
		/// </returns>
		public string Transliterate(string text, int textCapacityMultiplier = 3)
		{
			if (textCapacityMultiplier < 1)
				throw new ArgumentException(nameof(textCapacityMultiplier));

			var unicodeBytes = Encoding.Unicode.GetBytes(text);

			var textLength = text.Length;
			var textCapacity = textLength * textCapacityMultiplier;
			var start = 0;
			var limit = textLength;
			const int charSize = sizeof(char);

			Debug.Assert(textCapacity * charSize >= unicodeBytes.Length);

			// it's tempting to use Marshal.SystemDefaultCharSize instead of sizeof(char).
			// However, on Linux for whatever reason that returns 1 instead of the expected 2.
			var textPtr = Marshal.AllocHGlobal(textCapacity * charSize);
			Marshal.Copy(unicodeBytes, 0, textPtr, unicodeBytes.Length);

			NativeMethods.utrans_transUChars(_transliteratorHandle, textPtr, ref textLength,
				textCapacity, start, ref limit, out var status);
			ExceptionFromErrorCode.ThrowIfError(status);

			var result = Marshal.PtrToStringUni(textPtr, textLength);
			Marshal.FreeHGlobal(textPtr);

			return result;
		}

		#region Disposable pattern
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_transliteratorHandle?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#endregion
	}
}
