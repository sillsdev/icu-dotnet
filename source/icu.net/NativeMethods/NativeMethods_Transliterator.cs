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

			internal utrans_openIDsDelegate utrans_openIDs;

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

	}
}
