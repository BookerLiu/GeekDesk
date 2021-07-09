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
        private DataContextInfo dataContextInfo = new DataContextInfo();

        private IconfontWindow(List<IconfontInfo> listInfo, MenuInfo menuInfo)
        {
            InitializeComponent();
            dataContextInfo.iconListSystem = listInfo;
            this.DataContext = dataContextInfo;
            IconfontWindow.menuInfo = menuInfo;
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
            //this.DataContext = listInfo;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string tag = (MyTabControl.SelectedContent as TabItem).Tag.ToString();

            switch (tag)
            {
                case "Custom":
                    if (dataContextInfo.iconInfoCustom != null)
                    {
                        menuInfo.MenuGeometry = dataContextInfo.iconInfoCustom.Text;
                    }
                    break;
                default:
                    if (dataContextInfo.iconInfoSystem != null)
                    {
                        menuInfo.MenuGeometry = dataContextInfo.iconInfoSystem.Text;
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


        private class DataContextInfo
        {
            public List<IconfontInfo> iconListSystem;
            public List<IconfontInfo> iconListCustom;
            public IconfontInfo iconInfoSystem;
            public IconfontInfo iconInfoCustom;

            
        }
    }
}