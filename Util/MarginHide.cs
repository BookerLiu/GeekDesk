using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing; //添加引用
using System.Windows.Forms;//添加引用
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace GeekDesk.Util
{

    enum HidePosition
    {
        TOP = 1,
        LEFT = 2,
        RIGHT = 3
    }

    public class MarginHide
    {
        readonly Window window;//定义使用该方法的窗体

        private readonly int hideTime = 150;

        private readonly int taskTime = 200;

        private bool isHide;

        public Timer timer;
        //构造函数，传入将要匹配的窗体      
        public MarginHide(Window window)
        {
            this.window = window;
        }


        /// <summary>
        /// 窗体是否贴边
        /// </summary>
        /// <returns></returns>
        public bool IsMargin()
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
        private void TimerDealy(object o, EventArgs e)
        {
            if (window.Visibility != Visibility.Visible) return;

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
            if (mouseX < windowLeft || mouseX > windowLeft + windowWidth
                || mouseY < windowTop || mouseY > windowTop + windowHeight)
            {
                //上方隐藏条件
                if (windowTop <= screenTop)
                {
                    HideAnimation(windowTop, screenTop - windowHeight + 1, Window.TopProperty);
                    isHide = true;
                    return;
                }
                //左侧隐藏条件
                if (windowLeft <= screenLeft)
                {
                    HideAnimation(windowLeft, screenLeft - windowWidth + 1, Window.LeftProperty);
                    return;
                }
                //右侧隐藏条件
                if (windowLeft + windowWidth + Math.Abs(screenLeft) >= screenWidth)
                {
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - 1, Window.LeftProperty);
                    return;
                }
            } else if (mouseX >= windowLeft && mouseX <= windowLeft + windowWidth 
                && mouseY >= windowTop && mouseY <= windowTop + windowHeight)
            {
                //上方显示
                if (windowTop <= screenTop - 1)
                {
                    HideAnimation(windowTop, screenTop, Window.TopProperty);
                    return;
                }
                //左侧显示
                if (windowLeft <= screenLeft -1)
                {
                    HideAnimation(windowLeft, screenLeft, Window.LeftProperty);
                    return;
                }
                //右侧显示
                if (windowLeft + Math.Abs(screenLeft) == screenWidth -1)
                {
                    HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty);
                    return;
                }
            }

        }
        #endregion 


        public void TimerSet()
        {
            timer = new Timer();//添加timer计时器，隐藏功能
            #region 计时器设置，隐藏功能
            timer.Interval = taskTime;
            timer.Tick += TimerDealy;
            timer.Start();
            #endregion
        }

        public void TimerStop()
        {
            timer.Stop();
            //功能关闭 如果界面是隐藏状态  那么要显示界面 ↓

            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;

            double windowWidth = window.Width;

            double windowTop = window.Top;
            double windowLeft = window.Left;

            //左侧显示
            if (windowLeft <= screenLeft - 1)
            {
                HideAnimation(windowLeft, screenLeft, Window.LeftProperty);
                return;
            }

            //上方显示
            if (windowTop <= screenTop - 1)
            {
                HideAnimation(windowTop, screenTop, Window.TopProperty);
                return;
            }

            //右侧显示
            if (windowLeft + Math.Abs(screenLeft) == screenWidth - 1)
            {
                HideAnimation(windowLeft, screenWidth - Math.Abs(screenLeft) - windowWidth, Window.LeftProperty);
                return;
            }
        }


        private void HideAnimation(double from, double to, DependencyProperty property)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(hideTime))
            };
            da.Completed += (s, e) =>
            {
                window.BeginAnimation(property, null);
            };
            Timeline.SetDesiredFrameRate(da, 30);
            window.BeginAnimation(property, da);
        }
    }
}

