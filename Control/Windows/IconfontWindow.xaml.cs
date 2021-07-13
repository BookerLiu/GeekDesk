using GeekDesk.Control.Other;
using GeekDesk.Util;
using GeekDesk.ViewModel;

using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;


namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// IconfontWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IconfontWindow : Window
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        private static MenuInfo menuInfo;
        private static List<IconfontInfo> systemIcons;
        private static List<IconfontInfo> customIcons;
        public static IconfontViewModel vm;
        private IconfontWindow(List<IconfontInfo> icons, MenuInfo menuInfo)
        {
            
            InitializeComponent();

            systemIcons = icons;
            this.Topmost = true;
            IconfontWindow.menuInfo = menuInfo;
            vm = new IconfontViewModel
            {
                Iconfonts = systemIcons
            };
            this.DataContext = vm;
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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem ti = this.MyTabControl.SelectedItem as TabItem;

            switch (ti.Tag.ToString())
            {
                case "Custom":
                    CustomButton.IsEnabled = true;
                    if (StringUtil.IsEmpty(appConfig.CustomIconUrl) || StringUtil.IsEmpty(appConfig.CustomIconJsonUrl))
                    {
                        LoadingEle.Visibility = Visibility.Visible;
                        CustomIcon.Visibility = Visibility.Collapsed;
                        HandyControl.Controls.Dialog.Show(new CustomIconUrlDialog(appConfig));
                    } else
                    {
                        if (customIcons == null)
                        {
                            LoadingOnlineIcon();
                        } else
                        {
                            vm.Iconfonts = customIcons;
                            LoadingEle.Visibility = Visibility.Collapsed;
                            CustomIcon.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                default:
                    if (CustomButton != null)
                    {
                        CustomButton.IsEnabled = false;
                    }
                    if (vm != null)
                    {
                        vm.Iconfonts = systemIcons;
                    }
                    break;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = this.MyTabControl.SelectedItem as TabItem;
            int index;
            switch (ti.Tag.ToString())
            {
                case "Custom":
                    index = this.CustomIcon.IconListBox.SelectedIndex;
                    if (index != -1)
                    {
                        menuInfo.MenuGeometry = customIcons[index].Text;
                    }
                    break;
                default:
                    index = this.SystemIcon.IconListBox.SelectedIndex;
                    if (index != -1)
                    {
                        menuInfo.MenuGeometry = systemIcons[index].Text;
                    }
                    break;
            }
            this.Close();
        }


        private static System.Windows.Window window = null;
        public static void Show(List<IconfontInfo> listInfo, MenuInfo menuInfo)
        {
            if (window == null || !window.Activate())
            {
                window = new IconfontWindow(listInfo, menuInfo);
            }
            window.Show();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.Dialog.Show(new CustomIconUrlDialog(appConfig));
        }


        private void CheckSettingUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckSettingUrl.Text == "true")
            {
                LoadingOnlineIcon();
            } else
            {
                LoadingEle.IsRunning = true;
                CustomIcon.Visibility = Visibility.Collapsed;
            }
        }


        private void LoadingOnlineIcon()
        {
            try
            {
                string svgJsStr = HttpUtil.Get(appConfig.CustomIconUrl);
                string jsonStr = HttpUtil.Get(appConfig.CustomIconJsonUrl);
                List<IconfontInfo> icons = SvgToGeometry.GetIconfonts(svgJsStr, jsonStr);
                customIcons = icons;
                vm.Iconfonts = customIcons;
                LoadingEle.Visibility = Visibility.Collapsed;
                CustomIcon.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                HandyControl.Controls.Growl.WarningGlobal("加载远程图标异常!");
            }
        }


        public class IconfontViewModel : INotifyPropertyChanged
        {
            private List<IconfontInfo> iconfonts;
            private string isSettingUrl;

            public List<IconfontInfo> Iconfonts
            {
                get
                {
                    return iconfonts;
                }
                set
                {
                    iconfonts = value;
                    OnPropertyChanged("Iconfonts");
                }
            }

            public string IsSettingUrl
            {
                get
                {
                    return isSettingUrl;
                }
                set
                {
                    isSettingUrl = value;
                    OnPropertyChanged("IsSettingUrl");
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}