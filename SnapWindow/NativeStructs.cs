using System.Runtime.InteropServices;
using System.Windows;

namespace SnapWindow;

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }

    public RECT(Rect r)
    {
        Left = (int)r.Left;
        Top = (int)r.Top;
        Right = (int)r.Right;
        Bottom = (int)r.Bottom;
    }

    public static RECT FromXyWh(int x, int y, int width, int height)
    {
        return new RECT(x, y, x + width, y + height);
    }

    public Size Size => new Size(Right - Left, Bottom - Top);
}

[StructLayout(LayoutKind.Sequential)]
public struct POINTSTRUCT
{
    public int x;
    public int y;

    public POINTSTRUCT(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("06152247-6f50-465a-9245-118bfd3b6007")]
internal interface ID2D1Factory
{
    int ReloadSystemMetrics();

    [PreserveSig]
    void GetDesktopDpi(out float dpiX, out float dpiY);

    // the rest is not implemented as we don't need it
}

[StructLayout(LayoutKind.Sequential)]
public class ComRect
{
    public int bottom;
    public int left;
    public int right;
    public int top;

    public ComRect()
    {
    }

    public ComRect(Rect r)
    {
        left = (int)r.X;
        top = (int)r.Y;
        right = (int)r.Right;
        bottom = (int)r.Bottom;
    }

    public ComRect(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public static ComRect FromXyWh(int x, int y, int width, int height)
    {
        return new ComRect(x, y, x + width, y + height);
    }

    public override string ToString()
    {
        return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
public class MonitorInfo
{
    internal int cbSize = Marshal.SizeOf(typeof(MonitorInfo));

    internal RECT rcMonitor = new RECT();
    internal RECT rcWork = new RECT();
    internal int dwFlags = 0;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    internal char[] szDevice = new char[32];
}