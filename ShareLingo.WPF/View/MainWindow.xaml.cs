using Microsoft.Extensions.DependencyInjection;
using NetForge.Core;
using NetForge.Wpf;
using ShareLingo.Core.ViewModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ShareLingo.WPF.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    #region Fields
    // Windows API constants for window messages
    private const int WM_NCHITTEST = 0x0084;
    private const int HTCLIENT = 1;
    private const int HTCAPTION = 2;
    private const int HTLEFT = 10;
    private const int HTRIGHT = 11;
    private const int HTTOP = 12;
    private const int HTTOPLEFT = 13;
    private const int HTTOPRIGHT = 14;
    private const int HTBOTTOM = 15;
    private const int HTBOTTOMLEFT = 16;
    private const int HTBOTTOMRIGHT = 17;
    private const int WM_GETMINMAXINFO = 0x0024;


    private readonly WpfNavigationService navigationService;
    private bool _isMaximized = false;
    private Rect _normalBounds;
    #endregion

    public MainWindow()
    {
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
        this.navigationService = (App.Current.Services.GetService<INavigationService>() as  WpfNavigationService)!;
        InitializeComponent();

        // Store normal bounds
        _normalBounds = new Rect(Left, Top, Width, Height);

        // Setup window message handling
        SourceInitialized += MainWindow_SourceInitialized;

        navigationService.ContentContainer = NavigationContainer;
    }

    #region Override
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
        IntPtr handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
    }
    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_NCHITTEST:
                handled = true;
                return HandleHitTest(lParam);
            case WM_GETMINMAXINFO:
                handled = true;
                HandleGetMinMaxInfo(lParam);
                break;
        }
        return IntPtr.Zero;
    }
    private IntPtr HandleHitTest(IntPtr lParam)
    {
        Point screenPoint = new Point(
            (short)(lParam.ToInt32() & 0xFFFF),
            (short)(lParam.ToInt32() >> 16)
        );

        Point clientPoint = PointFromScreen(screenPoint);

        // Window resize areas
        const int resizeArea = 10;

        // Check for resize areas
        if (clientPoint.Y <= resizeArea)
        {
            if (clientPoint.X <= resizeArea) return new IntPtr(HTTOPLEFT);
            if (clientPoint.X >= ActualWidth - resizeArea) return new IntPtr(HTTOPRIGHT);
            return new IntPtr(HTTOP);
        }

        if (clientPoint.Y >= ActualHeight - resizeArea)
        {
            if (clientPoint.X <= resizeArea) return new IntPtr(HTBOTTOMLEFT);
            if (clientPoint.X >= ActualWidth - resizeArea) return new IntPtr(HTBOTTOMRIGHT);
            return new IntPtr(HTBOTTOM);
        }

        if (clientPoint.X <= resizeArea) return new IntPtr(HTLEFT);
        if (clientPoint.X >= ActualWidth - resizeArea) return new IntPtr(HTRIGHT);

        // Check if we're in the title bar area (first 60 pixels, excluding window controls)
        if (clientPoint.Y <= 60 && clientPoint.X < ActualWidth - 135)
        {
            return new IntPtr(HTCAPTION);
        }

        return new IntPtr(HTCLIENT);
    }
    private void HandleGetMinMaxInfo(IntPtr lParam)
    {
        MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

        // Get monitor info
        IntPtr monitor = MonitorFromWindow(new WindowInteropHelper(this).Handle, 2);
        if (monitor != IntPtr.Zero)
        {
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);

            if (GetMonitorInfo(monitor, ref monitorInfo))
            {
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;

                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }
        }

        mmi.ptMinTrackSize.X = 800;
        mmi.ptMinTrackSize.Y = 600;

        Marshal.StructureToPtr(mmi, lParam, true);
    }
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // Double-click to maximize/restore
            ToggleMaximize();
        }
        else
        {
            // Single click to drag
            DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleMaximize()
    {
        if (_isMaximized)
        {
            // Restore
            Left = _normalBounds.Left;
            Top = _normalBounds.Top;
            Width = _normalBounds.Width;
            Height = _normalBounds.Height;

            MaximizeButton.Content = "\uE739"; // Maximize icon
            _isMaximized = false;
        }
        else
        {
            // Store current bounds
            _normalBounds = new Rect(Left, Top, Width, Height);

            // Get work area (screen minus taskbar)
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Left;
            Top = workArea.Top;
            Width = workArea.Width;
            Height = workArea.Height;

            MaximizeButton.Content = "\uE923"; // Restore icon
            _isMaximized = true;
        }
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            // Handle system maximize
            MaximizeButton.Content = "\uE923"; // Restore icon
            _isMaximized = true;
        }
        else if (WindowState == WindowState.Normal)
        {
            MaximizeButton.Content = "\uE739"; // Maximize icon
            _isMaximized = false;
        }
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        // Update normal bounds when window is moved (but not maximized)
        if (!_isMaximized && WindowState == WindowState.Normal)
        {
            _normalBounds = new Rect(Left, Top, Width, Height);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // Remove default window chrome
        IntPtr handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle).CompositionTarget.BackgroundColor = System.Windows.Media.Colors.Transparent;
    }
    #endregion
}