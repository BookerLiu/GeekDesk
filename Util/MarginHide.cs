using GeekDesk.Constant;
using GeekDesk.MyThread;
using System;
using System.Threading;
using System.Windows;

using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GeekDesk.Util
{

    enum HideType
    {
        TOP_SHOW = 1,
        LEFT_SHOW = 2,
        RIGHT_SHOW = 3,
        TOP_HIDE = 4,
        LEFT_HIDE = 5,
        RIGHT_HIDE = 6
    }


    public class MarginHide
    {
        private static Window window;//定义使用该方法的窗体

        private static readonly int hideTime = 50;
        private static readonly int showTime = 30;

        private static int animalTime;

        private static readonly int fadeHideTime = 50;
        private static readonly int fadeShowTime = 50;
        private static readonly int taskTime = 200;

        public static readonly int shadowWidth = 20;

        public static bool ON_HIDE = false;


        private static double showMarginWidth = 1;

        public static bool IS_HIDE = false;

        private static System.Windows.Forms.Timer timer = null;

        public static void ReadyHide(Window window)
        {
            MarginHide.window = window;
        }


        /// <summary>
        /// 窗体是否贴边
        /// </summary>
        /// <returns></returns>
        public static bool IsMargin()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;

            double windowWidth = window.Width;

            double windowTop = window.Top;
            double windowLeft = window.Left;

            //窗体是否贴边
            return (windowLeft <= screenLeft
                || windowTop <= screenTop
                || windowLeft + windowWidth + Math.Abs(screenLeft) >= screenWidth);
        }



        #region 窗体贴边隐藏功能
        private static void HideWindow(object o, EventArgs e)
        {
            if (window.Visibility != Visibility.Visible
                || RunTimeStatus.MARGIN_HIDE_AND_OTHER_SHOW) return;

            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;

            double windowHeight = window.Height;
            double windowWidth = window.Width;

            double windowTop = window.Top;
            double windowLeft = window.Left;

            //获取鼠标位置
            System.Windows.Point p = MouseUtil.GetMousePosition();
            double mouseX = p.X;
            double mouseY = p.Y;

            //鼠标不在窗口上
            if ((mouseX < windowLeft || mouseX > windowLeft + windowWidth
                || mouseY < windowTop || mouseY > windowTop + windowHeight) && !IS_HIDE)
            {
                //上方隐藏条件
                if (windowTop <= screenTop)
                {
                    IS_HIDE = true;
                    //FadeAnimation(1, 0);
                    HideAnimation(windowTop, screenTop - windowHeight + showMarginWidth, Window.TopProperty, HideType.TOP_HIDE);
                    return;
                }
                //左侧隐藏条件
                if (windowLeft <= screenLeft)
                {
                    IS_HIDE = true;
                    //FadeAnimation(1, 0);
                    HideAnimation(windowLeft, screenLeft - windowWidth + showMarginWidth, Window.LeftProperty, HideType.LEFT_HIDE);
                    return;
                }
                //右侧隐藏条件
                if (windowLeft + windowWidth + Math.Abs(screenLeft) >= screenWidth)
                {
                    IS_HIDE = true;
                    //FadeAnimation(1, 0);
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - showMarginWidth, Window.LeftProperty, HideType.RIGHT_HIDE);
                    return;
                }
            }
            else if (mouseX >= windowLeft && mouseX <= windowLeft + windowWidth
              && mouseY >= windowTop && mouseY <= windowTop + windowHeight && IS_HIDE)
            {
                //上方显示
                if (windowTop <= screenTop - showMarginWidth)
                {
                    IS_HIDE = false;
                    HideAnimation(windowTop, screenTop, Window.TopProperty, HideType.TOP_SHOW);
                    //FadeAnimation(0, 1);
                    return;
                }
                //左侧显示
                if (windowLeft <= screenLeft - showMarginWidth)
                {
                    IS_HIDE = false;
                    HideAnimation(windowLeft, screenLeft, Window.LeftProperty, HideType.LEFT_SHOW);
                    //FadeAnimation(0, 1);
                    return;
                }
                //右侧显示
                if (windowLeft + Math.Abs(screenLeft) == screenWidth - showMarginWidth)
                {
                    IS_HIDE = false;
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty, HideType.RIGHT_SHOW);
                    //FadeAnimation(0, 1);
                    return;
                }
            }

        }
        #endregion 


        public static void StartHide()
        {
            ON_HIDE = true;
            if (timer != null) return;
            timer = new System.Windows.Forms.Timer
            {
                Interval = taskTime
            };//添加timer计时器，隐藏功能
            timer.Tick += HideWindow;
            timer.Start();
        }

        public static void StopHide()
        {
            ON_HIDE = false;
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
            timer = null;
            //功能关闭 如果界面是隐藏状态  那么要显示界面 ↓
            if (IS_HIDE)
            {
                double screenLeft = SystemParameters.VirtualScreenLeft;
                double screenTop = SystemParameters.VirtualScreenTop;
                double screenWidth = SystemParameters.VirtualScreenWidth;

                double windowWidth = window.Width;

                double windowTop = window.Top;
                double windowLeft = window.Left;

                //左侧显示
                if (windowLeft <= screenLeft - showMarginWidth)
                {
                    IS_HIDE = false;
                    //FadeAnimation(0, 1);
                    HideAnimation(windowLeft, screenLeft, Window.LeftProperty, HideType.LEFT_SHOW);
                    return;
                }

                //上方显示
                if (windowTop <= screenTop - showMarginWidth)
                {
                    IS_HIDE = false;
                    //FadeAnimation(0, 1);
                    HideAnimation(windowTop, screenTop, Window.TopProperty, HideType.TOP_SHOW);
                    return;
                }

                //右侧显示
                if (windowLeft + Math.Abs(screenLeft) == screenWidth - showMarginWidth)
                {
                    IS_HIDE = false;
                    //FadeAnimation(0, 1);
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty, HideType.RIGHT_SHOW);
                    return;
                }
            }
        }


        private static void HideAnimation(double from, double to, DependencyProperty property, HideType hideType)
        {
            new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    double abs = Math.Abs(Math.Abs(to) - Math.Abs(from));
                    double subLen = abs / hideTime;

                    if ((int)hideType <= 3)
                    {
                        animalTime = showTime;
                    } else
                    {
                        animalTime = hideTime;
                    }

                    int count = 0;
                    while (count < animalTime)
                    {
                        switch (hideType)
                        {
                            case HideType.LEFT_HIDE:
                                window.Left -= subLen;
                                break;
                            case HideType.LEFT_SHOW:
                                window.Left += subLen;
                                break;
                            case HideType.RIGHT_HIDE:
                                window.Left += subLen;
                                break;
                            case HideType.RIGHT_SHOW:
                                window.Left -= subLen;
                                break;
                            case HideType.TOP_HIDE:
                                window.Top -= subLen;
                                break;
                            case HideType.TOP_SHOW:
                                window.Top += subLen;
                                break;
                        }
                        count++;
                        Thread.Sleep(1);
                    }

                    switch (hideType)
                    {
                        case HideType.LEFT_HIDE:
                            window.Left = to;
                            break;
                        case HideType.LEFT_SHOW:
                            window.Left = to - 20;
                            break;
                        case HideType.RIGHT_HIDE:
                            window.Left = to;
                            break;
                        case HideType.RIGHT_SHOW:
                            window.Left = to + 20;
                            break;
                        case HideType.TOP_HIDE:
                            window.Top = to;
                            break;
                        case HideType.TOP_SHOW:
                            window.Top = to - 20;
                            break;
                    }

                    //double toTemp = to;
                    //double leftT = 0;
                    //double topT = 0;
                    //switch (hideType)
                    //{
                    //    case HideType.LEFT_HIDE:
                    //        to += leftT;
                    //        break;
                    //    case HideType.LEFT_SHOW:
                    //        to -= leftT;
                    //        break;
                    //    case HideType.RIGHT_HIDE:
                    //        to -= leftT;
                    //        break;
                    //    case HideType.RIGHT_SHOW:
                    //        to += leftT;
                    //        break;
                    //    case HideType.TOP_HIDE:
                    //        to += topT;
                    //        break;
                    //    case HideType.TOP_SHOW:
                    //        to -= topT;
                    //        break;
                    //}
                    //DoubleAnimation da = new DoubleAnimation
                    //{
                    //    From = from,
                    //    To = to,
                    //    Duration = new Duration(TimeSpan.FromMilliseconds(hideTime))
                    //};
                    //// 如果是显示 则贴屏幕侧不显示阴影
                    //bool isShow = false;
                    //int shadowWidthTemp = Constants.SHADOW_WIDTH;
                    //if (hideType <= HideType.RIGHT_SHOW)
                    //{
                    //    isShow = true;
                    //    if (hideType == HideType.RIGHT_SHOW)
                    //    {
                    //        shadowWidthTemp = -shadowWidthTemp;
                    //    }
                    //}
                    //da.Completed += (s, e) =>
                    //{
                    //    if ("Top".Equals(property.Name))
                    //    {
                    //        window.Top = isShow ? toTemp - shadowWidthTemp : toTemp;
                    //    }
                    //    else
                    //    {
                    //        window.Left = isShow ? toTemp - shadowWidthTemp : toTemp;
                    //    }
                    //    window.BeginAnimation(property, null);
                    //};

                    //Timeline.SetDesiredFrameRate(da, 60);
                    //window.BeginAnimation(property, da);
                });
            }).Start();
            
            
        }

        private static void FadeAnimation(double from, double to)
        {
            new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    double time;
                    if (to == 0D)
                    {
                        time = fadeHideTime;
                    }
                    else
                    {
                        time = fadeShowTime;
                    }
                    DoubleAnimation opacityAnimation = new DoubleAnimation
                    {
                        From = from,
                        To = to,
                        Duration = new Duration(TimeSpan.FromMilliseconds(time))
                    };
                    opacityAnimation.Completed += (s, e) =>
                    {
                        //window.Opacity = to;
                        window.BeginAnimation(Window.OpacityProperty, null);
                    };
                    Timeline.SetDesiredFrameRate(opacityAnimation, 60);
                    window.BeginAnimation(Window.OpacityProperty, opacityAnimation);
                });
            }).Start();
        }


        public static void WaitHide(int waitTime)
        {
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(waitTime);
                //修改状态为false 继续执行贴边隐藏
                RunTimeStatus.MARGIN_HIDE_AND_OTHER_SHOW = false;
            });
            t.IsBackground = true;
            t.Start();
        }





    }
}

