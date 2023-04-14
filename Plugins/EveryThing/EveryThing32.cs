using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Plugins.EveryThing
{
    public class EveryThing32
    {

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern UInt32 Everything_SetSearchW(string lpSearchString);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetRegex(bool bEnable);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetMax(UInt32 dwMax);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetOffset(UInt32 dwOffset);

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetMatchPath();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetMatchCase();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetMatchWholeWord();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetRegex();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetMax();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetOffset();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern IntPtr Everything_GetSearchW();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetLastError();

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_Query(bool bWait);

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SortResultsByPath();

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetNumFileResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetNumFolderResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetNumResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetTotFileResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetTotFolderResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetTotResults();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_IsVolumeResult(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_IsFolderResult(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_IsFileResult(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_Reset();

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultFileName(UInt32 nIndex);

        // Everything 1.4
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetSort(UInt32 dwSortType);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetSort();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetResultListSort();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetRequestFlags();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetResultListRequestFlags();
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultExtension(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultSize(UInt32 nIndex, out long lpFileSize);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultDateCreated(UInt32 nIndex, out long lpFileTime);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultDateModified(UInt32 nIndex, out long lpFileTime);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultDateAccessed(UInt32 nIndex, out long lpFileTime);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetResultAttributes(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultFileListFileName(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetResultRunCount(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultDateRun(UInt32 nIndex, out long lpFileTime);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_GetResultDateRecentlyChanged(UInt32 nIndex, out long lpFileTime);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultHighlightedFileName(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultHighlightedPath(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr Everything_GetResultHighlightedFullPathAndFileName(UInt32 nIndex);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_GetRunCountFromFileName(string lpFileName);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_SetRunCountFromFileName(string lpFileName, UInt32 dwRunCount);
        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern UInt32 Everything_IncRunCountFromFileName(string lpFileName);

        [DllImport(@"lib\Plugins\EveryThing\lib\Everything32.dll")]
        public static extern bool Everything_Exit();
    }
}
