using GeekDesk.Constant;
using GeekDesk.Util;
using System;
using System.ComponentModel;

namespace GeekDesk.ViewModel
{

    [Serializable]
    public class ToDoInfo : INotifyPropertyChanged
    {
        //private string id;   //任务唯一id
        private string title; //待办事项
        private string msg;  //事项详情
        private string exeTime;  //待办时间
        private string doneTime; //完成时间
        private TodoTaskExecType execType = TodoTaskExecType.SET_TIME;
        private string cron;  //cron表达式
        //private int status;  //状态 0 未处理  1 已处理

        public string Cron
        {
            get
            {
                return cron;
            }
            set
            {
                cron = value;
                OnPropertyChanged("Cron");
            }
        }


        public TodoTaskExecType ExecType
        {
            get
            {
                //兼容老版本 需要给个默认值
                if (execType == 0) return TodoTaskExecType.SET_TIME;
                return execType;
            }
            set
            {
                execType = value;
                OnPropertyChanged("ExecType");
            }
        }

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
            CommonCode.SaveAppData(MainWindow.appData, Constants.DATA_FILE_PATH);
        }
    }
}
