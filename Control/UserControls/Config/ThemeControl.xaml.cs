using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GeekDesk.Control.UserControls.Config
{
    /// <summary>
    /// MotionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThemeControl : System.Windows.Controls.UserControl
    {

        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        public ThemeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 修改背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGButton_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Multiselect = false, //只允许选中单个文件
                    Filter = "图像文件(*.png, *.jpg)|*.png;*.jpg;*.gif"
                };
                if (ofd.ShowDialog() == true)
                {
                    appConfig.BitmapImage = ImageUtil.GetBitmapImageByFile(ofd.FileName);
                    appConfig.BacImgName = ofd.FileName;
                }
            }
            catch (Exception)
            {
                HandyControl.Controls.Growl.WarningGlobal("修改背景失败,已重置为默认背景!");
            }

        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPanel.Visibility = Visibility.Visible;
        }

        private void ColorPicker_Canceled(object sender, EventArgs e)
        {
            ColorPanel.Visibility = Visibility.Collapsed;
        }

        private void ColorPicker_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            SolidColorBrush scb =  ColorPicker.SelectedBrush;
            appConfig.TextColor = scb.ToString();
            ColorPanel.Visibility = Visibility.Collapsed;
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
    }
}
