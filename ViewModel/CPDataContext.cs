using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeekDesk.ViewModel
{

    public class CPDataContext : INotifyPropertyChanged
    {
        private BitmapSource pixelIMG;

        private System.Drawing.Color pixelColor_D;

        public Color pixelColor;

        private string colorHtml;

        private string colorRGB;

        private string pixelXY;

        public BitmapSource PixelIMG
        {
            set
            {
                pixelIMG = value;
                OnPropertyChanged("PixelIMG");
            }
            get { return pixelIMG; }
        }

        public System.Drawing.Color PixelColor_D
        {
            set
            {
                pixelColor_D = value;
                ColorHtml = pixelColor_D.Name.ToUpper().Substring(2);
                ColorRGB = pixelColor_D.R + "," + pixelColor_D.G + "," + pixelColor_D.B;
                PixelColor = Color.FromArgb(pixelColor_D.A, pixelColor_D.R, pixelColor_D.G, pixelColor_D.B);
            }
            get { return pixelColor_D; }
        }

        public Color PixelColor
        {
            set
            {
                pixelColor = value;
                OnPropertyChanged("PixelColor");
            }
            get { return pixelColor; }
        }

        public string ColorRGB
        {
            set
            {
                colorRGB = value;
                OnPropertyChanged("ColorRGB");
            }
            get { return colorRGB; }
        }

        public string ColorHtml
        {
            set
            {
                colorHtml = value;
                OnPropertyChanged("ColorHtml");
            }
            get { return colorHtml; }
        }


        public string PixelXY
        {
            set
            {
                pixelXY = value;
                OnPropertyChanged("PixelXY");
            }
            get { return pixelXY; }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
