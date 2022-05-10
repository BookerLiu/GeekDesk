using GeekDesk.Util;
using GeekDesk.ViewModel;
using GeekDesk.ViewModel.Temp;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GradientBGDialog
    {
        public HandyControl.Controls.Dialog dialog;

        public GradientBGDialog()
        {
            this.DataContext = GradientBGParamList.GradientBGParams;
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            dialog.Close();
        }

        private void BGBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GradientBGParam bgParam = (sender as Border).Tag as GradientBGParam;
            MainWindow.appData.AppConfig.GradientBGParam = bgParam;
            BGSettingUtil.BGSetting();
        }
    }
}
