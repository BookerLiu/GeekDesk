using GeekDesk.Control.UserControls.Backlog;
using GeekDesk.Interface;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// BacklogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToDoWindow : IWindowCommon
    {
        private static TodoControl backlog = new TodoControl();
        private AppData appData = MainWindow.appData;
        private ToDoWindow()
        {
            InitializeComponent();
            RightCard.Content = backlog;
            backlog.BacklogList.ItemsSource = appData.ToDoList;
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

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MemuClick(object sender, RoutedEventArgs e)
        {
            SideMenuItem smi = sender as SideMenuItem;
            switch (smi.Tag.ToString())
            {
                case "History":
                    backlog.BacklogList.ItemsSource = appData.HiToDoList;
                    backlog.type = ToDoType.HISTORY;
                    break;
                default:
                    backlog.BacklogList.ItemsSource = appData.ToDoList;
                    backlog.type = ToDoType.NEW;
                    break;
            }
        }

        /// <summary>
        /// 新建待办
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBacklog_BtnClick(object sender, RoutedEventArgs e)
        {
            ToDoInfoWindow.ShowNone();
        }


        private static System.Windows.Window window = null;
#pragma warning disable CS0108 // “ToDoWindow.Show()”隐藏继承的成员“Window.Show()”。如果是有意隐藏，请使用关键字 new。
        public static void Show()
#pragma warning restore CS0108 // “ToDoWindow.Show()”隐藏继承的成员“Window.Show()”。如果是有意隐藏，请使用关键字 new。
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoWindow();
            }
            window.Show();
            Keyboard.Focus(window);
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
