using GeekDesk.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.MyThread
{
    public class RelativePathThread
    {
        public static void MakeRelativePath()
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);

                ObservableCollection<MenuInfo> menuList = MainWindow.appData.MenuList;

                string myExePath = Constants.APP_DIR + "GeekDesk.exe";
                foreach (MenuInfo mi in menuList)
                {
                    ObservableCollection<IconInfo> iconList = mi.IconList;
                    foreach (IconInfo icon in iconList)
                    {
                        icon.RelativePath_NoWrite = FileUtil.MakeRelativePath(myExePath, icon.Path);
                    }
                }
                CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
                CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_BAK_PATH);
            }).Start();
        }
    }
}
