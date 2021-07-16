using System;
using System.Configuration;
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
using GeekDesk.Util;
using GeekDesk.Constant;

namespace GeekDesk.Control.UserControls.Config
{
    /// <summary>
    /// AboutControl.xaml 的交互逻辑
    /// </summary>
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
            AppInfo.Text += ConfigurationManager.AppSettings["version"];
            PublicWeChat.Source = ImageUtil.Base64ToBitmapImage(Constants.PUBLIC_WE_CHAT_IMG_BASE64);
            WeChatCode.Source = ImageUtil.Base64ToBitmapImage(Constants.WE_CHAT_CODE_IMG_BASE64);
            ZFBCode.Source = ImageUtil.Base64ToBitmapImage(Constants.ZFB_CODE_IMG_BASE64);
        }
    }
}
