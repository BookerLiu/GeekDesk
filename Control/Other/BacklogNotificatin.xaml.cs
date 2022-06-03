using GeekDesk.Constant;
using GeekDesk.Task;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using Quartz;
using System;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// BacklogNotificatin.xaml 的交互逻辑
    /// </summary>
    public partial class BacklogNotificatin
    {

        private AppData appData = MainWindow.appData;
        public BacklogNotificatin(ToDoInfo info)
        {
            InitializeComponent();
            this.DataContext = info;
        }

        private void BacklogDone_Click(object sender, RoutedEventArgs e)
        {
            ToDoInfo info = this.DataContext as ToDoInfo;
            info.DoneTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (info.ExecType == TodoTaskExecType.CRON)
            {
                CronExpression exp = new CronExpression(info.Cron);
                DateTime nowTime = DateTime.Now;

                //计算下次执行时间
                DateTime nextTime = DateTime.SpecifyKind(exp.GetNextValidTimeAfter(nowTime).Value.LocalDateTime, DateTimeKind.Local);

                string nextTimeStr = nextTime.ToString("yyyy-MM-dd HH:mm:ss");
                info.ExeTime = nextTimeStr;

                TimeSpan ts = nextTime.Subtract(nowTime);
                int minutes = (int)Math.Ceiling(ts.TotalMinutes);
                if (minutes < 0)
                {
                    minutes = 0;
                }
                if (minutes > 60)
                {
                    int m = minutes % 60;
                    int h = minutes / 60;
                    Growl.SuccessGlobal("下次任务将在 " + h + " 小时零 " + m + " 分钟后提醒您!");
                }
                else
                {
                    Growl.SuccessGlobal("下次任务将在 " + minutes + " 分钟后提醒您!");
                }
            }
            else
            {
                appData.ToDoList.Remove(info); //执行任务删除
                appData.HiToDoList.Add(info);  //添加历史任务
            }
            ToDoTask.activityBacklog[info].Close(); //关闭桌面通知
            ToDoTask.activityBacklog.Remove(info);//激活任务删除
            CommonCode.SaveAppData(appData, Constants.DATA_FILE_PATH);
        }


        /// <summary>
        /// 只允许输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelayTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textBoxInt;
            //转化按下的键为数字，如果不是数字则会抓取到报错信息，不键入，反之则键入
            try
            {
                textBoxInt = int.Parse($"{e.Text}");
            }
            catch (FormatException)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 失去焦点前如果为空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelayTime_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int textBoxInt;
            //转化val为数字，如果不是数字则会抓取到报错信息
            try
            {
                textBoxInt = int.Parse(DelayTime.Text.Trim());
            }
            catch (FormatException)
            {
                DelayTime.Text = "10";
            }
        }

        /// <summary>
        /// 推迟提醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelayButton_Click(object sender, RoutedEventArgs e)
        {
            ToDoInfo info = this.DataContext as ToDoInfo;
            int time = int.Parse(DelayTime.Text);
            string type = DelayType.Text;
            switch (type)
            {
                case "分":
                    info.ExeTime = DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss");
                    Growl.SuccessGlobal("将在 " + time + " 分钟后再次提醒您!");
                    break;
                case "时":
                    info.ExeTime = DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss");
                    Growl.SuccessGlobal("将在 " + time + " 小时后再次提醒您!");
                    break;
            }
            ToDoTask.activityBacklog[info].Close(); //关闭桌面通知
            ToDoTask.activityBacklog.Remove(info);//激活任务删除
        }
    }
}
