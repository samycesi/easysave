

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text;
using System.Windows;
using RemoteMonitor.Model;
using System.Diagnostics;

namespace RemoteMonitor.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand ConfigViewCommand { get; }
        public RelayCommand MonitorViewCommand { get; }

        public ConfigViewModel ConfigVM { get; set; }
        public MonitorViewModel MonitorVM { get; set; }

        private Socket server { get; set; }


        public static ObservableCollection<MonitorBackup> MonitorBackups { get; set; }

        public event EventHandler ConnectionSucceed;

        protected virtual void OnConnectionSuccess()
        {
            ConnectionSucceed?.Invoke(this, EventArgs.Empty);
        }

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

        public MainViewModel()
        {
            MonitorBackups = new ObservableCollection<MonitorBackup>();

            ConfigVM = new ConfigViewModel(this);
            MonitorVM = new MonitorViewModel(this);

            ConfigViewCommand = new RelayCommand(o => CurrentView = ConfigVM);
            MonitorViewCommand = new RelayCommand(o => CurrentView = MonitorVM);

            CurrentView = ConfigVM;
        }

        public void SetConnection(string ipAddress, int port)
        {
            this.server = Connect(ipAddress, port);
            Trace.WriteLine("Connected to server");
            if (this.server != null)
            {
                OnConnectionSuccess();
                Listen(this.server);
            }
        }

        private Socket Connect(string ipAddress, int port)
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint ep = new IPEndPoint(ip, port);
            try
            {
                server.Connect(ep);
                return server;
            }
            catch (SocketException e)
            {
                Trace.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        private void Listen(Socket server)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[4096];
                    int receivedBytes = server.Receive(buffer);

                    string msgReceived = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

                    Trace.WriteLine(msgReceived);

                    while (msgReceived != "")
                    {
                        int msgLength = msgReceived.IndexOf("]") + 1;
                        ObservableCollection<MonitorBackup> jsonList = JsonSerializer.Deserialize<ObservableCollection<MonitorBackup>>(msgReceived.Substring(0, msgLength));
                        if (jsonList.Count >= MonitorBackups.Count)
                        {
                            for (int i = 0; i < jsonList.Count; i++)
                            {
                                if (MonitorBackups.Count > i && jsonList[i].Name == MonitorBackups[i].Name)
                                {
                                    MonitorBackups[i].Progress = jsonList[i].Progress;
                                }
                                else
                                {
                                    MonitorBackups = jsonList;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            int j = 0;

                            for (int i = 0; i < MonitorBackups.Count; i++)
                            {
                                if (MonitorBackups[j].Name == jsonList[i].Name)
                                {
                                    MonitorBackups[j].Progress = jsonList[i].Progress;
                                }
                                else
                                {
                                    MonitorBackups = jsonList;
                                    break;
                                }
                                j++;
                            }
                        }

                        msgReceived = msgReceived.Length > msgLength + 1 ? msgReceived.Substring(msgLength) : "";
                    }
                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Une erreur s'est produite lors de la réception des données du serveur.");
            }
        }

        public void SendAction(string action, int[] key)
        {
            object actionToSend = new
            {
                Action = action,
                KeysSelected = key
            };
            string data = JsonSerializer.Serialize(actionToSend);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            Trace.WriteLine(data);
            try
            {
                server.Send(buffer);
            }
            catch (SocketException e)
            {
                Trace.WriteLine("Error: " + e.Message);
            }
        }
    }
}
