using GeekDesk.Constant;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls.Config
{
    /// <summary>
    /// OtherControl.xaml 的交互逻辑
    /// </summary>
    public partial class OtherControl : UserControl
    {
        public OtherControl()
        {
            InitializeComponent();
        }

        private void SelfStartUpBox_Click(object sender, RoutedEventArgs e)
        {
            AppConfig appConfig = MainWindow.appData.AppConfig;
            RegisterUtil.SetSelfStarting(appConfig.SelfStartUp, Constants.MY_NAME);
        }
    }
}
