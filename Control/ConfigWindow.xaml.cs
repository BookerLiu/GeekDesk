
using GalaSoft.MvvmLight.Command;
using GeekDesk.Control.UserControls;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;

namespace GeekDesk.Control
{
    /// <summary>
    /// ConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow
    {
        public ConfigWindow(AppConfig appConfig)
        {
            InitializeComponent();
            this.DataContext = appConfig;
            LeftCard.Content = new SettingControl();
            this.Topmost = true;
        }
        
    }
}
