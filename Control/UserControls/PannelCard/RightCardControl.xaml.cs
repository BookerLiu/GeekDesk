﻿using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.Control.Windows;
using GeekDesk.Plugins.EveryThing;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using GeekDesk.ViewModel.Temp;
using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GeekDesk.Control.UserControls.PannelCard
{
    /// <summary>
    /// RightCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class RightCardControl : UserControl
    {
        private AppData appData = MainWindow.appData;

        ListBoxDragDropManager<IconInfo> dragMgr;

        //private Thread dropCheckThread = null;

        public RightCardControl()
        {
            InitializeComponent();
            this.Loaded += RightCardControl_Loaded;

        }

        private void RightCardControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.dragMgr = new ListBoxDragDropManager<IconInfo>(this.IconListBox);
        }


        //#region 图标拖动
        //DelegateCommand<int[]> _swap;
        //public DelegateCommand<int[]> SwapCommand
        //{
        //    get
        //    {
        //        if (_swap == null)
        //            _swap = new DelegateCommand<int[]>(
        //                (indexes) =>
        //                {
        //                    DROP_ICON = true;
        //                    if (appData.AppConfig.IconSortType != SortType.CUSTOM
        //                    && (dropCheckThread == null || !dropCheckThread.IsAlive))
        //                    {
        //                        dropCheckThread = new Thread(() =>
        //                        {
        //                            do
        //                            {
        //                                DROP_ICON = false;
        //                                Thread.Sleep(1000);
        //                            } while (DROP_ICON);

        //                            MainWindow.appData.AppConfig.IconSortType = SortType.CUSTOM;
        //                            App.Current.Dispatcher.Invoke(() =>
        //                            {
        //                                if (MainWindow.mainWindow.Visibility == Visibility.Collapsed
        //                                || MainWindow.mainWindow.Opacity != 1)
        //                                {
        //                                    Growl.WarningGlobal("已将图标排序规则重置为自定义!");
        //                                }
        //                                else
        //                                {
        //                                    Growl.Warning("已将图标排序规则重置为自定义!", "MainWindowGrowl");
        //                                }
        //                            });
        //                        });
        //                        dropCheckThread.Start();
        //                    }
        //                    int fromS = indexes[0];
        //                    int to = indexes[1];
        //                    ObservableCollection<IconInfo> iconList = appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList;
        //                    var elementSource = iconList[to];
        //                    var dragged = iconList[fromS];

        //                    iconList.Remove(dragged);
        //                    iconList.Insert(to, dragged);
        //                }
        //            );
        //        return _swap;
        //    }
        //}

        //#endregion 图标拖动




        private void Icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (appData.AppConfig.DoubleOpen)
            {
                IconClick(sender, e);
            }
        }

        private void Icon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!appData.AppConfig.DoubleOpen)
            {
                IconClick(sender, e);
            }
        }

        /// <summary>
        /// 图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconClick(object sender, MouseButtonEventArgs e)
        {
            if (appData.AppConfig.DoubleOpen && e.ClickCount >= 2)
            {
                IconInfo icon = (IconInfo)((Panel)sender).Tag;
                if (icon.AdminStartUp)
                {
                    StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }
            else if (!appData.AppConfig.DoubleOpen && e.ClickCount == 1)
            {
                IconInfo icon = (IconInfo)((Panel)sender).Tag;
                if (icon.AdminStartUp)
                {
                    StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }

        }

        /// <summary>
        /// 管理员方式启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconAdminStart(object sender, RoutedEventArgs e)
        {
            IconInfo icon = (IconInfo)((MenuItem)sender).Tag;
            StartIconApp(icon, IconStartType.ADMIN_STARTUP);
        }

        /// <summary>
        /// 打开文件所在位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowInExplore(object sender, RoutedEventArgs e)
        {
            IconInfo icon = (IconInfo)((MenuItem)sender).Tag;
            StartIconApp(icon, IconStartType.SHOW_IN_EXPLORE);
        }

        private void StartIconApp(IconInfo icon, IconStartType type, bool useRelativePath = false)
        {
            try
            {
                using (Process p = new Process())
                {
                    string startArg = icon.StartArg;

                    if (startArg != null && Constants.SYSTEM_ICONS.ContainsKey(startArg))
                    {
                        StartSystemApp(startArg, type);
                    }
                    else
                    {
                        string path;
                        if (useRelativePath)
                        {
                            string fullPath = Path.Combine(Constants.APP_DIR, icon.RelativePath);
                            path = Path.GetFullPath(fullPath);
                        }
                        else
                        {
                            path = icon.Path;
                        }
                        p.StartInfo.FileName = path;
                        if (!StringUtil.IsEmpty(startArg))
                        {
                            p.StartInfo.Arguments = startArg;
                        }
                        if (icon.IconType == IconType.OTHER)
                        {
                            if (!File.Exists(path) && !Directory.Exists(path))
                            {
                                //如果没有使用相对路径  那么使用相对路径启动一次
                                if (!useRelativePath)
                                {
                                    StartIconApp(icon, type, true);
                                    return;
                                }
                                else
                                {
                                    HandyControl.Controls.Growl.WarningGlobal("程序启动失败(文件路径不存在或已删除)!");
                                    return;
                                }
                            }
                            p.StartInfo.WorkingDirectory = path.Substring(0, path.LastIndexOf("\\"));
                            switch (type)
                            {
                                case IconStartType.ADMIN_STARTUP:
                                    //p.StartInfo.Arguments = "1";//启动参数
                                    p.StartInfo.Verb = "runas";
                                    //p.StartInfo.CreateNoWindow = false; //设置显示窗口
                                    p.StartInfo.UseShellExecute = true;//不使用操作系统外壳程序启动进程
                                    //p.StartInfo.ErrorDialog = false;
                                    if (appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                                    {
                                        //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                        if (appData.AppConfig.MarginHide)
                                        {
                                            if (!MarginHide.IsMargin())
                                            {
                                                MainWindow.HideApp();
                                            }
                                        }
                                        else
                                        {
                                            MainWindow.HideApp();
                                        }

                                    }
                                    break;// c#好像不能case穿透
                                case IconStartType.DEFAULT_STARTUP:
                                    if (appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                                    {
                                        //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                        if (appData.AppConfig.MarginHide)
                                        {
                                            if (!MarginHide.IsMargin())
                                            {
                                                MainWindow.HideApp();
                                            }
                                        }
                                        else
                                        {
                                            MainWindow.HideApp();
                                        }
                                    }
                                    break;
                                case IconStartType.SHOW_IN_EXPLORE:
                                    p.StartInfo.FileName = "Explorer.exe";
                                    p.StartInfo.Arguments = "/e,/select," + icon.Path;
                                    break;
                            }
                        }
                        else
                        {
                            if (appData.AppConfig.AppHideType == AppHideType.START_EXE && !RunTimeStatus.LOCK_APP_PANEL)
                            {
                                //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                if (appData.AppConfig.MarginHide)
                                {
                                    if (!MarginHide.IS_HIDE)
                                    {
                                        MainWindow.HideApp();
                                    }
                                }
                                else
                                {
                                    MainWindow.HideApp();
                                }
                            }
                        }
                        p.Start();
                        if (useRelativePath)
                        {
                            //如果使用相对路径启动成功 那么重新设置程序绝对路径
                            icon.Path = path;
                        }
                    }
                }
                icon.Count++;

                //隐藏搜索框
                if (RunTimeStatus.SEARCH_BOX_SHOW)
                {
                    MainWindow.mainWindow.HidedSearchBox();
                }
            }
            catch (Exception e)
            {
                if (!useRelativePath)
                {
                    StartIconApp(icon, type, true);
                }
                else
                {
                    HandyControl.Controls.Growl.WarningGlobal("程序启动失败(可能为不支持的启动方式)!");
                    LogUtil.WriteErrorLog(e, "程序启动失败:path=" + icon.Path + ",type=" + type);
                }
            }
        }



        private void StartSystemApp(string startArg, IconStartType type)
        {
            if (type == IconStartType.SHOW_IN_EXPLORE)
            {
                Growl.WarningGlobal("系统项目不支持打开文件位置操作!");
                return;
            }
            switch (startArg)
            {
                case "Calculator":
                    Process.Start("calc.exe");
                    break;
                case "Computer":
                    Process.Start("explorer.exe");
                    break;
                case "GroupPolicy":
                    Process.Start("gpedit.msc");
                    break;
                case "Notepad":
                    Process.Start("notepad");
                    break;
                case "Network":
                    Process.Start("ncpa.cpl");
                    break;
                case "RecycleBin":
                    Process.Start("shell:RecycleBinFolder");
                    break;
                case "Registry":
                    Process.Start("regedit.exe");
                    break;
                case "Mstsc":
                    if (type == IconStartType.ADMIN_STARTUP)
                    {
                        Process.Start("mstsc", "-admin");
                    }
                    else
                    {
                        Process.Start("mstsc");
                    }
                    break;
                case "Control":
                    Process.Start("Control");
                    break;
                case "CMD":
                    if (type == IconStartType.ADMIN_STARTUP)
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "cmd.exe";
                            process.StartInfo.Verb = "runas";
                            process.Start();
                        }
                    }
                    else
                    {
                        Process.Start("cmd");
                    }
                    break;
                case "Services":
                    Process.Start("services.msc");
                    break;
            }
            //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
            if (appData.AppConfig.MarginHide)
            {
                if (!MarginHide.IS_HIDE)
                {
                    MainWindow.HideApp();
                }
            }
            else
            {
                MainWindow.HideApp();
            }
        }

        /// <summary>
        /// 拖动添加项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wrap_Drop(object sender, DragEventArgs e)
        {
            Array dropObject = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            if (dropObject == null) return;
            foreach (object obj in dropObject)
            {
                string path = (string)obj;
                IconInfo iconInfo = CommonCode.GetIconInfoByPath(path);
                MainWindow.appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList.Add(iconInfo);
            }
            CommonCode.SortIconList();
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
        }

        /// <summary>
        /// 从列表删除图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveIcon(object sender, RoutedEventArgs e)
        {
            appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList.Remove((IconInfo)((MenuItem)sender).Tag);
        }

        private void SystemContextMenu(object sender, RoutedEventArgs e)
        {
            IconInfo icon = (IconInfo)((MenuItem)sender).Tag;
            DirectoryInfo[] folders = new DirectoryInfo[1];
            folders[0] = new DirectoryInfo(icon.Path);
            ShellContextMenu scm = new ShellContextMenu();
            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
            p.X -= 80;
            p.Y -= 80;
            scm.ShowContextMenu(folders, p);
        }

        /// <summary>
        /// 弹出Icon属性修改面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyConfig(object sender, RoutedEventArgs e)
        {
            IconInfo info = (IconInfo)((MenuItem)sender).Tag;
            switch (info.IconType)
            {
                case IconType.URL:
                    IconInfoUrlDialog urlDialog = new IconInfoUrlDialog(info);
                    urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog, "MainWindowDialog");
                    break;
                default:
                    IconInfoDialog dialog = new IconInfoDialog(info);
                    dialog.dialog = HandyControl.Controls.Dialog.Show(dialog, "MainWindowDialog");
                    break;
            }
        }

        private void MenuIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            RunTimeStatus.MOUSE_ENTER_ICON = true;
            if (!RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        IconInfo info = (sender as Panel).Tag as IconInfo;
                        MyPoptipContent.Text = info.Content;
                        MyPoptip.VerticalOffset = 30;
                        Thread.Sleep(50);
                        if (!RunTimeStatus.ICONLIST_MOUSE_WHEEL)
                        {
                            MyPoptip.IsOpen = true;
                        }
                    }));
                });
            }



            double width = appData.AppConfig.ImageWidth;
            double height = appData.AppConfig.ImageHeight;
            width += width * 0.15;
            height += height * 0.15;

            ThreadPool.QueueUserWorkItem(state =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ImgStoryBoard(sender, (int)width, (int)height, 1, true);
                }));
            });
        }

        private void MenuIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            RunTimeStatus.MOUSE_ENTER_ICON = false;
            MyPoptip.IsOpen = false;

            ThreadPool.QueueUserWorkItem(state =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ImgStoryBoard(sender, appData.AppConfig.ImageWidth, appData.AppConfig.ImageHeight, 260);
                }));
            });
        }


        private void ImgStoryBoard(object sender, int height, int width, int milliseconds, bool checkRmStoryboard = false)
        {

            if (appData.AppConfig.PMModel) return;

            //int count = 0;
            //Panel sp = sender as Panel;
            //Image img = sp.Children[0] as Image;


            //int nowH = (int)img.Height;

            //bool isSmall = nowH > height;

            //if (!isSmall)
            //{
            //    img.Height = height;
            //    img.Width = width;
            //    return;
            //}
            //double subLen = (double)Math.Abs(nowH - height) / (double)milliseconds;

            //new Thread(() =>
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        while (count < milliseconds)
            //        {
            //            if (!isSmall)
            //            {
            //                img.Height += subLen;
            //                img.Width += subLen;
            //            } else
            //            {
            //                //if (img.Height > 1)
            //                //{
            //                //    img.Height -= 1;
            //                //    img.Width -= 1;
            //                //}
            //                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            //                             new Action(delegate {
            //                                 img.Height -= subLen;
            //                                 img.Width -= subLen;
            //                             }));
            //                //img.Height -= subLen;
            //                //img.Width -= subLen;
            //            }
            //            count++;
            //            Thread.Sleep(1);
            //        }
            //        img.Height = height;
            //        img.Width = width;
            //    });
            //}).Start();




            Panel sp = sender as Panel;

            DependencyObject dos = sp.Parent;

            Image img = sp.Children[0] as Image;

            double afterHeight = img.Height;
            double afterWidth = img.Width;

            //动画定义
            Storyboard myStoryboard = new Storyboard();



            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = afterHeight,
                To = height,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
            };
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = afterWidth,
                To = width,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
            };


            Timeline.SetDesiredFrameRate(heightAnimation, 60);
            Timeline.SetDesiredFrameRate(widthAnimation, 60);

            Storyboard.SetTarget(widthAnimation, img);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTarget(heightAnimation, img);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));

            myStoryboard.Children.Add(heightAnimation);
            myStoryboard.Children.Add(widthAnimation);

            CheckRemoveStoryboard crs = new CheckRemoveStoryboard
            {
                sb = myStoryboard,
                sp = sp,
                heightAnimation = heightAnimation,
                widthAnimation = widthAnimation,
                img = img,
                isMouseOver = !checkRmStoryboard
            };

            heightAnimation.Completed += (s, e) =>
            {
                if (checkRmStoryboard)
                {
                    ThreadStart ts = new ThreadStart(crs.Remove);
                    System.Threading.Thread t = new System.Threading.Thread(ts);
                    t.IsBackground = true;
                    t.Start();
                }
                else
                {
                    img.BeginAnimation(WidthProperty, null);
                    img.BeginAnimation(HeightProperty, null);
                }
            };
            img.BeginAnimation(WidthProperty, widthAnimation);
            img.BeginAnimation(HeightProperty, heightAnimation);
            //###################################################################
            //myStoryboard.Completed += (s, e) =>
            //{
            //    if (checkRmStoryboard)
            //    {
            //        ThreadStart ts = new ThreadStart(crs.Remove);
            //        System.Threading.Thread t = new System.Threading.Thread(ts);
            //        t.Start();
            //    }
            //    else
            //    {
            //        myStoryboard.Remove();
            //    }
            //};
            //myStoryboard.Begin();
        }

        private class CheckRemoveStoryboard
        {
            public Storyboard sb;
            public Panel sp;
            public Image img;
            public DoubleAnimation heightAnimation;
            public DoubleAnimation widthAnimation;
            public bool isMouseOver;
            public void Remove()
            {
                while (true)
                {
                    if (sp.IsMouseOver == isMouseOver)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            img.BeginAnimation(WidthProperty, null);
                            img.BeginAnimation(HeightProperty, null);
                            //heightAnimation.FillBehavior = FillBehavior.Stop;
                            //widthAnimation.FillBehavior = FillBehavior.Stop;
                        }));
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
        }

        public void RemoveSB(Object sb)
        {
            Storyboard sb2 = sb as Storyboard;
            System.Threading.Thread.Sleep(500);
            sb2.Remove();
        }

        /// <summary>
        /// 添加URL项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUrlIcon(object sender, RoutedEventArgs e)
        {
            IconInfoUrlDialog urlDialog = new IconInfoUrlDialog();
            urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog, "MainWindowDialog");
        }

        /// <summary>
        /// 添加系统项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSystemIcon(object sender, RoutedEventArgs e)
        {
            SystemItemWindow.Show();
        }

        public void VisibilitySearchCard(Visibility vb)
        {
            VerticalCard.Visibility = vb;
            if (vb == Visibility.Visible)
            {
                WrapCard.Visibility = Visibility.Collapsed;
            }
            else
            {
                WrapCard.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 搜索Card点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerticalCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //隐藏搜索框
            if (RunTimeStatus.SEARCH_BOX_SHOW)
            {
                MainWindow.mainWindow.HidedSearchBox();
            }
        }

        /// <summary>
        /// 设置光标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CursorPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// 设置光标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CursorPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// 锁定/解锁主面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockAppPanel(object sender, RoutedEventArgs e)
        {
            RunTimeStatus.LOCK_APP_PANEL = !RunTimeStatus.LOCK_APP_PANEL;
        }

        private void WrapCard_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RunTimeStatus.LOCK_APP_PANEL)
            {
                CardLockCM.Header = "解锁主面板";
            }
            else
            {
                CardLockCM.Header = "锁定主面板";
            }
        }

        public void SearchListBoxIndexAdd()
        {
            //控制移动后 鼠标即使在图标上也不显示popup
            RunTimeStatus.MOUSE_MOVE_COUNT = 0;
            MyPoptip.IsOpen = false;

            if (SearchListBox.Items.Count > 0)
            {
                if (SearchListBox.SelectedIndex < SearchListBox.Items.Count - 1)
                {
                    SearchListBox.SelectedIndex += 1;
                }
            }
        }

        public void SearchListBoxIndexSub()
        {
            //控制移动后 鼠标即使在图标上也不显示popup
            RunTimeStatus.MOUSE_MOVE_COUNT = 0;
            MyPoptip.IsOpen = false;

            if (SearchListBox.Items.Count > 0)
            {
                if (SearchListBox.SelectedIndex > 0)
                {
                    SearchListBox.SelectedIndex -= 1;
                }
            }
        }

        public void StartupSelectionItem()
        {
            if (SearchListBox.SelectedItem != null)
            {
                IconInfo icon = SearchListBox.SelectedItem as IconInfo;
                if (icon.AdminStartUp)
                {
                    StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }
        }

        private void SearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchListBox.ScrollIntoView(SearchListBox.SelectedItem);
        }

        private void PDDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PDDialog.Visibility == Visibility.Visible)
            {
                RunTimeStatus.SHOW_MENU_PASSWORDBOX = true;
                PDDialog.ClearVal();
                PDDialog.ErrorMsg.Visibility = Visibility.Collapsed;
                PDDialog.PasswordGrid.Visibility = Visibility.Visible;
                PDDialog.HintGrid.Visibility = Visibility.Collapsed;
                PDDialog.count = 0;
                PDDialog.SetFocus();
            }
            else
            {
                RunTimeStatus.SHOW_MENU_PASSWORDBOX = false;
                PDDialog.ClearVal();
                MainWindow.mainWindow.Focus();
            }
        }

        /// <summary>
        /// 菜单结果icon 列表鼠标滚轮预处理时间  
        /// 主要使用自定义popup解决卡顿问题解决卡顿问题
        /// 以及滚动条首尾切换菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconListBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            //控制在滚动时不显示popup 否则会在低GPU性能机器上造成卡顿
            MyPoptip.IsOpen = false;
            if (RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                RunTimeStatus.MOUSE_WHEEL_WAIT_MS = 500;
            }
            else
            {
                RunTimeStatus.ICONLIST_MOUSE_WHEEL = true;

                new Thread(() =>
                {
                    while (RunTimeStatus.MOUSE_WHEEL_WAIT_MS > 0)
                    {
                        Thread.Sleep(1);
                        RunTimeStatus.MOUSE_WHEEL_WAIT_MS -= 1;
                    }
                    if (RunTimeStatus.MOUSE_ENTER_ICON)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MyPoptip.IsOpen = true;
                        }));
                    }
                    RunTimeStatus.MOUSE_WHEEL_WAIT_MS = 100;
                    RunTimeStatus.ICONLIST_MOUSE_WHEEL = false;
                }).Start();
            }

            //修改菜单时不切换菜单
            if (RunTimeStatus.IS_MENU_EDIT) return;


            //切换菜单
            System.Windows.Controls.ScrollViewer scrollViewer = sender as System.Windows.Controls.ScrollViewer;
            if (scrollViewer == null)
            {
                //在card 上获取的事件
                scrollViewer = ScrollUtil.FindSimpleVisualChild<System.Windows.Controls.ScrollViewer>(IconListBox);
            }
            if (e.Delta < 0)
            {
                int index = MainWindow.mainWindow.LeftCard.MenuListBox.SelectedIndex;
                if (ScrollUtil.IsBootomScrollView(scrollViewer))
                {
                    if (index < MainWindow.mainWindow.LeftCard.MenuListBox.Items.Count - 1)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                    MainWindow.mainWindow.LeftCard.MenuListBox.SelectedIndex = index;
                    scrollViewer.ScrollToVerticalOffset(0);
                }
            }
            else if (e.Delta > 0)
            {
                if (ScrollUtil.IsTopScrollView(scrollViewer))
                {
                    int index = MainWindow.mainWindow.LeftCard.MenuListBox.SelectedIndex;
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = MainWindow.mainWindow.LeftCard.MenuListBox.Items.Count - 1;
                    }
                    MainWindow.mainWindow.LeftCard.MenuListBox.SelectedIndex = index;
                    scrollViewer.ScrollToVerticalOffset(0);
                }
            }


        }

        /// <summary>
        /// 搜索结果icon 列表鼠标滚轮预处理时间  
        /// 主要使用自定义popup解决卡顿问题解决卡顿问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerticalIconList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //控制在滚动时不显示popup 否则会在低GPU性能机器上造成卡顿
            MyPoptip.IsOpen = false;
            if (RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                RunTimeStatus.MOUSE_WHEEL_WAIT_MS = 500;
            }
            else
            {
                RunTimeStatus.ICONLIST_MOUSE_WHEEL = true;

                new Thread(() =>
                {
                    while (RunTimeStatus.MOUSE_WHEEL_WAIT_MS > 0)
                    {
                        Thread.Sleep(1);
                        RunTimeStatus.MOUSE_WHEEL_WAIT_MS -= 1;
                    }
                    if (RunTimeStatus.MOUSE_ENTER_ICON)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MyPoptip.IsOpen = true;
                        }));
                    }
                    RunTimeStatus.MOUSE_WHEEL_WAIT_MS = 100;
                    RunTimeStatus.ICONLIST_MOUSE_WHEEL = false;
                }).Start();
            }
        }

        /// <summary>
        /// 查询结果 ICON 鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchIcon_MouseEnter(object sender, MouseEventArgs e)
        {

            //显示popup
            RunTimeStatus.MOUSE_ENTER_ICON = true;
            if (!RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                new Thread(() =>
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        IconInfo info = (sender as Panel).Tag as IconInfo;
                        MyPoptipContent.Text = info.Content;
                        MyPoptip.VerticalOffset = 30;
                        Thread.Sleep(100);
                        if (!RunTimeStatus.ICONLIST_MOUSE_WHEEL && RunTimeStatus.MOUSE_MOVE_COUNT > 1)
                        {
                            MyPoptip.IsOpen = true;
                        }
                    }));
                }).Start();
            }
        }

        /// <summary>
        /// 查询结果ICON鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            RunTimeStatus.MOUSE_ENTER_ICON = false;
            MyPoptip.IsOpen = false;
        }

        /// <summary>
        /// 查询结果ICON鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchIcon_MouseMove(object sender, MouseEventArgs e)
        {
            //控制首次刷新搜索结果后, 鼠标首次移动后显示popup
            RunTimeStatus.MOUSE_MOVE_COUNT++;

            //防止移动后不刷新popup content
            IconInfo info = (sender as Panel).Tag as IconInfo;
            MyPoptipContent.Text = info.Content;
            MyPoptip.VerticalOffset = 30;

            if (RunTimeStatus.MOUSE_MOVE_COUNT > 1 && !RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                MyPoptip.IsOpen = true;
            }
        }

        /// <summary>
        /// menu结果ICON鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuIcon_MouseMove(object sender, MouseEventArgs e)
        {
            //防止移动后不刷新popup content
            IconInfo info = (sender as Panel).Tag as IconInfo;
            MyPoptipContent.Text = info.Content;
            MyPoptip.VerticalOffset = 30;
        }

        private void VerticalCard_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (appData.AppConfig.EnableEveryThing == true && EveryThingUtil.HasNext())
            {
                HandyControl.Controls.ScrollViewer sv = sender as HandyControl.Controls.ScrollViewer;
                if (sv.ExtentHeight - (sv.ActualHeight + sv.VerticalOffset) < 200 && EveryThingUtil.HasNext())
                {
                    string[] split = MainWindow.mainWindow.TotalMsgBtn.Content.ToString().Split(' ');
                    long count = Convert.ToInt64(split[0]);
                    ObservableCollection<IconInfo> iconBakList = EveryThingUtil.NextPage();
                    count += iconBakList.Count;

                    this.Dispatcher.Invoke(() =>
                    {
                        MainWindow.mainWindow.TotalMsgBtn.Content = count + " of " + split[split.Length - 1];
                        foreach (IconInfo icon in iconBakList)
                        {
                            if (RunTimeStatus.EVERYTHING_NEW_SEARCH) return;
                            SearchIconList.IconList.Add(icon);
                        }
                    });
                    //异步加载图标
                    if (iconBakList != null && iconBakList.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            foreach (IconInfo icon in iconBakList)
                            {
                                if (RunTimeStatus.EVERYTHING_NEW_SEARCH) return;
                                this.Dispatcher.Invoke(() =>
                                {
                                    icon.BitmapImage_NoWrite = ImageUtil.GetBitmapIconByUnknownPath(icon.Path);
                                });
                            }
                        });
                    }

                }
            }
        }



    }
}
