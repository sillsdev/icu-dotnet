// Copyright (c) 2018-2025 SIL Global
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Icu
{
	/// <summary/>
	internal static partial class NativeMethods
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class TransliteratorMethodsContainer
		{
			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate SafeEnumeratorHandle utrans_openIDsDelegate(out ErrorCode errorCode);

			/// <summary />
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate Transliterator.SafeTransliteratorHandle utrans_openUDelegate(string id,
				int idLength, Transliterator.UTransDirection dir, string rules, int rulesLength,
				out ParseError parseError, out ErrorCode pErrorCode);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void utrans_closeDelegate(IntPtr trans);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate void utrans_transUCharsDelegate(
				Transliterator.SafeTransliteratorHandle trans, IntPtr text, ref int textLength,
				int textCapacity, int start, ref int limit, out ErrorCode status);

			internal utrans_openIDsDelegate utrans_openIDs;
			internal utrans_openUDelegate utrans_openU;
			internal utrans_closeDelegate utrans_close;
			internal utrans_transUCharsDelegate utrans_transUChars;
		}

		// ReSharper disable once InconsistentNaming
		private static TransliteratorMethodsContainer TransliteratorMethods = new TransliteratorMethodsContainer();

		/// <summary/>
		public static SafeEnumeratorHandle utrans_openIDs(out ErrorCode errorCode)
		{
			errorCode = ErrorCode.NoErrors;
			if (TransliteratorMethods.utrans_openIDs == null)
				TransliteratorMethods.utrans_openIDs = GetMethod<TransliteratorMethodsContainer.utrans_openIDsDelegate>(IcuI18NLibHandle, nameof(utrans_openIDs), true);
			return TransliteratorMethods.utrans_openIDs(out errorCode);
		}

		/// <summary />
		public static Transliterator.SafeTransliteratorHandle utrans_openU(string id,
			Transliterator.UTransDirection dir, string rules, out ParseError parseError, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			parseError = new ParseError();

			if (TransliteratorMethods.utrans_openU == null)
				TransliteratorMethods.utrans_openU = GetMethod<TransliteratorMethodsContainer.utrans_openUDelegate>(IcuI18NLibHandle, nameof(utrans_openU), true);

			var idLength = id?.Length ?? 0;
			var rulesLength = rules?.Length ?? 0;
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
		public static void utrans_transUChars(Transliterator.SafeTransliteratorHandle trans, IntPtr textPtr,
			ref int textLength, int textCapacity, int start, ref int limit, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (TransliteratorMethods.utrans_transUChars == null)
			{
				TransliteratorMethods.utrans_transUChars =
					GetMethod<TransliteratorMethodsContainer.utrans_transUCharsDelegate>(
						IcuI18NLibHandle, nameof(utrans_transUChars), true);
			}

			TransliteratorMethods.utrans_transUChars(trans, textPtr, ref textLength, textCapacity, start, ref limit, out status);
		}
	}
}
