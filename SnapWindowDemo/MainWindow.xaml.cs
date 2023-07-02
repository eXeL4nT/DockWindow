using System.Windows;
using SnapWindow;

namespace SnapWindowDemo;

public partial class MainWindow : DockWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Dock();
    }

    protected override void Expand()
    {
        base.Expand();

        StackPanel.Visibility = Visibility.Visible;
    }

    protected override void Shrink()
    {
        base.Shrink();
        
        StackPanel.Visibility = Visibility.Collapsed;
    }
}