using GeekDesk.Constant;
using GeekDesk.ViewModel;
using System;
using System.IO;
using System.Runtime.InteropServices;
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



        /// <summary>
        /// 判断当前屏幕(鼠标最后活动屏幕)是否有全屏化应用
        /// </summary>
        /// <returns></returns>
        public static bool IsPrimaryFullScreen()
        {
            RECT rect = new RECT();
            GetWindowRect(new HandleRef(null, GetForegroundWindow()), ref rect);

            int windowHeight = rect.bottom - rect.top;
            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            if (windowHeight >= screenHeight)
            {
                return true;
            }
            return false;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();






    }
}
