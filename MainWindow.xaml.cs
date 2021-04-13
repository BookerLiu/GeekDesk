using DraggAnimatedPanelExample;
using GalaSoft.MvvmLight;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GeekDesk
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private static AppData appData = CommonCode.GetAppData();
        public MainWindow()
        {
            InitializeComponent();
            loadData();
            List<string> menuList = new List<string>();

            Dictionary<string, List<IconInfo>> iconMap = new Dictionary<string, List<IconInfo>>();



            //this.DataContext = mainModel;
            //menu.Items = mainModel;
            //System.Diagnostics.Process.Start(@"D:\SoftWare\WeGame\wegame.exe");
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
        }

        private void loadData()
        {
            this.DataContext = appData;
            appData.MenuList.Add("Test1");
            this.Width = appData.AppConfig.WindowWidth;
            this.Height = appData.AppConfig.WindowHeight;


            List<IconInfo> iconList;
            if (appData.IconMap.ContainsKey("1"))
            {
                iconList = appData.IconMap["1"];
            }
            else
            {
                iconList = new List<IconInfo>();
                appData.IconMap.Add("1", iconList);
            }
            icons.ItemsSource = iconList;

        }


        DelegateCommand<int[]> _swap;
        public DelegateCommand<int[]> SwapCommand
        {
            get
            {
                if (_swap == null)
                    _swap = new DelegateCommand<int[]>(
                        (indexes) =>
                        {
                            int fromS = indexes[0];
                            int to = indexes[1];
                            var elementSource = icons.Items[to];
                            var dragged = icons.Items[fromS];
                            if (fromS > to)
                            {
                                icons.Items.Remove(dragged);
                                icons.Items.Insert(to, dragged);
                            }
                            else
                            {
                                icons.Items.Remove(dragged);
                                icons.Items.Insert(to, dragged);
                            }
                        }
                    );
                return _swap;
            }
        }
        DelegateCommand<int[]> _swap2;
        public DelegateCommand<int[]> SwapCommand2
        {
            get
            {
                if (_swap2 == null)
                    _swap2 = new DelegateCommand<int[]>(
                        (indexes) =>
                        {
                            int fromS = indexes[0];
                            int to = indexes[1];
                            var elementSource = menus.Items[to];
                            var dragged = menus.Items[fromS];
                            if (fromS > to)
                            {
                                menus.Items.Remove(dragged);
                                menus.Items.Insert(to, dragged);
                            }
                            else
                            {
                                menus.Items.Remove(dragged);
                                menus.Items.Insert(to, dragged);
                            }
                        }
                    );
                return _swap2;
            }
        }



        private void Wrap_Drop(object sender, DragEventArgs e)
        {
            Array dropObject = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            if (dropObject == null) return;
            foreach (object obj in dropObject)
            {
                string path = (string)obj;
                if (File.Exists(path))
                {
                    // 文件
                    BitmapImage bi = FileIcon.GetBitmapImage(path);
                    IconInfo iconInfo = new IconInfo();
                    iconInfo.Path = path;
                    iconInfo.BitmapImage = bi;
                    iconInfo.Name = Path.GetFileNameWithoutExtension(path);
                    List<IconInfo> iconList;
                    if (appData.IconMap.ContainsKey("1"))
                    {
                        iconList = appData.IconMap["1"];
                    }
                    else
                    {
                        iconList = new List<IconInfo>();
                        appData.IconMap.Add("1", iconList);
                    }
                    iconList.Add(iconInfo);
                    icons.ItemsSource = iconList;
                    CommonCode.SaveAppData(appData);

                }
                else if (Directory.Exists(path))
                {
                    //文件夹

                }
            }
            icons.Items.Refresh();



        }

        //菜单点击事件
        private void menuClick(object sender, MouseButtonEventArgs e)
        {

        }



        /// <summary>
        /// 图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataClick(object sender, MouseButtonEventArgs e)
        {
            IconInfo icon = (IconInfo)((StackPanel)sender).Tag;
            System.Diagnostics.Process.Start(icon.Path);
            icon.Count++;
            CommonCode.SaveAppData(appData);
        }

        /// <summary>
        /// data选中事件 设置不可选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (icons.SelectedIndex != -1) icons.SelectedIndex = -1;
        }

        #region Window_Loaded
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.menus.Items.Add(new ViewModel.Menu() { menu = "test1" });
            //this.menus.Items.Add(new ViewModel.Menu() { menu = "test2" });
            //this.menus.Items.Add(new ViewModel.Menu() { menu = "test3" });
        }
        #endregion // Window_Loaded

        //#region Window_Closing
        //void Window_Closing(object sender, CancelEventArgs e)
        //{
        //    Rect rect = this.RestoreBounds;
        //    AppConfig config = this.DataContext as AppConfig;
        //    config.WindowWidth = rect.Width;
        //    config.WindowHeight = rect.Height;
        //    CommonCode.SaveAppConfig(config);
        //}
        //#endregion // Window_Closing

        void MainWindow_Resize(object sender, System.EventArgs e)
        {
            if (this.DataContext != null)
            {
                AppData appData = this.DataContext as AppData;
                appData.AppConfig.WindowWidth = this.Width;
                appData.AppConfig.WindowHeight = this.Height;
                CommonCode.SaveAppData(appData);
            }
        }




        private void leftCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void deleteMenu(object sender, RoutedEventArgs e)
        {
            //if (data.SelectedIndex == -1)
            //{
            //    return;
            //}
            ViewModel.Menu pojo = (ViewModel.Menu)((ContextMenu)((MenuItem)sender).Parent).DataContext;
            string menuTitle = pojo.menu;
            int index = 0;
            foreach (object obj in menus.Items)
            {
                string test = ((ViewModel.Menu)obj).menu;
                if (test == menuTitle)
                {
                    menus.Items.RemoveAt(index);
                    menus.Items.Refresh();
                    return;
                }
                index++;
            }

        }



    }




    public class MainModel : ViewModelBase
    {
        public List<ViewModel.Menu> MenuList { get; set; }

        public List<ViewModel.IconInfo> DataList { get; set; }


    }


}
