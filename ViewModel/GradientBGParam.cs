using GeekDesk.Constant;
using GeekDesk.Util;
using System;
using System.ComponentModel;

namespace GeekDesk.ViewModel
{
    [Serializable]
    public class GradientBGParam : INotifyPropertyChanged
    {
        private string id;

        private string color1;

        private string color2;

        private string name;

        public GradientBGParam() { }

        public GradientBGParam(string id, string name, string color1, string color2)
        {
            this.id = id;
            this.name = name;
            this.color1 = color1;
            this.color2 = color2;
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Color1
        {
            get
            {
                return color1;
            }
            set
            {
                color1 = value;
                OnPropertyChanged("Color1");
            }
        }

        public string Color2
        {
            get
            {
                return color2;
            }
            set
            {
                color2 = value;
                OnPropertyChanged("Color2");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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
