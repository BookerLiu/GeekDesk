using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control;
using GeekDesk.Control.UserControls.Config;
using GeekDesk.Control.Windows;
using GeekDesk.Task;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using GlobalHotKey;
using HandyControl.Data;
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
        public static int hotKeyId = -1;
        public static MainWindow mainWindow;
        public HotKeyManager hkm = new HotKeyManager();
        public MainWindow()
        {
            LoadData();
            InitializeComponent();
            mainWindow = this;
            this.Topmost = true;
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
            BacklogTask.BackLogCheck();
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
            } else
            {
                ShowApp();
            }
           

        }

        /// <summary>
        /// 注册当前窗口的热键
        /// </summary>
        public static void RegisterHotKey()
        {
            try
            {
                //加载完毕注册热键
                hotKeyId = Hotkey.Regist(new WindowInteropHelper(MainWindow.mainWindow).Handle, appData.AppConfig.HotkeyModifiers, appData.AppConfig.Hotkey, () =>
                {
                    if (MotionControl.hotkeyFinished)
                    {
                        if (mainWindow.Visibility == Visibility.Collapsed)
                        {
                            ShowApp();
                        }
                        else
                        {
                            mainWindow.Visibility = Visibility.Collapsed;
                        }
                    }
                });
                //HandyControl.Controls.Growl.SuccessGlobal("快捷键注册成功(" + appData.AppConfig.HotkeyStr + ")!");
            }
            catch (Exception)
            {
                HandyControl.Controls.Growl.WarningGlobal("GeekDesk启动快捷键已被其它程序占用(" + appData.AppConfig.HotkeyStr + ")!");
            }
            
        }

        private void DisplayWindowHotKeyPress(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.Y)
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
        /// 右键任务栏图标 显示主面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowApp(object sender, RoutedEventArgs e)
        {
            ShowApp();
        }
        public static void ShowApp()
        {
            if (appData.AppConfig.FollowMouse)
            {
                ShowAppAndFollowMouse();
            } else
            {
                mainWindow.Visibility = Visibility.Visible;
            }
            Keyboard.Focus(mainWindow);
        }

        /// <summary>
        /// 随鼠标位置显示面板 (鼠标始终在中间)
        /// </summary>
        private static void ShowAppAndFollowMouse()
        {
            //获取鼠标位置
            System.Windows.Point p = MouseUtil.GetMousePosition();
            double left = SystemParameters.VirtualScreenLeft;
            double top = SystemParameters.VirtualScreenTop;
            double width = SystemParameters.VirtualScreenWidth;
            double height = SystemParameters.VirtualScreenHeight;
            double right = width - Math.Abs(left);
            double bottom = height - Math.Abs(top);


            if (p.X - mainWindow.Width / 2 < left)
            {
                //判断是否在最左边缘
                mainWindow.Left = left;
            }
            else if (p.X + mainWindow.Width / 2 > right)
            {
                //判断是否在最右边缘
                mainWindow.Left = right - mainWindow.Width;
            }
            else
            {
                mainWindow.Left = p.X - mainWindow.Width / 2;
            }


            if (p.Y - mainWindow.Height / 2 < top)
            {
                //判断是否在最上边缘
                mainWindow.Top = top;
            }
            else if (p.Y + mainWindow.Height / 2 > bottom)
            {
                //判断是否在最下边缘
                mainWindow.Top = bottom - mainWindow.Height;
            }
            else
            {
                mainWindow.Top = p.Y - mainWindow.Height / 2;
            }

            mainWindow.Visibility = Visibility.Visible;
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
            ConfigWindow.Show(appData.AppConfig, this);
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
        /// 设置图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigButtonClick(object sender, RoutedEventArgs e)
        {
            SettingMenus.IsOpen = true;
        }

        /// <summary>
        /// 设置菜单点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigMenuClick(object sender, RoutedEventArgs e)
        {
            ConfigWindow.Show(appData.AppConfig, this);
        }
        /// <summary>
        /// 待办任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BacklogMenuClick(object sender, RoutedEventArgs e)
        {
            BacklogWindow.Show();
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

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                AppData appData = this.DataContext as AppData;
                appData.AppConfig.WindowWidth = this.Width;
                appData.AppConfig.WindowHeight = this.Height;
            }
        }
    }




}
