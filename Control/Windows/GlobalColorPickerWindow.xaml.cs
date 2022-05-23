using GeekDesk.Interface;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// GlobalColorPickerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalColorPickerWindow : IWindowCommon
    {
        PixelColorPickerWindow colorPickerWindow = null;
        public GlobalColorPickerWindow()
        {
            this.Topmost = true;
            InitializeComponent();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DataContext = null;
                this.Close();
            }
        }


        /// <summary>
        /// 取消按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyColorPicker_Canceled(object sender, EventArgs e)
        {
            MyColorPickerClose();
        }
        private void MyColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            Color c = MyColorPicker.SelectedBrush.Color;
            Clipboard.SetData(DataFormats.Text, string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B));
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }


        private void MyColorPicker_Checked(object sender, RoutedEventArgs e)
        {           
            this.Hide();
            if (colorPickerWindow == null || !colorPickerWindow.Activate())
            {
                colorPickerWindow = new PixelColorPickerWindow(MyColorPicker);
            }
            colorPickerWindow.Show();
        }

        private void MyColorPickerClose()
        {
            this.Close();
        }

        private static System.Windows.Window window = null;

        public static void CreateNoShow()
        {
            if (window == null || !window.Activate())
            {
                window = new GlobalColorPickerWindow();
            }
            window.Hide();
            GlobalColorPickerWindow thisWindow = (GlobalColorPickerWindow)window;
            if (thisWindow.colorPickerWindow == null || !thisWindow.colorPickerWindow.Activate())
            {
                thisWindow.colorPickerWindow = new PixelColorPickerWindow(thisWindow.MyColorPicker);
            }
            thisWindow.colorPickerWindow.Show();
        }

        public static void Show()
        {
            if (window == null || !window.Activate())
            {
                window = new GlobalColorPickerWindow();
            }
            window.Show();
            Keyboard.Focus(window);
        }

        public static void ShowOrHide()
        {
            if (window == null || !window.Activate())
            {
                window = new GlobalColorPickerWindow();
                window.Show();
                Keyboard.Focus(window);
            }
            else
            {
                window.Close();
            }
        }

        private void MyColorPicker_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            Show();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Opacity = 1;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Opacity = 0.6;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
