using DraggAnimatedPanelExample;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private AppData appData = CommonCode.GetAppDataByFile();
        private int menuSelectIndexTemp = -1;
        public MainWindow()
        {
            InitializeComponent();
            loadData();
            //this.DataContext = mainModel;
            //menu.Items = mainModel;
            //System.Diagnostics.Process.Start(@"D:\SoftWare\WeGame\wegame.exe");
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
            this.Topmost = true;
        }

        private void loadData()
        {
            this.DataContext = appData;
            if (appData.MenuList.Count == 0)
            {
                appData.MenuList.Add(new MenuInfo() { MenuName = "NewGouop", MenuId = System.Guid.NewGuid().ToString(), MenuEdit = (int)Visibility.Collapsed});
            }
            //窗体大小
            this.Width = appData.AppConfig.WindowWidth;
            this.Height = appData.AppConfig.WindowHeight;
            //选中 菜单
            menus.SelectedIndex = appData.AppConfig.SelectedMenuIndex;
            //图标数据
            icons.ItemsSource = appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList;
        }


        #region 图标拖动
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
                            ObservableCollection<MenuInfo> menuList = appData.MenuList;
                            var elementSource = menuList[to];
                            var dragged = menuList[fromS];
                            if (fromS > to)
                            {
                                menuList.Remove(dragged);
                                menuList.Insert(to, dragged);
                            }
                            else
                            {
                                menuList.Remove(dragged);
                                menuList.Insert(to, dragged);
                            }
                            appData.MenuList = menuList;
                            //menus.Items.Refresh();

                        }
                    );
                return _swap2;
            }
        }
        #endregion 图标拖动


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
                    appData.MenuList[menus.SelectedIndex].IconList.Add(iconInfo);
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
        private void MenuClick(object sender, SelectionChangedEventArgs e)
        {
            //设置对应菜单的图标列表
            icons.ItemsSource = appData.MenuList[menus.SelectedIndex].IconList;
            appData.AppConfig.SelectedMenuIndex = menus.SelectedIndex;
            CommonCode.SaveAppData(appData);
        }



        /// <summary>
        /// 图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconClick(object sender, MouseButtonEventArgs e)
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
        private void IconSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (icons.SelectedIndex != -1) icons.SelectedIndex = -1;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //加载完毕注册热键
            Hotkey.Regist(this, HotkeyModifiers.MOD_CONTROL, Key.Y, ()=>
            {
                if (this.Visibility == Visibility.Collapsed)
                {
                    this.Visibility = Visibility.Visible;
                } else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            });
        }


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


        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            appData.MenuList.Remove(menuInfo);
            CommonCode.SaveAppData(appData);
        }

        private void DragMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// 重命名菜单 将textbox 设置为可见
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            menuInfo.MenuEdit = (int)Visibility.Visible;
            
        }

        /// <summary>
        /// 编辑菜单失焦或者敲下Enter键时保存修改后的菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LostFocusOrEnterDown(object sender, EventArgs e)
        {
            TextBox menuBox = null;
            if (e.GetType() == typeof(KeyEventArgs))
            {
                KeyEventArgs eKey = e as KeyEventArgs;
                if (eKey.Key == Key.Enter)
                {
                    menuBox = ((TextBox)sender);
                }
            } else if(e.GetType() == typeof(RoutedEventArgs))
            {
                menuBox = ((TextBox)sender);
            }

            if (menuBox != null)
            {
                MenuInfo menuInfo = menuBox.Tag as MenuInfo;
                string text = menuBox.Text;
                menuInfo.MenuName = text;
                menuInfo.MenuEdit = (int)Visibility.Collapsed;
                CommonCode.SaveAppData(appData);
            }
        }

        /// <summary>
        /// 当修改菜单元素可见时 设置全选并获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEditWhenVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box.Visibility == Visibility.Visible)
            {
                Keyboard.Focus(box);
                box.SelectAll();
            }
        }

        /// <summary>
        /// 当修改菜单元素可见时 设置原菜单为不可见 并且不可选中
        /// 修改菜单元素不可见时  原菜单可见 并 选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuWhenVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb.Visibility == Visibility.Collapsed)
            {
                if (menus.SelectedIndex != -1)
                {
                    menuSelectIndexTemp = menus.SelectedIndex;
                    menus.SelectedIndex = -1;
                } else
                {
                    menus.SelectedIndex = menuSelectIndexTemp;
                }
            }
        }

        /// <summary>
        /// 新建菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMenu(object sender, RoutedEventArgs e)
        {
            appData.MenuList.Add(new MenuInfo() { MenuEdit = (int)Visibility.Collapsed, MenuId = System.Guid.NewGuid().ToString(), MenuName = "NewGouop" });
            menus.SelectedIndex = appData.MenuList.Count - 1;
            //appData.MenuList[appData.MenuList.Count - 1].MenuEdit = (int)Visibility.Visible;
            CommonCode.SaveAppData(appData);
        }

        /// <summary>
        /// 关闭按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 图片图标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.Visibility = Visibility.Visible;
            } else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

    }




}
