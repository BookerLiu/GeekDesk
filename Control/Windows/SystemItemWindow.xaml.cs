using GeekDesk.Constant;
using GeekDesk.Interface;
using GeekDesk.Util;
using GeekDesk.ViewModel;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static GeekDesk.Util.ShowWindowFollowMouse;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// SystemItemWindow.xaml 的交互逻辑
    /// 添加系统项目到对应菜单
    /// </summary>
    public partial class SystemItemWindow : Window, IWindowCommon
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        private static SystemItemViewModel vm;
        private List<IconInfo> systemIcons;
        private List<IconInfo> startMenuIcons;
        private List<IconInfo> storeIcons;

        private SystemItemWindow()
        {
            vm = new SystemItemViewModel();
            this.DataContext = vm;
            InitializeComponent();
            this.Topmost = true;
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            this.Close();
        }

        /// <summary>
        /// 切换选项卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem ti = this.MyTabControl.SelectedItem as TabItem;

            List<IconInfo> systemInfos = vm.IconInfos;
            if (systemInfos == null)
            {
                systemInfos = new List<IconInfo>();
            }
            switch (ti.Tag.ToString())
            {
                case "StartMenu": //开始菜单
                    if (startMenuIcons == null)
                    {
                        vm.IconInfos = null;
                        System.Threading.Thread t = new System.Threading.Thread(new ThreadStart(GetStartMenuInfos))
                        {
                            IsBackground = true
                        };
                        t.Start();
                    }
                    else
                    {
                        StartMenuLoading.Visibility = Visibility.Collapsed;
                        vm.IconInfos = startMenuIcons;
                    }
                    break;
                case "Store": //应用商店
                    if (storeIcons == null)
                    {
                        vm.IconInfos = null;
                        storeIcons = new List<IconInfo>();
                        vm.IconInfos = storeIcons;
                    }
                    else
                    {
                        vm.IconInfos = storeIcons;
                    }
                    break;
                default: //默认系统项
                    if (systemIcons == null)
                    {
                        vm.IconInfos = null;
                        systemIcons = GetSysteIconInfos();
                        vm.IconInfos = systemIcons;
                    }
                    else
                    {
                        vm.IconInfos = systemIcons;
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取开始菜单路径下项目
        /// </summary>
        /// <returns></returns>
        private void GetStartMenuInfos()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                StartMenuLoading.Visibility = Visibility.Visible;
            }));

            List<IconInfo> infos = new List<IconInfo>();
            //获取开始菜单路径
            string path = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs";
            //递归获取信息
            GetInfos(path, infos);
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (StartMenu.IsSelected)
                {
                    startMenuIcons = infos;
                    vm.IconInfos = startMenuIcons;
                }
                StartMenuLoading.Visibility = Visibility.Collapsed;
            }));

        }

        /// <summary>
        /// 递归获取文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listInfos"></param>
        private void GetInfos(string filePath, List<IconInfo> listInfos)
        {
            DirectoryInfo di = new DirectoryInfo(filePath);
            string[] filePaths = Directory.GetFiles(filePath);
            string[] dirPaths = Directory.GetDirectories(filePath);

            string[] paths = new string[filePaths.Length + dirPaths.Length];
            filePaths.CopyTo(paths, 0);
            if (filePaths == null || filePaths.Length == 0)
            {
                dirPaths.CopyTo(paths, 0);
            }
            else
            {
                dirPaths.CopyTo(paths, filePaths.Length - 1);
            }

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    string ext = Path.GetExtension(path).ToLower();
                    if (".exe".Equals(ext) || ".lnk".Equals(ext))
                    {
                        try
                        {
                            IconInfo iconInfo = CommonCode.GetIconInfoByPath_NoWrite(path);
                            if (iconInfo.Path_NoWrite != null)
                            {
                                iconInfo.Content_NoWrite = iconInfo.Path_NoWrite + "\n" + iconInfo.Name_NoWrite;
                                listInfos.Add(iconInfo);
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else if (Directory.Exists(path))
                {
                    GetInfos(path, listInfos);
                }
            }

            //FileSystemInfo[] fileInfoArr = di.GetFileSystemInfos();
            //foreach(FileSystemInfo fi in fileInfoArr)
            //{
            //    string path = fi.FullName;

            //}
        }

        /// <summary>
        /// 获取系统项目
        /// </summary>
        /// <returns></returns>
        private List<IconInfo> GetSysteIconInfos()
        {
            List<IconInfo> iconInfos = new List<IconInfo>();

            Hashtable systemIcons = Constants.SYSTEM_ICONS;
            IconInfo iconInfo;
            foreach (object key in systemIcons.Keys)
            {
                string keyStr = key.ToString();
                iconInfo = new IconInfo
                {
                    Name_NoWrite = systemIcons[key].ToString()
                };
                iconInfo.BitmapImage_NoWrite = new BitmapImage(
                        new Uri("pack://application:,,,/GeekDesk;component/Resource/Image/SystemIcon/" + keyStr + ".png"
                        , UriKind.RelativeOrAbsolute));
                iconInfo.StartArg = keyStr;
                iconInfo.Content_NoWrite = iconInfo.Name_NoWrite;
                iconInfos.Add(iconInfo);
            }
            return iconInfos;
        }

        public class SystemItemViewModel : INotifyPropertyChanged
        {
            private List<IconInfo> iconInfos;
            private AppConfig appConfig;

            public SystemItemViewModel()
            {
                this.AppConfig = MainWindow.appData.AppConfig;
            }

            public AppConfig AppConfig
            {
                get
                {
                    return appConfig;
                }
                set
                {
                    appConfig = value;
                    OnPropertyChanged("AppConfig");
                }
            }
            public List<IconInfo> IconInfos
            {
                get
                {
                    return iconInfos;
                }
                set
                {
                    iconInfos = value;
                    OnPropertyChanged("IconInfos");
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }




        private static System.Windows.Window window = null;
        public static void Show()
        {
            if (window == null || !window.Activate())
            {
                window = new SystemItemWindow();
            }
            window.Show();
            Keyboard.Focus(window);
            ShowWindowFollowMouse.Show(window, MousePosition.LEFT_CENTER, 0, 0);
        }


        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DataContext = null;
                this.Close();
            }
        }




    }
}