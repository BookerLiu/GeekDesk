using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    internal class ValueConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) {
                Dictionary<bool, string> dict = (Dictionary<bool, string>)parameter;
                bool val = (bool)value;
                return dict[val];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                Dictionary<bool, string> dict = (Dictionary<bool, string>)parameter;
                bool val = (bool)value;
                return dict[val];
            }
            return null;
        }
    }
}
