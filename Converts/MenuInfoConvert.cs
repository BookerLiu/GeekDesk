using System;
using System.Globalization;
using System.Windows.Data;

namespace GeekDesk.Converts
{
    public class MenuInfoConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int menuIndex = MainWindow.appData.AppConfig.SelectedMenuIndex;
            if (menuIndex == -1)
            {
                return "";
            }
            else
            {
                string type = parameter.ToString();
                if ("1".Equals(type))
                {
                    return MainWindow.appData.MenuList[menuIndex].MenuGeometry;
                }
                else
                {
                    return MainWindow.appData.MenuList[menuIndex].MenuName;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //bool isChecked = (bool)value;
            //if (!isChecked)
            //{
            //    return null;
            //}
            //return (AppHideType)int.Parse(parameter.ToString());
        }
    }
}
