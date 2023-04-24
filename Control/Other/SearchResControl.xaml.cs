using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Plugins.EveryThing;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using GeekDesk.ViewModel.Temp;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// SearchResControl.xaml 的交互逻辑
    /// </summary>
    public partial class SearchResControl : UserControl
    {
        public SearchResControl(ObservableCollection<IconInfo> iconList)
        {
            this.DataContext = iconList;
            InitializeComponent();
        }



        public void SearchListBoxIndexAdd()
        {
            //控制移动后 鼠标即使在图标上也不显示popup
            RunTimeStatus.MOUSE_MOVE_COUNT = 0;
            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = false;

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
            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = false;

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
                    ProcessUtil.StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    ProcessUtil.StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }
        }

        private void SearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchListBox.ScrollIntoView(SearchListBox.SelectedItem);
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
            MainWindow.mainWindow.RightCard.MyPoptipContent.Text = info.Content;
            MainWindow.mainWindow.RightCard.MyPoptip.VerticalOffset = 30;

            if (RunTimeStatus.MOUSE_MOVE_COUNT > 1 && !RunTimeStatus.ICONLIST_MOUSE_WHEEL)
            {
                MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = true;
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
                        MainWindow.mainWindow.RightCard.MyPoptipContent.Text = info.Content;
                        MainWindow.mainWindow.RightCard.MyPoptip.VerticalOffset = 30;
                        Thread.Sleep(100);
                        if (!RunTimeStatus.ICONLIST_MOUSE_WHEEL && RunTimeStatus.MOUSE_MOVE_COUNT > 1)
                        {
                            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = true;
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
            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = false;
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
            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = false;
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
                            MainWindow.mainWindow.RightCard.MyPoptip.IsOpen = true;
                        }));
                    }
                    RunTimeStatus.MOUSE_WHEEL_WAIT_MS = 100;
                    RunTimeStatus.ICONLIST_MOUSE_WHEEL = false;
                }).Start();
            }
        }


        private void Icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.appData.AppConfig.DoubleOpen)
            {
                IconClick(sender, e);
            }
        }

        private void Icon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.appData.AppConfig.DoubleOpen)
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
            if (MainWindow.appData.AppConfig.DoubleOpen && e.ClickCount >= 2)
            {
                IconInfo icon = (IconInfo)((Panel)sender).Tag;
                if (icon.AdminStartUp)
                {
                    ProcessUtil.StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    ProcessUtil.StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }
            else if (!MainWindow.appData.AppConfig.DoubleOpen && e.ClickCount == 1)
            {
                IconInfo icon = (IconInfo)((Panel)sender).Tag;
                if (icon.AdminStartUp)
                {
                    ProcessUtil.StartIconApp(icon, IconStartType.ADMIN_STARTUP);
                }
                else
                {
                    ProcessUtil.StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
                }
            }

        }


        private static volatile bool EveryThingRuning = false;
        private void VerticalCard_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (MainWindow.appData.AppConfig.EnableEveryThing == true && EveryThingUtil.HasNext())
            {
                HandyControl.Controls.ScrollViewer sv = sender as HandyControl.Controls.ScrollViewer;
                if (sv.ExtentHeight - (sv.ActualHeight + sv.VerticalOffset) < 100 
                    && EveryThingUtil.HasNext()
                    && !EveryThingRuning)
                {
                    EveryThingRuning = true;
                    MainWindow.mainWindow.RightCard.Loading_RightCard.Visibility = Visibility.Visible;
                    int everyThingCount = Convert.ToInt32(MainWindow.mainWindow.EverythingSearchCount.Text);

                    ObservableCollection<IconInfo> resList = this.DataContext as ObservableCollection<IconInfo>;

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        ObservableCollection<IconInfo> searchRes = EveryThingUtil.NextPage();
                        this.Dispatcher.Invoke(() =>
                        {
                            everyThingCount += searchRes.Count;
                            MainWindow.mainWindow.EverythingSearchCount.Text = Convert.ToString(everyThingCount);
                            foreach (IconInfo info in searchRes)
                            {
                                resList.Add(info);
                            }
                            MainWindow.mainWindow.RightCard.Loading_RightCard.Visibility = Visibility.Collapsed;
                            EveryThingRuning = false;
                        });
                    });

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
            ProcessUtil.StartIconApp(icon, IconStartType.ADMIN_STARTUP);
        }

        /// <summary>
        /// 打开文件所在位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowInExplore(object sender, RoutedEventArgs e)
        {
            IconInfo icon = (IconInfo)((MenuItem)sender).Tag;
            ProcessUtil.StartIconApp(icon, IconStartType.SHOW_IN_EXPLORE);
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









    }
}
