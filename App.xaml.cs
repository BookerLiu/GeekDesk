using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GeekDesk
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "GeekDesk", out ret);

            if (!ret)
            {
                MessageBox.Show("已有一个客户端正在运行,请先结束原来客户端!");
                Environment.Exit(0);
            }
            #region 设置程序开机自动运行(+注册表项)
            try
            {
                //SetSelfStarting(true, "GeekDesk.exe");
            }
            catch (Exception ex)
            {
            }

            #endregion
        }



        #region 注册表开机自启动


        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {
                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                //RegistryKey HKLM = Registry.CurrentUser;
                //key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals("-1"))
                            {
                                key.DeleteValue(exeName);//取消开机启动
                            }
                            key.SetValue(exeName, exeDir);//设置为开机启动
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName);//取消开机启动
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        #endregion
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
