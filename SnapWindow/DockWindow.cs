using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SnapWindow;

public class DockWindow : Window
{
    #region DependencyProperty ShrinkWidth

    public static readonly DependencyProperty ShrinkWidthProperty =
        DependencyProperty.Register(
            nameof(ShrinkWidth),
            typeof(int),
            typeof(DockWindow),
            new PropertyMetadata(3));

    public int ShrinkWidth
    {
        get => (int)GetValue(ShrinkWidthProperty);
        set => SetValue(ShrinkWidthProperty, value);
    }

    #endregion

    #region DependencyProperty ShrinkHeight

    public static readonly DependencyProperty ShrinkHeightProperty =
        DependencyProperty.Register(
            nameof(ShrinkHeight),
            typeof(int),
            typeof(DockWindow),
            new PropertyMetadata(3));

    public int ShrinkHeight
    {
        get => (int)GetValue(ShrinkHeightProperty);
        set => SetValue(ShrinkHeightProperty, value);
    }

    #endregion

    #region DependencyProperty ShrinkInterval

    public static readonly DependencyProperty ShrinkIntervalProperty =
        DependencyProperty.Register(
            nameof(ShrinkInterval),
            typeof(TimeSpan),
            typeof(DockWindow),
            new PropertyMetadata(TimeSpan.FromMilliseconds(250)));

    public TimeSpan ShrinkInterval
    {
        get => (TimeSpan)GetValue(ShrinkIntervalProperty);
        set => SetValue(ShrinkIntervalProperty, value);
    }

    #endregion

    private readonly DispatcherTimer _shrinkTimer;

    private bool _isDragging;
    private bool _isShrinked;
    private SnapEdge _snapEdge;
    private DockEdge _dockEdge;
    private double _expandWidth;
    private double _expandHeight;


    public DockWindow()
    {
        _shrinkTimer =
            new DispatcherTimer(
                ShrinkInterval,
                DispatcherPriority.Background,
                OnShrinkTimerTimeout,
                Dispatcher.CurrentDispatcher);
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);

        ChangeSnapPosition();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        _isDragging = true;

        _shrinkTimer.Stop();

        PreventAeroSnap();
        ChangeSnapPosition();
        SnapHelper.Show();
        DragMove();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        _isDragging = false;

        ResolveAeroSnapPreventing();
        SnapHelper.Hide();
        Dock();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        _shrinkTimer.Stop();
        Expand();
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        
        if (_isDragging)
            return;

        _shrinkTimer.Start();
    }

    protected void Dock()
    {
        if (!TryGetDockPosition(out var dockEdge, out var dockPosition))
            return;
        
        _dockEdge = dockEdge;

        if (dockEdge is DockEdge.Left)
        {
            Left = dockPosition.Left;
            Width = dockPosition.Width;
            Top = dockPosition.Top;
            Height = dockPosition.Height;
        }
        else if (dockEdge is DockEdge.Top)
        {
            Top = dockPosition.Top;
            Height = dockPosition.Height;
            Left = dockPosition.Left;
            Width = dockPosition.Width;
        }
        
        PreventAeroSnap();
    }
    
    protected virtual void Expand()
    {
        _isShrinked = false;

        switch (_dockEdge)
        {
            case DockEdge.Left:
                Width = _expandWidth;
                // BeginAnimation(WidthProperty, new DoubleAnimation
                // {
                //     From = Width,
                //     To = _expandWidth,
                //     Duration = new Duration(TimeSpan.FromMilliseconds(250))
                // });
                break;
            case DockEdge.Top:
                Height = _expandHeight;
                // BeginAnimation(HeightProperty, new DoubleAnimation
                // {
                //     From = Height,
                //     To = _expandHeight,
                //     Duration = new Duration(TimeSpan.FromMilliseconds(250))
                // });
                break;
        }
    }

    protected virtual void Shrink()
    {
        _isShrinked = true;

        switch (_dockEdge)
        {
            case DockEdge.Left:
                _expandWidth = Width;
                Width = ShrinkWidth;
                // BeginAnimation(WidthProperty, new DoubleAnimation
                // {
                //     From = Width,
                //     To = ShrinkWidth,
                //     Duration = new Duration(TimeSpan.FromMilliseconds(250))
                // });
                break;
            case DockEdge.Top:
                _expandHeight = Height;
                Height = ShrinkHeight;
                // BeginAnimation(HeightProperty, new DoubleAnimation
                // {
                //     From = Height,
                //     To = ShrinkHeight,
                //     Duration = new Duration(TimeSpan.FromMilliseconds(250))
                // });
                break;
        }
    }

    private void OnShrinkTimerTimeout(object sender, EventArgs e)
    {
        _shrinkTimer.Stop();
        Shrink();
    }

    private void PreventAeroSnap()
    {
        if (ResizeMode == ResizeMode.NoResize)
            return;

        ResizeMode = ResizeMode.NoResize;
        UpdateLayout();
    }

    private void ResolveAeroSnapPreventing()
    {
        if (ResizeMode != ResizeMode.NoResize)
            return;

        ResizeMode = ResizeMode.CanResizeWithGrip;
        UpdateLayout();
    }

    private void ChangeSnapPosition()
    {
        GetMouseDistance(out var topDistance, out var leftDistance);

        var currentScreen = Screen.FromWindow(this);
        var smallestDistance = Math.Min(topDistance, leftDistance);
        
        if (smallestDistance == topDistance)
        {
            _snapEdge = SnapEdge.Top;
            SnapHelper.ChangePosition(currentScreen, _snapEdge);
        }
        else if (smallestDistance == leftDistance)
        {
            _snapEdge = SnapEdge.Left;
            SnapHelper.ChangePosition(currentScreen, _snapEdge);
        }
    }

    private void GetMouseDistance(out int topDistance, out int leftDistance)
    {
        try
        {
            var mousePosition = Mouse.GetPosition(this);
            var screenPoint = PointToScreen(mousePosition);

            topDistance = (int)screenPoint.Y;
            leftDistance = (int)screenPoint.X;
        }
        catch (Exception)
        {
            topDistance = (int)Top;
            leftDistance = (int)Left;
        }
    }

    private bool TryGetDockPosition(out DockEdge dockEdge, out Rect dockPosition)
    {
        dockEdge = DockEdge.None;
        dockPosition = new Rect();

        var currentScreen = Screen.FromWindow(this);
        var screenArea = currentScreen.WorkingArea;

        switch (_snapEdge)
        {
            case SnapEdge.Left:
                dockEdge = DockEdge.Left;
                dockPosition.X = screenArea.Left;
                dockPosition.Y = 0;
                dockPosition.Width = 160;
                dockPosition.Height = screenArea.Height;
                break;
            case SnapEdge.Top:
                dockEdge = DockEdge.Top;
                dockPosition.X = screenArea.Left;
                dockPosition.Y = 0;
                dockPosition.Width = screenArea.Width;
                dockPosition.Height = 80;
                break;
            default:
                return false;
        }

        return true;
    }
}