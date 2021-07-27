using DraggAnimatedPanelExample;
using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls.PannelCard
{
    /// <summary>
    /// RightCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class RightCardControl : UserControl
    {
        private AppData appData = MainWindow.appData;
        public RightCardControl()
        {
            InitializeComponent();
        }


        #region 图标拖动
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
                            ObservableCollection<IconInfo> iconList = appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList;
                            var elementSource = iconList[to];
                            var dragged = iconList[fromS];

                            iconList.Remove(dragged);
                            iconList.Insert(to, dragged);
                        }
                    );
                return _swap;
            }
        }

        #endregion 图标拖动



        /// <summary>
        /// 图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconClick(object sender, MouseButtonEventArgs e)
        {
            IconInfo icon = (IconInfo)((StackPanel)sender).Tag;
            if (icon.AdminStartUp)
            {
                StartIconApp(icon, IconStartType.ADMIN_STARTUP);
            }
            else
            {
                StartIconApp(icon, IconStartType.DEFAULT_STARTUP);
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

                if (!File.Exists(icon.Path) && !Directory.Exists(icon.Path))
                {
                    HandyControl.Controls.Growl.WarningGlobal("程序启动失败(文件路径不存在或已删除)!");
                    return;
                } 

                Process p = new Process();
                p.StartInfo.FileName = icon.Path;

                switch (type)
                {
                    case IconStartType.ADMIN_STARTUP:
                        p.StartInfo.Arguments = "1";//启动参数
                        p.StartInfo.Verb = "runas";
                        p.StartInfo.CreateNoWindow = false; //设置显示窗口
                        p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动进程
                        p.StartInfo.ErrorDialog = false;
                        if (appData.AppConfig.AppHideType == AppHideType.START_EXE)
                        {
                            Window parentWin = Window.GetWindow(this);
                            parentWin.Visibility = Visibility.Collapsed;
                        }
                        break;// c#好像不能case穿透
                    case IconStartType.DEFAULT_STARTUP:
                        if (appData.AppConfig.AppHideType == AppHideType.START_EXE)
                        {
                            Window parentWin = Window.GetWindow(this);
                            parentWin.Visibility = Visibility.Collapsed;
                        }
                        break;
                    case IconStartType.SHOW_IN_EXPLORE:
                        p.StartInfo.FileName = "Explorer.exe";
                        p.StartInfo.Arguments = "/e,/select," + icon.Path;
                        break;
                }
                p.Start();
                icon.Count++;
            }
            catch (Exception)
            {
                HandyControl.Controls.Growl.WarningGlobal("程序启动失败(不支持的启动方式)!");
            }
        }




        private void Wrap_Drop(object sender, DragEventArgs e)
        {
            Array dropObject = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            if (dropObject == null) return;
            foreach (object obj in dropObject)
            {
                string path = (string)obj;

                //string base64 = ImageUtil.FileImageToBase64(path, ImageFormat.Jpeg);

                string ext = System.IO.Path.GetExtension(path).ToLower();

                if (".lnk".Equals(ext))
                {
                    string targetPath = FileUtil.GetTargetPathByLnk(path);
                    if (targetPath!=null)
                    {
                        path = targetPath;
                    }
                }

                IconInfo iconInfo = new IconInfo
                {
                    Path = path,
                    BitmapImage = ImageUtil.GetBitmapIconByPath(path)
                };
                iconInfo.DefaultImage = iconInfo.ImageByteArr;
                iconInfo.Name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (StringUtil.IsEmpty(iconInfo.Name))
                {
                    iconInfo.Name = path;
                }
                MainWindow.appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList.Add(iconInfo);
            }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            IconInfo icon = (IconInfo)((MenuItem)sender).Tag;
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + icon.Path;
            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 弹出Icon属性修改面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyConfig(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.Dialog.Show(new IconInfoDialog((IconInfo)((MenuItem)sender).Tag));
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ImgStroyBoard(sender, (int)MainWindowEnum.IMAGE_HEIGHT_AM, (int)MainWindowEnum.IMAGE_WIDTH_AM, 1);
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            ImgStroyBoard(sender, (int)MainWindowEnum.IMAGE_HEIGHT, (int)MainWindowEnum.IMAGE_WIDTH, 500);
        }


        private void ImgStroyBoard(object sender, int height, int width, int milliseconds)
        {

            StackPanel sp = sender as StackPanel;

            Image img = sp.Children[0] as Image;

            DoubleAnimation heightAnimation = new DoubleAnimation();
            DoubleAnimation widthAnimation = new DoubleAnimation();

            heightAnimation.From = img.Height;
            widthAnimation.From = img.Width;

            heightAnimation.To = height;
            widthAnimation.To = width;

            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            widthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));

            img.BeginAnimation(HeightProperty, heightAnimation);
            img.BeginAnimation(WidthProperty, widthAnimation);
        }

    }
}
