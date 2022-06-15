using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    class StringAppendConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           if (value == null)
            {
                value = "";
            }
            if (parameter == null)
            {
                return value.ToString();
            }
            else
            {
                string val = value.ToString();
                if (string.IsNullOrEmpty(val))
                {
                    return parameter.ToString()
                        .Replace("\\n", "")
                        .Replace("{}", "");
                }
                string param = parameter.ToString();
                param = param.Replace("\\n", "\n");
                return param.Replace("{}", val);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
