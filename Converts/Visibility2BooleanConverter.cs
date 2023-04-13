using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    internal class Visibility2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return (Visibility)value == Visibility.Visible;
            } else
            {
                return !((Visibility)value == Visibility.Visible);
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
