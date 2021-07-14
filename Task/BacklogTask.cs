using GeekDesk.Control;
using GeekDesk.Control.Other;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GeekDesk.Task
{
    public class BacklogTask
    {

        ///public static ObservableCollection<BacklogInfo> activityBacklog = new ObservableCollection<BacklogInfo>();

        public static Dictionary<BacklogInfo, Notification> activityBacklog = new Dictionary<BacklogInfo, Notification>();

        public static void BackLogCheck()
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 5000;
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Check);
        }


        private static void Check(object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindow.appData.ExeBacklogList.Count > 0)
                {
                    string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ObservableCollection<BacklogInfo> exeBacklogList = MainWindow.appData.ExeBacklogList;
                    foreach (BacklogInfo info in exeBacklogList)
                    {
                        if (info.ExeTime.CompareTo(nowTime) == -1 && !activityBacklog.ContainsKey(info))
                        {
                            activityBacklog.Add(info, Notification.Show(new BacklogNotificatin(info), ShowAnimation.Fade, true));
                        }
                    }
                }
            }));
        }



    }
}
