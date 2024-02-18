using easysave.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Forms;

namespace easysave.ViewModel
{
    public class CreateTaskViewModel : INotifyPropertyChanged
    {
        private string newBackupName;
        public string NewBackupName
        {
            get { return newBackupName; }
            set { newBackupName = value; NotifyPropertyChanged(); }
        }

        private string newBackupSource;
        public string NewBackupSource
        {
            get { return newBackupSource; }
            set { newBackupSource = value; NotifyPropertyChanged(); }
        }

        private string newBackupDestination;
        public string NewBackupDestination
        {
            get { return newBackupDestination; }
            set { newBackupDestination = value; NotifyPropertyChanged(); }
        }

        private string newBackupType;
        public string NewBackupType
        {
            get { return newBackupType; }
            set { newBackupType = value; NotifyPropertyChanged(); }
        }

        private BackupList backupList;
        private ObservableCollection<BackupTaskViewModel> backupTaskViewModels;

        public ObservableCollection<BackupModel> BackupTasks { get; }

        public RelayCommand BrowseSourceCommand { get; }
        public RelayCommand BrowseDestinationCommand { get; }
        public RelayCommand AddBackupCommand { get; }
        
        public CreateTaskViewModel(ObservableCollection<BackupTaskViewModel> backupTaskViewModels)
        {
            this.backupTaskViewModels = backupTaskViewModels;
            backupList = MainViewModel.BackupList;
            BackupTasks = new ObservableCollection<BackupModel>(backupList.BackupTasks.Values);

            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseDestinationCommand = new RelayCommand(BrowseDestination);
            AddBackupCommand = new RelayCommand(AddBackup);
        }

        /// <summary>
        /// Browse for the source folder
        /// </summary>
        /// <param name="parameter"></param>
        private void BrowseSource(object parameter)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog(); // Show the dialog 
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    NewBackupSource = folderDialog.SelectedPath; // Set the source folder
                }
            }
        }

        /// <summary>
        /// Browse for the destination folder
        /// </summary>
        /// <param name="parameter"></param>
        private void BrowseDestination(object parameter)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog(); // Show the dialog
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    NewBackupDestination = folderDialog.SelectedPath; // Set the destination folder
                }
            }
        }

        /// <summary>
        /// Add a new backup with the input values
        /// </summary>
        /// <param name="parameter"></param>
        private void AddBackup(object parameter)
        {
            BackupType backupType = NewBackupType.Contains("Full") || NewBackupType.Contains("Complète")? BackupType.Full : BackupType.Differential; // Set the backup type
            var newBackup = new BackupModel(NewBackupName, NewBackupSource, NewBackupDestination, backupType); // Create a new backup

            // Add new backup to the backup list
            backupList.AddBackupTask(newBackup);

            // Create a new BackupTaskViewModel for the new backup
            var newBackupTaskViewModel = new BackupTaskViewModel(backupList.BackupTasks.Count, newBackup);

            // Add the new BackupTaskViewModel to the collection
            backupTaskViewModels.Add(newBackupTaskViewModel);

            // Reset input values after adding
            NewBackupName = "";
            NewBackupSource = "";
            NewBackupDestination = "";
            NewBackupType = "Full";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
