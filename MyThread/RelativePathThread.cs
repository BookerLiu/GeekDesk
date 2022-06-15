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
                ObservableCollection<MenuInfo> menuList = MainWindow.appData.MenuList;

                string myExePath = Constants.APP_DIR + "GeekDesk.exe";
                foreach (MenuInfo mi in menuList)
                {
                    ObservableCollection<IconInfo> iconList = mi.IconList;
                    foreach (IconInfo icon in iconList)
                    {
                        string relativePath = FileUtil.MakeRelativePath(myExePath, icon.Path);
                        if (File.Exists(icon.Path) 
                            && !string.IsNullOrEmpty(relativePath) 
                            && !relativePath.Equals(icon.Path)) {
                            icon.RelativePath_NoWrite = relativePath;
                        }
                    }
                }
                CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
                CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_BAK_PATH);
            }).Start();
        }
    }
}
