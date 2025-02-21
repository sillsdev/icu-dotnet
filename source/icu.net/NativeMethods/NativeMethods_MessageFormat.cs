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
		private class MessageFormatMethodsContainer
		{
			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate IntPtr umsg_openDelegate(string pattern, int patternLen,
				string locale, out ParseError parseError, out ErrorCode status);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void umsg_closeDelegate(IntPtr format);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int umsg_formatDelegate(IntPtr format, IntPtr result,
				int resultLen, out ErrorCode status, double arg0, string arg1, string arg2);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			internal delegate int umsg_toPatternDelegate(IntPtr format, IntPtr result,
				int resultLen, out ErrorCode status);

			internal umsg_openDelegate umsg_open;
			internal umsg_closeDelegate umsg_close;
			internal umsg_formatDelegate umsg_format;
			internal umsg_toPatternDelegate umsg_toPattern;

		}

		// ReSharper disable once InconsistentNaming
		private static MessageFormatMethodsContainer MessageFormatMethods = new MessageFormatMethodsContainer();

		/// <summary/>
		public static IntPtr umsg_open(string pattern, int patternLen, string locale, out ParseError parseError, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (MessageFormatMethods.umsg_open == null)
				MessageFormatMethods.umsg_open = GetMethod<MessageFormatMethodsContainer.umsg_openDelegate>(IcuI18NLibHandle, nameof(umsg_open), true);
			return MessageFormatMethods.umsg_open(pattern, patternLen, locale, out parseError, out status);
		}

		/// <summary/>
		public static void umsg_close(IntPtr format)
		{
			if (MessageFormatMethods.umsg_close == null)
				MessageFormatMethods.umsg_close = GetMethod<MessageFormatMethodsContainer.umsg_closeDelegate>(IcuI18NLibHandle, nameof(umsg_close), true);
			MessageFormatMethods.umsg_close(format);
		}

		/// <summary/>
		public static int umsg_format(IntPtr format, IntPtr result, int resultLen, out ErrorCode status, double arg0, string arg1, string arg2)
		{
			if (MessageFormatMethods.umsg_format == null)
				MessageFormatMethods.umsg_format = GetMethod<MessageFormatMethodsContainer.umsg_formatDelegate>(IcuI18NLibHandle, nameof(umsg_format), true);
			return MessageFormatMethods.umsg_format(format, result, resultLen, out status, arg0, arg1, arg2);
		}

		/// <summary/>
		public static int umsg_toPattern(IntPtr format, IntPtr result, int resultLen, out ErrorCode status)
		{
			status = ErrorCode.NoErrors;
			if (MessageFormatMethods.umsg_toPattern == null)
				MessageFormatMethods.umsg_toPattern = GetMethod<MessageFormatMethodsContainer.umsg_toPatternDelegate>(IcuI18NLibHandle, nameof(umsg_toPattern), true);
			return MessageFormatMethods.umsg_toPattern(format, result, resultLen, out status);
		}

	}
}
