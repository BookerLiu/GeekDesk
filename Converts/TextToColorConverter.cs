using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GeekDesk.Converts
{
    internal class TextToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            string str = value as string;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(str));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            string str = value as string;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(str));
        }
    }
}
