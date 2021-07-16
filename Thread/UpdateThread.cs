using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.Thread
{
    public class UpdateThread
    {
        public static void Update()
        {
            System.Threading.Thread t = new System.Threading.Thread(new ThreadStart(updateApp));
            t.IsBackground = true;
            t.Start();
        }

        private static void updateApp()
        {

        }
    }
}
