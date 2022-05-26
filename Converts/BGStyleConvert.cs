using GeekDesk.Constant;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    class BGStyleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BGStyle)value == (BGStyle)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (BGStyle)int.Parse(parameter.ToString());
        }
    }
}
