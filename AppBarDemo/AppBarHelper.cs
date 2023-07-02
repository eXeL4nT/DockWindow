using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using AppBarDemo.Native;

namespace AppBarDemo;

public static class AppBarHelper
{
    public static bool AppBarRegistered { get; private set; }
    public static int UCallBack { get; private set; }
    
    public static void RegisterAppBar(Window window)
    {
        if (!TryGetHWndSource(window, out var hWndSource))
            return;
        
        var appBarData = new APPBARDATA();
        appBarData.cbSize = Marshal.SizeOf(appBarData);
        appBarData.hWnd = hWndSource.Handle;

        if (AppBarRegistered) 
            return;
        
        UCallBack = NativeMethods.RegisterWindowMessage("AppBarMessage");
        appBarData.uCallbackMessage = UCallBack;

        NativeMethods.SHAppBarMessage(NativeConstants.ABM_NEW, ref appBarData);
        AppBarRegistered = true;

        SetAppBarPosition(window);
    }

    public static void UnregisterAppBar(Window window)
    {
        if (!TryGetHWndSource(window, out var hWndSource))
            return;
        
        var appBarData = new APPBARDATA();
        appBarData.cbSize = Marshal.SizeOf(appBarData);
        appBarData.hWnd = hWndSource.Handle;

        if (!AppBarRegistered) 
            return;
        
        NativeMethods.SHAppBarMessage(NativeConstants.ABM_REMOVE, ref appBarData);
        AppBarRegistered = false;
    }

    public static void SetAppBarPosition(Window window)
    {
        if (!AppBarRegistered)
            return;
        if (!TryGetHWndSource(window, out var hWndSource))
            return;
        
        var appBarData = new APPBARDATA();
        appBarData.cbSize = Marshal.SizeOf(appBarData);
        appBarData.hWnd = hWndSource.Handle;
        appBarData.uEdge = Settings.UEdge;

        if (appBarData.uEdge is NativeConstants.ABE_LEFT or NativeConstants.ABE_RIGHT)
        {
            appBarData.rc.top = 0;
            appBarData.rc.bottom = (int)SystemParameters.PrimaryScreenHeight;
            
            if (appBarData.uEdge == NativeConstants.ABE_LEFT)
            {
                appBarData.rc.left = 0;
                appBarData.rc.right = (int)window.ActualWidth;
            }
            else
            {
                appBarData.rc.right = (int)SystemParameters.PrimaryScreenWidth;
                appBarData.rc.left = appBarData.rc.right - (int)window.ActualWidth;
            }
        }
        else
        {
            appBarData.rc.left = 0;
            appBarData.rc.right = (int)SystemParameters.PrimaryScreenWidth;
            
            if (appBarData.uEdge == NativeConstants.ABE_TOP)
            {
                appBarData.rc.top = 0;
                appBarData.rc.bottom = (int)window.ActualHeight;
            }
            else
            {
                appBarData.rc.bottom = (int)SystemParameters.PrimaryScreenHeight;
                appBarData.rc.top = appBarData.rc.bottom - (int)window.ActualHeight;
            }
        }

        NativeMethods.SHAppBarMessage(NativeConstants.ABM_QUERYPOS, ref appBarData);
        NativeMethods.SHAppBarMessage(NativeConstants.ABM_SETPOS, ref appBarData);
        
        NativeMethods.MoveWindow(
            appBarData.hWnd, 
            appBarData.rc.left, 
            appBarData.rc.top,
            appBarData.rc.right - appBarData.rc.left, 
            appBarData.rc.bottom - appBarData.rc.top, 
            true);
    }

    public static void ExtendGlass(Window window)
    {
        try
        {
            var isGlassEnabled = 0;
            NativeMethods.DwmIsCompositionEnabled(ref isGlassEnabled);

            if (Environment.OSVersion.Version.Major <= 5 || isGlassEnabled <= 0)
                return;
            if (!TryGetHWndSource(window, out var hWndSource))
                return;
            if (hWndSource.CompositionTarget is null)
                return;
            
            hWndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
            window.Background = Brushes.Transparent;
                    
            var margins = new MARGINS
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            NativeMethods.DwmExtendFrameIntoClientArea(hWndSource.Handle, ref margins);
        }
        catch (DllNotFoundException ex)
        {
            
        }
    }

    public static bool TryGetHWndSource(Window window, out HwndSource hWndSource)
    {
        var wHandle = new WindowInteropHelper(window).Handle;
        hWndSource = HwndSource.FromHwnd(wHandle);
        
        return hWndSource is not null;
    }
}