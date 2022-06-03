using GeekDesk.Constant;
using System;
using System.IO;
using System.Text;

namespace GeekDesk.Util
{
    public class LogUtil
    {
        public static void WriteErrorLog(object exception, string msg = "")
        {
            try
            {
                Exception ex = exception as Exception;
                if (!Directory.Exists(Constants.ERROR_FILE_PATH.Substring(0, Constants.ERROR_FILE_PATH.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(Constants.ERROR_FILE_PATH.Substring(0, Constants.ERROR_FILE_PATH.LastIndexOf("\\")));
                }
                using (FileStream fs = File.Open(Constants.ERROR_FILE_PATH, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);
                    byte[] buffer = Encoding.Default.GetBytes("-------------------------------------------------------\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + msg + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    if (ex != null)
                    {
                        buffer = Encoding.Default.GetBytes("成员名: " + ex.TargetSite + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes("引发异常的类: " + ex.TargetSite.DeclaringType + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes("异常信息: " + ex.Message + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes("引发异常的程序集或对象: " + ex.Source + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes("栈：" + ex.StackTrace + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        buffer = Encoding.Default.GetBytes("应用程序错误: " + exception.ToString() + "\r\n");
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {

            }

        }


        public static void WriteLog(string msg)
        {
            try
            {
                if (!Directory.Exists(Constants.LOG_FILE_PATH.Substring(0, Constants.LOG_FILE_PATH.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(Constants.LOG_FILE_PATH.Substring(0, Constants.LOG_FILE_PATH.LastIndexOf("\\")));
                }
                using (FileStream fs = File.Open(Constants.LOG_FILE_PATH, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);
                    byte[] buffer = Encoding.Default.GetBytes("-------------------------------------------------------\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + msg + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
            catch { }
        }


    }
}
