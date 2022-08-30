using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class MessageUtil
    {

        public const int WM_COPYDATA = 0x004A;
        public static bool SendMsgByPName(string processName, string msg)
        {
            try
            {
                Process[] processArr = Process.GetProcessesByName(processName);
                if (processArr != null && processArr.Length > 0)
                {
                    foreach (Process process in processArr)
                    {
                        IntPtr windowHandle = process.MainWindowHandle;
                        // 发送消息
                        CopyDataStruct cds = new CopyDataStruct(IntPtr.Zero, msg);
                        SendMessage(
                            windowHandle,
                            WM_COPYDATA,
                            0, ref cds);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            } catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, processName + "P发送消息失败!");
                return false;
            }
        }

        public static bool SendMsgByWName(string windowName, string msg)
        {
            try
            {
                IntPtr hwnd = FindWindow(null, windowName);
                if (hwnd != IntPtr.Zero)
                {
                    // 发送消息
                    CopyDataStruct cds = new CopyDataStruct(IntPtr.Zero, msg);
                    SendMessage(
                        hwnd,
                        WM_COPYDATA,
                        0, ref cds);
                } else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, windowName + "W发送消息失败!");
                return false;
            }
        }


        public static bool CheckProcessIsRuning(string processName)
        {
            try
            {
                Process[] processArr = Process.GetProcessesByName(processName);
                return (processArr != null && processArr.Length > 0);
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, processName + "检查进程名失败!");
                return false;
            }
        }

        public static bool CheckWindowIsRuning(string windowName)
        {
            try
            {
                IntPtr hwnd = FindWindow(null, windowName);
                return (hwnd != IntPtr.Zero);
            } catch(Exception)
            {
                return false;
            }
            
        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
                        IntPtr hWnd,                //目标窗体句柄
                        int Msg,                   //WM_COPYDATA
                        int wParam,                //自定义数值
                        ref CopyDataStruct lParam  //传递消息的结构体，
                        );

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public CopyDataStruct(IntPtr custom, string msg)
            {
                this.custom = custom;
                this.msg = msg;
                this.msgLength = msg.Length + 1;
            }
            public IntPtr custom;//用户定义数据  
            public int msgLength;//字符串长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string msg;//字符串
        }


        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(uint msg, int flags);

    }
}
