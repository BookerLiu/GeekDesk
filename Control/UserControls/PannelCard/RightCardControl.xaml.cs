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
                Process p = new Process();
                p.StartInfo.FileName = icon.Path;
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
                            p.StartInfo.Arguments = "1";//启动参数
                            p.StartInfo.Verb = "runas";
                            p.StartInfo.CreateNoWindow = false; //设置显示窗口
                            p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动进程
                            p.StartInfo.ErrorDialog = false;
                            if (appData.AppConfig.AppHideType == AppHideType.START_EXE)
                            {
                                //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                if (appData.AppConfig.MarginHide)
                                {
                                    if (!MainWindow.hide.IsMargin())
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
                            if (appData.AppConfig.AppHideType == AppHideType.START_EXE)
                            {
                                //如果开启了贴边隐藏 则窗体不贴边才隐藏窗口
                                if (appData.AppConfig.MarginHide)
                                {
                                    if (!MainWindow.hide.IsMargin())
                                    {
                                        MainWindow.HideApp();
                                    }
                                } else
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

                //string base64 = ImageUtil.FileImageToBase64(path, ImageFormat.Png);

                string ext = System.IO.Path.GetExtension(path).ToLower();

                if (".lnk".Equals(ext))
                {
                    string targetPath = FileUtil.GetTargetPathByLnk(path);
                    if (targetPath!=null)
                    {
                        path = targetPath;
                    }
                }

                BitmapImage bi = ImageUtil.GetBitmapIconByPath(path);
                IconInfo iconInfo = new IconInfo
                {
                    Path = path,
                    BitmapImage = bi
                };
                iconInfo.DefaultImage = iconInfo.ImageByteArr;
                iconInfo.Name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (StringUtil.IsEmpty(iconInfo.Name))
                {
                    iconInfo.Name = path;
                }
                MainWindow.appData.MenuList[appData.AppConfig.SelectedMenuIndex].IconList.Add(iconInfo);
                CommonCode.SaveAppData(MainWindow.appData);
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
                    urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog);
                    break;
                default:
                    IconInfoDialog dialog = new IconInfoDialog(info);
                    dialog.dialog = HandyControl.Controls.Dialog.Show(dialog);
                    break;
            }
            
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ImgStoryBoard(sender, (int)CommonEnum.IMAGE_HEIGHT_AM, (int)CommonEnum.IMAGE_WIDTH_AM, 1);
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            ImgStoryBoard(sender, (int)CommonEnum.IMAGE_HEIGHT, (int)CommonEnum.IMAGE_WIDTH, 220);
        }


        private void ImgStoryBoard(object sender, int height, int width, int milliseconds)
        {

            if (appData.AppConfig.PMModel) return;

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


            Timeline.SetDesiredFrameRate(heightAnimation, 60);
            Timeline.SetDesiredFrameRate(widthAnimation, 60);

            img.BeginAnimation(HeightProperty, null);
            img.BeginAnimation(WidthProperty, null);

            img.BeginAnimation(HeightProperty, heightAnimation);
            img.BeginAnimation(WidthProperty, widthAnimation);
        }

        private void AddUrlIcon(object sender, RoutedEventArgs e)
        {
            IconInfoUrlDialog urlDialog = new IconInfoUrlDialog();
            urlDialog.dialog = HandyControl.Controls.Dialog.Show(urlDialog);
        }
    }
}
