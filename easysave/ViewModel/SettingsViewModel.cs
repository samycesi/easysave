using easysave.Model;
using easysave.Model.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace easysave.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string selectedExtension;
        public string SelectedExtension
        {
            get { return selectedExtension; }
            set
            {
                selectedExtension = value;
                OnPropertyChanged();
            }
        }

        private string newDailyPath;
        public string NewDailyPath
        {
            get { return newDailyPath; }
            set
            {
                newDailyPath = value;
                OnPropertyChanged();
            }
        }

        private string newStateTrackPath;
        public string NewStateTrackPath
        {
            get { return newStateTrackPath; }
            set
            {
                newStateTrackPath = value;
                OnPropertyChanged();
            }
        }

        private string newBusinessSoftware;
        public string NewBusinessSoftware
        {
            get { return newBusinessSoftware; }
            set
            {
                newBusinessSoftware = value;
                OnPropertyChanged();
            }
        }

        private string extensionToEncrypt;
        public string ExtensionToEncrypt
        {
            get { return extensionToEncrypt; }
            set
            {
                if (extensionToEncrypt != value)
                {
                    extensionToEncrypt = value;
                    OnPropertyChanged(nameof(ExtensionToEncrypt));
                }
            }
        }

        private long thresholdFileSize;
        public long ThresholdFileSize
        {
            get { return thresholdFileSize; }
            set
            {
                if (thresholdFileSize != value)
                {
                    thresholdFileSize = value;
                    OnPropertyChanged(nameof(ThresholdFileSize));
                }
            }
        }

        private string newPriorityExtension;
        public string NewPriorityExtension
        {
            get { return newPriorityExtension; }
            set
            {
                if (newPriorityExtension != value)
                {
                    newPriorityExtension = value;
                    OnPropertyChanged(nameof(NewPriorityExtension));
                }
            }
        }

        private ObservableCollection<PriorityExtensionViewModel> priorityExtensions;
        public ObservableCollection<PriorityExtensionViewModel> PriorityExtensions
        {
            get { return priorityExtensions; }
            set
            {
                priorityExtensions = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseDailyPathCommand { get; }
        public ICommand BrowseStatePathCommand { get; }
        public ICommand BrowseBusinessSoftwareCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand AddExtensionToListCommand { get; }
        public ICommand RemovePriorityExtensionsFromListCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DailyLogger dailyLogger;
        private StateTrackLogger stateTrackLogger;

        public SettingsViewModel(DailyLogger dailyLogger, StateTrackLogger stateTrackLogger)
        {
            this.dailyLogger = dailyLogger;
            this.stateTrackLogger = stateTrackLogger;

            SelectedExtension = App.appConfigData.LogExtension;
            NewDailyPath = App.appConfigData.DailyPath;
            NewStateTrackPath = App.appConfigData.StateTrackPath;
            NewBusinessSoftware = App.appConfigData.BusinessSoftwarePath;
            ExtensionToEncrypt = App.appConfigData.FileExtensionToEncrypt;
            ThresholdFileSize = App.appConfigData.ThresholdFileSize;
            PriorityExtensions = PriorityExtensions = new ObservableCollection<PriorityExtensionViewModel>(
                App.appConfigData.PriorityExtensions.Select(ext => new PriorityExtensionViewModel
                {
                    Extension = ext
                })
            );
            BrowseDailyPathCommand = new RelayCommand(BrowseDailyPath);
            BrowseStatePathCommand = new RelayCommand(BrowseStatePath);
            BrowseBusinessSoftwareCommand = new RelayCommand(BrowseBusinessSoftwarePath);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            AddExtensionToListCommand = new RelayCommand(AddExtensionToList);
            RemovePriorityExtensionsFromListCommand = new RelayCommand(RemovePriorityExtensionsFromList);
        }

        /// <summary>
        /// Browse for a new daily log path
        /// </summary>
        /// <param name="obj"></param>
        private void BrowseDailyPath(object obj)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                NewDailyPath = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Browse for a new state track log path
        /// </summary>
        /// <param name="obj"></param>
        private void BrowseStatePath(object obj)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                NewStateTrackPath = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Browse for a new business software file path
        /// </summary>
        /// <param name="obj"></param>
        private void BrowseBusinessSoftwarePath(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Business Software";
            openFileDialog.Filter = "(*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                NewBusinessSoftware = openFileDialog.FileName;

            }
        }

        /// <summary>
        /// Save the settings
        /// </summary>
        /// <param name="obj"></param>
        private void SaveSettings(object obj)
        {
            string selectedExtension;
            if (SelectedExtension.ToLower().Contains("json"))
            {
                selectedExtension = ".json";
            }
            else
            {
                selectedExtension = ".xml";
            }

            SelectedExtension = selectedExtension;
            App.appConfigData.BusinessSoftwarePath = NewBusinessSoftware;
            App.appConfigData.FileExtensionToEncrypt = ExtensionToEncrypt;
            App.appConfigData.LogExtension = SelectedExtension;
            // Check if dailypath changed 
            if (!App.appConfigData.DailyPath.Equals(NewDailyPath))
            {
                ChangeLogPath(NewDailyPath, dailyLogger);
                App.appConfigData.DailyPath = NewDailyPath;
            }
            // Check if 
            if (!App.appConfigData.StateTrackPath.Equals(NewStateTrackPath))
            {
                ChangeLogPath(NewStateTrackPath, stateTrackLogger);
                App.appConfigData.StateTrackPath = NewStateTrackPath;
            }
            App.appConfigData.ThresholdFileSize = ThresholdFileSize;
            App.appConfigData.PriorityExtensions = PriorityExtensions.Select(item => item.Extension).ToArray();

            App.appConfigData.SaveToFile();
            Console.WriteLine("Settings saved");
            Console.WriteLine("Language: " + App.appConfigData.Language);
            Console.WriteLine("Log extension: " + App.appConfigData.LogExtension);
            Console.WriteLine("Daily path: " + App.appConfigData.DailyPath);
            Console.WriteLine("State track path: " + App.appConfigData.StateTrackPath);
            Console.WriteLine("Threshold file size : " + App.appConfigData.ThresholdFileSize);
            Console.WriteLine("Priority extensions : " + App.appConfigData.PriorityExtensions);
            ChangeLogTypes(SelectedExtension);
        }

        /// <summary>
        /// Change the log path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="logger"></param>
        private void ChangeLogPath(string path, Logger logger)
        {
            if (File.Exists(logger.FilePath))
            {
                string newFilePath = Path.Combine(path, Path.GetFileName(logger.FilePath));
                File.WriteAllText(newFilePath, File.ReadAllText(logger.FilePath));
                File.Delete(logger.FilePath);
                logger.FilePath = newFilePath;
                logger.FolderPath = path;
                UpdateConfigLogs();
            }
        }

        /// <summary>
        /// Change the log types
        /// </summary>
        /// <param name="newType"></param>
        private void ChangeLogTypes(string newType)
        {
            Console.WriteLine("Changing log types");
            Console.WriteLine(newType);
            Console.WriteLine(Path.GetExtension(dailyLogger.FilePath));
            Console.WriteLine(Path.GetExtension(stateTrackLogger.FilePath));
            if (newType != Path.GetExtension(dailyLogger.FilePath) && newType != Path.GetExtension(stateTrackLogger.FilePath))
            {
                switch (newType)
                {
                    case ".xml":
                        dailyLogger.ConvertJSONtoXML();
                        stateTrackLogger.ConvertJSONtoXML();
                        break;
                    case ".json":
                        dailyLogger.ConvertXMLtoJSON();
                        stateTrackLogger.ConvertXMLtoJSON();
                        break;
                }
            }
            // UpdateConfigLogs();
            UpdateConfigLogType(newType);
        }

        /// <summary>
        /// Update the config logs
        /// </summary>
        private void UpdateConfigLogs()
        {
            Console.WriteLine("Updating config logs");
            App.appConfigData.DailyPath = dailyLogger.FolderPath;
            App.appConfigData.StateTrackPath = stateTrackLogger.FolderPath;
            Console.WriteLine("Daily path: " + App.appConfigData.DailyPath);
            Console.WriteLine("State track path: " + App.appConfigData.StateTrackPath);
            // ICI CA ERASE LES LOGS
            App.appConfigData.SaveToFile();
        }

        /// <summary>
        /// Update the config log type
        /// </summary>
        /// <param name="newType"></param>
        private void UpdateConfigLogType(string newType)
        {
            App.appConfigData.LogExtension = newType;
            App.appConfigData.SaveToFile();
        }

        private void AddExtensionToList(object obj)
        {
            if (!string.IsNullOrEmpty(NewPriorityExtension))
            {
                PriorityExtensionViewModel newPriorityExtensionItem = new PriorityExtensionViewModel
                {
                    Extension = NewPriorityExtension
                };

                PriorityExtensions.Add(newPriorityExtensionItem);

                NewPriorityExtension = string.Empty;
            }
        }
        private void RemovePriorityExtensionsFromList(object obj)
        {
            var elementsASupprimer = PriorityExtensions.Where(e => e.IsPrioritySelected).ToList();
            foreach (var element in elementsASupprimer)
            {
                PriorityExtensions.Remove(element);
            }
        }
    }
}