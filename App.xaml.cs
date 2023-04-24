﻿using GeekDesk.Constant;
using GeekDesk.Util;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GeekDesk
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, Constants.MY_NAME, out bool ret);
            if (!ret)
            {
                System.Threading.Thread.Sleep(2000);
                mutex = new System.Threading.Mutex(true, Constants.MY_NAME, out ret);
                if (!ret)
                {
                    MessageUtil.SendMsgByWName(
                        "GeekDesk_Main_" + Constants.MY_UUID,
                        "ShowApp"
                        );
                    Environment.Exit(0);
                }
            }
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
            Mouse.OverrideCursor = null;
            LogUtil.WriteErrorLog(e, "未捕获异常!");
            if (Constants.DEV)
            {
                MessageBox.Show("GeekDesk遇到一个问题, 不用担心, 这不影响其它操作!");
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogUtil.WriteErrorLog(e, "严重异常!");
            //MessageBox.Show("GeekDesk遇到未知问题崩溃!");
        }
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(
                    delegate (object f)
                    {
                        ((DispatcherFrame)f).Continue = false;
                        return null;
                    }), frame);
            Dispatcher.PushFrame(frame);
        }


    }

}
