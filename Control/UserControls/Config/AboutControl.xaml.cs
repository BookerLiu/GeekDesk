using GeekDesk.Constant;
using GeekDesk.Util;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            AppInfo.Text += ConfigurationManager.AppSettings["Version"];
            string showPublicWeChat = ConfigurationManager.AppSettings["ShowPublicWeChat"];
            if ("Y".Equals(showPublicWeChat))
            {
                PublicWeChatPanel.Visibility = Visibility.Visible;
            } else
            {
                PublicWeChatPanel.Visibility = Visibility.Collapsed;
            }
            
            PublicWeChat.Source = ImageUtil.Base64ToBitmapImage(Constants.PUBLIC_WE_CHAT_IMG_BASE64);
            WeChatCode.Source = ImageUtil.Base64ToBitmapImage(Constants.WE_CHAT_CODE_IMG_BASE64);
            ZFBCode.Source = ImageUtil.Base64ToBitmapImage(Constants.ZFB_CODE_IMG_BASE64);
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

        private void SC_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void SC_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
    }
}
