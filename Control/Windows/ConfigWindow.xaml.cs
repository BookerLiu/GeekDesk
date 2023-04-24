using GeekDesk.Control.UserControls.Config;
using GeekDesk.Interface;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
        private static List<UserControl> ucList = new List<UserControl>();
        static ConfigWindow()
        {
            ucList.Add(about);
            ucList.Add(theme);
            ucList.Add(motion);
            ucList.Add(other);
        }
        public MainWindow mainWindow;

        private ConfigWindow(AppConfig appConfig, MainWindow mainWindow)
        {
            InitializeComponent();
            //BG.Source = ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_BAC_IMAGE_BASE64);
            this.DataContext = appConfig;
            RightCard.Content = about;
            WindowUtil.SetOwner(this, mainWindow);
            this.mainWindow = mainWindow;
            UFG.Visibility = Visibility.Collapsed;
            UFG.Visibility = Visibility.Visible;
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
                    UFG.Visibility = Visibility.Collapsed;
                    RightCard.Content = motion;
                    UFG.Visibility = Visibility.Visible;
                    break;
                case "Theme":
                    UFG.Visibility = Visibility.Collapsed;
                    RightCard.Content = theme;
                    UFG.Visibility = Visibility.Visible;
                    break;
                case "Other":
                    UFG.Visibility = Visibility.Collapsed;
                    RightCard.Content = other;
                    UFG.Visibility = Visibility.Visible;
                    break;
                default:
                    UFG.Visibility = Visibility.Collapsed;
                    RightCard.Content = about;
                    UFG.Visibility = Visibility.Visible;
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
                this.DataContext = null;
                this.Close();
            }
        }
    }
}
