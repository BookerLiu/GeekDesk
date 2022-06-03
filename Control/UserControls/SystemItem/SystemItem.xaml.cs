using GeekDesk.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeekDesk.Control.UserControls.SystemItem
{
    /// <summary>
    /// IconPannel.xaml 的交互逻辑
    /// </summary>
    public partial class SystemItem : UserControl
    {
        public SystemItem()
        {
            InitializeComponent();
        }
        private void Icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IconInfo thisInfo = (sender as StackPanel).Tag as IconInfo;

            MenuInfo menuInfo = MainWindow.appData.MenuList[MainWindow.appData.AppConfig.SelectedMenuIndex];

            string startArg = thisInfo.StartArg;
            IconInfo iconInfo;
            if (Constants.SYSTEM_ICONS.ContainsKey(startArg))
            {
                //系统项
                iconInfo = new IconInfo
                {
                    Name_NoWrite = thisInfo.Name_NoWrite,
                    Path_NoWrite = thisInfo.Path_NoWrite,
                    StartArg_NoWrite = thisInfo.StartArg_NoWrite,
                    BitmapImage_NoWrite = thisInfo.BitmapImage_NoWrite
                };
                iconInfo.Content_NoWrite = iconInfo.Name_NoWrite
                    + "\n使用次数:" + iconInfo.Count;
            }
            else
            {
                //startupMenu or Store
                iconInfo = CommonCode.GetIconInfoByPath(thisInfo.LnkPath_NoWrite);
            }
            menuInfo.IconList.Add(iconInfo);
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
        }
    }
}
