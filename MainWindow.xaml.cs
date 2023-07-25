using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Clock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
       
       

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        IntPtr Handle;
        DispatcherTimer timer;
        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion


        public MainWindow()
        {
     
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Zmien;
            timer.Start();
            Left = 0;
            Top = 0;
            menu.Visibility = Visibility.Hidden;

            

            Window w = new Window();
            w.Width = 10;
            w.Height = 10;
            w.Left = 0;
            w.Top = 0;
            w.Background = Brushes.Black;
            w.WindowStyle = WindowStyle.None;
            w.AllowsTransparency = true;
            w.Topmost = true;
            w.ShowInTaskbar = false;
            w.MouseEnter += Clock_MouseEnter;
            w.MouseLeave += Clock_MouseLeave;
            w.MouseRightButtonDown += label_MouseRightButtonDown;
            w.Show();
            
        }

        private void Clock_MouseLeave(object sender, EventArgs e)
        {

            Handle = new WindowInteropHelper(this).Handle;
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            
        }

        private void Clock_MouseEnter(object sender, EventArgs e)
        {
            Topmost = true;
            Topmost = false;

        }

        private void Zmien(object sender, EventArgs e)
        {
            string[] time = new string[3];
            string[] time2 = new string[3];
            if (DateTime.Now.Hour.ToString().Length == 1)
            {
                time[0] = "0"+DateTime.Now.Hour.ToString();
            }
            else
            {
                time[0] = DateTime.Now.Hour.ToString();
            }

            if (DateTime.Now.Minute.ToString().Length == 1)
            {
                time[1] = "0" + DateTime.Now.Minute.ToString();
            }
            else
            {
                time[1] = DateTime.Now.Minute.ToString();
            }

            if (DateTime.Now.Second.ToString().Length == 1)
            {
                time[2] = "0" + DateTime.Now.Second.ToString();
            }
            else
            {
                time[2] = DateTime.Now.Second.ToString();
                
            }
            


            label.Content = time[0] + ':' + time[1] + ':' + time[2];
            

        }

        private void label_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (label.Foreground == Brushes.Black)
            {
                label.Foreground = Brushes.White;
                maturka.Foreground = Brushes.White;
            }
            else
            {
                label.Foreground = Brushes.Black;
                maturka.Foreground = Brushes.Black;
            }
        }

        private void cont_men(object sender, MouseButtonEventArgs e)
        {
            
            menu.Margin = new Thickness(Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y,0,0);
            menu.Visibility = Visibility.Visible;

        }


        private void hide_menu(object sender, EventArgs e)
        {
            menu.Visibility = Visibility.Hidden;
        }

        private void cont_hid(object sender, MouseButtonEventArgs e)
        {
            menu.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
    }
}
