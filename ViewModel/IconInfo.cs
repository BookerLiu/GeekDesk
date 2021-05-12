using GeekDesk.Constant;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace GeekDesk.ViewModel
{
    [Serializable]
    public class IconInfo : INotifyPropertyChanged
    {
        private string path; //路径
        private string name; //文件名
        private int count = 0; //打开次数
        [field: NonSerialized]
        private BitmapImage bitmapImage; //位图
        private byte[] imageByteArr; //图片 base64
        private string content; //显示信息
        private int imageWidth = (int)DefaultConstant.IMAGE_WIDTH;
        private int imageHeight = (int)DefaultConstant.IMAGE_HEIGHT;

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                Content = Path + "\n" + Name + "\n使用次数: " + Count;
                OnPropertyChanged("Count");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                Content = Path + "\n" + Name + "\n使用次数: " + Count;
                OnPropertyChanged("Name");
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                Content = Path + "\n" + Name + "\n使用次数: " + Count;
                OnPropertyChanged("Path");
            }
        }

        public BitmapImage BitmapImage
        {
            get
            {

                return ToImage(ImageByteArr);
            }
            set
            {
                bitmapImage = value;
                ImageByteArr = getJPGFromImageControl(bitmapImage);
                OnPropertyChanged("BitmapImage");
            }
        }

        public byte[] ImageByteArr
        {
            get
            {

                return imageByteArr;
            }
            set
            {
                imageByteArr = value;
                OnPropertyChanged("ImageByteArr");
            }
        }

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                OnPropertyChanged("Content");
            }
        }

        public int ImageWidth
        {
            get
            {
                return imageWidth;
            }
            set
            {
                imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        public int ImageHeight
        {
            get
            {
                return imageHeight;
            }
            set
            {
                imageHeight = value;
                OnPropertyChanged("ImageHeight");
            }
        }





        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BitmapImage ToImage(byte[] array)
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

        public byte[] getJPGFromImageControl(BitmapImage bi)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(memStream);
                return memStream.GetBuffer();
            }
        }

    }
}