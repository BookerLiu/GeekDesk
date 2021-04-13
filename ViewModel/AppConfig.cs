
using GeekDesk.Constant;
using System;
using System.ComponentModel;

namespace GeekDesk.ViewModel
{

    [Serializable]
    public class AppConfig : System.ComponentModel.INotifyPropertyChanged
    {
        private int menuSortType = (int)SortType.CUSTOM; //菜单排序类型
        private int iconSortType = (int)SortType.CUSTOM; //图表排序类型
        private double windowWidth = (double)DefaultConstant.WINDOW_WIDTH;  //窗口宽度
        private double windowHeight = (double)DefaultConstant.WINDOW_HEIGHT; //窗口高度
        private double menuCardWidth = (double)DefaultConstant.MENU_CARD_WIDHT;//菜单栏宽度



        #region GetSet
        public int MenuSortType
        {
            get
            {
                return menuSortType;
            }
            set
            {
                menuSortType = value;
                OnPropertyChanged("MenuSortType");
            }
        }

        public int IconSortType
        {
            get
            {
                return iconSortType;
            }
            set
            {
                iconSortType = value;
                OnPropertyChanged("IconSortType");
            }
        }

        public double WindowWidth
        {
            get
            {
                return windowWidth;
            }
            set
            {
                windowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }

        public double WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                windowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }

        public double MenuCardWidth
        {
            get
            {
                return menuCardWidth;
            }
            set
            {
                menuCardWidth = value;
                OnPropertyChanged("MenuCardWidth");
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
