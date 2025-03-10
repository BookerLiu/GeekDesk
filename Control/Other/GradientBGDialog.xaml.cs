using GeekDesk.Util;
using GeekDesk.ViewModel;
using GeekDesk.ViewModel.Temp;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GradientBGDialog
    {
        public HandyControl.Controls.Dialog dialog;

        public GradientBGDialog()
        {
            ObservableCollection<GradientBGParam> bgArr = DeepCopyUtil.DeepCopy(GradientBGParamList.GradientBGParams);
            foreach(var bg in MainWindow.appData.AppConfig.CustomBGParams)
            {
                bgArr.Add(bg);
            }

            this.DataContext = DeepCopyUtil.DeepCopy(bgArr);
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            dialog.Close();
        }

        private void BGBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GradientBGParam bgParam = (sender as Border).Tag as GradientBGParam;
            MainWindow.appData.AppConfig.GradientBGParam = bgParam;
            BGSettingUtil.BGSetting();
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
                Window.GetWindow(this).DragMove();
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.Growl.Ask("确认删除吗?", isConfirmed =>
            {
                if (isConfirmed)
                {
                    GradientBGParam bg = (GradientBGParam)(((MenuItem)sender).Tag);
                    ObservableCollection<GradientBGParam> bgArr = (ObservableCollection<GradientBGParam>)this.DataContext;
                    bgArr.Remove(bg);
                    MainWindow.appData.AppConfig.CustomBGParams.Remove(bg);
                    for (int i = MainWindow.appData.AppConfig.CustomBGParams.Count - 1; i >= 0; i--)
                    {
                        var cbg = MainWindow.appData.AppConfig.CustomBGParams[i];
                        if (cbg.Id == null)
                        {
                            if (cbg.Color1.Equals(bg.Color1) && cbg.Color2.Equals(bg.Color2))
                            {
                                MainWindow.appData.AppConfig.CustomBGParams.RemoveAt(i);
                            }
                        } else
                        {
                            if (cbg.Id.Equals(bg.Id))
                            {
                                MainWindow.appData.AppConfig.CustomBGParams.RemoveAt(i);
                            }
                        }
                        
                    }
                    MainWindow.appData.AppConfig.CustomBGParams = DeepCopyUtil.DeepCopy(MainWindow.appData.AppConfig.CustomBGParams);
                }
                return true;
            }, "ConfigWindowAskGrowl");
        }
    }
}
