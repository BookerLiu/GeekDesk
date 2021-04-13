using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GeekDesk.ViewModel
{
    [Serializable]
    class AppData : INotifyPropertyChanged
    {
        private List<string> menuList = new List<string>();
        private Dictionary<string, List<IconInfo>> iconMap = new Dictionary<string, List<IconInfo>>();
        private AppConfig appConfig = new AppConfig();

        public List<string> MenuList
        {
            get
            {
                return menuList;
            }
            set
            {
                menuList = value;
                OnPropertyChanged("MenuList");
            }
        }

        public Dictionary<string, List<IconInfo>> IconMap
        {
            get
            {
                return iconMap;
            }
            set
            {
                iconMap = value;
                OnPropertyChanged("IconMap");
            }
        }

        public AppConfig AppConfig
        {
            get
            {
                return appConfig;
            }
            set
            {
                appConfig = value;
                OnPropertyChanged("AppConfig");
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
