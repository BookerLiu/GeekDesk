using GeekDesk.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

/// <summary>
/// 程序数据
/// </summary>
namespace GeekDesk.ViewModel
{
    [Serializable]
    public class AppData : INotifyPropertyChanged
    {
        private ObservableCollection<MenuInfo> menuList = new ObservableCollection<MenuInfo>();
        private AppConfig appConfig = new AppConfig();

        public ObservableCollection<MenuInfo> MenuList
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
            CommonCode.SaveAppData(MainWindow.appData);
        }

    }
}
