using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class SystemIcon
    {
        [DllImport("Shell32.dll")]
        public static extern int ExtractIcon(IntPtr h, string strx, int ii);

        public static Icon MyExtractIcon(string FileName, int iIndex, IntPtr h)
        {
            try
            {
                IntPtr hIcon = (IntPtr)ExtractIcon(h, FileName, iIndex);
                if (!hIcon.Equals(null))
                {
                    Icon icon = Icon.FromHandle(hIcon);
                    return icon;
                }
            }
            catch (Exception ex)
            { 
            }
            return null;
        }

}
}
