using easysave.Model;
using easysave.Model.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace easysave.ViewModel
{

    public class MainViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<BackupTaskViewModel> backupTaskViewModels;
        public ObservableCollection<BackupTaskViewModel> BackupTaskViewModels
        {
            get { return backupTaskViewModels; }
            set
            {
                backupTaskViewModels = value;
                UpdateDataToSend();
                OnPropertyChanged();
            }
        }

        public static BackupList BackupList { get; set; }
        public static DailyLogger DailyLogger { get; set; }
        public static StateTrackLogger StateTrackLogger { get; set; }

        public RelayCommand HomeViewCommand { get; }
        public RelayCommand CreateSaveViewCommand { get; }
        public RelayCommand TasksViewCommand { get; }
        public RelayCommand SettingsViewCommand { get; }

        public HomeViewModel HomeVM { get; set; }
        public CreateTaskViewModel CreateTaskVM { get; set; }
        public TaskViewModel TaskVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        public Socket Client { get; set; }
        public Socket Listener { get; set; }

        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            BackupList = new BackupList();

            DailyLogger = new DailyLogger(App.appConfigData.DailyPath, DateTime.Today.ToString("yyyy-MM-dd") + App.appConfigData.LogExtension);
            StateTrackLogger = new StateTrackLogger(App.appConfigData.StateTrackPath, "state" + App.appConfigData.LogExtension, BackupList);

            BackupTaskViewModels = new ObservableCollection<BackupTaskViewModel>(
                BackupList.BackupTasks.Select(task => new BackupTaskViewModel(task.Key, task.Value))
            );

            foreach (var backup in BackupTaskViewModels)
            {
                backup.Backup.State.StateUpdated += HandleStateUpdated;
            }

            HomeViewCommand = new RelayCommand(o => { ChangeView(HomeVM); });
            CreateSaveViewCommand = new RelayCommand(o => { ChangeView(CreateTaskVM); });
            TasksViewCommand = new RelayCommand(o => { ChangeView(TaskVM); });
            SettingsViewCommand = new RelayCommand(o => { ChangeView(SettingsVM); });

            HomeVM = new HomeViewModel(this);
            CreateTaskVM = new CreateTaskViewModel(BackupTaskViewModels);
            TaskVM = new TaskViewModel(BackupTaskViewModels);
            SettingsVM = new SettingsViewModel(DailyLogger, StateTrackLogger);

            CurrentView = HomeVM;

            SetConnection();
        }

        private void ChangeView(object view)
        {
            if (IsPathsValid())
            {
                CurrentView = view;
            }
            else
            {
                CurrentView = SettingsVM;
            }
        }

        private bool IsPathsValid()
        {
            Console.WriteLine(App.appConfigData.DailyPath);
            Console.WriteLine(App.appConfigData.StateTrackPath);
            Console.WriteLine(Directory.Exists(App.appConfigData.DailyPath));
            Console.WriteLine(Directory.Exists(App.appConfigData.StateTrackPath));
            return (Directory.Exists(App.appConfigData.DailyPath) && Directory.Exists(App.appConfigData.StateTrackPath)) || File.Exists(App.appConfigData.DailyPath) || File.Exists(App.appConfigData.StateTrackPath);
        }

        public void SetConnection()
        {
            Task.Run(() =>
            {
                Listener = Connect();
                Client = AcceptConnection(Listener);
                Trace.WriteLine("Connected to server : serv");
                SendData(Client);
                Trace.WriteLine("Data sent : serv");
                ListenAction(Client);
            });
        }

        private Socket Connect()
        {
            string ipAddress = "127.0.0.1";
            int port = 8080;

            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint ep = new IPEndPoint(ip, port);

            try
            {
                listenerSocket.Bind(ep);
                listenerSocket.Listen(10);
                return listenerSocket;
            }
            catch (SocketException e)
            {
                return null;
            }
        }

        private Socket AcceptConnection(Socket listener)
        {
            return listener.Accept();
        }

        private void SendData(Socket client)
        {
            List<MonitorBackup> monitorBackups = new List<MonitorBackup>();

            foreach (var backup in BackupList.BackupTasks)
            {
                monitorBackups.Add(new MonitorBackup(backup.Value.Name, backup.Value.State.Progress, backup.Value.State.State));
            }

            string data = JsonSerializer.Serialize(monitorBackups);
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            try
            {
                client.Send(buffer);
            }
            catch (SocketException e)
            {
                return;
            }
        }

        public void UpdateDataToSend()
        {
            if (Client != null)
            {
                Task.Run(() =>
                {
                    SendData(Client);
                });
            }
        }

        private void HandleStateUpdated(object sender, EventArgs e)
        {
            UpdateDataToSend();
        }

        private void ListenAction(Socket client)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int receivedBytes = client.Receive(buffer);

                    string msgReceived = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    ActionReceived action = JsonSerializer.Deserialize<ActionReceived>(msgReceived);

                    foreach (int key in action.KeysSelected)
                    {
                        BackupTaskViewModels[key].IsSelected = true;
                        if (action.Action == "start")
                        {
                            TaskVM.ExecuteBackupCommand.Execute(null);
                        }
                        else if (action.Action == "pause")
                        {
                            TaskVM.PauseBackupCommand.Execute(null);
                        }
                        else if (action.Action == "stop")
                        {
                            TaskVM.StopBackupCommand.Execute(null);
                        }
                    }
                }
                catch (SocketException e)
                {
                    return;
                }
            }
        }

    }

}