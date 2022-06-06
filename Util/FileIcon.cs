using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace GeekDesk.Util
{
    public class FileIcon
    {
        static List<string> blurExtsList;
        static List<int[]> pixelList;
        static FileIcon() {
            blurExtsList = new List<string>
            {
                ".exe",
                ".cer",
                ".lnk",
                ".chm"
            };
            pixelList = new List<int[]>
            {
                new int[]{ 256 / 4, 256 / 4 },
                new int[]{ 256 / 2, 256 / 4 },
                new int[]{ 256 / 4 * 3, 256 / 4 },
                new int[]{ 256 / 4, 256 / 2 },
                new int[]{ 256 / 4, 256 / 4 * 3 },
                new int[]{ 256 / 4 * 3, 256 / 2 },
                new int[]{ 256 / 4 * 3, 256 / 4 * 3 },
                new int[]{ 256 / 2, 256 / 4 * 3 },
            };
        }

      

        [DllImport("User32.dll")]
        public static extern int PrivateExtractIcons(
           string lpszFile, //文件名可以是exe,dll,ico,cur,ani,bmp
           int nIconIndex,  //从第几个图标开始获取
           int cxIcon,      //获取图标的尺寸x
           int cyIcon,      //获取图标的尺寸y
           IntPtr[] phicon, //获取到的图标指针数组
           int[] piconid,   //图标对应的资源编号
           int nIcons,      //指定获取的图标数量，仅当文件类型为.exe 和 .dll时候可用
           int flags        //标志，默认0就可以，具体可以看LoadImage函数
       );


        public static BitmapImage GetBitmapImage(string filePath)
        {
            Icon ico;
            //选中文件中的图标总数
            var iconTotalCount = PrivateExtractIcons(filePath, 0, 0, 0, null, null, 0, 0);
            //用于接收获取到的图标指针
            IntPtr[] hIcons = new IntPtr[iconTotalCount];
            //对应的图标id
            int[] ids = new int[iconTotalCount];
            //成功获取到的图标个数
            var successCount = PrivateExtractIcons(filePath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            string ext = Path.GetExtension(filePath).ToLower();

            IntPtr ip = IntPtr.Zero;
            if (successCount > 0)
            {
                ip = hIcons[0];
                ico = Icon.FromHandle(ip);
            }
            else if (blurExtsList.Contains(ext))
            {
                ico = Icon.ExtractAssociatedIcon(filePath);
            }
            else
            {
                ip = GetJumboIcon(GetIconIndex(filePath));
                ico = Icon.FromHandle(ip);
                if (CheckIsSmallIco(ico.ToBitmap()))
                {
                    ico = Icon.ExtractAssociatedIcon(filePath);
                }
            }

            

            Bitmap bmp = ico.ToBitmap();
            MemoryStream strm = new MemoryStream();

            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/png");
            Encoder myEncoder = Encoder.Quality;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 75L);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = myEncoderParameter;

            bmp.Save(strm, myImageCodecInfo, myEncoderParameters);

            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            strm.Seek(0, SeekOrigin.Begin);
            bmpImage.StreamSource = strm;
            bmpImage.EndInit();
            if (ip != IntPtr.Zero)
            {
                Shell32.DestroyIcon(ip);
            }
            DeleteObject(bmp.GetHbitmap());
            return bmpImage.Clone();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private static bool CheckIsSmallIco(Bitmap bm)
        {
            Color color;
            int count = 0;
            foreach (int[] arr in pixelList)
            {
                color = bm.GetPixel(arr[0], arr[1]);
                if (color.A == 0)
                {
                    count++;
                }
            }
            DeleteObject(bm.GetHbitmap());
            if (count >= 6)
            {
                return true;
            }
            return false;
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);



        public static int GetIconIndex(string pszFile)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            Shell32.SHGetFileInfo(pszFile
                , 0
                , ref sfi
                , (uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi)
                , (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
            return sfi.iIcon;
        }

        // 256*256
        public static IntPtr GetJumboIcon(int iImage)
        {
            IImageList spiml = null;
            Guid guil = new Guid(IID_IImageList);//or IID_IImageList

            Shell32.SHGetImageList(Shell32.SHIL_JUMBO, ref guil, ref spiml);
            IntPtr hIcon = IntPtr.Zero;
            spiml.GetIcon(iImage, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon);

            return hIcon;
        }

        const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
        const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";

        public static class Shell32
        {

            public const int SHIL_LARGE = 0x0;
            public const int SHIL_SMALL = 0x1;
            public const int SHIL_EXTRALARGE = 0x2;
            public const int SHIL_SYSSMALL = 0x3;
            public const int SHIL_JUMBO = 0x4;
            public const int SHIL_LAST = 0x4;

            public const int ILD_TRANSPARENT = 0x00000001;
            public const int ILD_IMAGE = 0x00000020;

            [DllImport("shell32.dll", EntryPoint = "#727")]
            public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

            [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
            public static unsafe extern int DestroyIcon(IntPtr hIcon);

            [DllImport("shell32.dll")]
            public static extern uint SHGetIDListFromObject([MarshalAs(UnmanagedType.IUnknown)] object iUnknown, out IntPtr ppidl);

            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags
            );
        }

        [Flags]
        enum SHGFI : uint
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public const int NAMESIZE = 80;
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;    // x offest from the upperleft of bitmap
            public int yBitmap;    // y offset from the upperleft of bitmap
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }
        [ComImportAttribute()]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig]
            int Add(
            IntPtr hbmImage,
            IntPtr hbmMask,
            ref int pi);

            [PreserveSig]
            int ReplaceIcon(
            int i,
            IntPtr hicon,
            ref int pi);

            [PreserveSig]
            int SetOverlayImage(
            int iImage,
            int iOverlay);

            [PreserveSig]
            int Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
            IntPtr hbmImage,
            int crMask,
            ref int pi);

            [PreserveSig]
            int Draw(
            ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(int i);

            [PreserveSig]
            int GetIcon(
            int i,
            int flags,
            ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
            int i,
            ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
            int iDst,
            IImageList punkSrc,
            int iSrc,
            int uFlags);

            [PreserveSig]
            int Merge(
            int i1,
            IImageList punk2,
            int i2,
            int dx,
            int dy,
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int Clone(
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
            int i,
            ref RECT prc);

            [PreserveSig]
            int GetIconSize(
            ref int cx,
            ref int cy);

            [PreserveSig]
            int SetIconSize(
            int cx,
            int cy);

            [PreserveSig]
            int GetImageCount(ref int pi);

            [PreserveSig]
            int SetImageCount(
            int uNewCount);

            [PreserveSig]
            int SetBkColor(
            int clrBk,
            ref int pclr);

            [PreserveSig]
            int GetBkColor(
            ref int pclr);

            [PreserveSig]
            int BeginDrag(
            int iTrack,
            int dxHotspot,
            int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
            IntPtr hwndLock,
            int x,
            int y);

            [PreserveSig]
            int DragLeave(
            IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
            int x,
            int y);

            [PreserveSig]
            int SetDragCursorImage(
            ref IImageList punk,
            int iDrag,
            int dxHotspot,
            int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
            int fShow);

            [PreserveSig]
            int GetDragImage(
            ref POINT ppt,
            ref POINT pptHotspot,
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
            int i,
            ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
            int iOverlay,
            ref int piIndex);
        };

    }

}
