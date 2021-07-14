/*
* Virtual Router v1.0 - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 8, 7 and 2008 R2
* Copyright (c) 2013 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace GeekDesk.Util
{
    public static class AeroGlassHelper
    {
        #region "Generic Static Methods"

        public static void ExtendGlass(IntPtr windowHandle)
        {
            ExtendGlass(windowHandle, -1, -1, -1, -1);
        }

        public static void ExtendGlass(IntPtr windowHandle, int left, int right, int top, int bottom)
        {
            internalExtendGlass(windowHandle, left, right, top, bottom);
        }

        private static int internalExtendGlass(IntPtr windowHandle, int left, int right, int top, int bottom)
        {
            var retVal = -1; // Returning less than zero will indicate that Aero Glass could not be extended

            // Calculate the Aero Glass Margins
            Win32.Margins margins = Win32.GetDpiAdjustedMargins(windowHandle, left, right, top, bottom);

            try
            {
                // Actually Enable Aero Glass
                retVal = Win32.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
            }
            catch (Exception)
            {
                retVal = -1;
            }

            return retVal;
        }

        #endregion

        #region "WPF Static Methods"

        public static void ExtendGlass(Window win)
        {
            ExtendGlass(win, -1, -1, -1, -1);
        }

        public static void ExtendGlass(Window win, int left, int right, int top, int bottom)
        {
            Brush originalBackgroundBrush = win.Background;
            try
            {
                int retVal = -1;
                if (Win32.DwmIsCompositionEnabled())
                {
                    win.Background = Brushes.Transparent;

                    // Obtain the window handle for WPF application
                    WindowInteropHelper windowInterop = new WindowInteropHelper(win);
                    IntPtr windowHandle = windowInterop.Handle;

                    // Set the Window background to be Transparent so the Aero Glass will show through
                    HwndSource mainWindowSrc = HwndSource.FromHwnd(windowHandle);
                    mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                    retVal = internalExtendGlass(windowHandle, left, right, top, bottom);
                }
                if (retVal < 0)
                {
                    throw new NotSupportedException("Operation Failed: Aero Glass Not Supported");
                }
            }
            catch
            {
                // If an error occurred then change the Window.Background back to what it was
                win.Background = originalBackgroundBrush;
            }
        }

        #endregion

        //#region "Windows.Forms Static Methods"

        //public static void ExtendGlass(Form form)
        //{
        //    ExtendGlass(form, -1, -1, -1, -1);
        //}

        //public static void ExtendGlass(Form form, int left, int right, int top, int bottom)
        //{
        //    System.Drawing.Color oldBackColor = form.BackColor;
        //    System.Drawing.Color oldTransparencyKey = form.TransparencyKey;

        //    int retVal = -1;

        //    try
        //    {
        //        form.TransparencyKey = System.Drawing.Color.Beige;
        //        form.BackColor = form.TransparencyKey;

        //        retVal = internalExtendGlass(form.Handle, left, right, top, bottom);
        //    }
        //    catch (Exception)
        //    {
        //        retVal = -1;
        //    }

        //    if (retVal < 0)
        //    {
        //        form.BackColor = oldBackColor;
        //        form.TransparencyKey = oldTransparencyKey;
        //    }
        //}

        //#endregion

        #region "Win32 / pinvoke"

        private static class Win32
        {
            [DllImport("DwmApi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern bool DwmIsCompositionEnabled();


            [StructLayout(LayoutKind.Sequential)]
            public struct Margins
            {
                public int Left;    // width of left border that retains its size
                public int Right;   // width of right border that retains its size
                public int Top;     // height of top border that retains its size
                public int Bottom;  // height of bottom border that retains its size
            }

            public static Win32.Margins GetDpiAdjustedMargins(IntPtr windowHandle, int left, int right, int top, int bottom)
            {
                float DesktopDpiX;
                float DesktopDpiY;
                // Get System Dpi
                using (System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(windowHandle))
                {
                    DesktopDpiX = desktop.DpiX;
                    DesktopDpiY = desktop.DpiY;
                }

                // Set Margins
                Win32.Margins margins = new Win32.Margins();

                // Note that the default desktop Dpi is 96dpi. The  margins are
                // adjusted for the system Dpi.
                margins.Left = Convert.ToInt32(left * (DesktopDpiX / 96));
                margins.Right = Convert.ToInt32(right * (DesktopDpiX / 96));
                margins.Top = Convert.ToInt32(top * (DesktopDpiX / 96));
                margins.Bottom = Convert.ToInt32(bottom * (DesktopDpiX / 96));

                return margins;
            }
        }

        #endregion
    }
}