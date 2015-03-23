namespace UI.Native
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Input;

    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);
        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        internal static MONITORINFO GetMonitorInfo(IntPtr hwnd)
        {
            IntPtr hMonitor = MonitorFromWindow(hwnd, 2);
            if (hMonitor != IntPtr.Zero)
            {
                MONITORINFO lpmi = new MONITORINFO();
                GetMonitorInfo(hMonitor, lpmi);
                return lpmi;
            }
            return null;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", EntryPoint="GetMonitorInfoW", CharSet=CharSet.Unicode, ExactSpelling=true)]
        internal static extern bool GetMonitorInfo([In] IntPtr hMonitor, [Out] MONITORINFO lpmi);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool RegisterHotKey(IntPtr handle, int id, uint modifiers, uint virtualCode);
        public static bool RegisterHotKey(IntPtr handle, int id, Key key, ModifierKeys modifiers)
        {
            int num = KeyInterop.VirtualKeyFromKey(key);
            return RegisterHotKey(handle, id, (uint) modifiers, (uint) num);
        }

        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint="SetClassLong")]
        internal static extern uint SetClassLongPtr32(IntPtr hWnd, int nIndex, uint dwNewLong);
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification="This is correct for 64bit Windows."), DllImport("user32.dll", EntryPoint="SetClassLongPtr")]
        internal static extern IntPtr SetClassLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError=true)]
        internal static extern bool SystemParametersInfo(SystemParametersInfoAction uiAction, uint uiParam, ref uint pvParam, uint fWinIni);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool UnregisterHotKey(IntPtr handle, int id);
        [DllImport("kernel32.dll")]
        public static extern int WerRegisterMemoryBlock(IntPtr pvAddress, uint dwSize);
        internal static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam, POINT minSize)
        {
            MINMAXINFO structure = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            IntPtr hMonitor = MonitorFromWindow(hwnd, 2);
            if (hMonitor != IntPtr.Zero)
            {
                MONITORINFO lpmi = new MONITORINFO();
                GetMonitorInfo(hMonitor, lpmi);
                RECT rcWork = lpmi.rcWork;
                RECT rcMonitor = lpmi.rcMonitor;
                structure.ptMaxPosition.x = Math.Abs((int) (rcWork.left - rcMonitor.left));
                structure.ptMaxPosition.y = Math.Abs((int) (rcWork.top - rcMonitor.top));
                structure.ptMaxSize.x = Math.Abs((int) (rcWork.right - rcWork.left));
                structure.ptMaxSize.y = Math.Abs((int) (rcWork.bottom - rcWork.top));
                structure.ptMinTrackSize.x = minSize.x;
                structure.ptMinTrackSize.y = minSize.y;
            }
            Marshal.StructureToPtr(structure, lParam, true);
        }
    }
}

