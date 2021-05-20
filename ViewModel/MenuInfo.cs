using GeekDesk.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace GeekDesk.ViewModel
{

    [Serializable]
    public class MenuInfo : INotifyPropertyChanged
    {
       

        private string menuName;
        private string menuId;
        private Visibility menuEdit = Visibility.Collapsed;
        private Visibility notMenuEdit = Visibility.Visible;
        private ObservableCollection<IconInfo> iconList = new ObservableCollection<IconInfo>();

        public string MenuName
        {
            get
            {
                return menuName;
            }
            set
            {
                menuName = value;
                OnPropertyChanged("MenuName");               
            }
        }

        public string MenuId
        {
            get
            {
                return menuId;
            }
            set
            {
                menuId = value;
                OnPropertyChanged("MenuId");
            }
        }

        public Visibility MenuEdit
        {
            get
            {
                return menuEdit;
            }
            set
            {
                menuEdit = value;
                if (menuEdit == Visibility.Visible)
                {
                    NotMenuEdit = Visibility.Collapsed;
                } else
                {
                    NotMenuEdit = Visibility.Visible;
                }
                OnPropertyChanged("MenuEdit");
            }
        }

        public Visibility NotMenuEdit
        {
            get
            {
                return notMenuEdit;
            }
            set
            {
                notMenuEdit = value;
                OnPropertyChanged("NotMenuEdit");
            }
        }

        public ObservableCollection<IconInfo> IconList
        {
            get
            {
                return iconList;
            }
            set
            {
                iconList = value;
                OnPropertyChanged("IconList");
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
