using System.Windows;
using System.Windows.Interop;

namespace SnapWindow;

public static class WindowHelper
{
    public static void AddHook(Window window, HwndSourceHook hWndSourceHook)
    {
        var hWnd = new WindowInteropHelper(window).Handle;
        HwndSource.FromHwnd(hWnd)?.AddHook(hWndSourceHook);
    }
}