using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GeekDesk.ViewModel
{
    public class DataInfos : ViewModelBase
    {
        private string path; //路径
        private string name; //文件名
        private int count = 0; //打开次数
        private BitmapImage bitmapImage; //位图

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        public BitmapImage BitmapImage
        {
            get
            {
                return bitmapImage;
            }
            set
            {
                bitmapImage = value;
                RaisePropertyChanged();
            }
        }
    }
}
