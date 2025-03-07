using GeekDesk.Util;
using Gma.System.MouseKeyHook;
using ShowSeconds.Common;
using GeekDesk.Util;
using ShowSeconds.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GeekDesk.MyThread;
using GeekDesk;
using System.Collections;

namespace ShowSeconds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondsWindow : Window
    {

        private System.Drawing.Color beforeColor;
        private System.Drawing.Color topBeforeColor;

        private bool expandClock = true; //是否展开时钟
        private System.Windows.Forms.Timer timer;

        private static double lProportion = 0.82;
        private static double tProportion = 0.03;
        private static int sleepTime = 1000;
        public SecondsWindow()
        {
            SecondsDataContext dc = new SecondsDataContext
            {
                Seconds = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second)
            };
            InitializeComponent();
            SolidColorBrush scb = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 47, 52, 44))
            {
                Opacity = 0.8
            };


            try
            {

                Hashtable settings = (Hashtable)ConfigurationManager.GetSection("ShowSecondsSettings");

                lProportion = Convert.ToDouble(settings["LProportion"]);
                tProportion = Convert.ToDouble(settings["TProportion"]);
                sleepTime = Convert.ToInt32(settings["DelayTime"]);
            }
            catch (Exception ex)
            {
                lProportion = 0.82;
                tProportion = 0.03;
                sleepTime = 1000;
            }

            BGBorder.Background = scb;
            this.DataContext = dc;
            this.Topmost = true;
            BGBorder.Visibility = Visibility.Collapsed;
            this.Show();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            Dispatcher secondsDP = DispatcherBuild.Build();
            IKeyboardMouseEvents secondsHook = Hook.GlobalEvents();
            secondsDP.Invoke((Action)(() =>
            {
                secondsHook.MouseDownExt += SecondsBakColorFun;
                secondsHook.MouseUpExt += SecondsHookSetFuc;
            }));

            HideWindowUtil.HideAltTab(this);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string str = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second);
            SecondsDataContext dc = this.DataContext as SecondsDataContext;
            dc.Seconds = str;
        }



        private static string FormatMS(int ms)
        {
            if (ms < 10)
            {
                return "0" + ms;
            }
            else
            {
                return ms.ToString();
            }
        }


        private void SecondsHookSetFuc(object sender, MouseEventExtArgs e)
        {

            //IntPtr taskBarWnd = WindowUtil.FindWindow("Shell_TrayWnd", null);
            //IntPtr tray = WindowUtil.FindWindowEx(taskBarWnd, IntPtr.Zero, "TrayNotifyWnd", null);
            ////IntPtr trayclock = WindowUtil.FindWindowEx(tray, IntPtr.Zero, "TrayClockWClass", null);
            //IntPtr trayclock = WindowUtil.GetForegroundWindow();
            //StringBuilder title = new StringBuilder(256);
            //WindowUtil.GetWindowText(trayclock, title, title.Capacity);//得到窗口的标题
            ////Console.WriteLine(title.ToString());
            //if (title.Equals("通知中心"))
            //{
                
            //}


            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (ScreenUtil.IsPrimaryFullScreen()) return;

                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                try
                {
                    int x = e.X;
                    int y = e.Y;
                    double w = 1920;
                    double h = 1080;
                    double width = SystemParameters.PrimaryScreenWidth;
                    double height = SystemParameters.PrimaryScreenHeight;
                    if (x > 1843 / w * width
                        && x < 1907 / w * width
                        && y > 1037 / h * height
                        && y < 1074 / h * height)
                    {

                        System.Drawing.Color c;
                        int count = sleepTime;
                        do
                        {
                            c = GetBottomBeforeColor();
                            if (c.A != beforeColor.A
                            || c.R != beforeColor.R
                            || c.G != beforeColor.G
                            || c.B != beforeColor.B)
                            {
                                break;
                            }
                            Thread.Sleep(50);
                            count -= 50;
                        } while (count > 0);

                        if (c.A != beforeColor.A
                            || c.R != beforeColor.R
                            || c.G != beforeColor.G
                            || c.B != beforeColor.B)
                        {
                            //判断是否展开时钟
                            System.Drawing.Color ct = GetTopBeforeColor();
                            if (ct.A != topBeforeColor.A
                            || ct.R != topBeforeColor.R
                            || ct.G != topBeforeColor.G
                            || ct.B != topBeforeColor.B)
                            {
                                expandClock = true;
                            }
                            else
                            {
                                expandClock = false;
                            }

                            if (!BGBorder.IsVisible)
                            {

                                System.Drawing.Color theamColor = GetColor(1919, 1079);
                                if (CalculateLight(theamColor) > 255 / 2)
                                {
                                    //light
                                    BGBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(theamColor.A, theamColor.R, theamColor.G, theamColor.B));
                                    SecondsText.Foreground = Constants.lightFont;
                                }
                                else
                                {
                                    // dark
                                    //BGBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(theamColor.A, theamColor.R, theamColor.G, theamColor.B));
                                    BGBorder.Background = Constants.darkBG;
                                    SecondsText.Foreground = Constants.darkFont;
                                }

                                SecondsDataContext dc = this.DataContext as SecondsDataContext;
                                dc.Seconds = (DateTime.Now.Hour).ToString() + ":" +
                                    FormatMS(DateTime.Now.Minute) + ":" +
                                    FormatMS(DateTime.Now.Second);

                                int sx = (int)(SystemParameters.PrimaryScreenWidth * lProportion);
                                int sMarginBottom = (int)(SystemParameters.WorkArea.Height * tProportion);
                                Left = sx - Width;
                                Top = SystemParameters.WorkArea.Height - Height;
                                BGBorder.Visibility = Visibility.Visible;
                                timer.Start();
                            }
                            else
                            {
                                BGBorder.Visibility = Visibility.Collapsed;
                                timer.Stop();
                            }
                        }
                    }
                    else if (true)
                    {
                        if ((expandClock && (x < 1574 / w * width
                                || x > 1906 / w * width
                                || y < 598 / h * height
                                || y > 1020 / h * height)
                                )
                                || !expandClock && (x < 1574 / w * width
                                || x > 1906 / w * width
                                || y < 950 / h * height
                                || y > 1020 / h * height)
                                )
                        {
                            BGBorder.Visibility = Visibility.Collapsed;
                            timer.Stop();
                        }
                    }
                    }
                    catch (Exception e1) { }
                }));
            }
           
        }


        private static System.Windows.Window window = null;
        public static void ShowWindow()
        {
            try
            {
                if (window == null || !window.Activate())
                {
                    window = new SecondsWindow();
                }
            } catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "打开显秒窗口异常!");
            }
            
        }

        public static void CloseWindow()
        {
            try
            {
                window.Close();
            } catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "关闭显秒窗口异常!");
            }
        }

        private void SecondsBakColorFun(object sender, MouseEventExtArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                beforeColor = GetBottomBeforeColor();
                topBeforeColor = GetTopBeforeColor();
            }
        }

        private static System.Drawing.Color GetBottomBeforeColor()
        {
            return GetColor(1760, 985);
        }

        private static System.Drawing.Color GetTopBeforeColor()
        {
            return GetColor(1751, 693);
        }

        private static System.Drawing.Color GetColor(int w2, int h2)
        {
            double w = 1920;
            double h = 1080;
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            System.Drawing.Point p = new System.Drawing.Point((int)(w2 / w * width), (int)(h2 / h * height));
            return ScreenUtil.GetColorAt(p);
        }


        private static int CalculateLight(System.Drawing.Color color)
        {
            int[] colorArr = new int[] { color.R, color.G, color.B };

            int max = 0;
            int min = 255;
            foreach (int i in colorArr)
            {
                max = Math.Max(max, i);
                min = Math.Min(min, i);
            }
            int avg = (max + min) / 2;
            return avg;
        }

    }
}
