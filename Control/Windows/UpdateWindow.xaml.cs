using GeekDesk.Constant;
using GeekDesk.Interface;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control.Windows
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window, IWindowCommon
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        private static string githubUrl = "";
        private static string giteeUrl = "";
        private UpdateWindow(JObject jo)
        {
            try
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.Topmost = true;
                InitializeComponent();
                DataHandle(jo);
            }
            catch (Exception)
            {

            }

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


        private void DataHandle(JObject jo)
        {
            Title.Text = StringUtil.IsEmpty(jo["title"]) ? "" : jo["title"].ToString();
            SubTitle.Text = StringUtil.IsEmpty(jo["subTitle"]) ? "" : jo["subTitle"].ToString();
            MsgTitle.Text = StringUtil.IsEmpty(jo["msgTitle"]) ? "" : jo["msgTitle"].ToString();
            JArray ja = JArray.Parse(StringUtil.IsEmpty(jo["msg"]) ? "[]" : jo["msg"].ToString());
            githubUrl = StringUtil.IsEmpty(jo["githubUrl"]) ? ConfigurationManager.AppSettings["GitHubUrl"] : jo["githubUrl"].ToString();
            giteeUrl = StringUtil.IsEmpty(jo["giteeUrl"]) ? ConfigurationManager.AppSettings["GiteeUrl"] : jo["giteeUrl"].ToString();
            string msg = "";
            for (int i = 0; i < ja.Count; i++)
            {
                msg += "• " + ja[i].ToString() + "\n";
            }
            Msg.Text = msg;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string packageUrl;
            switch (appConfig.UpdateType)
            {
                case UpdateType.GitHub:
                    packageUrl = githubUrl;
                    break;
                default:
                    packageUrl = giteeUrl;
                    break;
            }
            Process.Start(new ProcessStartInfo("cmd", $"/c start {packageUrl}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
            this.Close();
        }

        private static System.Windows.Window window = null;
        public static void Show(JObject jo)
        {
            if (window == null || !window.Activate())
            {
                window = new UpdateWindow(jo);
            }
            window.Show();
            window.Activate();
            Keyboard.Focus(window);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DataContext = null;
                this.Close();
            }
        }

        private void StarBtnClick(object sender, RoutedEventArgs e)
        {

            string githubUrl = ConfigurationManager.AppSettings["GitHubUrl"];
            string giteeUrl = ConfigurationManager.AppSettings["GiteeUrl"];

            if(!ReqGitUrl(githubUrl))
            {
                if (!ReqGitUrl(giteeUrl))
                {
                    OpenLinkUrl(githubUrl);
                } else
                {
                    OpenLinkUrl(giteeUrl);
                }
            } else
            {
                OpenLinkUrl(githubUrl);
            }
        }


        private bool ReqGitUrl(String url)
        {
            HttpWebResponse myResponse = null;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //创建Web访问对  象
                WebRequest myRequest = WebRequest.Create(url);

                myRequest.ContentType = "text/plain; charset=utf-8";
                //通过Web访问对象获取响应内容
                myResponse = (HttpWebResponse)myRequest.GetResponse();
            }
            catch 
            {
                return false;
            }

            return myResponse != null && myResponse.StatusCode == HttpStatusCode.OK;
        }

        private void OpenLinkUrl(String url)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

    }
}
