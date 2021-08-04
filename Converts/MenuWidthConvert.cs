using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class MenuWidthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().Length > 0)
            {
                if (parameter == null) parameter = 0.00;
                double p = System.Convert.ToDouble(parameter.ToString());
                return System.Convert.ToDouble(value.ToString()) - p;
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
