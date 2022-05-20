using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;

using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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


        //是否正在修改菜单
        private static bool IS_EDIT = false;

        public LeftCardControl()
        {
            InitializeComponent();


            this.Loaded += (s, e) =>
            {
                SelectLastMenu();
                SetMenuListBoxItemEvent();
            };
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
            ListBoxItem lbi = sender as ListBoxItem;

            SolidColorBrush fontColor = new SolidColorBrush(Colors.Black);

            lbi.MouseLeave -= Lbi_MouseLeave;
            lbi.Background = bac;
            lbi.Foreground = fontColor;
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
            MenuInfo info = new MenuInfo() { MenuEdit = Visibility.Collapsed, MenuId = System.Guid.NewGuid().ToString(), MenuName = "NewMenu" };
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
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            menuInfo.MenuEdit = (int)Visibility.Visible;
            IS_EDIT = true;
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
                IS_EDIT = false;
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
            //设置对应菜单的图标列表
            if (MenuListBox.SelectedIndex == -1)
            {
                //appData.AppConfig.SelectedMenuIcons = appData.MenuList[appData.MenuList.Count - 1].IconList;
            }
            else
            {
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[MenuListBox.SelectedIndex].IconList;
            }
        }


        /// <summary>
        /// 鼠标悬停切换菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (appData.AppConfig.HoverMenu && !IS_EDIT)
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
        }

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
            if (e.Delta < 0)
            {
                int index = MenuListBox.SelectedIndex;
                if (index < MenuListBox.Items.Count - 1)
                {
                    index ++;
                } else
                {
                    index = 0;
                }
                MenuListBox.SelectedIndex = index;
            } else if (e.Delta > 0)
            {
                int index = MenuListBox.SelectedIndex;
                if (index > 0)
                {
                    index --;
                }
                else
                {
                    index = MenuListBox.Items.Count - 1;
                }
                MenuListBox.SelectedIndex = index;
            }
        }
    }
}
