using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SnapWindow;

internal static class NativeMethods
{
    public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lpRcMonitor, IntPtr lParam);
    
    internal static readonly HandleRef NullHandleRef = new(null, IntPtr.Zero);
    
    [DllImport("shcore.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern IntPtr GetDpiForMonitor([In] IntPtr hMonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool GetMonitorInfo(HandleRef hMonitor, [In][Out] MonitorInfo info);
    
    [DllImport("user32.dll", ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern bool EnumDisplayMonitors(HandleRef hdc, ComRect rcClip, MonitorEnumProc lpFnEnum, IntPtr dwData);
    
    [DllImport("user32.dll", ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);
    
    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern int GetSystemMetrics(SystemMetric nIndex);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern bool SystemParametersInfo(Spi nAction, int nParam, ref RECT rc, SpiF nUpdate);
    
    [DllImport("user32.dll", ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    internal static extern IntPtr MonitorFromPoint(POINTSTRUCT pt, MonitorDefault flags);
    
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool IsProcessDPIAware();
    
    [DllImport("d2d1.dll")]
    internal static extern int D2D1CreateFactory(D2D1FactoryType factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid riId, IntPtr pFactoryOptions, out ID2D1Factory ppIFactory);
}