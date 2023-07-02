using System.Windows;
using System.Windows.Media;
using SnapWindow.Logger;

namespace SnapWindow;

public static class SnapHelper
{
    private static readonly SimpleLogger Logger = new();
    
    private static readonly SnapWindow SnapWindow;
    private static SnapEdge? _snapEdge;
    private static string _screenName;

    public static double SnapHeight { get; set; }
    public static double SnapWidth { get; set; }
    
    static SnapHelper()
    {
        SnapWindow = new SnapWindow
        {
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            ShowInTaskbar = false,
            Topmost = true,
            Background = Brushes.Transparent,
            Border =
            {
                Background = new SolidColorBrush(Colors.Orange),
                Opacity = 0.3,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10)
            }
        };

        SnapHeight = 100;
        SnapWidth = 100;
    }

    public static void Show()
    {
        if (SnapWindow.IsClosed)
            return;
        
        SnapWindow.Show();
    }

    public static void Hide()
    {
        if (SnapWindow.IsClosed)
            return;
        
        SnapWindow.Hide();
    }

    public static void ChangePosition(Screen screen, SnapEdge snapEdge)
    {
        if (_snapEdge == snapEdge && _screenName == screen.DeviceName)
            return;
        
        _snapEdge = snapEdge;
        _screenName = screen.DeviceName;
        
        var workingArea = screen.WorkingArea;
        var newPosition = new Rect();
        
        switch (snapEdge)
        {
            case SnapEdge.Top:
                newPosition.X = workingArea.Left;
                newPosition.Y = 0;
                newPosition.Height = SnapHeight;
                newPosition.Width = workingArea.Width;
                break;
            case SnapEdge.Left:
                newPosition.X = workingArea.Left;
                newPosition.Y = 0;
                newPosition.Height = workingArea.Height;
                newPosition.Width = SnapWidth;
                break;
        }
        
        Logger.Debug($"Change position to {newPosition.ToString()}");
        
        SnapWindow.ChangePosition(newPosition);
    }
}