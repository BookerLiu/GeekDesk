using System.ComponentModel;


namespace ShowSeconds.ViewModel
{
    public class SecondsDataContext : INotifyPropertyChanged
    {
        private string seconds;
        public string Seconds
        {
            set
            {
                seconds = value;
                OnPropertyChanged("Seconds");
            }
            get { return seconds; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
