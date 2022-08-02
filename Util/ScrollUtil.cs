using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeekDesk.Util
{
    public class ScrollUtil
    {

        public static bool IsBootomScrollView(ScrollViewer view)
        {
            try
            {
                bool isBottom = false;
                double dVer = view.VerticalOffset;
                double vViewport = view.ViewportHeight;
                double eextent = view.ExtentHeight;
                if (dVer + vViewport >= eextent)
                {
                    isBottom = true;
                }
                else
                {
                    isBottom = false;
                }
                return isBottom;
            } catch (Exception e)
            {
                return false;
            }
            
        }

        public static bool IsTopScrollView(ScrollViewer view)
        {
            try
            {
                return (int)view.VerticalOffset == 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static T FindSimpleVisualChild<T>(DependencyObject element) where T : class
        {
            try
            {
                while (element != null)
                {

                    if (element is T)
                        return element as T;
                    if (VisualTreeHelper.GetChildrenCount(element) > 0)
                    {
                        element = VisualTreeHelper.GetChild(element, 0);
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }

    }
}
