using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CustomIconUrlDialog
    {

        public CustomIconUrlDialog(AppConfig appConfig)
        {
            this.DataContext = appConfig;
            InitializeComponent();
        }

        public CustomIconUrlDialog()
        {
            InitializeComponent();
        }

    }
}
