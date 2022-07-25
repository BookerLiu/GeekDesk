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
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        private static IKeyboardMouseEvents middleHook = null;
        private static Dispatcher middleDP;

        public static void MiddleHook()
        {
            //使用dispatcher来单独监听UI线程  防止程序卡顿
            middleDP = DispatcherBuild.Build();
            middleHook = Hook.GlobalEvents();
            middleDP.Invoke((Action)(() =>
            {
                middleHook.MouseUpExt += MiddleHookFun;
            }));
        }


        private static Color GetBottomBeforeColor()
        {
            return GetColor(1760, 985);
        }

        private static Color GetTopBeforeColor()
        {
            return GetColor(1751, 693);
        }

        private static Color GetColor(int w2, int h2)
        {
            double w = 1920;
            double h = 1080;
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            System.Drawing.Point p = new System.Drawing.Point((int)(w2 / w * width), (int)(h2 / h * height));
            return ScreenUtil.GetColorAt(p);
        }

        

        /// <summary>
        /// 鼠标中键呼出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MiddleHookFun(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                //中键打开App
                if (appConfig.MouseMiddleShow)
                {
                    if (MotionControl.hotkeyFinished)
                    {
                        App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
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


        public static void DisposeMiddle()
        {
            try
            {
                if (middleHook != null)
                {
                    middleHook.MouseUpExt -= MiddleHookFun;
                    middleHook.Dispose();
                    middleDP.InvokeShutdown();
                }
               
            }
            catch (Exception ex) { }

        }


    }
}
