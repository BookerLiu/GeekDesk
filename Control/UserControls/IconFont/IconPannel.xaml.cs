using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls.IconFont
{
    /// <summary>
    /// IconPannel.xaml 的交互逻辑
    /// </summary>
    public partial class IconPannel : UserControl
    {
        public IconPannel()
        {
            //DataContext = this;
            InitializeComponent();
        }

        public static readonly DependencyProperty IconfontListProperty = DependencyProperty.Register("IconfontList", typeof(List<IconfontInfo>), typeof(IconPannel));
        public List<IconfontInfo> IconfontList
        {
            get
            {
                return (List<IconfontInfo>)GetValue(IconfontListProperty);
            }
            set
            {
                SetValue(IconfontListProperty, true);
            }
        }

        public static readonly DependencyProperty IconInfoProperty = DependencyProperty.Register("IconInfo", typeof(IconfontInfo), typeof(IconPannel));

        public IconfontInfo IconInfo
        {
            get
            {
                return (IconfontInfo)GetValue(IconInfoProperty);
            }
            set
            {
                SetValue(IconInfoProperty, true);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IconfontInfo info = IconfontList[IconListBox.SelectedIndex];
            IconInfo = info;
        }
    }
}
