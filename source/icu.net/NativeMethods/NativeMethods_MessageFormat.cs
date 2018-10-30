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
		private class MessageFormatMethodsContainer
		{
			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate IntPtr umsg_openDelegate(string pattern, int patternLen, string locale, out ParseError parseError, out ErrorCode status);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate void umsg_closeDelegate(IntPtr format);

			/// <summary/>
			[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
			internal delegate int umsg_formatDelegate(IntPtr format, StringBuilder result, int resultLen, out ErrorCode status, double arg0, string arg1, string arg2);

			internal umsg_openDelegate umsg_open;
			internal umsg_closeDelegate umsg_close;
			internal umsg_formatDelegate umsg_format;

		}

		private static MessageFormatMethodsContainer _MessageFormatMethods;

		private static MessageFormatMethodsContainer MessageFormatMethods =>
			_MessageFormatMethods ??
			(_MessageFormatMethods = new MessageFormatMethodsContainer());

		/// <summary/>
		public static IntPtr umsg_open(string pattern, int patternLen, string locale, out ParseError parseError, out ErrorCode status)
		{
			if (MessageFormatMethods.umsg_open == null)
				MessageFormatMethods.umsg_open = GetMethod<MessageFormatMethodsContainer.umsg_openDelegate>(IcuCommonLibHandle, nameof(umsg_open));
			return MessageFormatMethods.umsg_open(pattern, patternLen, locale, out parseError, out status);
		}

		/// <summary/>
		public static void umsg_close(IntPtr format)
		{
			if (MessageFormatMethods.umsg_close == null)
				MessageFormatMethods.umsg_close = GetMethod<MessageFormatMethodsContainer.umsg_closeDelegate>(IcuCommonLibHandle, nameof(umsg_close));
			MessageFormatMethods.umsg_close(format);
		}

		/// <summary/>
		public static int umsg_format(IntPtr format, StringBuilder result, int resultLen, out ErrorCode status, double arg0, string arg1, string arg2)
		{
			if (MessageFormatMethods.umsg_format == null)
				MessageFormatMethods.umsg_format = GetMethod<MessageFormatMethodsContainer.umsg_formatDelegate>(IcuCommonLibHandle, nameof(umsg_format));
			return MessageFormatMethods.umsg_format(format, result, resultLen, out status, arg0, arg1, arg2);
		}

	}
}
