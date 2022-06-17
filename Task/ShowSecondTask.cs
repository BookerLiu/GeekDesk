using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.Task
{
    internal class ShowSecondTask
    {

        public static void SHowSecond()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 5000
            };
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Process[] pcArr = Process.GetProcessesByName("ShellExperienceHost.exe");
            Thread.Sleep(1000);
        }
    }
}
