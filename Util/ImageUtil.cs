using GeekDesk.Constant;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDesk.Util
{
    class ImageUtil
    {
        private static readonly string SYSTEM_ITEM = "::{.*}";

        /// <summary>
        /// 图片数组转 BitmapImage
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BitmapImage ByteArrToImage(byte[] array)
        {
            if (array == null) return null;
            using (var ms = new System.IO.MemoryStream(array))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.LowQuality);
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        /// <summary>
        /// BitmapImage 转数组
        /// </summary>
        /// <param name="bi"></param>
        /// <returns></returns>
        public static byte[] BitmapImageToByte(BitmapImage bi)
        {
            if (bi == null) return null;
            using (MemoryStream memStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(memStream);
                return memStream.GetBuffer();
            }
        }

        /// <summary>
        /// byte[]转换成Image
        /// </summary>
        /// <param name="byteArrayIn">二进制图片流</param>
        /// <returns>Image</returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                ms.Flush();
                return returnImage;
            }
        }

        /// <summary>
        /// 图片base64 转 BitmapImage
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64);
            return ByteArrToImage(byteBuffer);
        }

        /// <summary>
        /// 获取文件 icon
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapIconByPath(string filePath)
        {
            if (filePath.Contains("%windir%"))
            {
                filePath = filePath.Replace("%windir%", System.Environment.GetEnvironmentVariable("windir"));
            }

            if (File.Exists(filePath) || IsSystemItem(filePath))
            {
                if (IsImage(filePath))
                {
                    //图片
                    return GetThumbnailByFile(filePath, 256, 256);
                }
                else
                { //其它文件
                    return FileIcon.GetBitmapImage(filePath);
                }
            }
            else if (Directory.Exists(filePath))
            {

                if ((filePath.IndexOf("\\") == filePath.LastIndexOf("\\")) && filePath.IndexOf("\\") == filePath.Length - 1)
                {
                    //磁盘
                    return ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_DISK_IMAGE_BASE64);
                }
                else
                {
                    //文件夹
                    return ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_DIR_IMAGE_BASE64);
                }

            }
            return null;
        }


        public static BitmapImage GetBitmapIconByUnknownPath(string path)
        {
            //string base64 = ImageUtil.FileImageToBase64(path, System.Drawing.Imaging.ImageFormat.Png);
            string ext = "";
            if (!ImageUtil.IsSystemItem(path))
            {
                ext = System.IO.Path.GetExtension(path).ToLower();
            }

            string iconPath = null;
            if (".lnk".Equals(ext))
            {

                string targetPath = FileUtil.GetTargetPathByLnk(path);
                iconPath = FileUtil.GetIconPathByLnk(path);
                if (targetPath != null)
                {
                    path = targetPath;
                }
            }
            if (StringUtil.IsEmpty(iconPath))
            {
                iconPath = path;
            }

            return ImageUtil.GetBitmapIconByPath(iconPath);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lcFilename">需要改变大小的图片位置</param>
        /// <param name="lnWidth">缩略图的宽度</param>
        /// <param name="lnHeight">缩略图的高度</param>
        /// <returns></returns>
        //public static BitmapImage GetThumbnail(string lcFilename, int lnWidth, int lnHeight)
        //{
        //    Bitmap bmpOut = null;
        //    try
        //    {
        //        Bitmap loBMP = new Bitmap(lcFilename);
        //        ImageFormat loFormat = loBMP.RawFormat;

        //        decimal lnRatio;
        //        int lnNewWidth = 0;
        //        int lnNewHeight = 0;

        //        //如果图像小于缩略图直接返回原图，因为upfront
        //        if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
        //            return BitmapToBitmapImage(loBMP);
        //        if (loBMP.Width > loBMP.Height)
        //        {
        //            lnRatio = (decimal)lnWidth / loBMP.Width;
        //            lnNewWidth = lnWidth;
        //            decimal lnTemp = loBMP.Height * lnRatio;
        //            lnNewHeight = (int)lnTemp;
        //        }
        //        else
        //        {
        //            lnRatio = (decimal)lnHeight / loBMP.Height;
        //            lnNewHeight = lnHeight;
        //            decimal lnTemp = loBMP.Width * lnRatio;
        //            lnNewWidth = (int)lnTemp;
        //        }
        //        bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
        //        Graphics g = Graphics.FromImage(bmpOut);
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
        //        g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
        //        loBMP.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        return Base64ToBitmapImage(Constants.DEFAULT_IMG_IMAGE_BASE64);
        //    }
        //    return BitmapToBitmapImage(bmpOut);
        //}

        public static BitmapImage GetThumbnailByFile(string filePath, int tWidth, int tHeight)
        {

            try
            {
                FileInfo file = new FileInfo(filePath);
                if (file.Exists && file.Length > 0 
                    && !System.IO.Path.GetExtension(filePath).Contains("psd"))
                {
                    Image img = Image.FromFile(filePath);
                    if (img.Width <= tWidth && img.Height <= tHeight)
                    {
                        return GetBitmapImageByFile(filePath);
                    }
                    else
                    {
                        Bitmap loBMP = new Bitmap(filePath);
                        ImageFormat loFormat = loBMP.RawFormat;

                        decimal lnRatio;
                        int lnNewWidth;
                        int lnNewHeight;
                        if (loBMP.Width > loBMP.Height)
                        {
                            lnRatio = (decimal)tWidth / loBMP.Width;
                            lnNewWidth = tWidth;
                            decimal lnTemp = loBMP.Height * lnRatio;
                            lnNewHeight = (int)lnTemp;
                        }
                        else
                        {
                            lnRatio = (decimal)tHeight / loBMP.Height;
                            lnNewHeight = tHeight;
                            decimal lnTemp = loBMP.Width * lnRatio;
                            lnNewWidth = (int)lnTemp;
                        }
                        Bitmap bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                        Graphics g = Graphics.FromImage(bmpOut);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(System.Drawing.Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                        g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
                        loBMP.Dispose();
                        string tempPath = Constants.APP_DIR + "\\temp";
                        if (File.Exists(tempPath))
                        {
                            File.Delete(tempPath);
                        }
                        bmpOut.Save(tempPath, loFormat);
                        BitmapImage bm = GetBitmapImageByFile(tempPath);
                        File.Delete(tempPath);
                        return bm;
                    }
                } else
                {
                    return Base64ToBitmapImage(Constants.DEFAULT_IMG_IMAGE_BASE64);
                }
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "获取文件缩略图失败!filePath=" + filePath);
                return Base64ToBitmapImage(Constants.DEFAULT_IMG_IMAGE_BASE64);
            }

        }


        public static BitmapImage GetBitmapImageByFile(string filePath)
        {
            BitmapImage bmImg = new BitmapImage();
            bmImg.BeginInit();
            bmImg.CacheOption = BitmapCacheOption.OnLoad;
            RenderOptions.SetBitmapScalingMode(bmImg, BitmapScalingMode.LowQuality);
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bmImg.StreamSource = fs;
                bmImg.EndInit();
            }
            return bmImg;
        }

        public static BitmapImage MemoryStremToBitMapImage(MemoryStream ms)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }


        /// <summary>
        /// Bitmap to BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            return BitmapToBitmapImage(bitmap, null);
        }

        public static BitmapImage BitmapToBitmapImage(Image bitmap, ImageFormat format)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                if (format == null)
                {
                    bitmap.Save(ms, bitmap.RawFormat);
                }
                else
                {
                    bitmap.Save(ms, format);
                }
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bit3 = new BitmapImage();
            bit3.BeginInit();
            bit3.StreamSource = ms;
            bit3.EndInit();
            return bit3;
        }


        /// <summary>
        /// 图片文件转base64
        /// </summary>
        /// <param name="Imagefilename"></param>
        /// <returns></returns>
        public static string FileImageToBase64(string Imagefilename, ImageFormat format)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, format);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception e)
            {
                LogUtil.WriteErrorLog(e, "图片文件转base64失败!Imagefilename=" + Imagefilename + ",ImageFormat=" + format);
                return null;
            }
        }

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsImage(string path)
        {
            try
            {
                string ext = Path.GetExtension(path);
                if (!string.IsNullOrEmpty(ext))
                {
                    string strExt = Path.GetExtension(path).Substring(1);
                    string suffixs = "bmp,jpg,png,tif,gif,pcx,tga,exif,fpx,svg,psd,cdr,pcd,dxf,ufo,eps,ai,raw,WMF,webp,avif";
                    string[] suffixArr = suffixs.Split(',');
                    foreach (string suffix in suffixArr)
                    {
                        if (suffix.Equals(strExt, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 判断是否为系统项
        /// </summary>
        /// <returns></returns>
        public static bool IsSystemItem(string path)
        {
            return Regex.IsMatch(path, SYSTEM_ITEM);
        }


    }
}
