using GeekDesk.Control.UserControls.Config;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GeekDesk.MyThread
{
    public class MouseHookThread
    {
        private static readonly AppConfig appConfig = MainWindow.appData.AppConfig;
        public static Dispatcher dispatcher;
        private static UserActivityHook hook;

        public static void Hook()
        {
            //使用dispatcher来单独监听UI线程 防止程序卡顿
            dispatcher = DispatcherBuild.Build();
            dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                hook = new UserActivityHook();

                if (appConfig.MouseMiddleShow)
                {
                    hook.OnMouseWheelUp += OnMouseWheelUp;
                }

                hook.Start(true, false);
            }));
        }

        private static void OnMouseLeftDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private static void OnMouseLeftUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private static void OnMouseWheelUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseWheelShowApp(sender, e);
        }

        /// <summary>
        /// 鼠标中键呼出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MouseWheelShowApp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //中键打开App
            if (appConfig.MouseMiddleShow && MotionControl.hotkeyFinished)
            {
                App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (MainWindow.mainWindow.Visibility == Visibility.Collapsed || MainWindow.mainWindow.Opacity == 0)
                    {
                        MainWindow.ShowApp();
                    }
                    else
                    {
                        MainWindow.HideApp();
                    }
                }));
            }
        }


        public static void Dispose()
        {
            try
            {
                if (hook != null)
                {
                    if (hook.MouseWheelUpEnable())
                    {
                        hook.OnMouseWheelUp -= OnMouseWheelUp;
                    }
                    if (hook.MouseLeftDownEnable())
                    {
                        hook.OnMouseLeftDown -= OnMouseLeftDown;
                    }
                    if (hook.MouseLeftUpEnable())
                    {
                        hook.OnMouseLeftUp -= OnMouseLeftUp;
                    }
                    hook.Stop();
                    dispatcher.InvokeShutdown();
                    hook = null;
                    dispatcher = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteErrorLog(ex, "关闭hook出错");
            }
        }


    }
}
