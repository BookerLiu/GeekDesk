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
using System.Windows.Shapes;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// BacklogInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToDoInfoWindow
    {

        private static int windowType = -1;
        private static readonly int NEW_TODO = 1;
        private static readonly int DETAIL_TODO = 2;

        private AppData appData = MainWindow.appData;

        private ToDoInfo info;

        private ToDoInfoWindow()
        {
            InitializeComponent();
            ExeTime.SelectedDateTime = DateTime.Now.AddMinutes(10);
            this.Topmost = true;
        }
        private ToDoInfoWindow(ToDoInfo info)
        {
            InitializeComponent();
            this.Topmost = true;
            Title.Text = info.Title;
            Msg.Text = info.Msg;
            ExeTime.Text = info.ExeTime;
            DoneTime.Text = info.DoneTime;
            this.info = info;
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
        /// 保存待办
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

            DateTime dt;
            if (Title.Text.Trim() == "" || ExeTime.Text.Trim() == "")
            {
                Growl.Warning("任务标题 和 待办时间不能为空!");
                return;
            } else
            {
                try
                {
                    dt = Convert.ToDateTime(ExeTime.Text);
                } catch (Exception)
                {
                    Growl.Warning("请输入正确的时间!");
                    return;
                }
            }
            if (windowType == NEW_TODO)
            {
                info = new ToDoInfo
                {
                    Title = Title.Text,
                    Msg = Msg.Text,
                    ExeTime = ExeTime.Text
                };
                appData.ToDoList.Add(info);
            } else
            {
                appData.HiToDoList.Remove(info);
                info.Title = Title.Text;
                info.Msg = Msg.Text;
                info.ExeTime = ExeTime.Text;
                info.DoneTime = null;
                appData.ToDoList.Add(info);
            }

            DateTime dtNow = DateTime.Now;
            TimeSpan ts = dt.Subtract(dtNow);
            int minutes = (int)Math.Ceiling(ts.TotalMinutes);
            if (minutes < 0)
            {
                minutes = 0;
            }
            if (minutes > 60)
            {
                int m = minutes % 60;
                int h = minutes / 60;
                Growl.SuccessGlobal("设置待办任务成功, 系统将在 " + h + " 小时零 " + m + " 分钟后提醒您!");

            } else
            {
                Growl.SuccessGlobal("设置待办任务成功, 系统将在 " + minutes + " 分钟后提醒您!");
            }

            CommonCode.SaveAppData(MainWindow.appData);
            this.Close();
        }

        private static System.Windows.Window window = null;
        public static void ShowNone()
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoInfoWindow();
                window.Show();
            }
            windowType = NEW_TODO;
            window.Visibility = Visibility.Visible;
        }

        public static void ShowOrHide()
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoInfoWindow();
                window.Show();
            } else
            {
                window.Close();
            }
        }


        public static System.Windows.Window GetThis()
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoInfoWindow();
                window.Show();
            }
            window.Visibility = Visibility.Collapsed;
            windowType = NEW_TODO;
            return window;
        }

        private static System.Windows.Window window2 = null;
        public static void ShowDetail(ToDoInfo info)
        {
            if (window2 == null || !window2.Activate())
            {
                window2 = new ToDoInfoWindow(info);
            }
            windowType = DETAIL_TODO;
            window2.Show();
        }
    }
}
