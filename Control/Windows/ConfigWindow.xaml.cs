
using GeekDesk.Constant;
using GeekDesk.Control.UserControls;
using GeekDesk.Control.UserControls.Config;
using GeekDesk.Interface;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// ConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : IWindowCommon
    {
        private static readonly AboutControl about = new AboutControl();
        private static readonly ThemeControl theme = new ThemeControl();
        private static readonly MotionControl motion = new MotionControl();
        private static readonly OtherControl other = new OtherControl();
        public MainWindow mainWindow;

        private ConfigWindow(AppConfig appConfig, MainWindow mainWindow)
        {
            InitializeComponent();
            //BG.Source = ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_BAC_IMAGE_BASE64);
            this.DataContext = appConfig;
            RightCard.Content = about;
            this.Topmost = true;
            this.mainWindow = mainWindow;
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
                case "Other":
                    RightCard.Content = other;
                    break;
                default:
                    RightCard.Content = about;
                    break;
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
                this.DragMove();
            }
        }

        private static System.Windows.Window window = null;
        public static void Show(AppConfig appConfig, MainWindow mainWindow)
        {
            if (window == null || !window.Activate())
            {
                window = new ConfigWindow(appConfig, mainWindow);
            }
            window.Show();
            Keyboard.Focus(window);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
