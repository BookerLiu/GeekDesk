using System;
using System.Collections.Generic;

using System.Windows;

using System.Windows.Input;
using System.Windows.Media.Imaging;

using GeekDesk.ViewModel;
using System.IO;
using GeekDesk.Util;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using WPF.JoshSmith.ServiceProviders.UI;
using DraggAnimatedPanelExample;
using System.ComponentModel;

namespace GeekDesk
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private static MainModel mainModel;

        ListViewDragDropManager<ViewModel.Menu> dragMgr;
        ListViewDragDropManager<ViewModel.DataInfos> dragMgr2;
        public MainWindow()
        {
            InitializeComponent();

            mainModel = new MainModel();
            //this.DataContext = mainModel;
            //menu.Items = mainModel;
            //System.Diagnostics.Process.Start(@"D:\SoftWare\WeGame\wegame.exe");
            this.Loaded += Window_Loaded;
            this.SizeChanged += MainWindow_Resize;
        }

        DelegateCommand<int[]> _swap;
        public DelegateCommand<int[]> SwapCommand
        {
            get
            {
                if (_swap == null)
                    _swap = new DelegateCommand<int[]>(
                        (indexes) =>
                        {
                            int fromS = indexes[0];
                            int to = indexes[1];
                            var elementSource = data.Items[to];
                            var dragged = data.Items[fromS];
                            if (fromS > to)
                            {
                                data.Items.Remove(dragged);
                                data.Items.Insert(to, dragged);
                            }
                            else
                            {
                                data.Items.Remove(dragged);
                                data.Items.Insert(to, dragged);
                            }
                        }
                    );
                return _swap;
            }
        }
        DelegateCommand<int[]> _swap2;
        public DelegateCommand<int[]> SwapCommand2
        {
            get
            {
                if (_swap2 == null)
                    _swap2 = new DelegateCommand<int[]>(
                        (indexes) =>
                        {
                            int fromS = indexes[0];
                            int to = indexes[1];
                            var elementSource = menu.Items[to];
                            var dragged = menu.Items[fromS];
                            if (fromS > to)
                            {
                                menu.Items.Remove(dragged);
                                menu.Items.Insert(to, dragged);
                            }
                            else
                            {
                                menu.Items.Remove(dragged);
                                menu.Items.Insert(to, dragged);
                            }
                        }
                    );
                return _swap2;
            }
        }



        private void Wrap_Drop(object sender, DragEventArgs e)
        {
            Array dropObject = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            if (dropObject == null) return;
            string path = (string)dropObject.GetValue(0);
            if (File.Exists(path))
            {
                // 文件
                BitmapImage bi = FileIcon.GetBitmapImage(path);
                DataInfos infos = new DataInfos();
                infos.Path = path;
                infos.BitmapImage = bi;
                infos.Name = Path.GetFileNameWithoutExtension(path);
                data.Items.Add(infos);
                data.Items.Refresh();
            }
            else if (Directory.Exists(path))
            {
                //文件夹

            }

        }

        //菜单点击事件
        private void menuClick(object sender, MouseButtonEventArgs e)
        {

        }



        /// <summary>
        /// 图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataClick(object sender, MouseButtonEventArgs e)
        {
            //string path = ((StackPanel)sender).Tag.ToString();
            //System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// data选中事件 设置不可选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (data.SelectedIndex != -1) data.SelectedIndex = -1;
        }

        #region Window_Loaded
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppConfig config = CommonCode.GetAppConfig();
            this.Width = config.WindowWidth;
            this.Height = config.WindowHeight;
            this.DataContext = config;

            this.menu.Items.Add(new ViewModel.Menu() { menu = "test1" });
            this.menu.Items.Add(new ViewModel.Menu() { menu = "test2" });
            this.menu.Items.Add(new ViewModel.Menu() { menu = "test3" });
        }
        #endregion // Window_Loaded

        #region Window_Closing
        void Window_Closing(object sender, CancelEventArgs e)
        {
            Rect rect = this.RestoreBounds;
            AppConfig config = this.DataContext as AppConfig;
            config.WindowWidth = rect.Width;
            config.WindowHeight = rect.Height;
            CommonCode.SaveAppConfig(config);
        }
        #endregion // Window_Closing

        void MainWindow_Resize(object sender, System.EventArgs e)
        {
            if (this.DataContext != null)
            {
                AppConfig config = this.DataContext as AppConfig;
                config.WindowWidth = this.Width;
                config.WindowHeight = this.Height;
                CommonCode.SaveAppConfig(config);
            }

        }



        #region dragMgr_ProcessDrop

        // Performs custom drop logic for the top ListView.
        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<object> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.

            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
                // The item came from the lower ListView
                // so just insert it.
                e.ItemsSource.Insert(higherIdx, e.DataItem);
            }
            else
            {
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
                e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = DragDropEffects.Move;
        }

        #endregion // dragMgr_ProcessDrop

        #region OnListViewDragEnter

        // Handles the DragEnter event for both ListViews.
        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        #endregion // OnListViewDragEnter

        #region OnListViewDrop

        // Handles the Drop event for both ListViews.
        void OnListViewDrop(object sender, DragEventArgs e)
        {
            if (e.Effects == DragDropEffects.None)
                return;
            ViewModel.Menu menuV = e.Data.GetData(typeof(ViewModel.Menu)) as ViewModel.Menu;
            DataInfos data = e.Data.GetData(typeof(DataInfos)) as DataInfos;

            if (sender == this.menu)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;

                // An item was dragged from the bottom ListView into the top ListView
                // so remove that item from the bottom ListView.
                (this.data.ItemsSource as ObservableCollection<DataInfos>).Remove(data);
            }
            else
            {
                if (this.dragMgr2.IsDragInProgress)
                    return;

                // An item was dragged from the top ListView into the bottom ListView
                // so remove that item from the top ListView.
                (this.menu.ItemsSource as ObservableCollection<ViewModel.Menu>).Remove(menuV);
            }
        }

        #endregion // OnListViewDrop

        private void leftCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void deleteMenu(object sender, RoutedEventArgs e)
        {
            //if (data.SelectedIndex == -1)
            //{
            //    return;
            //}
            ViewModel.Menu pojo = (ViewModel.Menu)((ContextMenu)((MenuItem)sender).Parent).DataContext;
            string menuTitle = pojo.menu;
            int index = 0;
            foreach (object obj in menu.Items)
            {
                string test = ((ViewModel.Menu)obj).menu;
                if (test == menuTitle)
                {
                    menu.Items.RemoveAt(index);
                    menu.Items.Refresh();
                    return;
                }
                index++;
            }

        }

        public Double ConvertString(string val)
        {
            return Convert.ToDouble(val);
        }

    }




    public class MainModel : ViewModelBase
    {
        public List<ViewModel.Menu> MenuList { get; set; }

        public List<ViewModel.DataInfos> DataList { get; set; }


    }


}
