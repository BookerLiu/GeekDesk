using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;


namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CustomIconUrlDialog
    {

        public CustomIconUrlDialog(AppConfig appConfig)
        {
            InitializeComponent();
            this.DataContext = appConfig;
        }

        public CustomIconUrlDialog()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            AppConfig appConfig = this.DataContext as AppConfig;
            appConfig.CustomIconJsonUrl = JsonUrl.Text.Trim();
            appConfig.CustomIconUrl = IconUrl.Text.Trim();
            if (!StringUtil.IsEmpty(IconUrl.Text) && !StringUtil.IsEmpty(JsonUrl.Text))
            {
                IconfontWindow.vm.IsSettingUrl = "true";
            }
            else
            {
                IconfontWindow.vm.IsSettingUrl = "false";
            }
        }


        private void Teach_Click(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings["CustomIconTeachUrl"];
            Process.Start(url);
        }
    }
}
