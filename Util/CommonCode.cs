using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.ViewModel;
using HandyControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media.Imaging;
using static GeekDesk.Control.Other.GlobalMsgNotification;

/// <summary>
/// 提取一些代码
/// </summary>
namespace GeekDesk.Util
{
    class CommonCode
    {

        /// <summary>
        /// 获取app 数据
        /// </summary>
        /// <returns></returns>
        public static AppData GetAppDataByFile()
        {
            AppData appData;
            if (!File.Exists(Constants.DATA_FILE_PATH))
            {
                using (FileStream fs = File.Create(Constants.DATA_FILE_PATH)) { }
                appData = new AppData();
                SaveAppData(appData, Constants.DATA_FILE_PATH);
            }
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(Constants.DATA_FILE_PATH, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        appData = bf.Deserialize(fs) as AppData;

                        //将菜单密码写入文件
                        if (!string.IsNullOrEmpty(appData.AppConfig.MenuPassword))
                        {
                            SavePassword(appData.AppConfig.MenuPassword);
                        }
                    }
                }
                catch
                {
                    if (File.Exists(Constants.DATA_FILE_BAK_PATH))
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(Constants.DATA_FILE_BAK_PATH, FileMode.Open))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                appData = bf.Deserialize(fs) as AppData;
                            }

                            DialogMsg msg = new DialogMsg();
                            msg.msg = "不幸的是, GeekDesk当前的数据文件已经损坏, " +
                                "现在已经启用系统自动备份的数据\n\n" +
                                "如果你有较新的备份, " +
                                "请退出GeekDesk, " +
                                "将备份文件重命名为:Data, " +
                                "然后将Data覆盖到GeekDesk的根目录即可\n\n" +
                                "系统上次备份时间: \n" + appData.AppConfig.SysBakTime +
                                "\n\n如果当前数据就是你想要的数据, 那么请不用管它";
                            GlobalMsgNotification gm = new GlobalMsgNotification(msg);
                            HandyControl.Controls.Notification ntf = HandyControl.Controls.Notification.Show(gm, ShowAnimation.Fade, true);
                            gm.ntf = ntf;
                        }
                        catch
                        {
                            MessageBox.Show("不幸的是, GeekDesk当前的数据文件已经损坏\n如果你有备份, 请将备份文件重命名为:Data 然后将Data覆盖到GeekDesk的根目录即可!");
                            Application.Current.Shutdown();
                            return null;
                        }

                    }
                    else
                    {
                        MessageBox.Show("不幸的是, GeekDesk当前的数据文件已经损坏\n如果你有备份, 请将备份文件重命名为:Data 然后将Data覆盖到GeekDesk的根目录即可!");
                        Application.Current.Shutdown();
                        return null;
                    }

                }
            }
            return appData;
        }

        private readonly static object _MyLock = new object();
        /// <summary>
        /// 保存app 数据
        /// </summary>
        /// <param name="appData"></param>
        public static void SaveAppData(AppData appData, string filePath)
        {
            lock (_MyLock)
            {
                if (filePath.Equals(Constants.DATA_FILE_BAK_PATH))
                {
                    appData.AppConfig.SysBakTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (!Directory.Exists(filePath.Substring(0, filePath.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, appData);
                }
            }
        }


        public static void SavePassword(string password)
        {
            using (StreamWriter sw = new StreamWriter(Constants.PW_FILE_BAK_PATH))
            {
                sw.Write(password);
            }
        }

        private static string GeneraterUUID()
        {
            try
            {
                if (!File.Exists(Constants.UUID_FILE_BAK_PATH) || string.IsNullOrEmpty(GetUniqueUUID()))
                {
                    using (StreamWriter sw = new StreamWriter(Constants.UUID_FILE_BAK_PATH))
                    {
                        string uuid = Guid.NewGuid().ToString() + "-" + Constants.MY_UUID;
                        sw.Write(uuid);
                        return uuid;
                    }
                }
            } catch (Exception) { }
            return "ERROR_UUID_GeneraterUUID_" + Constants.MY_UUID;
        }

        public static string GetUniqueUUID()
        {
            try
            {
                if (File.Exists(Constants.UUID_FILE_BAK_PATH))
                {
                    using (StreamReader reader = new StreamReader(Constants.UUID_FILE_BAK_PATH))
                    {
                        return reader.ReadToEnd().Trim();
                    }
                } else
                {
                    return GeneraterUUID();
                }
            } catch(Exception) { }
            return "ERROR_UUID_GetUniqueUUID_" + Constants.MY_UUID;
        }


        public static void BakAppData()
        {

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "备份文件",
                Filter = "bak文件(*.bak)|*.bak",
                FileName = "Data-GD-" + DateTime.Now.ToString("yyMMdd") + ".bak",
            };
            if (sfd.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, MainWindow.appData);
                }
            }
        }



        /// <summary>
        /// 根据路径获取文件图标等信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IconInfo GetIconInfoByPath(string path)
        {
            string tempPath = path;

            //string base64 = ImageUtil.FileImageToBase64(path, System.Drawing.Imaging.ImageFormat.Png);
            //string ext = "";
            //if (!ImageUtil.IsSystemItem(path))
            //{
            //    ext = System.IO.Path.GetExtension(path).ToLower();
            //}

            string iconPath;
            //if (".lnk".Equals(ext))
            //{

            string targetPath = FileUtil.GetTargetPathByLnk(path);
            iconPath = FileUtil.GetIconPathByLnk(path);
            if (targetPath != null)
            {
                path = targetPath;
            }
            //}
            if (StringUtil.IsEmpty(iconPath))
            {
                iconPath = path;
            }

            BitmapImage bi = ImageUtil.GetBitmapIconByPath(iconPath);
            IconInfo iconInfo = new IconInfo
            {
                Path_NoWrite = path,
                LnkPath_NoWrite = tempPath,
                BitmapImage_NoWrite = bi,
                StartArg_NoWrite = FileUtil.GetArgByLnk(tempPath)
            };
            iconInfo.DefaultImage_NoWrite = iconInfo.ImageByteArr;
            iconInfo.Name_NoWrite = System.IO.Path.GetFileNameWithoutExtension(tempPath);
            if (StringUtil.IsEmpty(iconInfo.Name))
            {
                iconInfo.Name_NoWrite = path;
            }
            string relativePath = FileUtil.MakeRelativePath(Constants.APP_DIR + "GeekDesk.exe", iconInfo.Path);
            if (!string.IsNullOrEmpty(relativePath) && !relativePath.Equals(iconInfo.Path))
            {
                iconInfo.RelativePath_NoWrite = relativePath;
            }
            return iconInfo;
        }


        public static IconInfo GetIconInfoByPath_NoWrite(string path)
        {
            string tempPath = path;

            //string base64 = ImageUtil.FileImageToBase64(path, System.Drawing.Imaging.ImageFormat.Png);
            string ext = "";
            if (!ImageUtil.IsSystemItem(path))
            {
                ext = System.IO.Path.GetExtension(path).ToLower();
            }

            string iconPath = null;
            if (".lnk".Equals(ext))
            {

                string targetPath = FileUtil.GetTargetPathByLnk(path);
                iconPath = FileUtil.GetIconPathByLnk(path);
                if (targetPath != null)
                {
                    path = targetPath;
                }
            }
            if (StringUtil.IsEmpty(iconPath))
            {
                iconPath = path;
            }

            BitmapImage bi = ImageUtil.GetBitmapIconByPath(iconPath);
            IconInfo iconInfo = new IconInfo
            {
                Path_NoWrite = path,
                LnkPath_NoWrite = tempPath,
                BitmapImage_NoWrite = bi,
                StartArg_NoWrite = FileUtil.GetArgByLnk(tempPath)
            };
            iconInfo.DefaultImage_NoWrite = iconInfo.ImageByteArr;
            iconInfo.Name = System.IO.Path.GetFileNameWithoutExtension(tempPath);
            if (StringUtil.IsEmpty(iconInfo.Name))
            {
                iconInfo.Name_NoWrite = path;
            }
            return iconInfo;
        }






        /// <summary>
        /// 排序图标
        /// </summary>
        public static void SortIconList()
        {
            if (MainWindow.appData.AppConfig.IconSortType != SortType.CUSTOM)
            {
                ObservableCollection<MenuInfo> menuList = MainWindow.appData.MenuList;
                //List<IconInfo> list = new List<IconInfo>(menuList[MainWindow.appData.AppConfig.SelectedMenuIndex].IconList);
                List<IconInfo> list;
                foreach (MenuInfo menuInfo in menuList)
                {
                    list = new List<IconInfo>(menuInfo.IconList);
                    switch (MainWindow.appData.AppConfig.IconSortType)
                    {
                        case SortType.COUNT_UP:
                            list.Sort((x, y) => x.Count.CompareTo(y.Count));
                            break;
                        case SortType.COUNT_LOW:
                            list.Sort((x, y) => y.Count.CompareTo(x.Count));
                            break;
                        case SortType.NAME_UP:
                            list.Sort((x, y) => x.Name.CompareTo(y.Name));
                            break;
                        case SortType.NAME_LOW:
                            list.Sort((x, y) => y.Name.CompareTo(x.Name));
                            break;
                    }
                    menuInfo.IconList = new ObservableCollection<IconInfo>(list);
                }
                MainWindow.appData.AppConfig.SelectedMenuIcons = MainWindow.appData.MenuList[MainWindow.appData.AppConfig.SelectedMenuIndex].IconList;
            }
        }



        /// <summary>
        /// 判断鼠标是否在窗口内
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool MouseInWindow(Window window)
        {
            double windowHeight = window.Height;
            double windowWidth = window.Width;

            double windowTop = window.Top;
            double windowLeft = window.Left;

            //获取鼠标位置
            System.Windows.Point p = MouseUtil.GetMousePosition();
            double mouseX = p.X;
            double mouseY = p.Y;

            //鼠标不在窗口上
            if (mouseX < windowLeft || mouseX > windowLeft + windowWidth
                || mouseY < windowTop || mouseY > windowTop + windowHeight)
            {
                return false;
            }
            return true;
        }

    }
}
