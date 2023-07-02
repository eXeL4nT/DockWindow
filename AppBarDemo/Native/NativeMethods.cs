using System;
using System.Runtime.InteropServices;

namespace AppBarDemo.Native;

internal static class NativeMethods
{
    [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
    internal static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
    
    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
    
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    internal static extern int RegisterWindowMessage(string msg);
    
    [DllImport("dwmapi.dll")]
    internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
    [DllImport("dwmapi.dll")]
    internal static extern int DwmIsCompositionEnabled(ref int en);
}