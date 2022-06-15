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
    public partial class IconInfoDialog
    {
        public HandyControl.Controls.Dialog dialog;

        public IconInfoDialog()
        {
            InitializeComponent();
        }

        public IconInfoDialog(IconInfo info)
        {
            this.DataContext = info;
            InitializeComponent();
        }

        /// <summary>
        /// 保存修改属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProperty(object sender, RoutedEventArgs e)
        {
            IconInfo info = this.DataContext as IconInfo;
            info.BitmapImage = IconImg.Source as BitmapImage;
            info.Name = IconName.Text;
            info.AdminStartUp = IconIsAdmin.IsChecked.Value;
            info.StartArg = StartArg.Text;
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
            dialog.Close();
        }

        /// <summary>
        /// 修改图标为默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReStoreImage(object sender, RoutedEventArgs e)
        {
            IconInfo info = this.DataContext as IconInfo;
            info.BitmapImage = ImageUtil.ByteArrToImage(info.DefaultImage);
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
        }

        /// <summary>
        /// 修改图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditImage(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Multiselect = false, //只允许选中单个文件
                    Filter = "所有文件(*.*)|*.*"
                };
                if (ofd.ShowDialog() == true)
                {
                    IconInfo info = this.DataContext as IconInfo;
                    info.BitmapImage = ImageUtil.GetBitmapIconByPath(ofd.FileName);
                    CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
                }
            }
            catch (Exception e1)
            {
                HandyControl.Controls.Growl.WarningGlobal("修改图标失败,已重置为默认图标!");
                LogUtil.WriteErrorLog(e1, "修改图标失败!");
            }

        }
    }
}
