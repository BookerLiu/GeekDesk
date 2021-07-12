using GeekDesk.Control.Other;
using GeekDesk.Util;
using GeekDesk.ViewModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// IconfontWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IconfontWindow : Window
    {

        private static MenuInfo menuInfo;
        private static List<IconfontInfo> systemIcons;
        private static List<IconfontInfo> customIcons;
        private IconfontWindow(List<IconfontInfo> icons, MenuInfo menuInfo)
        {
            systemIcons = icons;
            this.DataContext = systemIcons;
            this.Topmost = true;
            IconfontWindow.menuInfo = menuInfo;
            InitializeComponent();
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
                    this.DataContext = customIcons;
                    break;
                default:
                    if (CustomButton != null)
                    {
                        CustomButton.IsEnabled = false;
                    }
                    this.DataContext = systemIcons;
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
            
            
        }
    }
}