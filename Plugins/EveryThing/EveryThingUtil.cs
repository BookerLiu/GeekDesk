using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Plugins.EveryThing
{
    public class EveryThingUtil
    {


        public static void StartEveryThing(String pluginsPath)
        {
            bool Is64Bit = Environment.Is64BitOperatingSystem;
            string pluginPath = pluginsPath + "/EveryThing/" + (Is64Bit ? 64 : 32) + "/EveryThing.exe";

            //启动服务
            using (Process p = new Process())
            {
                p.StartInfo.FileName = pluginPath;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Arguments = " -svc";
                p.Start();
            }
            //启动程序
            using (Process p = new Process())
            {
                p.StartInfo.FileName = pluginPath;
                p.Start();
            }
        }
    }
}
