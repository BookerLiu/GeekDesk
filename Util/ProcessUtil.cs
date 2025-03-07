using GeekDesk.Constant;
using GeekDesk.MyThread;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeekDesk.Util
{
    public class ProcessUtil
    {


        public static void StartIconApp(IconInfo icon, IconStartType type, bool useRelativePath = false)
        {
            App.Current.Dispatcher.Invoke(() =>
            {

                try
                {
                    using (Process p = new Process())
                    {
                        string startArg = icon.StartArg;

                        if (startArg != null && Constants.SYSTEM_ICONS.ContainsKey(startArg))
                        {
                            StartSystemApp(startArg, type);
                        }
                        else
                        {
                            string path;
                            if (useRelativePath)
                            {
                                string fullPath = Path.Combine(Constants.APP_DIR, icon.RelativePath);
                                path = Path.GetFullPath(fullPath);
                            }
                            else
                            {
                                path = icon.Path;
                            }
                            p.StartInfo.FileName = path;
                            if (!StringUtil.IsEmpty(startArg))
                            {
                                p.StartInfo.Arguments = startArg;
                            }
                            if (icon.IconType == IconType.OTHER)
                            {
                                if (!File.Exists(path) && !Directory.Exists(path))
                                {
                                    //如果没有使用相对路径  那么使用相对路径启动一次
                                    if (!useRelativePath)
                                    {
                                        StartIconApp(icon, type, true);
                                        return;
                                    }
                                    else
                                    {
                                        HandyControl.Controls.Growl.WarningGlobal("程序启动失败(文件路径不存在或已删除)!");
                                        return;
                                    }
                                }
                                p.StartInfo.WorkingDirectory = path.Substring(0, path.LastIndexOf("\\"));
                                switch (type)
                                {
                                    case IconStartType.ADMIN_STARTUP:
                                        //p.StartInfo.Arguments = "1";//启动参数
                                        p.StartInfo.Verb = "runas";
                                        //p.StartInfo.CreateNoWindow = false; //设置显示窗口
                                        p.StartInfo.UseShellExecute = true;//不使用操作系统外壳程序启动进程
                                                                           //p.StartInfo.ErrorDialog = false;
                                        if (MainWindow.appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                                        {
                                            //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                            if (MainWindow.appData.AppConfig.MarginHide)
                                            {
                                                if (!MarginHide.IsMargin())
                                                {
                                                    MainWindow.HideApp();
                                                }
                                            }
                                            else
                                            {
                                                MainWindow.HideApp();
                                            }

                                        }
                                        break;// c#好像不能case穿透
                                    case IconStartType.DEFAULT_STARTUP:
                                        if (MainWindow.appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                                        {
                                            //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                            if (MainWindow.appData.AppConfig.MarginHide)
                                            {
                                                if (!MarginHide.IsMargin())
                                                {
                                                    MainWindow.HideApp();
                                                }
                                            }
                                            else
                                            {
                                                MainWindow.HideApp();
                                            }
                                        }
                                        break;
                                    case IconStartType.SHOW_IN_EXPLORE:
                                        p.StartInfo.FileName = "Explorer.exe";
                                        p.StartInfo.Arguments = "/e,/select," + icon.Path;
                                        break;
                                }
                            }
                            else
                            {
                                if (MainWindow.appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                                {
                                    //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                    if (MainWindow.appData.AppConfig.MarginHide)
                                    {
                                        if (!MarginHide.IS_HIDE)
                                        {
                                            MainWindow.HideApp();
                                        }
                                    }
                                    else
                                    {
                                        MainWindow.HideApp();
                                    }
                                }
                            }
                            p.Start();
                            if (useRelativePath)
                            {
                                //如果使用相对路径启动成功 那么重新设置程序绝对路径
                                icon.Path = path;
                            }
                        }
                    }
                    icon.Count++;

                    //隐藏搜索框
                    //if (RunTimeStatus.SEARCH_BOX_SHOW)
                    //{
                    //    MainWindow.mainWindow.HidedSearchBox();
                    //}
                }
                catch (Exception e)
                {
                    if (!useRelativePath)
                    {
                        StartIconApp(icon, type, true);
                    }
                    else
                    {
                        HandyControl.Controls.Growl.WarningGlobal("程序启动失败(可能为不支持的启动方式)!");
                        LogUtil.WriteErrorLog(e, "程序启动失败:path=" + icon.Path + ",type=" + type);
                    }
                }

                //启动后根据是否开启了使用次数排序判断是否执行一次排序
                CommonCode.SortIconList(MainWindow.appData.AppConfig.IconSortType == (SortType.COUNT_LOW|SortType.COUNT_UP) ? true : false);
            });
        }



        private static void StartSystemApp(string startArg, IconStartType type)
        {
            if (type == IconStartType.SHOW_IN_EXPLORE)
            {
                Growl.WarningGlobal("系统项目不支持打开文件位置操作!");
                return;
            }
            switch (startArg)
            {
                case "Calculator":
                    Process.Start("calc.exe");
                    break;
                case "Computer":
                    Process.Start("explorer.exe");
                    break;
                case "GroupPolicy":
                    Process.Start("gpedit.msc");
                    break;
                case "Notepad":
                    Process.Start("notepad");
                    break;
                case "Network":
                    Process.Start("ncpa.cpl");
                    break;
                case "RecycleBin":
                    Process.Start("shell:RecycleBinFolder");
                    break;
                case "Registry":
                    Process.Start("regedit.exe");
                    break;
                case "Mstsc":
                    if (type == IconStartType.ADMIN_STARTUP)
                    {
                        Process.Start("mstsc", "-admin");
                    }
                    else
                    {
                        Process.Start("mstsc");
                    }
                    break;
                case "Control":
                    Process.Start("Control");
                    break;
                case "CMD":
                    if (type == IconStartType.ADMIN_STARTUP)
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "cmd.exe";
                            process.StartInfo.Verb = "runas";
                            process.Start();
                        }
                    }
                    else
                    {
                        Process.Start("cmd");
                    }
                    break;
                case "Services":
                    Process.Start("services.msc");
                    break;
            }
            //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
            if (MainWindow.appData.AppConfig.MarginHide)
            {
                if (!MarginHide.IS_HIDE)
                {
                    MainWindow.HideApp();
                }
            }
            else
            {
                MainWindow.HideApp();
            }
        }


        public static void ReStartApp()
        {
            if (MainWindow.appData.AppConfig.MouseMiddleShow || MainWindow.appData.AppConfig.SecondsWindow == true)
            {
                MouseHookThread.Dispose();
            }

            Process p = new Process();
            p.StartInfo.FileName = Constants.APP_DIR + "GeekDesk.exe";
            p.StartInfo.WorkingDirectory = Constants.APP_DIR;
            p.Start();

            Application.Current.Shutdown();
        }

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            QueryLimitedInformation = 0x00001000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName(
        [In] IntPtr hProcess,
        [In] int dwFlags,
        [Out] StringBuilder lpExeName,
        ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(
        ProcessAccessFlags processAccess,
        bool bInheritHandle,
        int processId);

        public static String GetProcessFilename(Process p)
        {
            int capacity = 2000;
            StringBuilder builder = new StringBuilder(capacity);
            IntPtr ptr = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, p.Id);
            if (!QueryFullProcessImageName(ptr, 0, builder, ref capacity))
            {
                return String.Empty;
            }
            return builder.ToString();
        }


    }
}
