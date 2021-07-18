using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls.Backlog
{
    /// <summary>
    /// BacklogControl.xaml 的交互逻辑
    /// </summary>
    public partial class TodoControl : UserControl
    {
        private AppData appData = MainWindow.appData;
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
                    appData.ToDoList.Remove(info);
                    CommonCode.SaveAppData(MainWindow.appData);
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


        private void DataGridRow_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            BacklogList.SelectedIndex = ((DataGridRow)sender).GetIndex();
            Menu.IsOpen = true;
        }


    }
}
