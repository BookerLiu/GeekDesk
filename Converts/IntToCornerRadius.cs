using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    class IntToCornerRadius : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = int.Parse(value.ToString());
            var param = 0;
            if (parameter != null)
            {
                param = int.Parse(parameter.ToString());
            }
            if (val + param > 0)
            {
                val += param;
            }
            return new CornerRadius(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
