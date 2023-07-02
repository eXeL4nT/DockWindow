using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SnapWindow;

public class Screen
{
    private const int PrimaryMonitor = unchecked((int)0xBAADF00D);
    private const int MonitorInfoFPrimary = 0x00000001;
    
    private static readonly bool HasMultipleMonitors;
    
    private readonly IntPtr _monitorHandle;

    public static Screen PrimaryScreen
    {
        get
        {
            return HasMultipleMonitors ? AllScreens.FirstOrDefault(s => s.Primary) : new Screen(PrimaryMonitor);
        }
    }
    
    public static IEnumerable<Screen> AllScreens
    {
        get
        {
            if (!HasMultipleMonitors) 
                return new[] { new Screen(PrimaryMonitor) };
            
            var monitorEnumCallback = new MonitorEnumCallback();
            var monitorEnumProc = new NativeMethods.MonitorEnumProc(monitorEnumCallback.Callback);
                
            NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, monitorEnumProc, IntPtr.Zero);

            return monitorEnumCallback.Screens.Count > 0
                ? monitorEnumCallback.Screens.Cast<Screen>()
                : new[] { new Screen(PrimaryMonitor) };
        }
    }
    
    public string DeviceName { get; }
    public Rect Bounds { get; }
    public bool Primary { get; }
    public double ScaleFactor { get; } = 1.0;

    public Rect WorkingArea
    {
        get
        {
            Rect workingArea;

            if (!HasMultipleMonitors || _monitorHandle is PrimaryMonitor)
            {
                var rc = new RECT();

                NativeMethods.SystemParametersInfo(Spi.SPI_GETWORKAREA, 0, ref rc, SpiF.SPIF_SENDCHANGE);

                workingArea = new Rect(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);
            }
            else
            {
                var info = new MonitorInfo();
                NativeMethods.GetMonitorInfo(new HandleRef(null, _monitorHandle), info);

                workingArea = 
                    new Rect(
                        info.rcWork.Left, 
                        info.rcWork.Top, 
                        info.rcWork.Right - info.rcWork.Left, 
                        info.rcWork.Bottom - info.rcWork.Top);
            }

            return workingArea;
        }
    }
    
    static Screen()
    {
        HasMultipleMonitors = NativeMethods.GetSystemMetrics(SystemMetric.SM_CMONITORS) != 0;
    }

    private Screen(IntPtr monitor) : this(monitor, IntPtr.Zero)
    {
        
    }

    private Screen(IntPtr monitor, IntPtr hdc)
    {
        if (NativeMethods.IsProcessDPIAware())
        {
            uint dpiX;
            
            try
            {
                if (monitor is PrimaryMonitor)
                {
                    var ptr = NativeMethods.MonitorFromPoint(new POINTSTRUCT(0, 0), MonitorDefault.MONITOR_DEFAULTTOPRIMARY);
                    NativeMethods.GetDpiForMonitor(ptr, DpiType.EFFECTIVE, out dpiX, out _);
                }
                else
                {
                    NativeMethods.GetDpiForMonitor(monitor, DpiType.EFFECTIVE, out dpiX, out _);
                }
            }
            catch
            {
                var hr = 
                    NativeMethods.D2D1CreateFactory(
                        D2D1FactoryType.D2D1_FACTORY_TYPE_SINGLE_THREADED, 
                        typeof(ID2D1Factory).GUID, 
                        IntPtr.Zero,
                        out var factory);

                if (hr < 0)
                {
                    dpiX = 96;
                }
                else
                {
                    factory.GetDesktopDpi(out var x, out _);
                    Marshal.ReleaseComObject(factory);
                    dpiX = (uint)x;
                }
            }
            
            ScaleFactor = dpiX / 96.0;
        }

        if (!HasMultipleMonitors || monitor == PrimaryMonitor)
        {
            var size = new Size(
                NativeMethods.GetSystemMetrics(SystemMetric.SM_CXSCREEN),
                NativeMethods.GetSystemMetrics(SystemMetric.SM_CYSCREEN));

            Bounds = new Rect(0, 0, size.Width, size.Height);
            Primary = true;
            DeviceName = "DISPLAY";
        }
        else
        {
            var monitorInfo = new MonitorInfo();
            
            NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), monitorInfo);
            
            Bounds = new Rect(
                monitorInfo.rcMonitor.Left,
                monitorInfo.rcMonitor.Top,
                monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left,
                monitorInfo.rcMonitor.Bottom - monitorInfo.rcMonitor.Top);
            Primary = (monitorInfo.dwFlags & MonitorInfoFPrimary) != 0;
            DeviceName = new string(monitorInfo.szDevice).TrimEnd((char)0);
        }

        _monitorHandle = monitor;
    }

    public static Screen FromHandle(IntPtr hWnd)
    {
        return HasMultipleMonitors
            ? new Screen(NativeMethods.MonitorFromWindow(new HandleRef(null, hWnd), 2))
            : new Screen(PrimaryMonitor);
    }
    
    public static Screen FromWindow(Window window)
    {
        return FromHandle(new WindowInteropHelper(window).Handle);
    }
    
    private class MonitorEnumCallback
    {
        public ArrayList Screens { get; }

        public MonitorEnumCallback()
        {
            Screens = new ArrayList();
        }
        
        public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lpRcMonitor, IntPtr lParam)
        {
            Screens.Add(new Screen(monitor, hdc));
            return true;
        }
    }
}