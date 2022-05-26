using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class CountGreZero2BoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            int count = (int)value;
            if (count > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
