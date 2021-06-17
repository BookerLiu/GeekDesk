using GeekDesk.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.ViewModel
{

    [Serializable]
    public class BacklogInfo : INotifyPropertyChanged
    {
        //private string id;   //任务唯一id
        private string title; //待办事项
        private string msg;  //事项详情
        private string exeTime;  //待办时间
        private string doneTime; //完成时间
        //private int status;  //状态 0 未处理  1 已处理


        public string DoneTime
        {
            get
            {
                return doneTime;
            }
            set
            {
                doneTime = value;
                OnPropertyChanged("DoneTime");
            }
        }

        //public string Id
        //{
        //    get
        //    {
        //        return id;
        //    }
        //    set
        //    {
        //        id = value;
        //        OnPropertyChanged("Id");
        //    }
        //}

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Msg
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
                OnPropertyChanged("Msg");
            }
        }

        public string ExeTime
        {
            get
            {
                return exeTime;
            }
            set
            {
                exeTime = value;
                OnPropertyChanged("ExeTime");
            }
        }

        //public int Status
        //{
        //    get
        //    {
        //        return status;
        //    }
        //    set
        //    {
        //        status = value;
        //        OnPropertyChanged("status");
        //    }
        //}




        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommonCode.SaveAppData(MainWindow.appData);
        }
    }
}
