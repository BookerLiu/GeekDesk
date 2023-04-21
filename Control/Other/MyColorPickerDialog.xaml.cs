using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace GeekDesk.Control.Other
{

    public enum ColorType
    {
        COLOR_1 = 1,
        COLOR_2 = 2,
        TEXT_COLOR = 3
    }

    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MyColorPickerDialog
    {
        public static ColorType COLOR_TYPE = new ColorType();
        private static AppConfig appConfig = MainWindow.appData.AppConfig;

        public static HandyControl.Controls.Dialog dialog;
        private System.Windows.Controls.Primitives.ToggleButton toggleButton = null;

        private static ColorType colorType;


        public MyColorPickerDialog(string strType, string token)
        {
            InitializeComponent();
            switch (strType)
            {
                case "Color1":
                    colorType = ColorType.COLOR_1; break;
                case "Color2":
                    colorType = ColorType.COLOR_2; break;
                default:
                    colorType = ColorType.TEXT_COLOR; break;
            }
            dialog = HandyControl.Controls.Dialog.Show(this, token);
        }


        /// <summary>
        /// 取消按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyColorPicker_Canceled(object sender, EventArgs e)
        {
            MyColorPickerClose(sender);
        }
        private void MyColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            MyColorPickerClose(sender);
        }

        private void MyColorPicker_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            SolidColorBrush scb = MyColorPicker.SelectedBrush;
            Color c = scb.Color;
            switch (colorType)
            {
                case ColorType.COLOR_1:
                    appConfig.GradientBGParam.Color1 = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B); break;
                case ColorType.COLOR_2:
                    appConfig.GradientBGParam.Color2 = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B); break;
                default:
                    appConfig.TextColor = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B); break;
            }
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
                Window.GetWindow(this).DragMove();
            }
        }


        private void MyColorPicker_Checked(object sender, RoutedEventArgs e)
        {
            toggleButton = e.OriginalSource as System.Windows.Controls.Primitives.ToggleButton;

            PixelColorPickerWindow colorPickerWindow = new PixelColorPickerWindow(MyColorPicker);
            colorPickerWindow.Show();
        }

        private void MyColorPickerClose(object sender)
        {
            dialog.Close();
        }

    }
}
