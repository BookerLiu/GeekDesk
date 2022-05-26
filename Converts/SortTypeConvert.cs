using GeekDesk.Constant;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class SortTypeConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SortType)value == (SortType)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (SortType)int.Parse(parameter.ToString());
        }
    }
}
