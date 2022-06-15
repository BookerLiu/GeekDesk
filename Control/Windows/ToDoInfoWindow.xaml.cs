using GeekDesk.Constant;
using GeekDesk.Interface;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using Quartz;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// BacklogInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToDoInfoWindow : IWindowCommon
    {

        private static int windowType = -1;
        private static readonly int NEW_TODO = 1;
        private static readonly int DETAIL_TODO = 2;

        private AppData appData = MainWindow.appData;

        //private ToDoInfo info;

        private ToDoInfoWindow()
        {
            InitializeComponent();

            this.Topmost = true;
            DateTime time = DateTime.Now.AddMinutes(10);
            ExeTime.SelectedDateTime = time;
            SetTimePanel.Visibility = Visibility.Visible;
            this.DataContext = new ToDoInfo
            {
                ExeTime = time.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
        private ToDoInfoWindow(ToDoInfo info)
        {
            InitializeComponent();
            this.Topmost = true;
            this.DataContext = info;
            SetTimePanel.Visibility = Visibility.Visible;
            //Title.Text = info.Title;
            //Msg.Text = info.Msg;
            //ExeTime.Text = info.ExeTime;
            //DoneTime.Text = info.DoneTime;
            //this.info = info;
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
            string execTime;
            TodoTaskExecType execType;
            if (Title.Text.Trim() == "")
            {
                Growl.Warning("任务标题不能为空!");
                return;
            }
            else
            {
                if (SetTimePanel.Visibility == Visibility.Visible)
                {
                    execType = TodoTaskExecType.SET_TIME;
                    if (ExeTime.Text.Trim() == "")
                    {
                        Growl.Warning("执行时间不能为空!");
                        return;
                    }
                    try
                    {
                        dt = Convert.ToDateTime(ExeTime.Text);
                    }
                    catch (Exception)
                    {
                        Growl.Warning("请输入正确的时间!");
                        return;
                    }
                    execTime = ExeTime.Text;
                }
                else
                {
                    execType = TodoTaskExecType.CRON;
                    if (Cron.Text.Trim() == "")
                    {
                        Growl.Warning("Cron表达式不能为空!");
                        return;
                    }
                    try
                    {
                        bool isValid = CronExpression.IsValidExpression(Cron.Text);
                        if (!isValid) throw new Exception();
                    }
                    catch (Exception)
                    {
                        Growl.Warning("请输入正确的Cron表达式!");
                        return;
                    }
                    CronExpression exp = new CronExpression(Cron.Text);
                    DateTime dd = DateTime.Now;
                    DateTimeOffset ddo = DateTime.SpecifyKind(dd, DateTimeKind.Local);
                    ddo = (DateTimeOffset)exp.GetNextValidTimeAfter(ddo);
                    execTime = ddo.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            dt = Convert.ToDateTime(execTime);
            ToDoInfo info = new ToDoInfo
            {
                Title = Title.Text,
                Msg = Msg.Text,
                ExeTime = execTime,
                ExecType = execType,
                Cron = Cron.Text
            };
            if (windowType != NEW_TODO)
            {
                ToDoInfo tdi = this.DataContext as ToDoInfo;
                if (appData.HiToDoList.Contains(tdi))
                {
                    appData.HiToDoList.Remove(tdi);
                }
                else if (appData.ToDoList.Contains(tdi))
                {
                    appData.ToDoList.Remove(tdi);
                }
            }
            appData.ToDoList.Add(info);

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

            }
            else
            {
                Growl.SuccessGlobal("设置待办任务成功, 系统将在 " + minutes + " 分钟后提醒您!");
            }

            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
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
            }
            else
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
                Keyboard.Focus(window);
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
            Keyboard.Focus(window2);
        }

        private void ExecType_Checked(object sender, RoutedEventArgs e)
        {
            TodoTaskExecType tag = (TodoTaskExecType)Convert.ToInt32((sender as RadioButton).Tag.ToString());
            switch (tag)
            {
                case TodoTaskExecType.SET_TIME:
                    SetTimePanel.Visibility = Visibility.Visible;
                    CronPanel.Visibility = Visibility.Collapsed;
                    break;
                default:
                    CronPanel.Visibility = Visibility.Visible;
                    SetTimePanel.Visibility = Visibility.Collapsed;
                    break;
            }
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
