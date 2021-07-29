using GeekDesk.Constant;
using GeekDesk.ViewModel;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

/// <summary>
/// 提取一些代码
/// </summary>
namespace GeekDesk.Util
{
    class CommonCode
    {

        /// <summary>
        /// 获取app 数据
        /// </summary>
        /// <returns></returns>
        public static AppData GetAppDataByFile()
        {
            AppData appData;
            if (!File.Exists(Constants.DATA_FILE_PATH))
            {
                using (FileStream fs = File.Create(Constants.DATA_FILE_PATH)) { }
                appData = new AppData();
                SaveAppData(appData);

            }
            else
            {
                using (FileStream fs = new FileStream(Constants.DATA_FILE_PATH, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    appData = bf.Deserialize(fs) as AppData;
                }
            }
            return appData;
        }

        /// <summary>
        /// 保存app 数据
        /// </summary>
        /// <param name="appData"></param>
        public static void SaveAppData(AppData appData)
        {

            using (FileStream fs = new FileStream(Constants.DATA_FILE_PATH, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, appData);
            }
        }

        


    }
}
