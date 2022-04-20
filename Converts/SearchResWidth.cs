using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    class SearchResWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if ("1".Equals(param))
            {
                double menuLeftWidth = double.Parse(value.ToString());
                return MainWindow.mainWindow.Width - menuLeftWidth;
            } else
            {
                double menuLeftWidth = double.Parse(value.ToString());
                return (MainWindow.mainWindow.Width - menuLeftWidth) / 2;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
