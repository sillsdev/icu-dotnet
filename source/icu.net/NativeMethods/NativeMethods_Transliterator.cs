// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;

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

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Transliterator utrans_openDelegate(string id, UTransDirection dir, string rules, int rulesLength, out ParseError parseError, out ErrorCode status);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void utrans_closeDelegate(IntPtr trans);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void utrans_transUCharsDelegate(IntPtr trans, IntPtr stringptr_text, IntPtr intptr_textLength, int textCapacity, int start, IntPtr intptr_limit, out ErrorCode status);

			internal utrans_openIDsDelegate utrans_openIDs;
			internal utrans_openDelegate utrans_open;
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
		public static Transliterator utrans_open(string id, UTransDirection dir, out ParseError parseError, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			parseError = new ParseError();

			if (TransliteratorMethods.utrans_open == null)
				TransliteratorMethods.utrans_open = GetMethod<TransliteratorMethodsContainer.utrans_openDelegate>(IcuI18NLibHandle, nameof(utrans_open), true);

			return TransliteratorMethods.utrans_open(id, dir, null, -1, out parseError, out status);
		}

		/// <summary />
		public static void utrans_close(IntPtr trans)
		{
			if (TransliteratorMethods.utrans_close == null)
				TransliteratorMethods.utrans_close = GetMethod<TransliteratorMethodsContainer.utrans_closeDelegate>(IcuI18NLibHandle, nameof(utrans_close), true);

			TransliteratorMethods.utrans_close(trans);
		}

		/// <summary />
		public static string utrans_transUChars(IntPtr trans, string text, int textLength, int textCapacity, int start, int limit, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (TransliteratorMethods.utrans_transUChars == null)
				TransliteratorMethods.utrans_transUChars = GetMethod<TransliteratorMethodsContainer.utrans_transUCharsDelegate>(IcuI18NLibHandle, nameof(utrans_transUChars), true);

			IntPtr textPtr = Marshal.StringToHGlobalUni(text);
			IntPtr textLengthPtr = Marshal.AllocHGlobal(sizeof(int));
			IntPtr limitPtr = Marshal.AllocHGlobal(sizeof(int));

			Marshal.StructureToPtr(textLength, textLengthPtr, false);
			Marshal.StructureToPtr(limit, limitPtr, false);

			TransliteratorMethods.utrans_transUChars(trans, textPtr, textLengthPtr, textCapacity, start, limitPtr, out status);
			text = Marshal.PtrToStringUni(textPtr);
			textLength = Marshal.ReadInt32(textLengthPtr);
			limit = Marshal.ReadInt32(limitPtr);

			Marshal.FreeHGlobal(textPtr);
			Marshal.FreeHGlobal(textLengthPtr);
			Marshal.FreeHGlobal(limitPtr);
			return text;
		}
	}
}
