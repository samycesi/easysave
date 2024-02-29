using RemoteMonitor.Model;
using System.Collections.ObjectModel;

namespace RemoteMonitor.ViewModel
{
    public class MonitorViewModel
    {
        public ObservableCollection<MonitorBackup> MonitorBackups
        {
            get { return MainViewModel.MonitorBackups; }
        }

        public MainViewModel MainViewModel { get; set; }

        public RelayCommand RefreshCommand { get; }

        public MonitorViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        public void Start(int[] keys)
        {
            MainViewModel.SendAction("start", keys);
        }

        public void Pause(int[] keys)
        {
            MainViewModel.SendAction("pause", keys);
        }

        public void Stop(int[] keys)
        {
            MainViewModel.SendAction("stop", keys);
        }

    }
}
