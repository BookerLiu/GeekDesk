using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class FileUtil
    {

        private static readonly string NO_PATH = ".*{.*}.*";

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
                string path = shortcut.TargetPath;
                if (path==null || Regex.IsMatch(path, NO_PATH))
                {
                    path = ParseShortcut(filePath);
                }
                return path;
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                LogUtil.WriteErrorLog(e, "获取文件图标失败! filePath=" + filePath);
                return null;
            }
        }


        /*
        UINT MsiGetShortcutTarget(
            LPCTSTR szShortcutTarget,
            LPTSTR szProductCode,
            LPTSTR szFeatureId,
            LPTSTR szComponentCode
        );
        */
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        static extern int MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

        public enum InstallState
        {
            NotUsed = -7,
            BadConfig = -6,
            Incomplete = -5,
            SourceAbsent = -4,
            MoreData = -3,
            InvalidArg = -2,
            Unknown = -1,
            Broken = 0,
            Advertised = 1,
            Removed = 1,
            Absent = 2,
            Local = 3,
            Source = 4,
            Default = 5
        }

        public const int MaxFeatureLength = 38;
        public const int MaxGuidLength = 38;
        public const int MaxPathLength = 1024;

        /*
        INSTALLSTATE MsiGetComponentPath(
          LPCTSTR szProduct,
          LPCTSTR szComponent,
          LPTSTR lpPathBuf,
          DWORD* pcchBuf
        );
        */
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        public static string ParseShortcut(string file)
        {
            StringBuilder product = new StringBuilder(MaxGuidLength + 1);
            StringBuilder feature = new StringBuilder(MaxFeatureLength + 1);
            StringBuilder component = new StringBuilder(MaxGuidLength + 1);

            MsiGetShortcutTarget(file, product, feature, component);

            int pathLength = MaxPathLength;
            StringBuilder path = new StringBuilder(pathLength);

            InstallState installState = MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            if (installState == InstallState.Local)
            {
                return path.ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
