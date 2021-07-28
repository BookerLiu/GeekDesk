using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeekDesk.Util
{
    /// <summary>
    /// 保存文件信息的结构体
    /// </summary>
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    class NativeMethods
    {

        [DllImport("Shell32.dll", EntryPoint = "SHGetFileInfo", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestroyIcon(IntPtr hIcon);

        #region API 参数的常量定义

        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; //大图标 32×32
        public const uint SHGFI_SMALLICON = 0x1; //小图标 16×16
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        #endregion

        /// <summary>
        /// 获取文件类型的关联图标
        /// </summary>
        /// <param name="fileName">文件类型的扩展名或文件的绝对路径</param>
        /// <param name="isLargeIcon">是否返回大图标</param>
        /// <returns>获取到的图标</returns>
        static Icon GetIcon(string fileName, bool isLargeIcon)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr hI;

            if (isLargeIcon)
                hI = NativeMethods.SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_USEFILEATTRIBUTES | NativeMethods.SHGFI_LARGEICON);
            else
                hI = NativeMethods.SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_USEFILEATTRIBUTES | NativeMethods.SHGFI_SMALLICON);

            Icon icon = Icon.FromHandle(shfi.hIcon).Clone() as Icon;

            NativeMethods.DestroyIcon(shfi.hIcon); //释放资源
            return icon;
        }

    }
    


}
