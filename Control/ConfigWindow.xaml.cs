
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
        public ConfigWindow(AppConfig appConfig)
        {
            InitializeComponent();
            this.DataContext = appConfig;
            RightCard.Content = new SettingControl();
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
    }
}
