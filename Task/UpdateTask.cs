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
using System.Threading.Tasks;

namespace GeekDesk.Task
{
    internal class UpdateTask
    {

        private static AppConfig appConfig = MainWindow.appData.AppConfig;
        public static void Start()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 60 * 1000 * 60 * 12, //60秒 * 60 * 12  12小时触发一次
                //Interval = 6000,
            };
            timer.Start();
            timer.Elapsed += Timer_Elapsed; ;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string updateUrl = ConfigurationManager.AppSettings["GiteeUpdateUrl"];
                string nowVersion = ConfigurationManager.AppSettings["Version"];
                if (appConfig != null)
                {
                    switch (appConfig.UpdateType)
                    {
                        case UpdateType.GitHub:
                            updateUrl = ConfigurationManager.AppSettings["GitHubUpdateUrl"];
                            break;
                        default:
                            updateUrl = ConfigurationManager.AppSettings["GiteeUpdateUrl"];
                            break;
                    }
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
                    }
                    catch (Exception) { }



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
