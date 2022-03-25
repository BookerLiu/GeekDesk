using GeekDesk.Control.UserControls.Config;
using GeekDesk.ViewModel;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GeekDesk.MyThread
{
    public class MouseHookThread
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        private static IKeyboardMouseEvents m_GlobalHook = Hook.GlobalEvents();
        private static Dispatcher dispatcher;



        public static void MiddleHook()
        {
            //使用dispatcher来单独监听UI线程  防止程序卡顿
            dispatcher = DispatcherBuild.Build();
            dispatcher.Invoke((Action)(() =>
            {
                m_GlobalHook.MouseDownExt += M_GlobalHook_MouseDownExt;
            }));
        }

        public static void Dispose()
        {
            m_GlobalHook.MouseDownExt -= M_GlobalHook_MouseDownExt;
            m_GlobalHook.Dispose();
            dispatcher.InvokeShutdown();
        }

        /// <summary>
        /// 鼠标中键呼出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void M_GlobalHook_MouseDownExt(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (appConfig.MouseMiddleShow && e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (MotionControl.hotkeyFinished)
                {
                    MainWindow.mainWindow.Dispatcher.Invoke((Action)(() =>
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
        }

    }
}
