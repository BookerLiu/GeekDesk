using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GeekDesk.Thread
{
    public class UpdateThread
    {
        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        public static void Update()
        {
            System.Threading.Thread t = new System.Threading.Thread(new ThreadStart(UpdateApp))
            {
                IsBackground = true
            };
            t.Start();
        }

        private static void UpdateApp()
        {
            try
            {

                //等待1分钟后再检查更新  有的网络连接过慢
                System.Threading.Thread.Sleep(60 * 1000);

                string updateUrl;
                string nowVersion = ConfigurationManager.AppSettings["Version"];
                switch (appConfig.UpdateType)
                {
                    case UpdateType.GitHub:
                        updateUrl = ConfigurationManager.AppSettings["GitHubUpdateUrl"];
                        break;
                    default:
                        updateUrl = ConfigurationManager.AppSettings["GiteeUpdateUrl"];
                        break;
                }
                string updateInfo = HttpUtil.Get(updateUrl);
                if (!StringUtil.IsEmpty(updateInfo))
                {
                    JObject jo = JObject.Parse(updateInfo);
                    string onlineVersion = jo["version"].ToString();
                    if (onlineVersion.CompareTo(nowVersion) > 0)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            //检测到版本更新
                            UpdateWindow.Show(jo);
                        }));
                    }
                }
            } catch (Exception e)
            {
                //不做处理
                //MessageBox.Show(e.Message);
            }
        }
    }
}
