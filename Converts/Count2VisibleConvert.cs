using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class Count2VisibleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            int count = (int)value;

            if (parameter == null || "Y".Equals(parameter.ToString()))
            {
                if (count > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            } else
            {
                if (count <= 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
