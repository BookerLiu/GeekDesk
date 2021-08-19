using System;
using System.Windows;

namespace GeekDesk.Util
{
    public class ShowWindowFollowMouse
    {

        public enum MousePosition
        {
            CENTER = 1,
            LEFT_TOP = 2,
            LEFT_BOTTOM = 3,
            RIGHT_TOP = 4,
            RIGHT_BOTTOM = 5,
            LEFT_CENTER = 6,
            RIGHT_CENTER = 7
        }

        /// <summary>
        /// 随鼠标位置显示面板 
        /// </summary>
        public static void Show(Window window, MousePosition position, double widthOffset = 0, double heightOffset = 0, bool visibility = true)
        {
            //获取鼠标位置
            System.Windows.Point p = MouseUtil.GetMousePosition();
            double left = SystemParameters.VirtualScreenLeft;
            double top = SystemParameters.VirtualScreenTop;
            double width = SystemParameters.VirtualScreenWidth;
            double height = SystemParameters.VirtualScreenHeight;
            double right = width - Math.Abs(left);
            double bottom = height - Math.Abs(top);

            double afterWidth;
            double afterHeight;

            switch (position)
            {

                case MousePosition.LEFT_BOTTOM:
                    afterWidth = 0;
                    afterHeight = window.Height;
                    break;
                case MousePosition.LEFT_TOP:
                    afterWidth = 0;
                    afterHeight = 0;
                    break;
                case MousePosition.LEFT_CENTER:
                    afterWidth = 0;
                    afterHeight = window.Height / 2;
                    break;
                case MousePosition.RIGHT_BOTTOM:
                    afterWidth = window.Width;
                    afterHeight = window.Height;
                    break;
                case MousePosition.RIGHT_TOP:
                    afterWidth = window.Width;
                    afterHeight = 0;
                    break;
                case MousePosition.RIGHT_CENTER:
                    afterWidth = window.Width;
                    afterHeight = window.Height / 2;
                    break;
                default:
                    afterWidth = window.Width / 2;
                    afterHeight = window.Height / 2;
                    break;
            }

            afterWidth += widthOffset;
            afterHeight -= heightOffset;

            if (p.X - afterWidth < left)
            {
                //判断是否在最左边缘
                window.Left = left;
            }
            else if (p.X + afterWidth > right)
            {
                //判断是否在最右边缘
                window.Left = right - window.Width;
            }
            else
            {
                window.Left = p.X - afterWidth;
            }


            if (p.Y - afterHeight < top)
            {
                //判断是否在最上边缘
                window.Top = top;
            }
            else if (p.Y + afterHeight > bottom)
            {
                //判断是否在最下边缘
                window.Top = bottom - window.Height;
            }
            else
            {
                window.Top = p.Y - afterHeight;
            }
            if (visibility)
            {
                window.Opacity = 0;
                window.Visibility = Visibility.Visible;
            }
        }

    }
}
