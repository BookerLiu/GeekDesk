using GeekDesk.Interface;
using GeekDesk.Util;
using HandyControl.Controls;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// ColorPickerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PixelColorPickerWindow : IWindowCommon
    {
        private static int PIXEL_REC_LENGTH = 20;

        private static readonly int MIN_LENGTH = 10;
        private static readonly int MAX_LENGTH = 50;

        private static System.Drawing.Bitmap bgBitmap;

        private readonly ColorPicker colorPicker;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public PixelColorPickerWindow(ColorPicker colorPicker)
        {
            InitializeComponent();
            this.colorPicker = colorPicker;

            try
            {
                SetProcessDPIAware();
            }
            catch (Exception e) { }
            ColorPickerWindow_Init();
        }

        private void ColorPickerWindow_Init()
        {

            this.WindowState = WindowState.Normal;//还原窗口（非最小化和最大化）

            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;

            this.Left = SystemParameters.VirtualScreenLeft;
            this.Top = SystemParameters.VirtualScreenTop;

            DesktopBG.Width = this.Width;
            DesktopBG.Height = this.Height;
            this.Topmost = true;

            //获取缩放比例
            double scale = ScreenUtil.GetScreenScalingFactor();

            bgBitmap = new System.Drawing.Bitmap(
                    (int)(Width * scale),
                    (int)(Height * scale),
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bgBitmap))
            {
                g.CopyFromScreen(
                    0,
                    0,
                    0,
                    0,
                    bgBitmap.Size
                    );
            }
            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                                         bgBitmap.GetHbitmap(),
                                        IntPtr.Zero,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions()
                                    );

            DesktopBG.Source = bs;
            VisualBrush b = (VisualBrush)PixelBG.Fill;
            b.Visual = DesktopBG;
            Mouse.OverrideCursor = Cursors.Cross;
            SetPixelAbout(null);
        }


        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DataContext = null;
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;
            Point pos = e.MouseDevice.GetPosition(DesktopBG);
            System.Drawing.Color colorD = bgBitmap.GetPixel((int)pos.X, (int)pos.Y);
            colorPicker.SelectedBrush = new SolidColorBrush(Color.FromArgb(colorD.A, colorD.R, colorD.G, colorD.B));
            DeleteObject(bgBitmap.GetHbitmap());
            this.Close();
            ClickColorPickerToggleButton(colorPicker);
        }

        public void ClickColorPickerToggleButton(ColorPicker colorPicker)
        {
            const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type type = colorPicker.GetType();
            FieldInfo fi = type.GetField("_toggleButtonDropper", InstanceBindFlags);

            ToggleButton tb = (ToggleButton)fi.GetValue(colorPicker);
            if (tb != null && tb.IsChecked == true)
            {
                tb.IsChecked = false;
                MethodInfo mi = type.GetMethod("ToggleButtonDropper_Click", InstanceBindFlags);
                mi.Invoke(colorPicker, new object[] { null, null });
            }
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            SetPixelAbout(e);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);

        private void SetPixelAbout(MouseEventArgs e)
        {
            VisualBrush b = (VisualBrush)PixelBG.Fill;

            Point pos;
            if (e == null)
            {
                pos = MouseUtil.GetMousePosition();
            }
            else
            {
                pos = e.MouseDevice.GetPosition(DesktopBG);
            }
            Rect viewBox = b.Viewbox;

            viewBox.Width = PIXEL_REC_LENGTH;
            viewBox.Height = PIXEL_REC_LENGTH;

            viewBox.X = pos.X - PIXEL_REC_LENGTH / 2;
            viewBox.Y = pos.Y - PIXEL_REC_LENGTH / 2;
            b.Viewbox = viewBox;

            double x = pos.X + 10;
            double y = pos.Y + 10;
            if (x + ColorCanvas.Width > SystemParameters.VirtualScreenWidth)
            {
                x = pos.X - ColorCanvas.Width - 10;
            }
            if (y + ColorCanvas.Height > SystemParameters.VirtualScreenHeight)
            {
                y = pos.Y - ColorCanvas.Height - 10;
            }

            Canvas.SetLeft(ColorCanvas, x);
            Canvas.SetTop(ColorCanvas, y);


            System.Drawing.Color dColor = bgBitmap.GetPixel((int)pos.X, (int)pos.Y);

            PixelColor_HTML.Text = "#" + dColor.Name.ToUpper();
            PixelColor_RGB.Text = dColor.R + "," + dColor.G + "," + dColor.B;
            Pixel_XY.Text = pos.X + "*" + pos.Y;

            SolidColorBrush scb = (SolidColorBrush)PixelColor.Fill;
            scb.Color = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
        }

        /// <summary>
        /// 滚轮控制缩放区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta < 0 && PIXEL_REC_LENGTH < MAX_LENGTH)
            {
                //缩小
                PIXEL_REC_LENGTH += 5;
            }
            else if (e.Delta > 0 && PIXEL_REC_LENGTH > MIN_LENGTH)
            {
                //放大
                PIXEL_REC_LENGTH -= 5;
            }
            SetPixelAbout(e);
        }

        /// <summary>
        /// 右键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;
            GlobalColorPickerWindow.ShowOrHide();
            //关闭
            this.Close();
        }
    }
}
