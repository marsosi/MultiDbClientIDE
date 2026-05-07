using System;
using System.Runtime.InteropServices;

namespace MultiDbClientIDE.Infrastructure
{
	public static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		public const int WM_SETDREDRAW = 0x0b;
	}
}
