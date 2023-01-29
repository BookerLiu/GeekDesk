using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;

using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace GeekDesk.Control.UserControls.PannelCard
{
    /// <summary>
    /// LeftCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class LeftCardControl : UserControl
    {
        private int menuSelectIndexTemp = -1;
        private AppData appData = MainWindow.appData;
        private SolidColorBrush bac = new SolidColorBrush(Color.FromRgb(236, 236, 236));
        private FileSystemWatcher Watcher = new FileSystemWatcher();


        public LeftCardControl()
        {
            InitializeComponent();


            this.Loaded += (s, e) =>
            {
                SelectLastMenu();
                SetMenuListBoxItemEvent();
                BindWatcherEvent();
                BuildWatcher();
            };
        }

        private void BindWatcherEvent()
        {
            Watcher.Created += WatcherEvent;
            Watcher.Renamed += WatcherEvent;
            Watcher.Deleted += WatcherEvent;
        }

        public void BuildWatcher()
        {
            MenuInfo menuInfo = appData.MenuList[appData.AppConfig.SelectedMenuIndex];
            if (menuInfo.MenuType == MenuTypeEnum.RELATION_FOLDER)
            {
                Watcher.Path = menuInfo.RelationPath;
            }
            Watcher.EnableRaisingEvents = menuInfo.MenuType == MenuTypeEnum.RELATION_FOLDER;
        }

        private void WatcherEvent(object sender, EventArgs e)
        {
            MenuInfo menuInfo = appData.MenuList[appData.AppConfig.SelectedMenuIndex];
            appData.AppConfig.SelectedMenuIcons = menuInfo.IconList;
        }

        private void SetMenuListBoxItemEvent()
        {
            int size = MenuListBox.Items.Count;
            for (int i = 0; i < size; i++)
            {
                ListBoxItem lbi = (ListBoxItem)(MenuListBox.ItemContainerGenerator.ContainerFromIndex(i));
                if (lbi != null)
                {
                    SetListBoxItemEvent(lbi);
                }
            }
            //首次触发不了Selected事件
            object obj = MenuListBox.ItemContainerGenerator.ContainerFromIndex(MenuListBox.SelectedIndex);
            Lbi_Selected(obj, null);
        }

        private void SetListBoxItemEvent(ListBoxItem lbi)
        {
            lbi.MouseEnter += (s, me) =>
            {
                lbi.Background = bac;
            };
            lbi.Unselected += Lbi_Unselected;

            lbi.MouseLeave += Lbi_MouseLeave;

            lbi.Selected += Lbi_Selected;
        }

        private void SelectLastMenu()
        {
            if (appData.AppConfig.SelectedMenuIndex >= appData.MenuList.Count || appData.AppConfig.SelectedMenuIndex == -1)
            {
                MenuListBox.SelectedIndex = 0;
                appData.AppConfig.SelectedMenuIndex = MenuListBox.SelectedIndex;
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[0].IconList;
            }
            else
            {
                MenuListBox.SelectedIndex = appData.AppConfig.SelectedMenuIndex;
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList;
            }
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
                            ObservableCollection<MenuInfo> menuList = MainWindow.appData.MenuList;
                            var elementSource = menuList[to];
                            var dragged = menuList[fromS];
                            menuList.Remove(dragged);
                            menuList.Insert(to, dragged);
                            MenuListBox.SelectedIndex = to;
                            MainWindow.appData.MenuList = menuList;
                        }
                    );
                return _swap;
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
            StackPanel sp = sender as StackPanel;

            ListBoxItem lbi = (sp.TemplatedParent as ContentPresenter).TemplatedParent as ListBoxItem;
            if (sp.Visibility == Visibility.Collapsed)
            {
                lbi.MouseEnter += Lbi_MouseEnter;
                if (MenuListBox.SelectedIndex != -1)
                {
                    menuSelectIndexTemp = MenuListBox.SelectedIndex;
                    MenuListBox.SelectedIndex = -1;
                }
                else
                {
                    MenuListBox.SelectedIndex = menuSelectIndexTemp;
                }
            }
            else
            {
                lbi.MouseEnter += (s, me) =>
                {
                    lbi.Background = bac;
                };

                lbi.MouseLeave += Lbi_MouseLeave;
                lbi.Selected += Lbi_Selected;
            }
        }

        #region 设置菜单触发事件
        private void Lbi_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.Background = Brushes.Transparent;
        }

        private void Lbi_Unselected(object sender, RoutedEventArgs e)
        {
            //添加Leave效果
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.Background = Brushes.Transparent;
            lbi.MouseLeave += Lbi_MouseLeave;
        }

        private void Lbi_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxItem lbi = sender as ListBoxItem;

                SolidColorBrush fontColor = new SolidColorBrush(Colors.Black);

                lbi.MouseLeave -= Lbi_MouseLeave;
                lbi.Background = bac;
                lbi.Foreground = fontColor;
            }
            catch { }

        }

        private void Lbi_MouseLeave(object sender, MouseEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// 新建菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo info = new MenuInfo() { MenuEdit = Visibility.Collapsed, MenuId = System.Guid.NewGuid().ToString(), MenuName = "NewMenu", MenuType = MenuTypeEnum.NORMAL};
            appData.MenuList.Add(info);
            MenuListBox.SelectedIndex = appData.MenuList.Count - 1;
            appData.AppConfig.SelectedMenuIndex = MenuListBox.SelectedIndex;
            appData.AppConfig.SelectedMenuIcons = info.IconList;
            //首次触发不了Selected事件
            object obj = MenuListBox.ItemContainerGenerator.ContainerFromIndex(MenuListBox.SelectedIndex);
            SetListBoxItemEvent((ListBoxItem)obj);
            Lbi_Selected(obj, null);
        }


        /// <summary>
        /// 重命名菜单 将textbox 设置为可见
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameMenu(object sender, RoutedEventArgs e)
        {
            RunTimeStatus.IS_MENU_EDIT = true;
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            menuInfo.MenuEdit = (int)Visibility.Visible;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            if (appData.MenuList.Count == 1)
            {
                //如果删除以后没有菜单的话 先创建一个
                CreateMenu(null, null);
            }
            int index = appData.MenuList.IndexOf(menuInfo);
            if (index == 0)
            {
                index = 0;
            }
            else
            {
                index--;
            }

            appData.MenuList.Remove(menuInfo);
            // 选中下一个菜单
            MenuListBox.SelectedIndex = index;
            appData.AppConfig.SelectedMenuIndex = MenuListBox.SelectedIndex;
            appData.AppConfig.SelectedMenuIcons = appData.MenuList[index].IconList;
        }

        /// <summary>
        /// 编辑菜单失焦或者敲下Enter键时保存修改后的菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LostFocusOrEnterDown(object sender, EventArgs e)
        {
            bool done = true;
            TextBox menuBox = null;
            if (e.GetType() == typeof(KeyEventArgs))
            {
                KeyEventArgs eKey = e as KeyEventArgs;
                if (eKey.Key == Key.Enter)
                {
                    menuBox = ((TextBox)sender);
                }
                else
                {
                    done = false;
                }
            }
            else if (e.GetType() == typeof(RoutedEventArgs))
            {
                menuBox = ((TextBox)sender);
            }

            if (done)
            {
                if (menuBox != null)
                {
                    MenuInfo menuInfo = menuBox.Tag as MenuInfo;
                    string text = menuBox.Text;
                    menuInfo.MenuName = text;
                    menuInfo.MenuEdit = Visibility.Collapsed;
                }
                RunTimeStatus.IS_MENU_EDIT = false;
                //为了解决无法修改菜单的问题
                MainWindow.mainWindow.SearchBox.Focus();
                MenuListBox.SelectedIndex = menuSelectIndexTemp;
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
            MenuInfo mi = box.Tag as MenuInfo;
            if (box.Visibility == Visibility.Visible)
            {
                Keyboard.Focus(box);
                box.SelectAll();
            }
        }

        /// <summary>
        /// 修改菜单图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditMenuGeometry(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            IconfontWindow.Show(SvgToGeometry.GetIconfonts(), menuInfo);
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RunTimeStatus.IS_MENU_EDIT) return;

            if (appData.AppConfig.ItemSpradeAnimation)
            {
                //是否启用列表展开动画
                MainWindow.mainWindow.RightCard.WrapUFG.Visibility = Visibility.Collapsed;
            }

            //设置对应菜单的图标列表
            if (MenuListBox.SelectedIndex == -1)
            {
                //appData.AppConfig.SelectedMenuIcons = appData.MenuList[appData.MenuList.Count - 1].IconList;
            }
            else
            {
                if (appData.MenuList[MenuListBox.SelectedIndex].IsEncrypt)
                {
                    appData.AppConfig.SelectedMenuIcons = null;
                    RunTimeStatus.SHOW_MENU_PASSWORDBOX = true;
                    MainWindow.mainWindow.RightCard.PDDialog.Title.Text = "输入密码";
                    MainWindow.mainWindow.RightCard.PDDialog.type = PasswordType.INPUT;
                    MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Visible;
                }
                else
                {
                    MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Collapsed;
                    appData.AppConfig.SelectedMenuIcons = appData.MenuList[MenuListBox.SelectedIndex].IconList;
                    BuildWatcher();
                }
            }
            MainWindow.mainWindow.RightCard.WrapUFG.Visibility = Visibility.Visible;
            //App.DoEvents();
        }


        /// <summary>
        /// 鼠标悬停切换菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (appData.AppConfig.HoverMenu && !RunTimeStatus.IS_MENU_EDIT)
            {
                Thread t = new Thread(() =>
                {
                    Thread.Sleep(200);
                    this.Dispatcher.Invoke(() =>
                    {
                        ListBoxItem lbi = sender as ListBoxItem;
                        if (lbi.IsMouseOver)
                        {
                            int index = MenuListBox.ItemContainerGenerator.IndexFromContainer(lbi);
                            MenuListBox.SelectedIndex = index;
                        }
                    });
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        /// <summary>
        /// 点击菜单后  隐藏搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RunTimeStatus.SEARCH_BOX_SHOW)
            {
                MainWindow.mainWindow.HidedSearchBox();
            }

            ListBoxItem lbi = sender as ListBoxItem;
            MenuInfo mi = lbi.DataContext as MenuInfo;
            int index = MenuListBox.Items.IndexOf(mi);
            MenuListBox.SelectedIndex = index;
        }


        ///// <summary>
        ///// 点击菜单后  隐藏搜索框
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ListBoxItemPanel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (RunTimeStatus.SEARCH_BOX_SHOW)
        //    {
        //        MainWindow.mainWindow.HidedSearchBox();
        //    }
        //    MenuInfo mi = (sender as StackPanel).Tag as MenuInfo;
        //    int index = MenuListBox.Items.IndexOf(mi);
        //    MenuListBox.SelectedIndex = index;
        //}


        /// <summary>
        /// 隐藏搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RunTimeStatus.SEARCH_BOX_SHOW)
            {
                MainWindow.mainWindow.HidedSearchBox();
            }
        }

        private void Menu_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (RunTimeStatus.IS_MENU_EDIT) return;

            ScrollViewer scrollViewer = ScrollUtil.FindSimpleVisualChild<ScrollViewer>(MenuListBox);
            if (e.Delta < 0)
            {
                //判断是否到了最底部
                if (ScrollUtil.IsBootomScrollView(scrollViewer))
                {
                    int index = MenuListBox.SelectedIndex;
                    if (index < MenuListBox.Items.Count - 1)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                    MenuListBox.SelectedIndex = index;
                }
            }
            else if (e.Delta > 0)
            {
                if (ScrollUtil.IsTopScrollView(scrollViewer))
                {
                    int index = MenuListBox.SelectedIndex;
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = MenuListBox.Items.Count - 1;
                    }
                    MenuListBox.SelectedIndex = index;
                }
            }

            //滚动到选中项
            MenuListBox.ScrollIntoView(MenuListBox.SelectedItem);

        }


        private void Menu_PreviewDragLeave(object sender, DragEventArgs e)
        {
            MyPoptip.IsOpen = false;
        }

        private void Menu_PreviewDragEnter(object sender, DragEventArgs e)
        {
            MenuInfo mi = (sender as ListBoxItem).DataContext as MenuInfo;
            MyPoptipContent.Text = "移动至:" + mi.MenuName;
            MyPoptip.VerticalOffset = 30;
            MyPoptip.IsOpen = true;
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            MyPoptip.IsOpen = false;
        }

        private void Menu_Drop(object sender, DragEventArgs e)
        {
            MyPoptip.IsOpen = false;

            MenuInfo mi = (sender as ListBoxItem).DataContext as MenuInfo;
            IconInfo iconInfo = (IconInfo)e.Data.GetData(typeof(IconInfo));

            appData.MenuList[MenuListBox.SelectedIndex].IconList.Remove(iconInfo);
            appData.MenuList[MenuListBox.Items.IndexOf(mi)].IconList.Add(iconInfo);
        }

        private void EncryptMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            if (menuInfo.IsEncrypt)
            {
                MainWindow.mainWindow.RightCard.PDDialog.menuInfo = menuInfo;
                MainWindow.mainWindow.RightCard.PDDialog.Title.Text = "输入密码";
                MainWindow.mainWindow.RightCard.PDDialog.type = PasswordType.CANCEL;
                RunTimeStatus.SHOW_MENU_PASSWORDBOX = true;
                MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Visible;
                //单独设置焦点
                MainWindow.mainWindow.RightCard.PDDialog.SetFocus();
            }
            else
            {
                if (string.IsNullOrEmpty(appData.AppConfig.MenuPassword))
                {
                    MainWindow.mainWindow.RightCard.PDDialog.menuInfo = menuInfo;
                    MainWindow.mainWindow.RightCard.PDDialog.Title.Text = "设置新密码";
                    MainWindow.mainWindow.RightCard.PDDialog.type = PasswordType.CREATE;
                    RunTimeStatus.SHOW_MENU_PASSWORDBOX = true;
                    MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Visible;
                }
                else
                {
                    menuInfo.IsEncrypt = true;
                    HandyControl.Controls.Growl.Success(menuInfo.MenuName + " 已加密!", "MainWindowGrowl");
                }
            }
        }

        private void AlterPassword(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.RightCard.PDDialog.Title.Text = "输入旧密码";
            MainWindow.mainWindow.RightCard.PDDialog.type = PasswordType.ALTER;
            MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Visible;
            //单独设置焦点
            MainWindow.mainWindow.RightCard.PDDialog.SetFocus();
        }

        /// <summary>
        /// 右键点击进行处理 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCard_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RunTimeStatus.SHOW_RIGHT_BTN_MENU = true;
            new Thread(() =>
            {
                Thread.Sleep(50);
                RunTimeStatus.SHOW_RIGHT_BTN_MENU = false;
            }).Start();

            //在没有设置密码的情况下不弹出修改密码菜单
            if (string.IsNullOrEmpty(appData.AppConfig.MenuPassword))
            {
                AlterPW1.Visibility = Visibility.Collapsed;
            }
            else
            {
                AlterPW1.Visibility = Visibility.Visible;
            }
        }

        private void ListBoxItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            MenuInfo info = lbi.DataContext as MenuInfo;

            ItemCollection ics = lbi.ContextMenu.Items;

            foreach (object obj in ics)
            {
                MenuItem mi = (MenuItem)obj;
                if (mi.Header.Equals("修改密码"))
                {
                    if (string.IsNullOrEmpty(appData.AppConfig.MenuPassword))
                    {
                        mi.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        mi.Visibility = Visibility.Visible;
                    }
                    break;
                }
                if (mi.Header.Equals("加密此列表") || mi.Header.Equals("取消加密此列表"))
                {
                    if (info.IsEncrypt)
                    {
                        mi.Header = "取消加密此列表";
                    }
                    else
                    {
                        mi.Header = "加密此列表";
                    }
                }
            }

        }

        /// <summary>
        /// 创建关联文件夹的菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateFolderMenu(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
            {
                MenuInfo info = new MenuInfo()
                {
                    MenuEdit = Visibility.Collapsed, MenuId = System.Guid.NewGuid().ToString(), MenuName = new DirectoryInfo(dialog.SelectedPath).Name,

                    MenuType = MenuTypeEnum.RELATION_FOLDER, RelationPath = dialog.SelectedPath
                };

                appData.MenuList.Add(info);
                MenuListBox.SelectedIndex = appData.MenuList.Count - 1;
                appData.AppConfig.SelectedMenuIndex = MenuListBox.SelectedIndex;
                appData.AppConfig.SelectedMenuIcons = info.IconList;
                //首次触发不了Selected事件
                object obj = MenuListBox.ItemContainerGenerator.ContainerFromIndex(MenuListBox.SelectedIndex);
                SetListBoxItemEvent((ListBoxItem)obj);
                Lbi_Selected(obj, null);
            }
        }
    }
}
