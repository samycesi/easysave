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
        public static Barrier barrierPrio = new Barrier(0);
        public static Mutex mutex = new Mutex();
        public static CountdownEvent countdownEvent = new CountdownEvent(0);

        public event PropertyChangedEventHandler PropertyChanged;

        private BackupList backupList;

        public ObservableCollection<BackupTaskViewModel> BackupTasks { get; }

        public RelayCommand ExecuteBackupCommand { get; }
        public RelayCommand DeleteBackupCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand PauseBackupCommand { get; }
        public RelayCommand StopBackupCommand { get; }

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
            PauseBackupCommand = new RelayCommand(PauseBackup);
            StopBackupCommand = new RelayCommand(StopBackup);

            this.DailyLogger = MainViewModel.DailyLogger;
        }

        /// <summary>
        /// Executes the selected backups
        /// </summary>
        /// <param name="parameter"></param>
        private void Execute(List<BackupTaskViewModel> taskToExecute)
        {

            List<Task> backupTasks = new List<Task>();

            // Add the participants to the barrier (= the amount of selected backups)
            barrierPrio.AddParticipants(taskToExecute.Count);

            // Initialize or increment CountDownEvent to make sure backups are waiting for the priority files to be copied after the barrier
            if (countdownEvent == null || countdownEvent.IsSet)
            {
                countdownEvent = new CountdownEvent(taskToExecute.Count);
            }
            else
            {
                countdownEvent.AddCount(taskToExecute.Count);
            }

            foreach (var backup in taskToExecute)
            {
                Task task = Task.Run(() =>
                {

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

        private void ExecuteBackup(object parameter)
        {
            var selectedBackups = BackupTasks.Where(task => task.IsSelected).ToList();
            var taskToExecute = new List<BackupTaskViewModel>();
            foreach (var backup in selectedBackups)
            {
                if (backup.Backup.State.State == "PAUSED")
                {
                    backup.Backup.State.State = "ACTIVE";
                }
                else if (backup.Backup.State.State == "INACTIVE" || backup.Backup.State.State == "STOPPED")
                {
                    taskToExecute.Add(backup);
                }
            }
            if (taskToExecute.Count()>0)
            {
                Execute(taskToExecute);
            }
        }

        private void PauseBackup(object parameter)
        {
            var selectedBackups = BackupTasks.Where(task => task.IsSelected).ToList(); // Get the selected backups

            foreach (var backup in selectedBackups)
            {
                backup.Backup.State.State = "PAUSED";
                Trace.WriteLine("backup paused");
            }
        }

        private void StopBackup(object parameter)
        {
            var selectedBackups = BackupTasks.Where(task => task.IsSelected).ToList(); // Get the selected backups

            foreach (var backup in selectedBackups)
            {
                backup.Backup.State.State = "STOPPED";
            }
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