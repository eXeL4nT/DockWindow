using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppBarDemo.Native;

namespace AppBarDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _nclbd;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (AppBarHelper.TryGetHWndSource(this, out var hWndSource))
            {
                hWndSource.AddHook(WndProc);
                
                AppBarHelper.ExtendGlass(this);
                AppBarHelper.RegisterAppBar(this);
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (_nclbd)
            {
                if (AppBarHelper.AppBarRegistered)
                {
                    AppBarHelper.UnregisterAppBar(this);
                    // @TODO tp.Show();
                }
                
                // @TODO RefreshTransPrev();
                CalculateHorizontalEdge();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            AppBarHelper.UnregisterAppBar(this);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == AppBarHelper.UCallBack && wParam.ToInt32() == NativeConstants.ABN_POSCHANGED)
            {
                AppBarHelper.SetAppBarPosition(this);
                handled = true;
            }
            else if (msg == NativeConstants.WM_DWMCOMPOSITIONCHANGED)
            {
                AppBarHelper.ExtendGlass(this);
                handled = true;
            }
            else if (msg == NativeConstants.WM_NCLBUTTONDOWN)
            {
                _nclbd = true;
            }
            else if (msg == NativeConstants.WM_EXITSIZEMOVE)
            {
                _nclbd = false;
                // tp.Hide();
                CalculateHorizontalEdge();
                AppBarHelper.RegisterAppBar(this);
            }
            
            return IntPtr.Zero;
        }

        private void CalculateHorizontalEdge()
        {
            if (SystemParameters.PrimaryScreenWidth / 2 > Left)
            {
                Settings.UEdge = NativeConstants.ABE_LEFT;
            }
            else
            {
                Settings.UEdge = NativeConstants.ABE_RIGHT;   
            }
        }
    }
}