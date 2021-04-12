using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeekDesk.Constant;

namespace GeekDesk.ViewModel
{
    [Serializable]
    public class AppConfig  :  ViewModelBase
    {
        private int menuSortType = (int)SortType.CUSTOM; //菜单排序类型
        private int iconSortType = (int)SortType.CUSTOM; //图表排序类型
        private double windowWidth = (double)DefaultConstant.WINDOW_WIDTH;  //窗口宽度
        private double windowHeight = (double)DefaultConstant.WINDOW_HEIGHT; //窗口高度
        private double menuCardWidth = (double)DefaultConstant.MENU_CARD_WIDHT;//菜单栏宽度


        #region GetSet
        public int MenuSortType {
            get
            {
                return menuSortType;
            }
            set
            {
                menuSortType = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        #endregion

    }
}
