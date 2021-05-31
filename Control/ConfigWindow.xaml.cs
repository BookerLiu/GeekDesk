
using GalaSoft.MvvmLight.Command;
using GeekDesk.Control.UserControls;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control
{
    /// <summary>
    /// ConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow
    {
        private static AboutControl about = new AboutControl();
        private static ThemeControl theme = new ThemeControl();
        private static MotionControl motion = new MotionControl();

        public ConfigWindow(AppConfig appConfig)
        {
            InitializeComponent();
            this.DataContext = appConfig;
            RightCard.Content = about;
            this.Topmost = true;
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
                DragMove();
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MemuClick(object sender, RoutedEventArgs e)
        {
            SideMenuItem smi = sender as SideMenuItem;
            switch (smi.Tag.ToString())
            {
                case "Motion":
                    RightCard.Content = motion;
                    break;
                case "Theme":
                    RightCard.Content = theme;
                    break;
                default:
                    RightCard.Content = about;
                    break;
            }
        }
    }
}
