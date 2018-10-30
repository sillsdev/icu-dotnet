// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using System;
using System.Text;

namespace Icu
{
	public class MessageFormatter: IDisposable
	{
		private IntPtr _Formatter;

		/// <summary>
		/// Constructs a new MessageFormat using the given pattern and locale.
		/// </summary>
		/// <param name="pattern">Pattern used to construct object. </param>
		/// <param name="localeId">The locale to use for formatting dates and numbers. </param>
		/// <remarks>If the pattern cannot be parsed, an exception is thrown.</remarks>
		public MessageFormatter(string pattern, string localeId)
		{
			_Formatter = NativeMethods.umsg_open(pattern, pattern.Length, localeId,
				out var parseError, out var status);
			ExceptionFromErrorCode.ThrowIfError(status);
		}

		/// <summary>
		/// Constructs a new MessageFormat using the given pattern and locale.
		/// </summary>
		/// <param name="pattern">Pattern used to construct object. </param>
		/// <param name="localeId">The locale to use for formatting dates and numbers. </param>
		/// <param name="parseError">Struct to receive information on the position of an error
		/// within the pattern. </param>
		/// <param name="status">Input/output error code. If the pattern cannot be parsed, set
		/// to failure code. </param>
		public MessageFormatter(string pattern, string localeId, out ParseError parseError,
			out ErrorCode status)
		{
			_Formatter = NativeMethods.umsg_open(pattern, pattern.Length, localeId, out parseError,
				out status);
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

			if (_Formatter != IntPtr.Zero)
				NativeMethods.umsg_close(_Formatter);
			_Formatter = IntPtr.Zero;
		}
		#endregion

		/// <summary>
		/// Formats the given arguments into a user-readable string. 
		/// </summary>
		/// <returns>The user-readable string</returns>
		public string Format(double arg0, string arg1, string arg2)
		{
			var builder = new StringBuilder(255);
			var actualLen = NativeMethods.umsg_format(_Formatter, builder, 255, out var status,
				arg0, arg1, arg2);
			if (status.IsSuccess() && actualLen > 255)
			{
				builder = new StringBuilder(actualLen);
				NativeMethods.umsg_format(_Formatter, builder, 255, out status, arg0, arg1, arg2);
			}

			return status.IsSuccess() ? builder.ToString() : string.Empty;
		}

		/// <summary>
		/// Formats the given arguments into a user-readable string using the given pattern and
		/// locale.
		/// </summary>
		/// <param name="pattern">Pattern used to construct object. </param>
		/// <param name="localeId">The locale to use for formatting dates and numbers. </param>
		/// <returns>The user-readable string</returns>
		public static string Format(string pattern, string localeId,
			double arg0, string arg1, string arg2)
		{
			using (var formatter = new MessageFormatter(pattern, localeId))
			{
				return formatter.Format(arg0, arg1, arg2);
			}
		}

		/// <summary>
		/// Formats the given arguments into a user-readable string using the given pattern and
		/// locale.
		/// </summary>
		/// <param name="pattern">Pattern used to construct object. </param>
		/// <param name="localeId">The locale to use for formatting dates and numbers.</param>
		/// <param name="status">If the pattern cannot be parsed, set to failure code.</param>
		/// <returns>The user-readable string, or <c>null</c> if pattern cannot be parsed.</returns>
		public static string Format(string pattern, string localeId, out ErrorCode status,
			double arg0, string arg1, string arg2)
		{
			using (var formatter = new MessageFormatter(pattern, localeId, out var parseError, 
				out status))
			{
				return status.IsSuccess() ? formatter.Format(arg0, arg1, arg2) : null;
			}
		}
	}
}
