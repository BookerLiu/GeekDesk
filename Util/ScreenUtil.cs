using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace GeekDesk.Util
{
    public class ScreenUtil
    {

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        //取得前台窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        //取得桌面窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        //取得Shell窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        //取得窗口大小函数 
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);
        //获取窗口标题
        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowText(
            IntPtr hWnd,//窗口句柄
            StringBuilder lpString,//标题
            int nMaxCount //最大值
            );

        //获取类的名字
        [DllImport("user32.dll")]
        private static extern int GetClassName(
            IntPtr hWnd,//句柄
            StringBuilder lpString, //类名
            int nMaxCount); //最大值

        /// <summary>
        /// 判断当前屏幕(鼠标最后活动屏幕)是否有全屏化应用
        /// </summary>
        /// <returns></returns>
        public static bool IsPrimaryFullScreen()
        {

            //桌面窗口句柄 
            IntPtr desktopHandle; //Window handle for the desktop  
                                  //Shell窗口句柄 
            IntPtr shellHandle; //Window handle for the shell  因为桌面窗口和Shell窗口也是全屏，要排除在其他全屏程序之外。               //取得桌面和Shell窗口句柄 
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();    //取得前台窗口句柄并判断是否全屏 
            bool runningFullScreen = false;
            RECT appBounds;
            Rectangle screenBounds;
            IntPtr hWnd;
            //取得前台窗口 
            hWnd = GetForegroundWindow();

            StringBuilder sb = new StringBuilder(256);
            try
            {
                GetClassName(hWnd, sb, sb.Capacity);
            }
            catch { }
            if (sb.ToString().ToLower().Equals("workerw")) return false;
            if (hWnd != null && !hWnd.Equals(IntPtr.Zero))
            {
                //判断是否桌面或shell        
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                {
                    //取得窗口大小 
                    GetWindowRect(hWnd, out appBounds);
                    //判断是否全屏 
                    screenBounds = Screen.FromHandle(hWnd).Bounds;
                    if ((appBounds.bottom - appBounds.top) == screenBounds.Height
                        && (appBounds.right - appBounds.left) == screenBounds.Width)
                        runningFullScreen = true;
                }
            }
            return runningFullScreen;
        }


        public static Color GetColorAt(System.Drawing.Point location)
        {
            Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }


        [DllImport("gdi32")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public const int HORZRES = 8;
        public const int VERTRES = 10;
        public const int DESKTOPVERTRES = 117;
        public const int DESKTOPHORZRES = 118;
        /// <summary>
        /// 获取屏幕缩放比例
        /// </summary>
        /// <returns></returns>
        public static double GetScreenScalingFactor()
        {
            try
            {
                var g = Graphics.FromHwnd(IntPtr.Zero);
                IntPtr desktop = g.GetHdc();
                var physicalScreenHeight = GetDeviceCaps(desktop, (int)DESKTOPVERTRES);

                var screenScalingFactor =
                    (double)physicalScreenHeight / SystemParameters.PrimaryScreenHeight;
                //SystemParameters.PrimaryScreenHeight;

                return screenScalingFactor;
            } catch (Exception e)
            {
                return 1;
            }
            
        }

    }
}
