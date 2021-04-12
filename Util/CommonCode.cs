using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GeekDesk.ViewModel;

/// <summary>
/// 提取一些代码
/// </summary>
namespace GeekDesk.Util
{
    class CommonCode
    {
        private static string appConfigFilePath = AppDomain.CurrentDomain.BaseDirectory.Trim() + "\\config";
        /// <summary>
        /// 获取app配置
        /// </summary>
        /// <returns></returns>
        public static AppConfig GetAppConfig()
        {
            AppConfig config;
            if (!File.Exists(appConfigFilePath))
            {
                using (FileStream fs = File.Create(appConfigFilePath)) { }
                config = new AppConfig();
                SaveAppConfig(config);
                   
            }
            else
            {
                using (FileStream fs = new FileStream(appConfigFilePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string json = bf.Deserialize(fs) as string;
                    config = JsonConvert.DeserializeObject<AppConfig>(json);
                }
            }
            return config;
        }

        /// <summary>
        /// 保存app配置
        /// </summary>
        /// <param name="config"></param>
        public static void SaveAppConfig(AppConfig config)
        {
            using (FileStream fs = new FileStream(appConfigFilePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                string json = JsonConvert.SerializeObject(config);
                bf.Serialize(fs, json);
            }
        }
    }
}
