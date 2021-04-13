using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Util
{
    class MenuWidthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().Length > 0)
            {
                return System.Convert.ToDouble(value.ToString()) - 10d;
            }
            else
            {
                return 0d;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
