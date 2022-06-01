using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void StartIconApp(IconInfo icon, IconStartType type)
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
                        p.StartInfo.FileName = icon.Path;
                        if (!StringUtil.IsEmpty(startArg))
                        {
                            p.StartInfo.Arguments = startArg;
                        }
                        if (icon.IconType == IconType.OTHER)
                        {
                            if (!File.Exists(icon.Path) && !Directory.Exists(icon.Path))
                            {
                                HandyControl.Controls.Growl.WarningGlobal("程序启动失败(文件路径不存在或已删除)!");
                                return;
                            }
                            p.StartInfo.WorkingDirectory = icon.Path.Substring(0, icon.Path.LastIndexOf("\\"));
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
                HandyControl.Controls.Growl.WarningGlobal("程序启动失败(不支持的启动方式)!");
                LogUtil.WriteErrorLog(e, "程序启动失败:path=" + icon.Path + ",type=" + type);
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
            CommonCode.SaveAppData(MainWindow.appData);
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
                    urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog, "IconInfoDialog");
                    break;
                default:
                    IconInfoDialog dialog = new IconInfoDialog(info);
                    dialog.dialog = HandyControl.Controls.Dialog.Show(dialog, "IconInfoDialog");
                    break;
            }
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {

            double width = appData.AppConfig.ImageWidth;
            double height = appData.AppConfig.ImageHeight;
            width += width * 0.15;
            height += height * 0.15;
            Thread t = new Thread(() =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ImgStoryBoard(sender, (int)width, (int)height, 1, true);
                }));
            });
            t.IsBackground = true;
            t.Start();

        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {

            Thread t = new Thread(() =>
            {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ImgStoryBoard(sender, appData.AppConfig.ImageWidth, appData.AppConfig.ImageHeight, 260);
            }));
            });
            t.IsBackground = true;
            t.Start();

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
            urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog, "IconInfoDialog");
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
            } else
            {
                CardLockCM.Header = "锁定主面板";
            }
        }
    }
}
