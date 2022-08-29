using GeekDesk;
using GeekDesk.MyThread;
using GeekDesk.Util;
using ShowSeconds.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShowSeconds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondsWindow : Window
    {

        private static Color beforeColor;
        private static Color topBeforeColor;

        //dark theam
        private readonly static System.Windows.Media.SolidColorBrush darkBG
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(46, 50, 54),
                Opacity = 0.8
            };
        private readonly static System.Windows.Media.SolidColorBrush darkFont
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(255, 255, 255)
            };

        //light theam
        private readonly static System.Windows.Media.SolidColorBrush lightBG
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(236, 244, 251),
                Opacity = 1
            };
        private readonly static System.Windows.Media.SolidColorBrush lightFont
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(65, 63, 61),
            };

        private static bool expandClock = true; //是否展开时钟
        private static System.Windows.Forms.Timer timer;
        private static double lProportion = 0.82;
        private static double tProportion = 0.03;
        private static int sleepTime = 800;
        public SecondsWindow()
        {
            SecondsDataContext dc = new SecondsDataContext
            {
                Seconds = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second)
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
                sleepTime = 800;
            }

            InitializeComponent();
            this.DataContext = dc;
            this.Topmost = true;
            BGBorder.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;         
            HideWindowUtil.HideAltTab(this);
        }

        public static void SecondsHookSetFuc(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                window.Dispatcher.Invoke(() =>
                {
                    if (ScreenUtil.IsPrimaryFullScreen()) return;

                    App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                    {
                        int x = e.X;
                        int y = e.Y;

                        //获取实际坐标  windows可能会有缩放
                        IntPtr hdc = GetDC(IntPtr.Zero);
                        double scale = ScreenUtil.GetScreenScalingFactor();

                        x = (int)(x / scale);
                        y = (int)(y / scale);

                        double w = 1920;
                        double h = 1080;
                        double width = SystemParameters.PrimaryScreenWidth;
                        double height = SystemParameters.PrimaryScreenHeight;

                        if (x > 1843 / w * width
                            && x < 1907 / w * width
                            && y > 1037 / h * height
                            && y < 1074 / h * height)
                        {
                            Color c;
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
                                Color ct = GetTopBeforeColor();
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

                                if (!window.BGBorder.IsVisible)
                                {
                                    Color theamColor = GetColor(1919, 1079);
                                    if (CalculateLight(theamColor) > 255 / 2)
                                    {
                                        //light
                                        window.BGBorder.Background = lightBG;
                                        window.SecondsText.Foreground = lightFont;
                                    }
                                    else
                                    {
                                        // dark
                                        window.BGBorder.Background = darkBG;
                                        window.SecondsText.Foreground = darkFont;
                                    }

                                    SecondsDataContext dc = window.DataContext as SecondsDataContext;
                                    dc.Seconds = (DateTime.Now.Hour).ToString() + ":" +
                                        FormatMS(DateTime.Now.Minute) + ":" +
                                        FormatMS(DateTime.Now.Second);

                                    int sx = (int)(width * lProportion);
                                    int sMarginBottom = (int)(height * tProportion);
                                    window.Left = sx - window.Width;
                                    window.Top = SystemParameters.WorkArea.Height - window.Height;
                                    window.BGBorder.Visibility = Visibility.Visible;
                                    timer.Start();
                                }
                                else
                                {
                                    window.BGBorder.Visibility = Visibility.Collapsed;
                                    timer.Stop();
                                }
                            }
                        }
                        else if ((expandClock && (x < 1574 / w * width
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
                            window.BGBorder.Visibility = Visibility.Collapsed;
                            timer.Stop();
                        }
                    }));
                });
            });
           
        }

        public static void SecondsBakColorFun(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                window.Dispatcher.Invoke(() =>
                {
                    beforeColor = GetBottomBeforeColor();
                    topBeforeColor = GetTopBeforeColor();
                });

            });
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            string str = (DateTime.Now.Hour).ToString() + ":" +
                        FormatMS(DateTime.Now.Minute) + ":" +
                        FormatMS(DateTime.Now.Second);
            SecondsDataContext dc = this.DataContext as SecondsDataContext;
            dc.Seconds = str;

            this.DataContext = null;
            this.DataContext = dc;
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
            double scale = ScreenUtil.GetScreenScalingFactor();

            System.Drawing.Point p = new System.Drawing.Point((int)(w2 / w * width * scale), (int)(h2 / h * height * scale));
            return ScreenUtil.GetColorAt(p);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MessageUtil.WM_COPYDATA)
            {
                MessageUtil.CopyDataStruct cds = (MessageUtil.CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(MessageUtil.CopyDataStruct));
                
                if ("Shutdown".Equals(cds.msg))
                {
                    Application.Current.Shutdown();
                }
            }
            return hwnd;
        }


        private static int CalculateLight(Color color)
        {
            int[] colorArr = new int[] {color.R, color.G, color.B};

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

        private static SecondsWindow window = null;
        public static void ShowWindow()
        {
            if (window == null || !window.Activate())
            {
                window = new SecondsWindow();
            }
            window.Show();
            Keyboard.Focus(window);
        }


        //#######################################################
     

        


        /// <summary>
        /// 该函数检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄，
        /// 以后可以在GDI函数中使用该句柄来在设备上下文环境中绘图。
        /// </summary>
        /// <param name="hWnd">设备上下文环境被检索的窗口的句柄，如果该值为NULL，GetDC则检索整个屏幕的设备上下文环境。</param>
        /// <returns>如果成功，返回指定窗口客户区的设备上下文环境；如果失败，返回值为Null。</returns>
        [DllImport("user32")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        /// <summary>
        /// 该函数释放设备上下文环境（DC）供其他应用程序使用。函数的效果与设备上下文环境类型有关。
        /// 它只释放公用的和设备上下文环境，对于类或私有的则无效。
        /// </summary>
        /// <param name="hWnd">指向要释放的设备上下文环境所在的窗口的句柄。</param>
        /// <param name="hDC">指向要释放的设备上下文环境的句柄。</param>
        /// <returns>如果释放成功，则返回值为1；如果没有释放成功，则返回值为0。</returns>
        [DllImport("user32")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32")]
        public static extern bool GetCursorPos(out System.Drawing.Point pt);

    }
}
