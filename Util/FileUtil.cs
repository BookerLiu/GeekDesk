using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GeekDesk.Util
{
    public class FileUtil
    {

        private static readonly string NO_PATH = "{(.*)}";
        private static readonly string NO_ICO = "^,(.*)";
        private static readonly string HAVE_ICO = "(.*),(.*)";

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
                if (path == null || Regex.IsMatch(path, NO_PATH))
                {
                    path = ParseShortcut(filePath);
                }
                return path;
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "获取目标路径失败! filePath=" + filePath);
                return null;
            }
        }

        /// <summary>
        /// 获取启动参数
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetArgByLnk(string filePath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                return shortcut.Arguments;
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "获取启动参数失败! filePath=" + filePath);
                return "";
            }
        }

        /// <summary>
        /// 获取iconpath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetIconPathByLnk(string filePath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                var iconPath = shortcut.IconLocation;

                if (StringUtil.IsEmpty(iconPath)
                    || Regex.IsMatch(iconPath, NO_ICO)
                    || Regex.IsMatch(iconPath, NO_PATH)
                    || !Regex.IsMatch(iconPath, HAVE_ICO))
                {
                    return null;
                }
                else
                {
                    return iconPath.Split(',')[0];
                }
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "获取图标路径失败! filePath=" + filePath);
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

        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

    }
}
