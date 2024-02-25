using easysave.Model.Logger;
using easysave.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace easysave.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private BackupList backupList;

        public ObservableCollection<BackupTaskViewModel> BackupTasks { get; }

        public RelayCommand ExecuteBackupCommand { get; }
        public RelayCommand DeleteBackupCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public DailyLogger DailyLogger { get; set; }

        private AutoResetEvent fileAccessEvent = new AutoResetEvent(true);

        private string _consoleOutput;

        public string ConsoleOutput
        {
            get { return _consoleOutput; }
            set
            {
                _consoleOutput = value;
                OnPropertyChanged();
            }
        }

        public TaskViewModel(ObservableCollection<BackupTaskViewModel> backupTasks)
        {
            backupList = MainViewModel.BackupList;
            BackupTasks = backupTasks;
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup);
            RefreshCommand = new RelayCommand(RefreshBackupTasks);

            this.DailyLogger = MainViewModel.DailyLogger;
        }

        /// <summary>
        /// Executes the selected backups
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteBackup(object parameter)
        {
            var selectedBackups = BackupTasks.Where(task => task.IsSelected).ToList(); // Get the selected backups

            List<Task> backupTasks = new List<Task>();

            foreach (var backup in selectedBackups)
            { 
                Task task = Task.Run(() =>
                {
                    // Check if the business software process is running
                    while (IsBusinessSoftwareRunning())
                    {
                        // Log a message indicating that the backup execution is paused
                        string pauseMessage = "Backup execution paused because the business software is running";
                        AppendToConsole(pauseMessage);

                        // Sleep for a short duration before checking again
                        Thread.Sleep(1000); // Sleep for 1 second (adjust as needed)
                    }

                    // Log the backup execution in the console
                    string logMessage = $"Task {backup.Backup.Name} launched";
                    AppendToConsole(logMessage);

                    // Execute the backup
                    (BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) = backupList.ExecuteTaskByKey(backup.Key);

                    // Log the backup execution in the daily log
                    switch (App.appConfigData.LogExtension)
                    {
                        case ".json":
                            fileAccessEvent.WaitOne();
                            DailyLogger.WriteDailyLogJSON(task, fileSize, fileTransferTime, totalEncryptionTime);
                            fileAccessEvent.Set();
                            break;
                        case ".xml":
                            fileAccessEvent.WaitOne();
                            DailyLogger.WriteDailyLogXML(task, fileSize, fileTransferTime, totalEncryptionTime);
                            fileAccessEvent.Set();
                            break;
                    }

                    // Log the backup execution in the console
                    logMessage = $"Task {task.Name} finished successfully in {fileTransferTime}ms";
                    AppendToConsole(logMessage);
                });

                backupTasks.Add(task);
            }
        }

        private bool IsBusinessSoftwareRunning()
        {
            string softwarePath = App.appConfigData.BusinessSoftwarePath;
            // Check if the business software process is running
            string processName = Path.GetFileNameWithoutExtension(softwarePath);
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                if (process.MainModule.FileName.Equals(softwarePath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Deletes the selected backups
        /// </summary>
        /// <param name="parameter"></param>
        private void DeleteBackup(object parameter)
        {
            var selectedKeys = BackupTasks.Where(task => task.IsSelected).Select(task => task.Key).ToList(); // Get the selected keys
            foreach (var key in selectedKeys)
            {
                backupList.DeleteBackupTask(key); // Delete the backup
                String logMessage = $"Task successfully deleted";
                AppendToConsole(logMessage);
            }
            backupList.ReorderKeys(); // Reorder the keys
            RefreshBackupTasks(null); // Refresh the backup tasks
        }

        /// <summary>
        /// Refreshes the backup tasks
        /// </summary>
        /// <param name="parameter"></param>
        private void RefreshBackupTasks(object parameter)
        {
            BackupTasks.Clear(); // Clear the backup tasks
            foreach (var backup in backupList.BackupTasks)
            {
                // Add the backup tasks to the collection
                BackupTasks.Add(new BackupTaskViewModel(backup.Key, backup.Value));
            }
        }

        /// <summary>
        /// Appends a message to the console
        /// </summary>
        /// <param name="message"></param>
        private void AppendToConsole(string message)
        {
            
            ConsoleOutput += "> " + message + "\n";
            Console.WriteLine(message);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}