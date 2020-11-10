// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	/// <summary/>
	internal static partial class NativeMethods
	{
		private class TransliteratorMethodsContainer
		{
			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate SafeEnumeratorHandle utrans_openIDsDelegate(out ErrorCode errorCode);

			/// <summary />
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Transliterator utrans_openUDelegate(string id, int idLength, UTransDirection dir, string rules, int rulesLength, out ParseError parseError, out ErrorCode pErrorCode);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void utrans_closeDelegate(IntPtr trans);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void utrans_transUCharsDelegate(IntPtr trans, IntPtr text, ref int textLength, int textCapacity, int start, ref int limit, out ErrorCode status);

			internal utrans_openIDsDelegate utrans_openIDs;
			internal utrans_openUDelegate utrans_openU;
			internal utrans_closeDelegate utrans_close;
			internal utrans_transUCharsDelegate utrans_transUChars;
		}

		private static TransliteratorMethodsContainer _TransliteratorMethods;

		private static TransliteratorMethodsContainer TransliteratorMethods =>
			_TransliteratorMethods ??
			(_TransliteratorMethods = new TransliteratorMethodsContainer());

		/// <summary/>
		public static SafeEnumeratorHandle utrans_openIDs(out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (TransliteratorMethods.utrans_openIDs == null)
				TransliteratorMethods.utrans_openIDs = GetMethod<TransliteratorMethodsContainer.utrans_openIDsDelegate>(IcuI18NLibHandle, nameof(utrans_openIDs), true);
			return TransliteratorMethods.utrans_openIDs(out errorCode);
		}

		/// <summary />
		public static Transliterator utrans_openU(string id, UTransDirection dir, string rules, out ParseError parseError, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			parseError = new ParseError();

			if (TransliteratorMethods.utrans_openU == null)
				TransliteratorMethods.utrans_openU = GetMethod<TransliteratorMethodsContainer.utrans_openUDelegate>(IcuI18NLibHandle, nameof(utrans_openU), true);

			int idLength = id?.Length ?? 0;
			int rulesLength = rules?.Length ?? 0;
			return TransliteratorMethods.utrans_openU(id, idLength, dir, rules, rulesLength, out parseError, out status);
		}

		/// <summary />
		public static void utrans_close(IntPtr trans)
		{
			if (TransliteratorMethods.utrans_close == null)
				TransliteratorMethods.utrans_close = GetMethod<TransliteratorMethodsContainer.utrans_closeDelegate>(IcuI18NLibHandle, nameof(utrans_close), true);

			TransliteratorMethods.utrans_close(trans);
		}

		/// <summary />
		public static string utrans_transUChars(IntPtr trans, string text, int textCapacityMultiplier, out ErrorCode status)
		{
			if (TransliteratorMethods.utrans_transUChars == null)
				TransliteratorMethods.utrans_transUChars = GetMethod<TransliteratorMethodsContainer.utrans_transUCharsDelegate>(IcuI18NLibHandle, nameof(utrans_transUChars), true);

			byte[] unicodeBytes = Encoding.Unicode.GetBytes(text);

			int textLength = text.Length;
			int textCapacity = textLength * textCapacityMultiplier;
			int start = 0;
			int limit = textLength;

			int cb = textCapacity * Marshal.SystemDefaultCharSize;
			IntPtr textPtr = Marshal.AllocHGlobal(cb);
			Marshal.Copy(unicodeBytes, 0, textPtr, unicodeBytes.Length);

			status = ErrorCode.NoErrors;
			TransliteratorMethods.utrans_transUChars(trans, textPtr, ref textLength, textCapacity, start, ref limit, out status);

			string result = Marshal.PtrToStringUni(textPtr);
			Marshal.FreeHGlobal(textPtr);

			// transUChars does not null-terminate the string. need to trim to textLength.
			result = result.Substring(start, textLength);
			return result;
		}
	}
}
