using GeekDesk.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class BGNmaeDialog
    {
        public HandyControl.Controls.Dialog dialog;

        public BGNmaeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, RoutedEventArgs e)
        {
            GradientBGParam bg = new GradientBGParam();
            bg.Id = Guid.NewGuid().ToString();
            bg.Name = BGName.Text;
            bg.Color1 = MainWindow.appData.AppConfig.GradientBGParam.Color1;
            bg.Color2 = MainWindow.appData.AppConfig.GradientBGParam.Color2;
            MainWindow.appData.AppConfig.CustomBGParams.Add(bg);
            MainWindow.appData.AppConfig.CustomBGParams = DeepCopyUtil.DeepCopy(MainWindow.appData.AppConfig.CustomBGParams);
            dialog.Close();
        }

    }
}
