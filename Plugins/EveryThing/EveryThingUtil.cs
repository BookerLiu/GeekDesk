using GeekDesk.Constant;
using GeekDesk.Plugins.EveryThing.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.Plugins.EveryThing
{

    public class EveryThingUtil
    {
        //检查是否是由GeekDesk启动的EveryThing
        private static bool IsByGeekDesk = true;

        //每次加载20条
        private static long pageCount = 20;
        private static UInt32 ui = 0;

        private static Process serviceProcess = null;
        private static Process exeProcess = null;


        public static void EnableEveryThing(int delayTime = 2000)
        {
            string pluginsPath = Constants.PLUGINS_PATH;

            bool Is64Bit = Environment.Is64BitOperatingSystem;
            string everyThingPath = pluginsPath + "/EveryThing/" + (Is64Bit ? 64 : 32) + "/EveryThing.exe";

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(delayTime);

                    //判断EveryThing服务是否已启动
                    bool enabled = false;
                    Process[] processList = Process.GetProcesses();
                    foreach (System.Diagnostics.Process process in processList)
                    {
                        if (process.ProcessName.ToUpper().Equals("EVERYTHING"))
                        {
                            enabled = true;
                            IsByGeekDesk = false;
                            break;
                        }
                    }

                    if (!enabled)
                    {
                        //启动服务
                        serviceProcess = new Process();
                        serviceProcess.StartInfo.FileName = everyThingPath;
                        serviceProcess.StartInfo.UseShellExecute = true;
                        serviceProcess.StartInfo.Verb = "runas";
                        serviceProcess.StartInfo.Arguments = " -svc";
                        serviceProcess.Start();
                    }

                    Thread.Sleep(2000);
                    processList = Process.GetProcesses();

                    //启动程序
                    exeProcess = new Process();
                    exeProcess.StartInfo.FileName = everyThingPath;
                    exeProcess.Start();
                    int waitTime = 5000;
                    while (true && waitTime > 0)
                    {
                        Thread.Sleep(100);
                        waitTime -= 100;
                        exeProcess.CloseMainWindow();
                    }

                } catch (Exception e)
                {

                }
                
            }).Start();

        }



        public static void DisableEveryThing()
        {
            try
            {
                if (IsByGeekDesk)
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        EveryThing64.Everything_Exit();
                    }
                    else
                    {
                        EveryThing32.Everything_Exit();
                    }
                }
                if (exeProcess != null) exeProcess.Kill();
                if (serviceProcess != null) serviceProcess.Kill();
            } catch (Exception e)
            {
                LogUtil.WriteErrorLog(e);
            }
        }


        public static bool HasNext()
        {
            return ui < Everything_GetNumResults();
        }


        public static ObservableCollection<IconInfo> Search(string text)
        {
            ui = 0;
            //EveryThing全盘搜索
            Everything_Reset();
            EveryThingUtil.Everything_SetSearchW(text);
            EveryThingUtil.Everything_SetRequestFlags(
                EveryThingConst.EVERYTHING_REQUEST_FILE_NAME
                | EveryThingConst.EVERYTHING_REQUEST_PATH
                | EveryThingConst.EVERYTHING_REQUEST_DATE_MODIFIED
                | EveryThingConst.EVERYTHING_REQUEST_SIZE);
            EveryThingUtil.Everything_SetSort(13);
            EveryThingUtil.Everything_QueryW(true);
            return NextPage();
        }

        public static ObservableCollection<IconInfo> NextPage()
        {
            if (ui == 0)
            {
                pageCount = 40;
            } else
            {
                pageCount = 20;
            }

            string filePath;
            const int bufsize = 260;
            StringBuilder buf = new StringBuilder(bufsize);
            ObservableCollection<IconInfo> iconBakList = new ObservableCollection<IconInfo>();
            for (long count = 0; ui < Everything_GetNumResults() && count < pageCount; count++, ui++)
            {
                buf.Clear();
                EveryThingUtil.Everything_GetResultFullPathName(ui, buf, bufsize);
                filePath = buf.ToString();

                string tempPath = filePath;

                string ext = "";
                if (!ImageUtil.IsSystemItem(filePath))
                {
                    ext = System.IO.Path.GetExtension(filePath).ToLower();
                }

                if (".lnk".Equals(ext))
                {

                    string targetPath = FileUtil.GetTargetPathByLnk(filePath);
                    if (targetPath != null)
                    {
                        filePath = targetPath;
                    }
                }


                string name = System.IO.Path.GetFileNameWithoutExtension(tempPath);
                if (String.IsNullOrEmpty(name))
                {
                    name = tempPath.Substring(tempPath.LastIndexOf("\\"));
                }

                IconInfo iconInfo = new IconInfo
                {
                    Path_NoWrite = filePath,
                    LnkPath_NoWrite = tempPath,
                    BitmapImage_NoWrite = ImageUtil.GetBitmapIconByUnknownPath(filePath),
                    StartArg_NoWrite = FileUtil.GetArgByLnk(tempPath),
                    Name_NoWrite = name,
                };

                //缓存信息  异步加载图标
                iconBakList.Add(iconInfo);
            }
            return iconBakList;
        }






        public static UInt32 Everything_SetSearchW(string lpSearchString)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return EveryThing64.Everything_SetSearchW(lpSearchString);
            } else
            {
                return EveryThing32.Everything_SetSearchW(lpSearchString);
            }
        }

        public static void Everything_SetRequestFlags(UInt32 dwRequestFlags)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                EveryThing64.Everything_SetRequestFlags(dwRequestFlags);
            }
            else
            {
                EveryThing32.Everything_SetRequestFlags(dwRequestFlags);
            }
        }

        public static void Everything_SetSort(UInt32 dwSortType)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                EveryThing64.Everything_SetSort(dwSortType);
            }
            else
            {
                EveryThing32.Everything_SetSort(dwSortType);
            }
        }

        public static bool Everything_QueryW(bool bWait)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return EveryThing64.Everything_QueryW(bWait);
            }
            else
            {
                return EveryThing32.Everything_QueryW(bWait);
            }
        }


        public static UInt32 Everything_GetNumResults()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return EveryThing64.Everything_GetNumResults();
            }
            else
            {
                return EveryThing32.Everything_GetNumResults();
            }
        }


        public static void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                EveryThing64.Everything_GetResultFullPathName(nIndex, lpString, nMaxCount);
            }
            else
            {
                EveryThing32.Everything_GetResultFullPathName(nIndex, lpString, nMaxCount);
            }
        }

        public static void Everything_Reset()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                EveryThing64.Everything_Reset();
            }
            else
            {
                EveryThing32.Everything_Reset();
            }
        }


    }
}
