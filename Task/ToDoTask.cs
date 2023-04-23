using GeekDesk.Control.Other;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Timers;

namespace GeekDesk.Task
{
    public class ToDoTask
    {

        ///public static ObservableCollection<ToDoInfo> activityBacklog = new ObservableCollection<ToDoInfo>();

        public static Dictionary<ToDoInfo, Notification> activityBacklog = new Dictionary<ToDoInfo, Notification>();

        public static void BackLogCheck()
        {

            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 5000
            };
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Check);
        }


        private static void Check(object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    if (MainWindow.appData.ToDoList.Count > 0)
                    {
                        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ObservableCollection<ToDoInfo> exeBacklogList = MainWindow.appData.ToDoList;
                        foreach (ToDoInfo info in exeBacklogList)
                        {
                            if (info.ExeTime.CompareTo(nowTime) == -1 && !activityBacklog.ContainsKey(info))
                            {
                                activityBacklog.Add(info, Notification.Show(new BacklogNotificatin(info), ShowAnimation.Fade, true));
                            }
                        }
                    }
                }
                catch (Exception e1) { }
                //ClearMemory();
            }));
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        #endregion


    }
}
