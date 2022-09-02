using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowSeconds.Common
{
    public class Constants
    {
        public readonly static string MY_UUID = "8400A17AEEF7C029";

        //dark theam
        public readonly static System.Windows.Media.SolidColorBrush darkBG
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(46, 50, 54),
                Opacity = 0.8
            };
        public readonly static System.Windows.Media.SolidColorBrush darkFont
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(255, 255, 255)
            };

        //light theam
        public readonly static System.Windows.Media.SolidColorBrush lightBG
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(236, 244, 251),
                Opacity = 1
            };
        public readonly static System.Windows.Media.SolidColorBrush lightFont
            = new System.Windows.Media.SolidColorBrush
            {
                Color = System.Windows.Media.Color.FromRgb(65, 63, 61),
            };
    }
}
