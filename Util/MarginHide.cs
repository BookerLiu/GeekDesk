using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

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

        private static readonly int hideTime = 200;
        private static readonly int fadeHideTime = 180;
        private static readonly int fadeShowTime = 200;
        private static readonly int taskTime = 250;

        private static double showMarginWidth = 1;

        public static bool IS_RUN = false;
        public static bool IS_HIDE = false;

        private static Timer timer = null;

        public static void ReadyHide(Window window)
        {
            MarginHide.window = window;
            if (timer != null) return;
            timer = new Timer();//添加timer计时器，隐藏功能
            timer.Interval = taskTime;
            timer.Tick += HideWindow;
            timer.Start();
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
            if (window.Visibility != Visibility.Visible || !IS_RUN) return;

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
                    FadeAnimation(1, 0);
                    HideAnimation(windowTop, screenTop - windowHeight + showMarginWidth, Window.TopProperty, HideType.TOP_HIDE);
                    return;
                }
                //左侧隐藏条件
                if (windowLeft <= screenLeft)
                {
                    IS_HIDE = true;
                    FadeAnimation(1, 0);
                    HideAnimation(windowLeft, screenLeft - windowWidth + showMarginWidth, Window.LeftProperty, HideType.LEFT_HIDE);
                    return;
                }
                //右侧隐藏条件
                if (windowLeft + windowWidth + Math.Abs(screenLeft) >= screenWidth)
                {
                    IS_HIDE = true;
                    FadeAnimation(1, 0);
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
                    FadeAnimation(0, 1);
                    return;
                }
                //左侧显示
                if (windowLeft <= screenLeft - showMarginWidth)
                {
                    IS_HIDE = false;
                    HideAnimation(windowLeft, screenLeft, Window.LeftProperty, HideType.LEFT_SHOW);
                    FadeAnimation(0, 1);
                    return;
                }
                //右侧显示
                if (windowLeft + Math.Abs(screenLeft) == screenWidth - showMarginWidth)
                {
                    IS_HIDE = false;
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty, HideType.RIGHT_SHOW);
                    FadeAnimation(0, 1);
                    return;
                }
            }

        }
        #endregion 


        public static void StartHide()
        {
            IS_RUN = true;
        }

        public static void StopHide()
        {
            IS_RUN = false;
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
                    FadeAnimation(0, 1);
                    HideAnimation(windowLeft, screenLeft, Window.LeftProperty, HideType.LEFT_SHOW);
                    return;
                }

                //上方显示
                if (windowTop <= screenTop - showMarginWidth)
                {
                    IS_HIDE = false;
                    FadeAnimation(0, 1);
                    HideAnimation(windowTop, screenTop, Window.TopProperty, HideType.TOP_SHOW);
                    return;
                }

                //右侧显示
                if (windowLeft + Math.Abs(screenLeft) == screenWidth - showMarginWidth)
                {
                    IS_HIDE = false;
                    FadeAnimation(0, 1);
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty, HideType.RIGHT_SHOW);
                    return;
                }
            }
        }


        private static void HideAnimation(double from, double to, DependencyProperty property, HideType hideType)
        {

            double toTemp = to;
            double leftT = window.Width / 4 * 3;
            double topT = window.Height / 4 * 3;
            switch (hideType)
            {
                case HideType.LEFT_HIDE:
                    to += leftT;
                    break;
                case HideType.LEFT_SHOW:
                    to -= leftT;
                    break;
                case HideType.RIGHT_HIDE:
                    to -= leftT;
                    break;
                case HideType.RIGHT_SHOW:
                    to += leftT;
                    break;
                case HideType.TOP_HIDE:
                    to += topT;
                    break;
                case HideType.TOP_SHOW:
                    to -= topT;
                    break;
            }
            DoubleAnimation da = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(hideTime))
            };
            da.Completed += (s, e) =>
            {
                if ("Top".Equals(property.Name))
                {
                    window.Top = toTemp;
                }
                else
                {
                    window.Left = toTemp;
                }
                window.BeginAnimation(property, null);
            };

            Timeline.SetDesiredFrameRate(da, 60);
            window.BeginAnimation(property, da);
        }

        private static void FadeAnimation(double from, double to)
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
        }


    }
}

