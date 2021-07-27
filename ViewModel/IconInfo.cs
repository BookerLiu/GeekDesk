using GeekDesk.Constant;
using GeekDesk.Util;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

/// <summary>
/// 图标信息
/// </summary>
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
        private byte[] imageByteArr; //图片 byte数组
        private string content; //显示信息
        private int imageWidth = (int)MainWindowEnum.IMAGE_WIDTH; //图片宽度
        private int imageHeight = (int)MainWindowEnum.IMAGE_HEIGHT; //图片高度
        private bool adminStartUp = false; //始终管理员方式启动  默认否
        private byte[] defaultImage; //默认图标



        public byte[] DefaultImage
        {
            get
            {
                return defaultImage;
            }
            set
            {
                defaultImage = value;
                OnPropertyChanged("DefaultImage");
            }
        }

        public bool AdminStartUp
        {
            get
            {
                return adminStartUp;
            }
            set
            {
                adminStartUp = value;
                OnPropertyChanged("AdminStartUp");
            }
        }

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

                return ImageUtil.ByteArrToImage(ImageByteArr);
            }
            set
            {
                bitmapImage = value;
                ImageByteArr = ImageUtil.BitmapImageToByte(bitmapImage);
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
                // 为了兼容旧版 暂时使用默认
                return (int)MainWindowEnum.IMAGE_WIDTH;
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
                // 为了兼容旧版 暂时使用默认
                return (int)MainWindowEnum.IMAGE_HEIGHT;
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
            CommonCode.SaveAppData(MainWindow.appData);
        }


    }
}