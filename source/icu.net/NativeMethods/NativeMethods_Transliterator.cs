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
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
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
			if (TransliteratorMethods.utrans_openIDs == null)
				TransliteratorMethods.utrans_openIDs = GetMethod<TransliteratorMethodsContainer.utrans_openIDsDelegate>(IcuCommonLibHandle, nameof(utrans_openIDs));
			return TransliteratorMethods.utrans_openIDs(out errorCode);
		}

	}
}
