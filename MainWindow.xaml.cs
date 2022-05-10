using GeekDesk.Constant;
using GeekDesk.Control.UserControls.Config;
using GeekDesk.Control.Windows;
using GeekDesk.Interface;
using GeekDesk.Task;
using GeekDesk.MyThread;
using GeekDesk.Util;
using GeekDesk.ViewModel;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static GeekDesk.Util.ShowWindowFollowMouse;
using System.Collections.ObjectModel;
using NPinyin;
using GeekDesk.ViewModel.Temp;
using System.Threading;
using DraggAnimatedPanelExample;

namespace GeekDesk
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window, IWindowCommon
    {

        public static AppData appData = CommonCode.GetAppDataByFile();
        public static ToDoInfoWindow toDoInfoWindow;
        public static int hotKeyId = -1;
        public static int toDoHotKeyId = -1;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            LoadData();
            InitializeComponent();
            mainWindow = this;
            this.Topmost = true;
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
            ToDoTask.BackLogCheck();


            ////实例化隐藏 Hide类，进行时间timer设置
            MarginHide.ReadyHide(this);
            if (appData.AppConfig.MarginHide)
            {
                MarginHide.StartHide();
            }
        }


        /// <summary>
        /// 显示搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchItem(object sender, CanExecuteRoutedEventArgs e)
        {
            RunTimeStatus.SEARCH_BOX_SHOW = true;
            RightCard.VisibilitySearchCard(Visibility.Visible);
            SearchBox.Visibility = Visibility.Visible;
            SearchBox.Focus();
        }
        /// <summary>
        /// 搜索开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = SearchBox.Text.ToLower();
            RightCard.VerticalUFG.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(inputText))
            {
                SearchIconList.IconList.Clear();
                ObservableCollection<MenuInfo> menuList = appData.MenuList;
                foreach (MenuInfo menu in menuList)
                {
                    ObservableCollection<IconInfo> iconList = menu.IconList;
                    foreach (IconInfo icon in iconList)
                    {
                        string pyName = Pinyin.GetInitials(icon.Name).ToLower();
                        if (icon.Name.Contains(inputText) || pyName.Contains(inputText))
                        {
                            SearchIconList.IconList.Add(icon);
                        }
                    }
                }
            }
            else
            {
                SearchIconList.IconList.Clear();
            }
            RightCard.VerticalUFG.Visibility = Visibility.Visible;
        }

        public void HidedSearchBox()
        {
            RunTimeStatus.SEARCH_BOX_SHOW = false;
            SearchBox.Visibility = Visibility.Collapsed;
            SearchIconList.IconList.Clear();
            RightCard.VisibilitySearchCard(Visibility.Collapsed);
            SearchBox.Text = "";
        }


        /// <summary>
        /// 加载缓存数据
        /// </summary>
        private void LoadData()
        {
            this.DataContext = appData;
            if (appData.MenuList.Count == 0)
            {
                appData.MenuList.Add(new MenuInfo() { MenuName = "NewMenu", MenuId = System.Guid.NewGuid().ToString(), MenuEdit = Visibility.Collapsed });
            }

            this.Width = appData.AppConfig.WindowWidth;
            this.Height = appData.AppConfig.WindowHeight;
        }


        /// <summary>
        /// 窗口加载完毕 执行方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BGSettingUtil.BGSetting();
            if (!appData.AppConfig.StartedShowPanel)
            {
                if (appData.AppConfig.AppAnimation)
                {
                    this.Opacity = 0;
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ShowApp();
            }

            //给任务栏图标一个名字
            BarIcon.Text = Constants.MY_NAME;

            //注册热键
            RegisterHotKey(true);
            RegisterCreateToDoHotKey(true);

            //注册自启动
            if (!appData.AppConfig.SelfStartUped && !Constants.DEV)
            {
                RegisterUtil.SetSelfStarting(appData.AppConfig.SelfStartUp, Constants.MY_NAME);
            }

            //注册鼠标中键监听事件
            if (appData.AppConfig.MouseMiddleShow)
            {
                MouseHookThread.MiddleHook();
            }

            //更新线程开启  检测更新
            UpdateThread.Update();
        }

        /// <summary>
        /// 注册当前窗口的热键
        /// </summary>
        public static void RegisterHotKey(bool first)
        {
            try
            {
                if (appData.AppConfig.HotkeyModifiers != 0)
                {

                    hotKeyId = GlobalHotKey.RegisterHotKey(appData.AppConfig.HotkeyModifiers, appData.AppConfig.Hotkey, () =>
                    {
                        if (MotionControl.hotkeyFinished)
                        {
                            if (mainWindow.Visibility == Visibility.Collapsed || mainWindow.Opacity == 0 || MarginHide.IS_HIDE)
                            {
                                ShowApp();
                            }
                            else
                            {
                                HideApp();
                            }
                        }
                    });
                }
                if (!first)
                {
                    HandyControl.Controls.Growl.Success("GeekDesk快捷键注册成功(" + appData.AppConfig.HotkeyStr + ")!", "HotKeyGrowl");
                }
            }
            catch (Exception)
            {
                if (first)
                {
                    HandyControl.Controls.Growl.WarningGlobal("GeekDesk启动快捷键已被其它程序占用(" + appData.AppConfig.HotkeyStr + ")!");
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("GeekDesk启动快捷键已被其它程序占用(" + appData.AppConfig.HotkeyStr + ")!", "HotKeyGrowl");

                }
            }
        }



        /// <summary>
        /// 注册新建待办的热键
        /// </summary>
        public static void RegisterCreateToDoHotKey(bool first)
        {
            try
            {

                if (appData.AppConfig.ToDoHotkeyModifiers != 0)
                {
                    //加载完毕注册热键
                    toDoHotKeyId = GlobalHotKey.RegisterHotKey(appData.AppConfig.ToDoHotkeyModifiers, appData.AppConfig.ToDoHotkey, () =>
                    {
                        if (MotionControl.hotkeyFinished)
                        {
                            ToDoInfoWindow.ShowOrHide();
                        }
                    });
                }
                if (!first)
                {
                    HandyControl.Controls.Growl.Success("新建待办任务快捷键注册成功(" + appData.AppConfig.ToDoHotkeyStr + ")!", "HotKeyGrowl");
                }
            }
            catch (Exception)
            {
                if (first)
                {
                    HandyControl.Controls.Growl.WarningGlobal("新建待办任务快捷键已被其它程序占用(" + appData.AppConfig.ToDoHotkeyStr + ")!");
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("新建待办任务快捷键已被其它程序占用(" + appData.AppConfig.ToDoHotkeyStr + ")!", "HotKeyGrowl");
                }
            }
        }


        /// <summary>
        /// 重置窗体大小 写入缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Resize(object sender, System.EventArgs e)
        {
            if (this.DataContext != null)
            {
                AppData appData = this.DataContext as AppData;
                appData.AppConfig.WindowWidth = this.Width;
                appData.AppConfig.WindowHeight = this.Height;
            }
        }



        /// <summary>
        /// 程序窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragMove(object sender, MouseEventArgs e)
        {

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
            if (appData.AppConfig.AppAnimation)
            {
                FadeStoryBoard(0, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Collapsed);
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
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
        public void ShowApp(object sender, RoutedEventArgs e)
        {
            ShowApp();
        }
        public static void ShowApp()
        {
            //有全屏化应用则不显示
            //if (CommonCode.IsPrimaryFullScreen())
            //{
            //    return;
            //}

            //修改贴边隐藏状态为未隐藏
            if (MarginHide.ON_HIDE)
            {
                MarginHide.IS_HIDE = false;
                if (!CommonCode.MouseInWindow(mainWindow))
                {
                    RunTimeStatus.MARGIN_HIDE_AND_OTHER_SHOW = true;
                    MarginHide.WaitHide(3000);
                }
            }

            if (appData.AppConfig.FollowMouse)
            {
                ShowWindowFollowMouse.Show(mainWindow, MousePosition.CENTER, 0, 0, false);
            }

            FadeStoryBoard(1, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Visible);

            Keyboard.Focus(mainWindow.EmptyTextBox);
        }

        public static void HideApp()
        {
            if (!MarginHide.IS_HIDE)
            {
                if (RunTimeStatus.SEARCH_BOX_SHOW)
                {
                    mainWindow.HidedSearchBox();
                    new Thread(() =>
                    {
                        Thread.Sleep(100);
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            FadeStoryBoard(0, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Collapsed);
                        }));
                    }).Start();
                }
                else
                {
                    FadeStoryBoard(0, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Collapsed);
                }
            } else
            {
                ShowApp();
            }
            
        }

        /// <summary>
        /// 淡入淡出效果
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="milliseconds"></param>
        /// <param name="visibility"></param>
        public static void FadeStoryBoard(int opacity, int milliseconds, Visibility visibility)
        {
            if (appData.AppConfig.AppAnimation)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    From = mainWindow.Opacity,
                    To = opacity,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
                };
                opacityAnimation.Completed += (s, e) =>
                {
                    mainWindow.BeginAnimation(OpacityProperty, null);
                    if (visibility == Visibility.Visible)
                    {
                        mainWindow.Opacity = 1;
                    }
                    else
                    {
                        mainWindow.Opacity = 0;
                        CommonCode.SortIconList();
                    }
                };
                Timeline.SetDesiredFrameRate(opacityAnimation, 60);
                mainWindow.BeginAnimation(OpacityProperty, opacityAnimation);
            }
            else
            {
                //防止关闭动画后 窗体仍是0透明度
                mainWindow.Opacity = 1;
                mainWindow.Visibility = visibility;
                if (visibility == Visibility.Collapsed)
                {
                    CommonCode.SortIconList();
                }
            }
        }




        /// <summary>
        /// 图片图标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Collapsed || this.Opacity == 0)
            {
                ShowApp();
            }
            else
            {
                HideApp();
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
        /// 右键任务栏图标打开程序目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenThisDir(object sender, RoutedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Explorer.exe";
            p.StartInfo.Arguments = "/e,/select," + Constants.APP_DIR + Constants.MY_NAME + ".exe";
            p.Start();
        }




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
            ToDoWindow.Show();
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

        private void App_LostFocus(object sender, EventArgs e)
        {
            if (appData.AppConfig.AppHideType == AppHideType.LOST_FOCUS
                && this.Opacity == 1)
            {
                //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                if (!appData.AppConfig.MarginHide || (appData.AppConfig.MarginHide && !MarginHide.IS_HIDE))
                {
                    HideApp();
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                AppData appData = this.DataContext as AppData;
                appData.AppConfig.WindowWidth = this.Width;
                appData.AppConfig.WindowHeight = this.Height;
            }
        }



        /// <summary>
        /// 右键任务栏图标退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            if (appData.AppConfig.MouseMiddleShow)
            {
                MouseHookThread.Dispose();
            }
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReStartApp(object sender, RoutedEventArgs e)
        {
            if (appData.AppConfig.MouseMiddleShow)
            {
                MouseHookThread.Dispose();
            }

            Process p = new Process();
            p.StartInfo.FileName = Constants.APP_DIR + "GeekDesk.exe";
            p.StartInfo.WorkingDirectory = Constants.APP_DIR;
            p.Start();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// 关闭托盘图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBarIcon(object sender, RoutedEventArgs e)
        {
            appData.AppConfig.ShowBarIcon = false;
        }


        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideApp();
            }
        }

        /// <summary>
        /// 为了让修改菜单的textBox失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmptyTextBox.Focus();
        }

        /// <summary>
        /// 鼠标进入后 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            //防止延迟贴边隐藏
            RunTimeStatus.MARGIN_HIDE_AND_OTHER_SHOW = false;
        }
    }
}
