using GeekDesk.Task;
using GeekDesk.Util;
using GeekDesk.ViewModel;
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

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// BacklogNotificatin.xaml 的交互逻辑
    /// </summary>
    public partial class BacklogNotificatin
    {

        private AppData appData = MainWindow.appData;
        public BacklogNotificatin(BacklogInfo info)
        {
            InitializeComponent();
            this.DataContext = info;
        }

        private void BacklogDone_Click(object sender, RoutedEventArgs e)
        {
            BacklogInfo info = this.DataContext as BacklogInfo;
            info.DoneTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            appData.ExeBacklogList.Remove(info); //执行任务删除
            appData.HiBacklogList.Add(info);  //添加历史任务
            BacklogTask.activityBacklog[info].Close(); //关闭桌面通知
            BacklogTask.activityBacklog.Remove(info);//激活任务删除
            CommonCode.SaveAppData(appData);
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
            BacklogInfo info = this.DataContext as BacklogInfo;
            int time = int.Parse(DelayTime.Text);
            string type = DelayType.Text;
            switch(type)
            {
                case "分":
                    info.ExeTime = DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "时":
                    info.ExeTime = DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
            }
            BacklogTask.activityBacklog[info].Close(); //关闭桌面通知
            BacklogTask.activityBacklog.Remove(info);//激活任务删除
        }
    }
}
