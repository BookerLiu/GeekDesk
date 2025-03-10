using GeekDesk.Constant;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static GeekDesk.Util.ShowWindowFollowMouse;

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

        public static void FollowMouse(Window window)
        {
            // Get the mouse position
            var mousePosition = System.Windows.Forms.Control.MousePosition;

            // Get the window size
            var windowWidth = window.Width;
            var windowHeight = window.Height;

            Console.WriteLine("windowWidth +  windowHeight:" + windowWidth + "+" +  windowHeight);

            // Get the screen where the mouse is located
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(mousePosition.X, mousePosition.Y));
            var workingArea = screen.WorkingArea;

            // Get the DPI scaling factor for the screen
            //float dpiX, dpiY;
            //using (var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            //{
            //    dpiX = graphics.DpiX / 96f; // 96 is the default DPI
            //    dpiY = graphics.DpiY / 96f; // 96 is the default DPI
            //}

            float dpiX = GetDpi( true);
            float dpiY = GetDpi(false);

            // Convert mouse position to logical pixels based on DPI
            double mouseX = mousePosition.X / dpiX;
            double mouseY = mousePosition.Y / dpiY;

            // Calculate target position to center the window on the mouse
            double targetLeft = mouseX - windowWidth / 2;
            double targetTop = mouseY - windowHeight / 2;

            // Ensure the window does not exceed the screen boundaries
            if (targetLeft < workingArea.Left / dpiX)
                targetLeft = workingArea.Left / dpiX;
            if (targetLeft + windowWidth / dpiX > workingArea.Right / dpiX)
                targetLeft = workingArea.Right / dpiX - windowWidth / dpiX;

            if (targetTop < workingArea.Top / dpiY)
                targetTop = workingArea.Top / dpiY;
            if (targetTop + windowHeight / dpiY > workingArea.Bottom / dpiY)
                targetTop = workingArea.Bottom / dpiY - windowHeight / dpiY;

            // Update window position
            window.Left = targetLeft * dpiX;
            window.Top = targetTop * dpiY;
        }

        private static float GetDpi(bool isX)
        {
            IntPtr hdc = WindowUtil.GetDC(IntPtr.Zero);
            int dpi = isX ? WindowUtil.GetDeviceCaps(hdc, LOGPIXELSX) : WindowUtil.GetDeviceCaps(hdc, LOGPIXELSY);
            WindowUtil.ReleaseDC(IntPtr.Zero, hdc);
            return dpi / 96f;
        }

        private static IntPtr GetScreenHandleFromMouse()
        {
            // Get the mouse position
            var mousePosition = System.Windows.Forms.Control.MousePosition;

            // Convert mouse position to a POINT structure
            System.Drawing.Point point = new System.Drawing.Point(mousePosition.X, mousePosition.Y);

            // Get the window handle from the point
            IntPtr windowHandle = WindowUtil.WindowFromPoint(point);

            return windowHandle;
        }

        // Constants for DPI
        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

       

        /// <summary>
        /// 随鼠标位置显示面板  后面三个参数暂时没有使用
        /// </summary>
        public static void Show(Window window, MousePosition position, double widthOffset = 0, double heightOffset = 0)
        {
            //获取鼠标位置
            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

            // 获取鼠标所在的屏幕
            Screen currentScreen = Screen.FromPoint(p);
            float dpiX = GetDpi(true);
            float dpiY = GetDpi(false);

            p.X = (int)(p.X / dpiX);
            p.Y = (int)(p.Y / dpiY);

            //工作区宽度
            double screenWidth = currentScreen.WorkingArea.Width / dpiX;
            //工作区高度
            double screenHeight = currentScreen.WorkingArea.Height / dpiY;
            double screenX = currentScreen.WorkingArea.X / dpiX;
            double screenY = currentScreen.WorkingArea.Y / dpiY;

            //判断是否超出最左边缘
            if (p.X - window.Width / 2 < screenX)
            {
                //超出最左边缘
                window.Left = screenX - Constants.SHADOW_WIDTH;
            } else
            {
                window.Left = p.X - window.Width / 2 + Constants.SHADOW_WIDTH;
            }

            //判断是否超出最右边缘
            if (p.X + window.Width / 2 > screenWidth + screenX)
            {
                //超出最右边缘
                window.Left = screenWidth + screenX - window.Width + Constants.SHADOW_WIDTH;
            }
            

            //判断是否超出最上边缘
            if (p.Y - window.Height / 2 < screenY)
            {
                //超出最上边缘
                window.Top = screenY - Constants.SHADOW_WIDTH;
            } else
            {
                window.Top = p.Y - window.Height / 2 + Constants.SHADOW_WIDTH;
            }

            //判断是否超出最下边缘
            if (p.Y + window.Height / 2 > screenHeight + screenY)
            {
                //超出最下边缘
                window.Top = screenHeight + screenY - window.Height + Constants.SHADOW_WIDTH;
            }
            





            //// 显示屏幕信息
            //Console.WriteLine("鼠标当前所在屏幕的信息:");
            //Console.WriteLine($"屏幕设备名称: {currentScreen.DeviceName}");
            //Console.WriteLine($"屏幕分辨率: {currentScreen.Bounds.Width}x{currentScreen.Bounds.Height}");
            //Console.WriteLine($"屏幕工作区: {currentScreen.WorkingArea}");
            //Console.WriteLine($"主屏幕: {currentScreen.Primary}");

            //Console.WriteLine(p.X + "=" + p.Y);
            //float dpiX = GetDpi(true);
            //float dpiY = GetDpi(false);
            //p.X = (int)(p.X / dpiX);
            //p.Y = (int)(p.Y / dpiY);
            //Console.WriteLine(p.X + "=" + p.Y);
            //double left = SystemParameters.VirtualScreenLeft;
            //double top = SystemParameters.VirtualScreenTop;
            //double width = SystemParameters.VirtualScreenWidth;
            //double height = SystemParameters.WorkArea.Bottom;

            //Console.WriteLine("VirtualScreenTop:" + SystemParameters.VirtualScreenTop);
            //Console.WriteLine("VirtualScreenLeft:" + SystemParameters.VirtualScreenLeft);
            //Console.WriteLine("VirtualScreenWidth:" + SystemParameters.VirtualScreenWidth);
            //Console.WriteLine("VirtualScreenHeight:" + SystemParameters.VirtualScreenHeight);
            //Console.WriteLine("WorkArea.Bottom:" + SystemParameters.WorkArea.Bottom);
            //Console.WriteLine(" window.Height:" + window.Height);
            //double right = width - Math.Abs(left);
            //double bottom = height - Math.Abs(top);

            //double afterWidth;
            //double afterHeight;

            //switch (position)
            //{

            //    case MousePosition.LEFT_BOTTOM:
            //        afterWidth = 0;
            //        afterHeight = window.Height;
            //        break;
            //    case MousePosition.LEFT_TOP:
            //        afterWidth = 0;
            //        afterHeight = 0;
            //        break;
            //    case MousePosition.LEFT_CENTER:
            //        afterWidth = 0;
            //        afterHeight = window.Height / 2;
            //        break;
            //    case MousePosition.RIGHT_BOTTOM:
            //        afterWidth = window.Width;
            //        afterHeight = window.Height;
            //        break;
            //    case MousePosition.RIGHT_TOP:
            //        afterWidth = window.Width;
            //        afterHeight = 0;
            //        break;
            //    case MousePosition.RIGHT_CENTER:
            //        afterWidth = window.Width;
            //        afterHeight = window.Height / 2;
            //        break;
            //    default:
            //        afterWidth = window.Width / 2;
            //        afterHeight = window.Height / 2;
            //        break;
            //}

            //afterWidth += widthOffset;
            //afterHeight -= heightOffset;

            //if (p.X - afterWidth < left)
            //{
            //    //判断是否在最左边缘
            //    window.Left = left - Constants.SHADOW_WIDTH;
            //}
            //else if (p.X + afterWidth > right)
            //{
            //    //判断是否在最右边缘
            //    window.Left = right - window.Width + Constants.SHADOW_WIDTH;
            //}
            //else
            //{
            //    window.Left = p.X - afterWidth;
            //}


            //if (p.Y - afterHeight < top)
            //{
            //    //判断是否在最上边缘
            //    window.Top = top - Constants.SHADOW_WIDTH;
            //}
            //else if (p.Y + afterHeight > bottom)
            //{
            //    Console.WriteLine("p.Y:" + p.Y);
            //    Console.WriteLine("afterHeight:" + afterHeight);
            //    Console.WriteLine("bottom:" + bottom);
            //    //判断是否在最下边缘
            //    window.Top = bottom - window.Height + Constants.SHADOW_WIDTH;
            //}
            //else
            //{
            //    window.Top = p.Y - afterHeight;
            //}
        }

    }
}
