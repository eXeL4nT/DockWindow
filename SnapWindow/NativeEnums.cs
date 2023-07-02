using System;

namespace SnapWindow;

public enum SystemMetric
{
    SM_CXSCREEN = 0,
    SM_CYSCREEN = 1,
    SM_XVIRTUALSCREEN = 76,
    SM_YVIRTUALSCREEN = 77,
    SM_CXVIRTUALSCREEN = 78,
    SM_CYVIRTUALSCREEN = 79,
    SM_CMONITORS = 80
}

public enum DpiType
{
    EFFECTIVE = 0,
    ANGULAR = 1,
    RAW = 2
}

public enum MonitorDefault
{
    /// <summary>If the point is not contained within any display monitor, return a handle to the display monitor that is nearest to the point.</summary>
    MONITOR_DEFAULTTONEAREST = 0x00000002,

    /// <summary>If the point is not contained within any display monitor, return NULL.</summary>
    MONITOR_DEFAULTTONULL = 0x00000000,

    /// <summary>If the point is not contained within any display monitor, return a handle to the primary display monitor.</summary>
    MONITOR_DEFAULTTOPRIMARY = 0x00000001
}

public enum D2D1FactoryType
{
    D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
    D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
}

public enum Spi : uint
{
    /// <summary>
    /// Retrieves the size of the work area on the primary display monitor. The work area is the portion of the screen not obscured
    /// by the system taskbar or by application desktop toolbars. The pvParam parameter must point to a RECT structure that receives
    /// the coordinates of the work area, expressed in virtual screen coordinates.
    /// To get the work area of a monitor other than the primary display monitor, call the GetMonitorInfo function.
    /// </summary>
    SPI_GETWORKAREA = 0x0030
}

[Flags]
public enum SpiF
{
    None = 0x00,

    /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
    SPIF_UPDATEINIFILE = 0x01,

    /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
    SPIF_SENDCHANGE = 0x02,

    /// <summary>Same as SPIF_SENDCHANGE.</summary>
    SPIF_SENDWININICHANGE = 0x02
}
