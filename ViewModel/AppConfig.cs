
using GeekDesk.Constant;
using GeekDesk.Util;
using System;
using System.ComponentModel;
using System.Windows;

/// <summary>
/// 程序设置
/// </summary>
namespace GeekDesk.ViewModel
{

    [Serializable]
    public class AppConfig : INotifyPropertyChanged
    {
        private SortType menuSortType = SortType.CUSTOM; //菜单排序类型
        private SortType iconSortType = SortType.CUSTOM; //图表排序类型
        private double windowWidth = (double)DefaultConstant.WINDOW_WIDTH;  //窗口宽度
        private double windowHeight = (double)DefaultConstant.WINDOW_HEIGHT; //窗口高度
        private double menuCardWidth = (double)DefaultConstant.MENU_CARD_WIDHT;//菜单栏宽度
        private int selectedMenuIndex = 0;  //上次选中菜单索引
        private bool followMouse = true;  //面板跟随鼠标 默认是
        private Visibility configIconVisible = Visibility.Visible; // 设置按钮是否显示
        private AppHideType appHideType = AppHideType.START_EXE;  //面板关闭方式 (默认启动程序后)
        private bool startedShowPanel = true;  //启动时是否显示主面板  默认显示


        #region GetSet

        public bool StartedShowPanel
        {
            get
            {
                return startedShowPanel;
            }
            set
            {
                startedShowPanel = value;
                OnPropertyChanged("StartedShowPanel");
            }
        }

        public AppHideType AppHideType
        {
            get
            {
                return appHideType;
            }
            set
            {
                appHideType = value;
                OnPropertyChanged("AppHideType");
            }
        }

        public Visibility ConfigIconVisible
        {
            get
            {
                return configIconVisible;
            }
            set
            {
                configIconVisible = value;
                OnPropertyChanged("ConfigIconVisible");
            }
        }


        public bool FollowMouse
        {
            get
            {
                return followMouse;
            }
            set
            {
                followMouse = value;
                OnPropertyChanged("FollowMouse");
            }
        }

        public int SelectedMenuIndex
        {
            get
            {
                return selectedMenuIndex;
            }
            set
            {
                selectedMenuIndex = value;
                OnPropertyChanged("SelectedMenuIndex");
            }
        }

        public SortType MenuSortType
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

        public SortType IconSortType
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
            CommonCode.SaveAppData(MainWindow.appData);
        }

        #endregion

    }
}
