using GeekDesk.Control.UserControls.Backlog;
using GeekDesk.Interface;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// BacklogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToDoWindow : IWindowCommon
    {
        private static TodoControl backlog = new TodoControl();
        private AppData appData = MainWindow.appData;
        private ToDoWindow()
        {
            InitializeComponent();
            RightCard.Content = backlog;
            backlog.BacklogList.ItemsSource = appData.ToDoList;
            backlog.type = ToDoType.NEW;
            this.Topmost = true;
            if (backlog.BacklogList.Items.Count > 0)
            {
                backlog.NoData.Visibility = Visibility.Collapsed;
                backlog.BacklogList.Visibility = Visibility.Visible;
            }
            else
            {
                backlog.NoData.Visibility = Visibility.Visible;
                backlog.BacklogList.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MemuClick(object sender, RoutedEventArgs e)
        {
            SideMenuItem smi = sender as SideMenuItem;
            switch (smi.Tag.ToString())
            {
                case "History":
                    UFG.Visibility = Visibility.Collapsed;
                    //排序历史待办 倒序

                    List<ToDoInfo> list = appData.HiToDoList.OrderByDescending(v=>v.DoneTime).ToList();
                    appData.HiToDoList.Clear();
                    foreach (var item in list)
                    {
                        appData.HiToDoList.Add(item);
                    }
                    backlog.BacklogList.ItemsSource = appData.HiToDoList;
                    if (backlog.BacklogList.Items.Count > 0)
                    {
                        backlog.NoData.Visibility = Visibility.Collapsed;
                        backlog.BacklogList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        backlog.NoData.Visibility = Visibility.Visible;
                        backlog.BacklogList.Visibility = Visibility.Collapsed;
                    }
                    backlog.type = ToDoType.HISTORY;
                    backlog.IsNewToDo.Text = "N";
                    UFG.Visibility = Visibility.Visible;
                    break;
                default:
                    UFG.Visibility = Visibility.Collapsed;
                    backlog.BacklogList.ItemsSource = appData.ToDoList;
                    if (backlog.BacklogList.Items.Count > 0)
                    {
                        backlog.NoData.Visibility = Visibility.Collapsed;
                        backlog.BacklogList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        backlog.NoData.Visibility = Visibility.Visible;
                        backlog.BacklogList.Visibility = Visibility.Collapsed;
                    }
                    backlog.type = ToDoType.NEW;
                    backlog.IsNewToDo.Text = "Y";
                    UFG.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 新建待办
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBacklog_BtnClick(object sender, RoutedEventArgs e)
        {
            ToDoInfoWindow.ShowNone();
        }


        private static System.Windows.Window window = null;
        public static void Show()
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoWindow();
            }
            window.Show();
            Keyboard.Focus(window);
        }

        public static void ShowOrHide()
        {
            if (window == null || !window.Activate())
            {
                window = new ToDoWindow();
                window.Show();
                Keyboard.Focus(window);
            }
            else
            {
                window.Close();
            }
        }


        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DataContext = null;
                this.Close();
            }
        }
    }
}
