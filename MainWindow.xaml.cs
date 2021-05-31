using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace GeekDesk
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public static AppData appData = CommonCode.GetAppDataByFile();
        public MainWindow()
        {
            LoadData();
            InitializeComponent();
            this.Topmost = true;
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
        }

        private void LoadData()
        {
            this.DataContext = appData;
            if (appData.MenuList.Count == 0)
            {
                appData.MenuList.Add(new MenuInfo() { MenuName = "NewMenu", MenuId = System.Guid.NewGuid().ToString(), MenuEdit = Visibility.Collapsed});
            }

            this.Width = appData.AppConfig.WindowWidth;
            this.Height = appData.AppConfig.WindowHeight;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!appData.AppConfig.StartedShowPanel)
            {
                this.Visibility = Visibility.Collapsed;
            }
            //加载完毕注册热键
            Hotkey.Regist(this, HotkeyModifiers.MOD_CONTROL, Key.Y, ()=>
            {
                if (this.Visibility == Visibility.Collapsed)
                {
                    ShowApp();
                } else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            });
        }


        void MainWindow_Resize(object sender, System.EventArgs e)
        {
            if (this.DataContext != null)
            {
                AppData appData = this.DataContext as AppData;
                appData.AppConfig.WindowWidth = this.Width;
                appData.AppConfig.WindowHeight = this.Height;
            }
        }


       

        private void DragMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var windowMode = this.ResizeMode;
                if (this.ResizeMode != ResizeMode.NoResize)
                {
                    this.ResizeMode = ResizeMode.NoResize;
                }

                this.UpdateLayout();


                /* 当点击拖拽区域的时候，让窗口跟着移动
                (When clicking the drag area, make the window follow) */
                DragMove();


                if (this.ResizeMode != windowMode)
                {
                    this.ResizeMode = windowMode;
                }

                this.UpdateLayout();
            }
        }




        /// <summary>
        /// 关闭按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

       

        ///// <summary>
        ///// 左侧栏宽度改变 持久化
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void LeftCardResize(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        //{
        //    appData.AppConfig.MenuCardWidth = LeftColumn.Width.Value;
        //}

        /// <summary>
        /// 随鼠标位置显示面板 (鼠标始终在中间)
        /// </summary>
        private void ShowAppAndFollowMouse()
        {
            //获取鼠标位置
            System.Windows.Point p = MouseUtil.GetMousePosition();
            double left = SystemParameters.VirtualScreenLeft;
            double top = SystemParameters.VirtualScreenTop;
            double width = SystemParameters.VirtualScreenWidth;
            double height = SystemParameters.VirtualScreenHeight;
            double right = width - Math.Abs(left);
            double bottom = height - Math.Abs(top);

            
            if (p.X - this.Width / 2 < left)
            {
                //判断是否在最左边缘
                this.Left = left;
            } else if (p.X + this.Width / 2 > right) 
            {
                //判断是否在最右边缘
                this.Left = right - this.Width;
            } else
            {
                this.Left = p.X - this.Width / 2;
            }

            
            if (p.Y - this.Height / 2 < top)
            {
                //判断是否在最上边缘
                this.Top = top;
            } else if (p.Y + this.Height/2 > bottom) 
            {
                //判断是否在最下边缘
                this.Top = bottom - this.Height;
            } else
            {
                this.Top = p.Y - this.Height / 2;
            }

            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 右键任务栏图标 显示主面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowApp(object sender, RoutedEventArgs e)
        {
            ShowApp();
        }
        private void ShowApp()
        {
            if (appData.AppConfig.FollowMouse)
            {
                ShowAppAndFollowMouse();
            } else
            {
                this.Visibility = Visibility.Visible;
            }
            Keyboard.Focus(this);
        }

        /// <summary>
        /// 图片图标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                ShowApp();
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 右键任务栏图标 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigApp(object sender, RoutedEventArgs e)
        {
            //MenuInfo menuInfo = ((MenuItem)sender).Tag as MenuInfo;
            //appData.MenuList.Remove(menuInfo);



            //CommonCode.SaveAppData(appData);
        }

        /// <summary>
        /// 右键任务栏图标退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        

        //public static void ShowContextMenu(IntPtr hAppWnd, Window taskBar, System.Windows.Point pt)
        //{
        //    WindowInteropHelper helper = new WindowInteropHelper(taskBar);
        //    IntPtr callingTaskBarWindow = helper.Handle;
        //    IntPtr wMenu = GetSystemMenu(hAppWnd, false);
        //    // Display the menu 
        //    uint command = TrackPopupMenuEx(wMenu, TPM.LEFTBUTTON | TPM.RETURNCMD, (int) pt.X, (int) pt.Y, callingTaskBarWindow, IntPtr.Zero);
        //    if (command == 0) return; 
        //    PostMessage(hAppWnd, WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        //}

        /// <summary>
        /// 设置按钮左键弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigButtonClick(object sender, RoutedEventArgs e)
        {
            //SettingMenu.IsOpen = true;
            new ConfigWindow(appData.AppConfig).Show();
        }

        /// <summary>
        /// 禁用设置按钮右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingButton_Initialized(object sender, EventArgs e)
        {
            SettingButton.ContextMenu = null;
        }

        private void App_LostFocus(object sender, RoutedEventArgs e)
        {
            if (appData.AppConfig.AppHideType == AppHideType.LOST_FOCUS)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void window_Deactivated(object sender, EventArgs e)
        {
            if (appData.AppConfig.AppHideType == AppHideType.LOST_FOCUS)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
    }




}
