using System.ComponentModel;
using System.Diagnostics;

namespace RemoteMonitor.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        private string ipAddress;
        private string port;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel MainViewModel { get; set; }

        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public string Port
        {
            get { return port; }
            set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private bool _HasConnectionSucceed;
        public bool HasConnectionSucceed
        {
            get { return _HasConnectionSucceed; }
            set
            {
                _HasConnectionSucceed = value;
                OnPropertyChanged(nameof(HasConnectionSucceed));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand ConnectCommand { get; }

        public ConfigViewModel(MainViewModel mainViewModel)
        {
            ConnectCommand = new RelayCommand(HandleConnectCommand);
            MainViewModel = mainViewModel;
            mainViewModel.ConnectionSucceed += HandleConnectionSucceed;
        }

        private void HandleConnectionSucceed(object? sender, EventArgs e)
        {
            HasConnectionSucceed = true;
        }

        private void HandleConnectCommand(object parameter)
        {
            Task.Run(() =>
            {
                MainViewModel.SetConnection(IpAddress, Int32.Parse(Port));
                Trace.WriteLine("Connected to server");
            });
        }
    }
}
