using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Threading;

namespace GeekDesk.MyThread
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
                int sleepTime = 60 * 1000;
                if (Constants.DEV)
                {
                    sleepTime = 1;
                }

                System.Threading.Thread.Sleep(sleepTime);

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

                    try
                    {
                        if (jo["statisticUrl"] != null)
                        {
                            string statisticUrl = jo["statisticUrl"].ToString();
                            if (!string.IsNullOrEmpty(statisticUrl))
                            {
                                //用户统计  只通过uuid统计用户数量  不收集任何信息
                                statisticUrl += "?uuid=" + CommonCode.GetUniqueUUID();
                                HttpUtil.Get(statisticUrl);
                            }
                        }
                    } catch (Exception){}

                   

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
            }
            catch (Exception ex)
            {
                LogUtil.WriteErrorLog(ex, "获取更新失败!");
            }
        }
    }
}
