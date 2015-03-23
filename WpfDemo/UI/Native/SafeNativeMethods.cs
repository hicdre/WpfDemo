namespace UI.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool ChangeWindowMessageFilter(uint msg, ChangeWindowMessageFilterFlags flags);
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateSolidBrush(int crColor);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("dwmapi.dll", PreserveSig=false)]
        internal static extern bool DwmIsCompositionEnabled();
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);
        [DllImport("shcore.dll")]
        internal static extern uint GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
        [DllImport("shcore.dll")]
        internal static extern int GetProcessDpiAwareness(IntPtr hWnd, out PROCESS_DPI_AWARENESS awareness);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOpts dwFlags);
        [DllImport("user32.dll")]
        internal static extern int SetProcessDPIAware();
        [DllImport("shcore.dll")]
        internal static extern uint SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    }
}

