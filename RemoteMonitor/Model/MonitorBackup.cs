using System.ComponentModel;

namespace RemoteMonitor.Model
{
    public class MonitorBackup : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int _progress;

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
