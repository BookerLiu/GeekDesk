using GeekDesk.Constant;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GeekDesk
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {

            bool ret;
            mutex = new System.Threading.Mutex(true, Constants.MY_NAME, out ret);
            if (!ret)
            {
                Environment.Exit(0);
            }
        }
    }



      
    //    private void WriteLog(object exception)
    //    {
    //        Exception ex = exception as Exception;

    //        using (FileStream fs = File.Open(".//ErrorLog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
    //        {
    //            fs.Seek(0, SeekOrigin.End);
    //            byte[] buffer = Encoding.Default.GetBytes("-------------------------------------------------------\r\n");
    //            fs.Write(buffer, 0, buffer.Length);

    //            buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + "\r\n");
    //            fs.Write(buffer, 0, buffer.Length);

    //            if (ex != null)
    //            {
    //                buffer = Encoding.Default.GetBytes("成员名: " + ex.TargetSite + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);

    //                buffer = Encoding.Default.GetBytes("引发异常的类: " + ex.TargetSite.DeclaringType + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);

    //                buffer = Encoding.Default.GetBytes("异常信息: " + ex.Message + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);

    //                buffer = Encoding.Default.GetBytes("引发异常的程序集或对象: " + ex.Source + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);

    //                buffer = Encoding.Default.GetBytes("栈：" + ex.StackTrace + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);
    //            }
    //            else
    //            {
    //                buffer = Encoding.Default.GetBytes("应用程序错误: " + exception.ToString() + "\r\n");
    //                fs.Write(buffer, 0, buffer.Length);
    //            }
    //        }

    //}
}
