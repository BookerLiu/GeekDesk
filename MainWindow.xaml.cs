using GeekDesk.Constant;
using GeekDesk.Control.UserControls.Config;
using GeekDesk.Control.UserControls.PannelCard;
using GeekDesk.Control.Windows;
using GeekDesk.Interface;
using GeekDesk.MyThread;
using GeekDesk.Plugins.EveryThing;
using GeekDesk.Plugins.EveryThing.Constant;
using GeekDesk.Task;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using GeekDesk.ViewModel.Temp;
using Microsoft.Win32;
using NPinyin;
using ShowSeconds;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using static GeekDesk.Util.ShowWindowFollowMouse;

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
        public static int colorPickerHotKeyId = -1;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            //加载数据
            LoadData();
            InitializeComponent();

            //用于其他类访问
            mainWindow = this;

            //执行待办提醒
            ToDoTask.BackLogCheck();

            ////实例化隐藏 Hide类，进行时间timer设置
            MarginHide.ReadyHide(this);
            if (appData.AppConfig.MarginHide)
            {
                MarginHide.StartHide();
            }

        }



    


        /// <summary>
        /// 搜索快捷键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchHotKeyDown(object sender, CanExecuteRoutedEventArgs e)
        {
            if (appData.AppConfig.SearchType == SearchType.HOT_KEY)
            {
                ShowSearchBox();
            }
        }

        /// <summary>
        /// 显示搜索框
        /// </summary>
        private void ShowSearchBox()
        {
            RunTimeStatus.SEARCH_BOX_SHOW = true;
            RightCard.VisibilitySearchCard(Visibility.Visible);
            SearchBox.Width = 400;
            SearchBox.Focus();

            //执行一遍a查询
            SearchBox_TextChanged(null, null);
        }
        /// <summary>
        /// 搜索开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!RunTimeStatus.SEARCH_BOX_SHOW
               && appData.AppConfig.SearchType != SearchType.KEY_DOWN
               )
            {
                SearchBox.TextChanged -= SearchBox_TextChanged;
                SearchBox.Clear();
                SearchBox.TextChanged += SearchBox_TextChanged;
                return;
            }

            if (!RunTimeStatus.SEARCH_BOX_SHOW) ShowSearchBox();

            //刷新搜索后 鼠标移动次数置为0
            RunTimeStatus.MOUSE_MOVE_COUNT = 0;
            //隐藏popup
            RightCard.MyPoptip.IsOpen = false;

            string inputText = SearchBox.Text.ToLower();
            RightCard.VerticalUFG.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(inputText))
            {
                RunTimeStatus.EVERYTHING_SEARCH_DELAY_TIME = 300;
                if (!RunTimeStatus.EVERYTHING_NEW_SEARCH)
                {                    
                    RunTimeStatus.EVERYTHING_NEW_SEARCH = true;
                    SearchDelay(null, null);
                }
            } else
            {
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        SearchIconList.RemoveAll();
                    });
                }).Start();
            }
            
        }

        private void SearchDelay(object sender, EventArgs args)
        {

            new Thread(() =>
            {

                while (RunTimeStatus.EVERYTHING_SEARCH_DELAY_TIME > 0)
                {
                    Thread.Sleep(10);
                    RunTimeStatus.EVERYTHING_SEARCH_DELAY_TIME -= 10;
                }
                RunTimeStatus.EVERYTHING_NEW_SEARCH = false;


                this.Dispatcher.Invoke(() =>
                {
                    if (SearchIconList.IconList.Count > 0)
                    {
                        SearchIconList.RemoveAll();
                    }
                    //DelayHelper dh = sender as DelayHelper;
                    //string inpuText = dh.Source as string;
                    string inputText = SearchBox.Text.ToLower().Trim();

                    int count = 0;
                    //GeekDesk数据搜索
                    ObservableCollection<MenuInfo> menuList = appData.MenuList;
                    foreach (MenuInfo menu in menuList)
                    {
                        ObservableCollection<IconInfo> iconList = menu.IconList;
                        foreach (IconInfo icon in iconList)
                        {
                            if (RunTimeStatus.EVERYTHING_NEW_SEARCH) return;
                            string pyName = Pinyin.GetInitials(icon.Name).ToLower();
                            if (icon.Name.Contains(inputText) || pyName.Contains(inputText))
                            {
                                SearchIconList.IconList.Add(icon);
                                count++;
                            }
                        }
                    }


                    if (appData.AppConfig.EnableEveryThing == true)
                    {
                        new Thread(() =>
                        {
                            //EveryThing全盘搜索
                            ObservableCollection<IconInfo> iconBakList = EveryThingUtil.Search(inputText);
                            count += iconBakList.Count;
                            this.Dispatcher.Invoke(() =>
                            {
                                TotalMsgBtn.Visibility = Visibility.Visible;
                                TotalMsgBtn.Content = count + " of " + Convert.ToInt64(EveryThingUtil.Everything_GetNumResults());
                                foreach (IconInfo icon in iconBakList)
                                {
                                    if (RunTimeStatus.EVERYTHING_NEW_SEARCH) return;
                                    SearchIconList.IconList.Add(icon);
                                }
                            });

                            //异步加载图标
                            if (iconBakList != null && iconBakList.Count > 0)
                            {
                                new Thread(() =>
                                {
                                    foreach (IconInfo icon in iconBakList)
                                    {
                                        if (RunTimeStatus.EVERYTHING_NEW_SEARCH) return;
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            icon.BitmapImage_NoWrite = ImageUtil.GetBitmapIconByUnknownPath(icon.Path);
                                        });
                                    }
                                }).Start();
                            }
                        }).Start();
                    }



                    if (RightCard.SearchListBox.Items.Count > 0)
                    {
                        RightCard.SearchListBox.SelectedIndex = 0;
                    }
                    RightCard.VerticalUFG.Visibility = Visibility.Visible;
                });

            }).Start();
        }

        /// <summary>
        /// 隐藏搜索框
        /// </summary>
        public void HidedSearchBox()
        {
            RunTimeStatus.EVERYTHING_NEW_SEARCH = true;
            new Thread(() =>
            {
                Thread.Sleep(1000);
                RunTimeStatus.EVERYTHING_NEW_SEARCH = false;
            }).Start();

            Keyboard.Focus(SearchBox);
            RunTimeStatus.SEARCH_BOX_SHOW = false;
            SearchBox.TextChanged -= SearchBox_TextChanged;
            SearchBox.Clear();
            SearchBox.TextChanged += SearchBox_TextChanged;
            SearchBox.Width = 0;
            TotalMsgBtn.Content = "0 of 0";
            TotalMsgBtn.Visibility = Visibility.Hidden;
            RightCard.VisibilitySearchCard(Visibility.Collapsed);

            SearchIconList.RemoveAll();

            //App.DoEvents();
            //new Thread(() =>
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
                    
            //    });
            //}).Start();

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
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                ShowApp();
            }
            //给任务栏图标一个名字
            BarIcon.Text = Constants.MY_NAME;

            //注册热键
            if (true == appData.AppConfig.EnableAppHotKey)
            {
                RegisterHotKey(true);
            }
            if (true == appData.AppConfig.EnableTodoHotKey)
            {
                RegisterCreateToDoHotKey(true);
            }

            if (true == appData.AppConfig.EnableColorPickerHotKey)
            {
                RegisterColorPickerHotKey(true);
            }

            //注册自启动
            if (!appData.AppConfig.SelfStartUped && !Constants.DEV)
            {
                RegisterUtil.SetSelfStarting(appData.AppConfig.SelfStartUp, Constants.MY_NAME);
            }

            //注册鼠标监听事件
            if (appData.AppConfig.MouseMiddleShow)
            {
                MouseHookThread.Hook();
            }

            //显秒插件
            if (appData.AppConfig.SecondsWindow == true)
            {
                SecondsWindow.ShowWindow();
            }

            //监听实时文件夹菜单
            FileWatcher.StartLinkMenuWatcher(appData);


            //更新线程开启  检测更新
            UpdateThread.Update();

            //建立相对路径
            RelativePathThread.MakeRelativePath();

            //毛玻璃  暂时未解决阴影问题
            //BlurGlassUtil.EnableBlur(this);

            WindowUtil.SetOwner(this, WindowUtil.GetDesktopHandle(this, DesktopLayer.Progman));

            if (appData.AppConfig.EnableEveryThing == true)
            {
                //开启EveryThing插件
                EveryThingUtil.EnableEveryThing();
            }

            Keyboard.Focus(SearchBox);

            MessageUtil.ChangeWindowMessageFilter(MessageUtil.WM_COPYDATA, 1);
        }

        /// <summary>
        /// 注册当前窗口的热键
        /// </summary>
        public static void RegisterHotKey(bool first)
        {
            try
            {
                if (appData.AppConfig.HotkeyModifiers != GlobalHotKey.HotkeyModifiers.None)
                {
                    hotKeyId = GlobalHotKey.RegisterHotKey(appData.AppConfig.HotkeyModifiers, appData.AppConfig.Hotkey, () =>
                    {
                        if (RunTimeStatus.MAIN_HOT_KEY_DOWN) return;
                        RunTimeStatus.MAIN_HOT_KEY_DOWN = true;
                        new Thread(() =>
                        {
                            Thread.Sleep(RunTimeStatus.MAIN_HOT_KEY_TIME);
                            RunTimeStatus.MAIN_HOT_KEY_DOWN = false;
                        }).Start();

                        if (MotionControl.hotkeyFinished)
                        {
                            if (CheckSholeShowApp())
                            {
                                ShowApp();
                            }
                            else
                            {
                                HideApp();
                            }
                        }
                    });
                    if (!first)
                    {
                        HandyControl.Controls.Growl.Success("GeekDesk快捷键注册成功(" + appData.AppConfig.HotkeyStr + ")!", "HotKeyGrowl");
                    }
                }
                else
                {
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

                if (appData.AppConfig.HotkeyModifiers != GlobalHotKey.HotkeyModifiers.None)
                {
                    //加载完毕注册热键
                    toDoHotKeyId = GlobalHotKey.RegisterHotKey(appData.AppConfig.ToDoHotkeyModifiers, appData.AppConfig.ToDoHotkey, () =>
                    {
                        if (MotionControl.hotkeyFinished)
                        {
                            ToDoWindow.ShowOrHide();
                        }
                    });
                    if (!first)
                    {
                        HandyControl.Controls.Growl.Success("新建待办任务快捷键注册成功(" + appData.AppConfig.ToDoHotkeyStr + ")!", "HotKeyGrowl");
                    }
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
        /// 注册新建待办的热键
        /// </summary>
        public static void RegisterColorPickerHotKey(bool first)
        {
            try
            {
                if (appData.AppConfig.HotkeyModifiers != GlobalHotKey.HotkeyModifiers.None)
                {
                    //加载完毕注册热键
                    colorPickerHotKeyId = GlobalHotKey.RegisterHotKey(appData.AppConfig.ColorPickerHotkeyModifiers, appData.AppConfig.ColorPickerHotkey, () =>
                    {
                        if (MotionControl.hotkeyFinished)
                        {
                            GlobalColorPickerWindow.CreateNoShow();
                        }
                    });
                    if (!first)
                    {
                        HandyControl.Controls.Growl.Success("拾色器快捷键注册成功(" + appData.AppConfig.ColorPickerHotkeyStr + ")!", "HotKeyGrowl");
                    }
                }
            }
            catch (Exception)
            {
                if (first)
                {
                    HandyControl.Controls.Growl.WarningGlobal("拾色器快捷键已被其它程序占用(" + appData.AppConfig.ColorPickerHotkeyStr + ")!");
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("拾色器快捷键已被其它程序占用(" + appData.AppConfig.ColorPickerHotkeyStr + ")!", "HotKeyGrowl");
                }
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
            HideApp();
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

            if (MarginHide.ON_HIDE)
            {
                //修改贴边隐藏状态为未隐藏
                MarginHide.IS_HIDE = false;
                if (!CommonCode.MouseInWindow(mainWindow))
                {
                    RunTimeStatus.MARGIN_HIDE_AND_OTHER_SHOW = true;
                    MarginHide.WaitHide(3000);
                }
            }

            if (appData.AppConfig.FollowMouse)
            {
                ShowWindowFollowMouse.Show(mainWindow, MousePosition.CENTER, 0, 0);
            }


            MainWindow.mainWindow.Activate();
            mainWindow.Show();
            //mainWindow.Visibility = Visibility.Visible;
            if (appData.AppConfig.AppAnimation)
            {
                appData.AppConfig.IsShow = true;
            }
            else
            {
                appData.AppConfig.IsShow = null;
                //防止永远不显示界面
                if (mainWindow.Opacity < 1)
                {
                    mainWindow.Opacity = 1;
                }
            }


            //FadeStoryBoard(1, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Visible);

            Keyboard.Focus(mainWindow);
            if (RunTimeStatus.SHOW_MENU_PASSWORDBOX)
            {
                mainWindow.RightCard.PDDialog.SetFocus();
            }
            else
            {
                Keyboard.Focus(mainWindow.SearchBox);
            }
        }

        public static void HideApp()
        {
            if (appData.AppConfig.AppAnimation)
            {
                appData.AppConfig.IsShow = false;
            }
            else
            {
                appData.AppConfig.IsShow = null;
                HideAppVis();
            }

        }

        private static void HideAppVis()
        {
            //关闭锁定
            RunTimeStatus.LOCK_APP_PANEL = false;
            if (RunTimeStatus.SEARCH_BOX_SHOW)
            {
                mainWindow.HidedSearchBox();
            }
            mainWindow.Visibility = Visibility.Collapsed;
            //if (!MarginHide.IS_HIDE)
            //{

            //}
            //else
            //{
            //    ShowApp();
            //}
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
            if (CheckSholeShowApp())
            {
                ShowApp();
            }
            else
            {
                HideApp();
            }
        }

        private static bool CheckSholeShowApp()
        {
            return mainWindow.Visibility == Visibility.Collapsed
                || mainWindow.Opacity == 0
                || MarginHide.IS_HIDE
                || !WindowUtil.WindowIsTop(mainWindow);
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


        private void AppWindowLostFocus()
        {
            if (appData.AppConfig.AppHideType == AppHideType.LOST_FOCUS
                && this.Opacity == 1 && !RunTimeStatus.LOCK_APP_PANEL)
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
            if (appData.AppConfig.MouseMiddleShow || appData.AppConfig.SecondsWindow == true)
            {
                MouseHookThread.Dispose();
            }
            if (appData.AppConfig.EnableEveryThing == true)
            {
                EveryThingUtil.DisableEveryThing();
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
            if (appData.AppConfig.MouseMiddleShow || appData.AppConfig.SecondsWindow == true)
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
            //char c = (char)e.Key;

            if (e.Key == Key.Escape)
            {
                HideApp();
            }

            if (RunTimeStatus.SEARCH_BOX_SHOW && (e.Key == Key.Up
                || e.Key == Key.Down
                || e.Key == Key.Tab
                || e.Key == Key.Enter
                ))
            {
                if (e.Key == Key.Down || e.Key == Key.Tab)
                {
                    RightCard.SearchListBoxIndexAdd();
                }
                else if (e.Key == Key.Up)
                {
                    RightCard.SearchListBoxIndexSub();
                }
                else if (e.Key == Key.Enter)
                {
                    RightCard.StartupSelectionItem();
                }
            }
        }



        /// <summary>
        /// 为了让修改菜单的textBox失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
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

        /// <summary>
        /// 打开屏幕拾色器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPicker(object sender, RoutedEventArgs e)
        {
            TaskbarContextMenu.IsOpen = false;
            GlobalColorPickerWindow.CreateNoShow();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            // 如果没有在修改菜单 并且不是右键点击了面板
            if (!RunTimeStatus.IS_MENU_EDIT
                && !RunTimeStatus.SHOW_RIGHT_BTN_MENU
                && !RunTimeStatus.APP_BTN_IS_DOWN)
            {
                if (RunTimeStatus.SHOW_MENU_PASSWORDBOX)
                {
                    //必须在其它文本框没有工作的时候才给密码框焦点
                    RightCard.PDDialog.SetFocus();
                }
                else
                {
                    //必须在其它文本框没有工作的时候才给搜索框焦点
                    Keyboard.Focus(SearchBox);
                }

            }

        }

        private void AppWindow_Deactivated(object sender, EventArgs e)
        {
            AppWindowLostFocus();
        }

        /// <summary>
        /// 备份数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void BakDataFile(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                CommonCode.BakAppData();
            });
            t.ApartmentState = ApartmentState.STA;
            t.Start();
        }

        private void AppButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //点击了面板
            RunTimeStatus.APP_BTN_IS_DOWN = true;
            new Thread(() =>
            {
                Thread.Sleep(50);
                RunTimeStatus.APP_BTN_IS_DOWN = false;
            }).Start();
        }


        private ICommand _hideCommand;
        public ICommand HideCommand
        {
            get
            {
                if (_hideCommand == null)
                {
                    _hideCommand = new RelayCommand(
                        p =>
                        {
                            return true;
                        },
                        p =>
                        {
                            HideAppVis();
                        });
                }
                return _hideCommand;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MessageUtil.WM_COPYDATA)
            {
                MessageUtil.CopyDataStruct cds = (MessageUtil.CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(MessageUtil.CopyDataStruct));
                if ("ShowApp".Equals(cds.msg))
                {
                    ShowApp();
                }
            }
            return hwnd;
        }



    }
}
