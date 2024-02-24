using easysave.Model.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace easysave.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string selectedLanguage;
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand BrowseDailyPathCommand { get; }
        public ICommand BrowseStatePathCommand { get; }
        public ICommand SaveSettingsCommand { get; }

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

            SelectedLanguage = App.appConfigData.Language;
            SelectedExtension = App.appConfigData.LogExtension;
            NewDailyPath = App.appConfigData.DailyPath;
            NewStateTrackPath = App.appConfigData.StateTrackPath;

            BrowseDailyPathCommand = new RelayCommand(BrowseDailyPath);
            BrowseStatePathCommand = new RelayCommand(BrowseStatePath);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
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
        /// Save the settings
        /// </summary>
        /// <param name="obj"></param>
        private void SaveSettings(object obj)
        {
            string selectedLanguage;
            if (SelectedLanguage.ToLower().Contains("english"))
            {
                selectedLanguage = "en";
            }
            else
            {
                selectedLanguage = "fr";
            }
            SelectedLanguage = selectedLanguage;
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
            App.appConfigData.Language = SelectedLanguage;
            App.appConfigData.LogExtension = SelectedExtension;
            App.appConfigData.DailyPath = NewDailyPath;
            App.appConfigData.StateTrackPath = NewStateTrackPath;
            App.appConfigData.SaveToFile();
            Console.WriteLine("Settings saved");
            Console.WriteLine("Language: " + App.appConfigData.Language);
            Console.WriteLine("Log extension: " + App.appConfigData.LogExtension);
            Console.WriteLine("Daily path: " + App.appConfigData.DailyPath);
            Console.WriteLine("State track path: " + App.appConfigData.StateTrackPath);

            ChangeLogPath(NewDailyPath, dailyLogger);
            ChangeLogPath(NewStateTrackPath, stateTrackLogger);
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
            UpdateConfigLogs();
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
    }
}
