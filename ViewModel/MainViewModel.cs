using easysave.Model;
using easysave.Model.Logger;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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

            HomeViewCommand = new RelayCommand(o => { ChangeView(HomeVM); });
            CreateSaveViewCommand = new RelayCommand(o => { ChangeView(CreateTaskVM); });
            TasksViewCommand = new RelayCommand(o => { ChangeView(TaskVM); });
            SettingsViewCommand = new RelayCommand(o => { ChangeView(SettingsVM); });

            HomeVM = new HomeViewModel(this);
            CreateTaskVM = new CreateTaskViewModel(BackupTaskViewModels);
            TaskVM = new TaskViewModel(BackupTaskViewModels);
            SettingsVM = new SettingsViewModel(DailyLogger, StateTrackLogger);

            CurrentView = HomeVM;
        }

        private void ChangeView(object view)
        {
            if (IsPathsValid())
            {
                CurrentView = view;
            } else
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

    }

}