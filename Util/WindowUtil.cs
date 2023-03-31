using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;

namespace GeekDesk.Util
{
    public class WindowUtil
    {

        public enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [Flags]
        public enum SetWindowPosFlags
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400
        }

        //取得前台窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        //取得桌面窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetWindow(HandleRef hWnd, int nCmd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr child, IntPtr parent);
        [DllImport("user32.dll", EntryPoint = "GetDCEx", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr window, IntPtr handle);



        public static void SetOwner(Window source, IntPtr parentHandle)
        {
            WindowInteropHelper helper = new WindowInteropHelper(source);
            helper.Owner = parentHandle;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool WindowIsTop(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            IntPtr deskHandle = GetDesktopHandle(window, DesktopLayer.Progman);
            IntPtr deskHandle2 = GetDesktopHandle(window, DesktopLayer.FolderView);
            IntPtr deskHandle3 = GetDesktopHandle(window, DesktopLayer.SHELLDLL);
            IntPtr topHandle = GetForegroundWindow();
            return (topHandle.Equals(handle) || topHandle.Equals(deskHandle));
        }




        public const int GW_CHILD = 5;
   
        public static IntPtr GetDesktopHandle(Window window, DesktopLayer layer)
        {
            HandleRef hWnd;
            IntPtr hDesktop = new IntPtr();
            switch (layer)
            {
                case DesktopLayer.Progman:
                    hDesktop = FindWindow("Progman", null);//第一层桌面  
                    break;
                case DesktopLayer.SHELLDLL:
                    hDesktop = FindWindow("Progman", null);//第一层桌面    
                    hWnd = new HandleRef(window, hDesktop);
                    hDesktop = GetWindow(hWnd, GW_CHILD);//第2层桌面     
                    break;
                case DesktopLayer.FolderView:
                    hDesktop = FindWindow("Progman", null);//第一层桌面    
                    hWnd = new HandleRef(window, hDesktop);
                    hDesktop = GetWindow(hWnd, GW_CHILD);//第2层桌面      
                    hWnd = new HandleRef(window, hDesktop);
                    hDesktop = GetWindow(hWnd, GW_CHILD);//第3层桌面    
                    break;
            }
            return hDesktop;
        }

        public void EmbedDesktop(Object embeddedWindow, IntPtr childWindow, IntPtr parentWindow)
        {
            Form window = (Form)embeddedWindow;
            HandleRef HWND_BOTTOM = new HandleRef(embeddedWindow, new IntPtr(1));
            const int SWP_FRAMECHANGED = 0x0020;//发送窗口大小改变消息
            SetParent(childWindow, parentWindow);
            SetWindowPos(new HandleRef(window, childWindow), HWND_BOTTOM, 300, 300, window.Width, window.Height, SWP_FRAMECHANGED);
        }
    }

    public enum DesktopLayer
    {
        Progman = 0,
        SHELLDLL = 1,
        FolderView = 2
    }







}
