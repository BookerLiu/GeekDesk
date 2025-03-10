using GeekDesk.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace GeekDesk.MyThread
{
    public class RelativePathThread
    {
        public static void MakeRelativePath()
        {
            new Thread(() =>
            {
                try
                {
                    ObservableCollection<MenuInfo> menuList = MainWindow.appData.MenuList;

                    string myExePath = Constants.APP_DIR + "GeekDesk.exe";
                    foreach (MenuInfo mi in menuList)
                    {
                        ObservableCollection<IconInfo> iconList = mi.IconList;
                        if (iconList == null) continue;
                        foreach (IconInfo icon in iconList)
                        {
                            if (icon == null) continue;
                            string relativePath = FileUtil.MakeRelativePath(myExePath, icon.Path);
                            if (File.Exists(icon.Path)
                                && !string.IsNullOrEmpty(relativePath)
                                && !relativePath.Equals(icon.Path))
                            {
                                icon.RelativePath_NoWrite = relativePath;
                            }
                        }
                    }
                    CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
                    //CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_BAK_PATH);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteErrorLog(ex, "init相对路径出错!");
                }
            }).Start();
        }
    }
}
