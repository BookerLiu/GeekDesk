using GeekDesk.Constant;
using GeekDesk.Util;
using System;
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
        private ObservableCollection<MenuInfo> menuList; //菜单信息及菜单对应icon信息
        private AppConfig appConfig = new AppConfig(); //程序设置信息
        private ObservableCollection<ToDoInfo> hiToDoList; //历史待办
        private ObservableCollection<ToDoInfo> toDoList; //未处理待办 为了提高任务效率 分开处理


        public ObservableCollection<ToDoInfo> HiToDoList
        {
            get
            {
                if (hiToDoList == null)
                {
                    hiToDoList = new ObservableCollection<ToDoInfo>();

                }
                return hiToDoList;
            }
            set
            {
                hiToDoList = value;
                OnPropertyChanged("HiToDoList");
            }
        }

        public ObservableCollection<ToDoInfo> ToDoList
        {
            get
            {
                if (toDoList == null)
                {
                    toDoList = new ObservableCollection<ToDoInfo>();
                }
                return toDoList;
            }
            set
            {
                toDoList = value;
                OnPropertyChanged("ToDoList");
            }
        }

        public ObservableCollection<MenuInfo> MenuList
        {
            get
            {
                if (menuList == null)
                {
                    menuList = new ObservableCollection<MenuInfo>();
                }
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
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
        }

    }
}
