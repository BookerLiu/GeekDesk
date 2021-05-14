using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace GeekDesk.Control
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class IconInfoDialog
    {

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
            CommonCode.SaveAppData(MainWindow.appData);
        }

        /// <summary>
        /// 修改图标为默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditImageToDefault(object sender, RoutedEventArgs e)
        {
            IconInfo info = ((Button)sender).Tag as IconInfo;
            info.BitmapImage = ImageUtil.ByteArrToImage(info.DefaultImage);
            CommonCode.SaveAppData(MainWindow.appData);
        }

        /// <summary>
        /// 修改图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditImage(object sender, RoutedEventArgs e)
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
                CommonCode.SaveAppData(MainWindow.appData);
            }
        }
    }
}
