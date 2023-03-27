using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class DelayHelper
    {
        public event EventHandler Idled = delegate { };
        public int WaitingMilliSeconds { get; set; }

        public object Source { get; set; }

        readonly System.Threading.Timer waitingTimer;

        public DelayHelper(int waitingMilliSeconds = 600)
        {
            WaitingMilliSeconds = waitingMilliSeconds;
            waitingTimer = new System.Threading.Timer(p =>
            {
                Idled(this, EventArgs.Empty);
            });
        }


        public void DelayExecute(object source)
        {
            this.Source = source;
            waitingTimer.Change(WaitingMilliSeconds, System.Threading.Timeout.Infinite);
        }
    }
}
