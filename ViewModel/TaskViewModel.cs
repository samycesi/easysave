using easysave.Model.Logger;
using easysave.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

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
        public StateTrackLogger StateTrackLogger { get; set; }

        public TaskViewModel(ObservableCollection<BackupTaskViewModel> backupTasks)
        {
            backupList = MainViewModel.BackupList;
            BackupTasks = backupTasks;
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup);
            RefreshCommand = new RelayCommand(RefreshBackupTasks);

            this.DailyLogger = MainViewModel.DailyLogger;
            this.StateTrackLogger = MainViewModel.StateTrackLogger;
        }

        /// <summary>
        /// Executes the selected backups
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteBackup(object parameter)
        {
            var selectedBackups = BackupTasks.Where(task => task.IsSelected).ToList(); // Get the selected backups
            foreach (var backup in selectedBackups)
            {
                // Execute the backup
                (BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) = backupList.ExecuteTaskByKey(backup.Key, StateTrackLogger);
                // Log the backup execution in the daily log
                switch (App.appConfigData.LogExtension)
                {
                    case ".json":
                        DailyLogger.WriteDailyLogJSON(task, fileSize, fileTransferTime, totalEncryptionTime);
                        break;
                    case ".xml":
                        DailyLogger.WriteDailyLogXML(task, fileSize, fileTransferTime, totalEncryptionTime);
                        break;
                }
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
            }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}