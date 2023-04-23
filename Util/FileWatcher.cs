using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class FileWatcher
    {
        public static Dictionary<FileSystemWatcher, MenuInfo> linkMenuMap = new Dictionary<FileSystemWatcher, MenuInfo>();
        /// <summary>
        /// 实时文件夹监听
        /// </summary>
        /// <param name="path"></param>
        public static void LinkMenuWatcher(MenuInfo menuInfo)
        {
            try
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher
                {
                    Path = menuInfo.LinkPath,
                };
                linkMenuMap.Add(fileSystemWatcher, menuInfo);
                fileSystemWatcher.EnableRaisingEvents = true;
                fileSystemWatcher.Changed += LinkIcon_Changed;
                fileSystemWatcher.Deleted += LinkIcon_Deleted;
                fileSystemWatcher.Created += LinkIcon_Created;
                fileSystemWatcher.Renamed += LinkIcon_Renamed;
            } catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "添加LinkMenu监听异常");
            }
            
        }

        private static void LinkIcon_Renamed(object sender, RenamedEventArgs e)
        {
            IconInfo iconInfo = getIconInfoByPath(sender, e.OldFullPath);
            iconInfo.Name = e.Name;
            iconInfo.Path = e.FullPath;
        }

        private static void LinkIcon_Changed(object sender, FileSystemEventArgs e)
        {
            IconInfo iconInfo = getIconInfoByPath(sender, e.FullPath);
            if (iconInfo != null)
            {
                IconInfo newIconInfo = CommonCode.GetIconInfoByPath(e.FullPath);
                iconInfo.BitmapImage = newIconInfo.BitmapImage;
            }
            
        }
        private static void LinkIcon_Deleted(object sender, FileSystemEventArgs e)
        {
            IconInfo iconInfo = getIconInfoByPath(sender, e.FullPath);
            App.Current.Dispatcher.Invoke(() =>
            {
                linkMenuMap[sender as FileSystemWatcher].IconList.Remove(iconInfo);
            });
        }
        private static void LinkIcon_Created(object sender, FileSystemEventArgs e)
        {
            IconInfo iconInfo = CommonCode.GetIconInfoByPath(e.FullPath);
            App.Current.Dispatcher.Invoke(() =>
            {
                linkMenuMap[sender as FileSystemWatcher].IconList.Add(iconInfo);
            });
        }

        private static IconInfo getIconInfoByPath(object sender, string path)
        {
            MenuInfo menuInfo = linkMenuMap[sender as FileSystemWatcher];
            foreach (IconInfo iconInfo in menuInfo.IconList)
            {
                if (iconInfo.Path.Equals(path))
                {
                    return iconInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 开启所有菜单监听
        /// </summary>
        /// <param name="appData"></param>
        public static void EnableLinkMenuWatcher(AppData appData)
        {
            foreach (MenuInfo menuInfo in appData.MenuList)
            {
                if (menuInfo.MenuType == Constant.MenuType.LINK)
                {
                    LinkMenuWatcher(menuInfo);
                }
            }
            RefreshLinkMenuIcon(appData);
        }

        private static void RefreshLinkMenuIcon(AppData appData)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (MenuInfo menuInfo in appData.MenuList)
                    {
                        if (menuInfo.MenuType == Constant.MenuType.LINK)
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(menuInfo.LinkPath);
                            FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();

                            ObservableCollection<IconInfo> iconList = new ObservableCollection<IconInfo>();
                            foreach (FileSystemInfo fileInfo in fileInfos)
                            {
                                IconInfo iconInfo = CommonCode.GetIconInfoByPath_NoWrite(fileInfo.FullName);
                                iconList.Add(iconInfo);
                            }
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                foreach (IconInfo iconInfo in iconList)
                                {
                                    menuInfo.IconList = null;
                                    menuInfo.IconList = iconList;
                                }
                            });
                        }
                    }
                }
                catch (Exception) { }
            }).Start();
        }

        /// <summary>
        /// 移除菜单监听
        /// </summary>
        /// <param name="menuInfo"></param>
        public static void RemoveLinkMenuWatcher(MenuInfo menuInfo)
        {
            try
            {
                foreach (FileSystemWatcher watcher in linkMenuMap.Keys)
                {
                    if (linkMenuMap[watcher] == menuInfo)
                    {
                        //释放资源
                        watcher.Changed -= LinkIcon_Changed;
                        watcher.Created -= LinkIcon_Created;
                        watcher.Deleted -= LinkIcon_Deleted;
                        watcher.Renamed -= LinkIcon_Renamed;
                        watcher.EnableRaisingEvents = false;
                        watcher.Dispose();
                        linkMenuMap.Remove(watcher);
                    }
                }
            }
            catch (Exception e)
            {
                //nothing
            }
        }






      
    }
}
