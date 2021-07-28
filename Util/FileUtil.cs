using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class FileUtil
    {

        public static string GetTargetPathByLnk(string filePath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);

                if (StringUtil.IsEmpty(shortcut.TargetPath))
                {
                    return null;
                }
                return shortcut.TargetPath;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
