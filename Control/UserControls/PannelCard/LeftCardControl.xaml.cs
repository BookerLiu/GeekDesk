using DraggAnimatedPanelExample;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;

using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;


namespace GeekDesk.Control.UserControls.PannelCard
{
    /// <summary>
    /// LeftCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class LeftCardControl : UserControl
    {
        private int menuSelectIndexTemp = -1;
        private AppData appData = MainWindow.appData;

        public LeftCardControl()
        {
            InitializeComponent();
            if (appData.AppConfig.SelectedMenuIndex >= appData.MenuList.Count || appData.AppConfig.SelectedMenuIndex == -1)
            {
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[0].IconList;
            }
            else
            {
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

            ////菜单点击事件
        private void MenuClick(object sender, MouseButtonEventArgs e)
        {
            //设置对应菜单的图标列表
            MenuInfo mi = (MenuInfo)(((StackPanel)sender).Tag);
            appData.AppConfig.SelectedMenuIcons = mi.IconList;
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
        }

        /// <summary>
        /// 新建菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMenu(object sender, RoutedEventArgs e)
        {
            MenuInfo info = new MenuInfo() { MenuEdit = Visibility.Collapsed, MenuId = System.Guid.NewGuid().ToString(), MenuName = "NewMenu" };
            appData.MenuList.Add(info);
            MenuListBox.Items.Refresh();
            MenuListBox.SelectedIndex = appData.MenuList.Count - 1;
            appData.AppConfig.SelectedMenuIndex = MenuListBox.SelectedIndex;
            appData.AppConfig.SelectedMenuIcons = info.IconList;
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
            if(index == 0)
            {
                index = 0;
            } else
            {
                index = index - 1;
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
            TextBox menuBox = null;
            if (e.GetType() == typeof(KeyEventArgs))
            {
                KeyEventArgs eKey = e as KeyEventArgs;
                if (eKey.Key == Key.Enter)
                {
                    menuBox = ((TextBox)sender);
                }
            }
            else if (e.GetType() == typeof(RoutedEventArgs))
            {
                menuBox = ((TextBox)sender);
            }

            if (menuBox != null)
            {
                MenuInfo menuInfo = menuBox.Tag as MenuInfo;
                string text = menuBox.Text;
                menuInfo.MenuName = text;
                menuInfo.MenuEdit = Visibility.Collapsed;
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
        /// 修改菜单图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditMenuGeometry(object sender, RoutedEventArgs e)
        {
            MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            IconfontWindow.Show(SvgToGeometry.GetIconfonts(), menuInfo);
        }

        private void menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //设置对应菜单的图标列表
            if (MenuListBox.SelectedIndex == -1)
            {
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[appData.MenuList.Count - 1].IconList;
            }
            else
            {
                appData.AppConfig.SelectedMenuIcons = appData.MenuList[MenuListBox.SelectedIndex].IconList;
            }
        }
    }
}
