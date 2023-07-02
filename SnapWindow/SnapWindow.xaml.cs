using System;
using System.Windows;

namespace SnapWindow;

public partial class SnapWindow : Window
{
    public bool IsClosed { get; private set; }

    public SnapWindow()
    {
        InitializeComponent();
    }

    public void ChangePosition(Rect newPosition)
    {
        Left = newPosition.Left;
        Top = newPosition.Top;
        Height = newPosition.Height;
        Width = newPosition.Width;
    }
    
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        
        IsClosed = true;
    }
}