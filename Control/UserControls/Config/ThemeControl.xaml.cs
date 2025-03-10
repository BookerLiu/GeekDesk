using GeekDesk.Constant;
using GeekDesk.Control.Other;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GeekDesk.Control.UserControls.Config
{

    /// <summary>
    /// MotionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThemeControl : System.Windows.Controls.UserControl
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;

        public ThemeControl()
        {

            InitializeComponent();
            if (appConfig.BGStyle != BGStyle.GradientBac)
            {
                GradientBGConf.Visibility = Visibility.Collapsed;
                ImgBGConf.Visibility = Visibility.Visible;
            }
            else
            {
                ImgBGConf.Visibility = Visibility.Collapsed;
                GradientBGConf.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 修改背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Multiselect = false, //只允许选中单个文件
                    Filter = "图像文件(*.png, *.jpg, *.gif)|*.png;*.jpg;*.gif"
                };
                if (ofd.ShowDialog() == true)
                {
                    appConfig.BitmapImage = ImageUtil.GetBitmapImageByFile(ofd.FileName);
                    appConfig.BacImgName = ofd.FileName;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteErrorLog(ex, "修改背景失败,已重置为默认背景!");
                HandyControl.Controls.Growl.WarningGlobal("修改背景失败,已重置为默认背景!");
            }
            BGSettingUtil.BGSetting();
        }


        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                appConfig.BitmapImage = ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_BAC_IMAGE_BASE64);
                appConfig.BacImgName = "系统默认";
            }
            catch (Exception ex)
            {
                LogUtil.WriteErrorLog(ex, "修改背景失败2,已重置为默认背景!");
                HandyControl.Controls.Growl.WarningGlobal("修改背景失败,已重置为默认背景!");
            }
            BGSettingUtil.BGSetting();
        }


        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            new MyColorPickerDialog(tag, "ConfigWindowDialog");
        }


        private void PreviewSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckButtonUpClass cbu = new CheckButtonUpClass
            {
                e = e
            };
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(cbu.CheckButtonUp);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.IsBackground = true;
            t.Start();
        }

        private class CheckButtonUpClass
        {
            public MouseButtonEventArgs e;

            public void CheckButtonUp()
            {
                while (true)
                {
                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            AppData appData = MainWindow.appData;
                            ObservableCollection<IconInfo> selectIcons = appData.AppConfig.SelectedMenuIcons;
                            appData.AppConfig.SelectedMenuIcons = null;
                            appData.AppConfig.SelectedMenuIcons = selectIcons;
                        }));
                        return;
                    }
                    System.Threading.Thread.Sleep(50);
                }
            }
        }


        public void BGStyle_Changed(object sender, RoutedEventArgs e)
        {
            BGSettingUtil.BGSetting();
            if (appConfig.BGStyle != BGStyle.GradientBac)
            {
                GradientBGConf.Visibility = Visibility.Collapsed;
                ImgBGConf.Visibility = Visibility.Visible;
            }
            else
            {
                ImgBGConf.Visibility = Visibility.Collapsed;
                GradientBGConf.Visibility = Visibility.Visible;
            }
        }

        private void BGOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BGSettingUtil.BGSetting();
        }

        private void Color_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            BGSettingUtil.BGSetting();
        }

        private void SysBG_Click(object sender, RoutedEventArgs e)
        {
            GradientBGDialog gbg = new GradientBGDialog();
            gbg.dialog = HandyControl.Controls.Dialog.Show(gbg, "ConfigWindowDialog");
        }


        private void Animation_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.mainWindow.Visibility == Visibility.Collapsed)
            {
                appConfig.IsShow = true;
            }
            appConfig.IsShow = null;
        }

        /// <summary>
        /// 保存当前颜色到系统预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewBGBtn_Click(object sender, RoutedEventArgs e)
        {
            BGNmaeDialog dialog = new BGNmaeDialog();
            dialog.dialog = HandyControl.Controls.Dialog.Show(dialog, "ConfigWindowDialog");
        }
    }
}
