using System.ComponentModel;

namespace RemoteMonitor.Model
{
    public class MonitorBackup : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int _progress;
        private string _status;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
