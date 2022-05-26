using Microsoft.Win32;
using System;

namespace GeekDesk.Util
{
    class RegisterUtil
    {

        #region 注册表开机自启动
        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public static bool SetSelfStarting(bool started, string exeName)
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
                        LogUtil.WriteErrorLog(ex, "设置开机/取消失败!started=" + started);
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
                        LogUtil.WriteErrorLog(ex, "取消开机启动失败!started=" + started);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteErrorLog(ex, "取消/开机/失败!started=" + started);
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        #endregion
    }
}
