using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class OpcityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = double.Parse(value.ToString());
            return (double)(Math.Round((decimal)(val / 100.00), 2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
