using GeekDesk.Constant;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace GeekDesk.Util
{
    class ImageUtil
    {

        /// <summary>
        /// 图片数组转 BitmapImage
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BitmapImage ByteArrToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
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
            using (MemoryStream memStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(memStream);
                return memStream.GetBuffer();
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
            if (File.Exists(filePath))
            {
                if (IsImage(filePath)) {
                    //图片
                    return GetThumbnail(filePath, 256, 256);
                } else
                { //其它文件
                    return FileIcon.GetBitmapImage(filePath);
                }
            } else if(Directory.Exists(filePath)) {
                //文件夹
                return ImageUtil.Base64ToBitmapImage(Constants.DEFAULT_DIR_IMAGE_BASE64);
            }
            return null;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="lcFilename">需要改变大小的图片位置</param>
        /// <param name="lnWidth">缩略图的宽度</param>
        /// <param name="lnHeight">缩略图的高度</param>
        /// <returns></returns>
        public static BitmapImage GetThumbnail(string lcFilename, int lnWidth, int lnHeight)
        {
            Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(lcFilename);
                ImageFormat loFormat = loBMP.RawFormat;

                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //如果图像小于缩略图直接返回原图，因为upfront
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return BitmapToBitmapImage(bmpOut);
                if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBMP.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
                loBMP.Dispose();
            }
            catch
            {
                return Base64ToBitmapImage(Constants.DEFAULT_IMG_IMAGE_BASE64);
            }
            return BitmapToBitmapImage(bmpOut);
        }


        /// <summary>
        /// Bitmap to BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
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
            catch (Exception)
            {
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
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
