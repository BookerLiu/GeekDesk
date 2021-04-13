using System;

namespace GeekDesk.Constant
{
    class AppConstant
    {
        private static string APP_DIR = AppDomain.CurrentDomain.BaseDirectory.Trim();
        /// <summary>
        /// app数据文件路径
        /// </summary>
        public static string DATA_FILE_PATH = APP_DIR + "//Data";  //app数据文件路径
    }
}
