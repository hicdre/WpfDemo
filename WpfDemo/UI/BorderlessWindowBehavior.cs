
namespace UI
{
    using UI.Helpers;
    using UI.Native;
    using ReactiveUI;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using System.Windows.Media;
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        // Fields
        private double currentDpiRatio;
        private IDisposable dragMoveHandle;
        private IntPtr hwnd;
        private HwndSource hwndSource;
        private readonly IntPtr intPtrOne = new IntPtr(1);
        private bool isHardwareRenderingEnabled;
        private static bool? isWin81OrHigher = null;
        //private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private POINT minimumSize;
        private readonly IntPtr setDropShadow = new IntPtr(0x1025);

        // Methods
        static BorderlessWindowBehavior()
        {
            InitializeDpiAwareness();
        }

        private void AddHwndHook()
        {
            this.hwndSource = PresentationSource.FromVisual(base.AssociatedObject) as HwndSource;
            this.hwndSource.AddHook(new HwndSourceHook(this.HwndHook));
            this.hwnd = new WindowInteropHelper(base.AssociatedObject).Handle;
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.AddHwndHook();
            if (IsWindows81OrHigher() && this.IsPerMonitorDpiAware())
            {
                this.currentDpiRatio = this.GetScaleRatioForWindow();
                this.UpdateDpiScaling(this.currentDpiRatio);
            }
            this.SetDefaultBackgroundColor();
        }

        private static void EnableDragDropFromLowPrivUIPIProcesses()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                uint[] numArray = new uint[] { 0x233, 0x48, 0x49 };
                foreach (uint num in numArray)
                {
                    SafeNativeMethods.ChangeWindowMessageFilter(num, ChangeWindowMessageFilterFlags.Add);
                }
            }
        }

        private double GetScaleRatioForWindow()
        {
            uint num2;
            uint num3;
            double num = 96.0 * PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow).CompositionTarget.TransformToDevice.M11;
            if (!IsWindows81OrHigher())
            {
                return (num / 96.0);
            }
            SafeNativeMethods.GetDpiForMonitor(SafeNativeMethods.MonitorFromWindow(this.hwndSource.Handle, MonitorOpts.MONITOR_DEFAULTTONEAREST), MonitorDpiType.MDT_EFFECTIVE_DPI, out num2, out num3);
            return (((double)num2) / num);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "GitHub.Extensions.Windows.Native.UnsafeNativeMethods.DwmExtendFrameIntoClientArea(System.IntPtr,GitHub.Extensions.Windows.Native.MARGINS@)")]
        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (message)
            {
                case NativeConstants.WM_NCCALCSIZE:
                    if (wParam == this.intPtrOne)
                    {
                        handled = true;
                    }
                    break;

                case NativeConstants.WM_NCPAINT:
                    if (this.isHardwareRenderingEnabled)
                    {
                        MARGINS pMarInset = new MARGINS
                        {
                            bottomHeight = 10,
                            leftWidth = 10,
                            rightWidth = 10,
                            topHeight = 10
                        };
                        UnsafeNativeMethods.DwmExtendFrameIntoClientArea(this.hwnd, ref pMarInset);
                    }
                    break;

                case NativeConstants.WM_NCACTIVATE:
                    {
                        IntPtr ptr = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));
                        handled = true;
                        return ptr;
                    }
                case NativeConstants.WM_DPICHANGED:
                    {
                        RECT rect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                        SafeNativeMethods.SetWindowPos(hWnd, IntPtr.Zero, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.DoNotChangeOwnerZOrder | SetWindowPosFlags.IgnoreZOrder);
                        double scaleRatioForWindow = this.GetScaleRatioForWindow();
                        if (scaleRatioForWindow != this.currentDpiRatio)
                        {
                            this.UpdateDpiScaling(scaleRatioForWindow);
                        }
                        break;
                    }
                case NativeConstants.WM_SETTINGCHANGE:
                    if (wParam == this.setDropShadow)
                    {
                        MessageBus.Current.SendMessage<Unit>(Unit.Default, "WindowDropShadowChanged");
                    }
                    break;

                case NativeConstants.WM_GETMINMAXINFO:
                    UnsafeNativeMethods.WmGetMinMaxInfo(hWnd, lParam, this.minimumSize);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static void InitializeDpiAwareness()
        {
            try
            {
                if (IsWindows81OrHigher())
                {
                    SetPerMonitorDpiAwarenessWithFallback();
                }
                else
                {
                    TrySetProcessDpiAware();
                }
            }
            catch (Exception exception)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Could not set DPI awareness. Is Windows 8.1 or higher: {0}", new object[] { IsWindows81OrHigher() });
                //log.ErrorException(message, exception);
            }
        }

        private bool IsPerMonitorDpiAware()
        {
            try
            {
                PROCESS_DPI_AWARENESS process_dpi_awareness;
                return ((SafeNativeMethods.GetProcessDpiAwareness(IntPtr.Zero, out process_dpi_awareness) == 0) && (process_dpi_awareness == PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsWindows81OrHigher()
        {
            if (!isWin81OrHigher.HasValue)
            {
                try
                {
                    Version version = Environment.OSVersion.Version;
                    if (version != null)
                    {
                        isWin81OrHigher = new bool?(version >= new Version(6, 3));
                    }
                    else
                    {
                        //log.Warn("Could not determine real OS version");
                        isWin81OrHigher = false;
                    }
                }
                catch (Exception exception)
                {
                    //log.WarnException("Could not determine if Windows 8.1 or higher", exception);
                    isWin81OrHigher = false;
                }
            }
            return isWin81OrHigher.Value;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not Urgent. We can fix when we need to change this method.")]
        protected override void OnAttached()
        {
            if (base.AssociatedObject.IsInitialized)
            {
                this.AddHwndHook();
            }
            else
            {
                base.AssociatedObject.SourceInitialized += new EventHandler(this.AssociatedObject_SourceInitialized);
            }
            base.AssociatedObject.WindowStyle = WindowStyle.None;
            //base.AssociatedObject.ResizeMode = ResizeMode.CanResizeWithGrip;
            this.dragMoveHandle = ObservableExtensions.Subscribe<MouseButtonEventArgs>(
                Observable.Where<MouseButtonEventArgs>(base.AssociatedObject.Events().MouseLeftButtonDown, 
                    e => e.LeftButton == MouseButtonState.Pressed), 
                _ => base.AssociatedObject.DragMove());
            DateTimeOffset lastDoubleClick = DateTimeOffset.MinValue;
            ObservableExtensions.Subscribe<double>(Observable.Where<double>(Observable.Select<MouseButtonEventArgs, double>(Observable.Where<MouseButtonEventArgs>(base.AssociatedObject.Events().MouseDoubleClick, e => e.LeftButton == MouseButtonState.Pressed), x => x.GetPosition(base.AssociatedObject).Y), x => x <= 28.0), delegate(double _)
            {
                this.AssociatedObject.WindowState = (this.AssociatedObject.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
                lastDoubleClick = DateTimeOffset.Now;
            });
            IObservable<Point> mouseDown = Observable.Select<MouseButtonEventArgs, Point>(Observable.Where<MouseButtonEventArgs>(Observable.Where<MouseButtonEventArgs>(base.AssociatedObject.Events().MouseLeftButtonDown, _ => base.AssociatedObject.WindowState == WindowState.Maximized), x => x.LeftButton == MouseButtonState.Pressed), x => x.GetPosition(base.AssociatedObject));
            IObservable<Point> observable = Observable.Where<Point>(Observable.Select<System.Windows.Input.MouseEventArgs, Point>(Observable.Where<System.Windows.Input.MouseEventArgs>(Observable.Where<System.Windows.Input.MouseEventArgs>(base.AssociatedObject.Events().MouseMove, _ => base.AssociatedObject.WindowState == WindowState.Maximized), x => x.LeftButton == MouseButtonState.Pressed), x => x.GetPosition(base.AssociatedObject)), x => x.Y <= 28.0);
            ObservableExtensions.Subscribe<Tuple<Point, Point>>(Observable.Where<Tuple<Point, Point>>(Observable.Where<Tuple<Point, Point>>(Observable.Where<Tuple<Point, Point>>(Observable.Where<Tuple<Point, Point>>(Observable.Join<Point, Point, Point, Unit, Tuple<Point, Point>>(mouseDown, observable, _ => mouseDown, _ => Observable.Empty<Unit>(), new Func<Point, Point, Tuple<Point, Point>>(Tuple.Create<Point, Point>)), _ => (DateTimeOffset.Now - lastDoubleClick) > TimeSpan.FromSeconds(0.5)), _ => Mouse.LeftButton == MouseButtonState.Pressed), _ => base.AssociatedObject.WindowState == WindowState.Maximized), x => ((x.Item1.Y <= 28.0) && (x.Item2.Y <= 28.0)) && ((x.Item2.Y - x.Item1.Y) > 1.0)), delegate(Tuple<Point, Point> x)
            {
                MONITORINFO monitorInfo = UnsafeNativeMethods.GetMonitorInfo(this.hwnd);
                int num = Math.Abs((int)(monitorInfo.rcWork.right - monitorInfo.rcWork.left));
                double num2 = x.Item1.X;
                base.AssociatedObject.WindowState = WindowState.Normal;
                double actualWidth = base.AssociatedObject.ActualWidth;
                double num4 = (num2 * actualWidth) / ((double)num);
                base.AssociatedObject.Top = (x.Item2.Y - x.Item1.Y) + monitorInfo.rcMonitor.top;
                base.AssociatedObject.Left = (num2 - num4) + monitorInfo.rcMonitor.left;
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Width {0} to {1}. X {2} to {3}. Left = {4}", new object[] { num, actualWidth, num2, num4, base.AssociatedObject.Left }));
                base.AssociatedObject.Cursor = System.Windows.Input.Cursors.Arrow;
                base.AssociatedObject.DragMove();
            });
            ObservableExtensions.Subscribe<Unit>(Observable.ObserveOn<Unit>(base.AssociatedObject.WhenAny<Window, Unit, double, double>(x => x.MinHeight, x => x.MinWidth, (w, h) => Unit.Default), RxApp.MainThreadScheduler), delegate(Unit _)
            {
                PresentationSource source = PresentationSource.FromVisual(base.AssociatedObject);
                if (source != null)
                {
                    Point point = source.CompositionTarget.TransformToDevice.Transform(new Point(base.AssociatedObject.MinWidth, base.AssociatedObject.MinHeight));
                    this.minimumSize = new POINT((int)point.X, (int)point.Y);
                }
            });
            this.isHardwareRenderingEnabled = (Environment.OSVersion.Version.Major >= 6) && !HardwareRenderingHelper.IsInSoftwareMode;
            EnableDragDropFromLowPrivUIPIProcesses();
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.dragMoveHandle.Dispose();
            this.RemoveHwndHook();
            base.OnDetaching();
        }

        private void RemoveHwndHook()
        {
            base.AssociatedObject.SourceInitialized -= new EventHandler(this.AssociatedObject_SourceInitialized);
            this.hwndSource.RemoveHook(new HwndSourceHook(this.HwndHook));
        }

        public static void Resize(Window borderlessWindow, ResizeDirection direction)
        {
            if ((direction < ResizeDirection.Left) || (direction > ResizeDirection.BottomRight))
            {
                throw new ArgumentOutOfRangeException("direction", "Invalid direction: " + direction);
            }
            HwndSource source = (HwndSource)PresentationSource.FromVisual(borderlessWindow);
            if (source != null)
            {
                UnsafeNativeMethods.SendMessage(source.Handle, 0x112, (IntPtr)(0xf000 + direction), IntPtr.Zero);
            }
        }

        private static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
            {
                return UnsafeNativeMethods.SetClassLongPtr64(hWnd, nIndex, dwNewLong);
            }
            return new IntPtr((long)UnsafeNativeMethods.SetClassLongPtr32(hWnd, nIndex, (uint)dwNewLong.ToInt32()));
        }

        private void SetDefaultBackgroundColor()
        {
            SolidColorBrush background = base.AssociatedObject.Background as SolidColorBrush;
            if (background != null)
            {
                int crColor = (background.Color.R | (background.Color.G << 8)) | (background.Color.B << 0x10);
                IntPtr hObject = SetClassLong(this.hwnd, -10, SafeNativeMethods.CreateSolidBrush(crColor));
                if (hObject != IntPtr.Zero)
                {
                    UnsafeNativeMethods.DeleteObject(hObject);
                }
            }
        }

        private static void SetPerMonitorDpiAwarenessWithFallback()
        {
            try
            {
                SafeNativeMethods.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
            }
            catch (Exception)
            {
                TrySetProcessDpiAware();
            }
        }

        private static void TrySetProcessDpiAware()
        {
            try
            {
                SafeNativeMethods.SetProcessDPIAware();
            }
            catch (Exception exception)
            {
                //log.ErrorException("Could not set DPI awareness.", exception);
            }
        }

        private void UpdateDpiScaling(double newDpiRatio)
        {
            this.currentDpiRatio = newDpiRatio;
            ((Visual)VisualTreeHelper.GetChild(base.AssociatedObject, 0)).SetValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(this.currentDpiRatio, this.currentDpiRatio));
        }

        // Properties
        public static bool IsDropShadowEnabled
        {
            get
            {
                uint pvParam = 0;
                return (UnsafeNativeMethods.SystemParametersInfo(SystemParametersInfoAction.SPI_GETDROPSHADOW, 0, ref pvParam, 0) && (pvParam != 0));
            }
        }

        public static bool IsDwmEnabled
        {
            get
            {
                return ((((Environment.OSVersion.Version.Major >= 6) && !HardwareRenderingHelper.IsInSoftwareMode) && SafeNativeMethods.DwmIsCompositionEnabled()) && !SystemInformation.TerminalServerSession);
            }
        }
    }


}
