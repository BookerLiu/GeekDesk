using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeekDesk.Control.UserControls.Backlog
{

    public enum ToDoType
    {
        HISTORY = 1,
        NEW = 2
    }
    /// <summary>
    /// BacklogControl.xaml 的交互逻辑
    /// </summary>
    public partial class TodoControl : UserControl
    {
        public ToDoType type;
        public TodoControl()
        {
            InitializeComponent();
        }

        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            ToDoInfo info = BacklogList.SelectedItem as ToDoInfo;
            Growl.Ask("确认删除吗?", isConfirmed =>
            {
                if (isConfirmed)
                {
                    if (type == ToDoType.NEW)
                    {
                        MainWindow.appData.ToDoList.Remove(info);
                    }
                    else
                    {
                        MainWindow.appData.HiToDoList.Remove(info);
                    }
                    CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
                }
                return true;
            }, "DeleteConfirm");
        }

        private void DetailMenu_Click(object sender, RoutedEventArgs e)
        {
            ToDoInfo info = BacklogList.SelectedItem as ToDoInfo;
            ToDoInfoWindow.ShowDetail(info);
        }

        /// <summary>
        /// 禁用设置按钮右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridMenu_Initialized(object sender, EventArgs e)
        {
            BacklogList.ContextMenu = null;
        }

        /// <summary>
        /// 打开右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index;
            ToDoInfo info = ((Border)sender).DataContext as ToDoInfo;
            if (type == ToDoType.NEW)
            {
                index = MainWindow.appData.ToDoList.IndexOf(info);
            }
            else
            {
                index = MainWindow.appData.HiToDoList.IndexOf(info);
            }
            BacklogList.SelectedIndex = index;
            Menu.IsOpen = true;
        }

        /// <summary>
        /// 选中时颜色变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
            Color c = Color.FromRgb(91, 192, 222);
            SolidColorBrush b = new SolidColorBrush
            {
                Color = c,
                Opacity = 0.9
            };
            ((DataGridRow)sender).Background = b;
        }
    }
}
