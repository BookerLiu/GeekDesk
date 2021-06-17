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
using System.Threading.Tasks;

namespace GeekDesk.Task
{
    class BacklogTask
    {
      
        private static void BackLogCheck()
        {
            while (true)
            {
                string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ObservableCollection<BacklogInfo> exeBacklogList = MainWindow.appData.ExeBacklogList;
                foreach (BacklogInfo info in exeBacklogList)
                {
                    if (info.ExeTime.CompareTo(nowTime) == -1)
                    {
                        Notification.Show(new BacklogNotificatin(info), ShowAnimation.Fade, true);
                    }
                }
            }
        }

    }
}
