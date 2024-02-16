using System;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using Easysave.Controller;
using Easysave.Logger;
using Easysave.Model;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;
using System.Net.Http.Json;
using System.ComponentModel;

namespace Easysave.View
{
    public class ConsoleView
    {
        private readonly BackupController backupController;
        private ResourceManager resourceManager;
        private CultureInfo cultureInfo;

        public ConsoleView(string stateTrackPath, string dailyPath, string logFileType)
        {
            resourceManager = new ResourceManager("EasySave.View.Messages", typeof(ConsoleView).Assembly);
            cultureInfo = new CultureInfo("en");

            DailyLogger dailyLogger;
            StateTrackLogger stateTrackLogger;

            if (dailyPath.Length == 0 && stateTrackPath.Length == 0 && logFileType.Length == 0)
            {
                string dailyFolderPath = RequireValidPath("dailyPath");
                string stateTrackFolderPath = RequireValidPath("statePath");
                logFileType = ChooseLogFileType();


                dailyLogger = new DailyLogger(dailyFolderPath, DateTime.Today.ToString("yyyy-MM-dd") + logFileType);
                stateTrackLogger = new StateTrackLogger(stateTrackFolderPath, "state"+logFileType);
            }
            else
            {
                // new daily log file if date is different
                if(String.Equals(Path.GetFileNameWithoutExtension(dailyPath), DateTime.Today.ToString("yyyy-MM-dd")))
                {
                    dailyLogger = new DailyLogger(dailyPath);
                }
                else
                {
                    dailyLogger = new DailyLogger(Path.GetDirectoryName(dailyPath), DateTime.Today.ToString("yyyy-MM-dd") + logFileType);
                }
                stateTrackLogger = new StateTrackLogger(stateTrackPath);
            }
            // Update Config
            AppConfigData appConfig = new AppConfigData(stateTrackLogger.FilePath, dailyLogger.FilePath, logFileType);
            string updatedJson = JsonSerializer.Serialize(appConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(BackupController.ConfigFilePath, updatedJson);
            backupController = new BackupController(dailyLogger, stateTrackLogger);
        }

        private void ChangeLanguage()
        {
            Console.WriteLine();
            Console.WriteLine("1. English");
            Console.WriteLine("2. French");
            Console.Write("Select language: ");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    cultureInfo = new CultureInfo("en");
                    break;
                case "2":
                    cultureInfo = new CultureInfo("fr");
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
            Console.WriteLine();

        }

        public void DisplayMenu()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                Console.WriteLine($"\n--------- {resourceManager.GetString("MenuTitle", cultureInfo)} ---------");
                Console.WriteLine($"1. {resourceManager.GetString("AddBackupJob", cultureInfo)}");
                Console.WriteLine($"2. {resourceManager.GetString("ListBackupJobs", cultureInfo)}");
                Console.WriteLine($"3. {resourceManager.GetString("ExecuteBackupJob", cultureInfo)}");
                Console.WriteLine($"4. {resourceManager.GetString("ExecuteAllBackupJobs", cultureInfo)}");
                Console.WriteLine($"5. {resourceManager.GetString("DeleteBackup", cultureInfo)}");
                Console.WriteLine($"6. {resourceManager.GetString("SettingsLogs", cultureInfo)}");
                Console.WriteLine($"7. {resourceManager.GetString("ChangeLanguage", cultureInfo)}");
                Console.WriteLine($"8. {resourceManager.GetString("Exit", cultureInfo)}");

                Console.Write($"{resourceManager.GetString("SelectOption", cultureInfo)}: ");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        AddBackupTask();
                        break;
                    case "2":
                        ListBackupTasks();
                        break;
                    case "3":
                        ExecuteBackupTasks();
                        break;
                    case "4":
                        //backupController.ExecuteAllBackupTask();
                        Console.WriteLine(resourceManager.GetString("AllJobsExecuted", cultureInfo));
                        break;
                    case "5":
                        DeleteBackupTask();
                        break;
                    case "6":
                        SettingsLogs();
                        break;
                    case "7":
                        ChangeLanguage();
                        break;
                    case "8":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption", cultureInfo));
                        break;
                }

            }
        }

        private void SettingsLogs()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                Console.WriteLine();
                Console.WriteLine($"1. {resourceManager.GetString("ChangeLogPath", cultureInfo)}");
                Console.WriteLine($"2. {resourceManager.GetString("ChangeLogType", cultureInfo)}");
                Console.WriteLine($"3. {resourceManager.GetString("Cancel", cultureInfo)}");
                Console.Write($"{resourceManager.GetString("SelectOption", cultureInfo)}: ");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        ChangeLogPaths();
                        break;
                    case "2":
                        ChangeLogTypes();
                        break;
                    case "3":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption", cultureInfo));
                        break;
                }

            }
        }

        private void ChangeLogPaths()
        {
            Console.WriteLine(); 
            Console.WriteLine($"1. {resourceManager.GetString("DailyLog", cultureInfo)}");
            Console.WriteLine($"2. {resourceManager.GetString("StateTrackLog", cultureInfo)}");
            Console.WriteLine(resourceManager.GetString("SelectLog", cultureInfo));
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine();
                    Console.WriteLine($"{resourceManager.GetString("CurrentPath", cultureInfo)}{Path.GetDirectoryName(backupController.DailyLogger.FilePath)}");
                    string dailyFolderPath = RequireValidPath("dailyPath");
                    backupController.ChangeLogPath(dailyFolderPath, (LoggerModel) backupController.DailyLogger);
                    break;
                case "2":
                    Console.WriteLine();
                    Console.WriteLine($"{resourceManager.GetString("CurrentPath", cultureInfo)}{Path.GetDirectoryName(backupController.StateTrackLogger.FilePath)}");
                    string stateTrackFolderPath = RequireValidPath("statePath");
                    backupController.ChangeLogPath(stateTrackFolderPath, (LoggerModel)backupController.StateTrackLogger);
                    break;
            }
        }

        private void ChangeLogTypes()
        {
            Console.WriteLine();
            string newLogFileType = ChooseLogFileType();
            // Update files
            backupController.ChangeLogTypes(newLogFileType);
        }

        private void AddBackupTask()
        {
            Console.WriteLine();
            Console.WriteLine($"\n------ {resourceManager.GetString("MenuAddBackupJob", cultureInfo)} ------");
            Console.Write(resourceManager.GetString("EnterBackupJobName", cultureInfo));
            var name = Console.ReadLine();

            string sourceDirectory;
            do
            {
                Console.Write(resourceManager.GetString("EnterSourceDirectoryPath", cultureInfo));
                sourceDirectory = Console.ReadLine();
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine(resourceManager.GetString("InvalidPath", cultureInfo));
                }
            } while (!Directory.Exists(sourceDirectory));

            string targetDirectory;
            bool isValidPath = false;
            do
            {
                Console.Write(resourceManager.GetString("EnterTargetDirectoryPath", cultureInfo));
                targetDirectory = Console.ReadLine();

                try
                {
                    var root = Path.GetPathRoot(targetDirectory);
                    if (Directory.Exists(root))
                    {
                        isValidPath = true;
                        if (!Directory.Exists(targetDirectory))
                        {
                            Console.WriteLine(resourceManager.GetString("CreatingTargetPath", cultureInfo));
                            Directory.CreateDirectory(targetDirectory);
                        }
                    }
                    else
                    {
                        Console.WriteLine(resourceManager.GetString("InvalidPath", cultureInfo));
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(resourceManager.GetString("InvalidPath", cultureInfo));
                }
            } while (!isValidPath);

            Console.Write(resourceManager.GetString("SelectBackupType", cultureInfo));
            var typeInput = Console.ReadLine();
            BackupType type = (typeInput == "1") ? BackupType.Full : BackupType.Differential;

            backupController.AddBackupTask(new BackupModel(name, sourceDirectory, targetDirectory, type));
            Console.WriteLine(resourceManager.GetString("BackupJobAddedSuccess", cultureInfo));
            Console.WriteLine();
        }

        private void ListBackupTasks()
        {
            Console.WriteLine();
            Console.WriteLine(resourceManager.GetString("ListBackup", cultureInfo));
            for (int i =BackupController.MinTask = 1; i <= BackupController.MaxTask; i++)
            {
                if (backupController.BackupTasks.ContainsKey(i))
                {
                    Console.WriteLine($"{i}. {backupController.BackupTasks[i].Name}");
                }
                else
                {
                    Console.WriteLine($"{i}");
                }
            }
        }

        private void ExecuteBackupTasks()
        {
            Regex uniqueJob = new Regex($@"[{BackupController.MinTask}-{BackupController.MaxTask}]", RegexOptions.IgnoreCase);
            Regex toJob = new Regex($@"[{BackupController.MinTask}-{BackupController.MaxTask}]-[{BackupController.MinTask}-{BackupController.MaxTask}]", RegexOptions.IgnoreCase);
            Regex andJob = new Regex($@"[{BackupController.MinTask}-{BackupController.MaxTask}];[{BackupController.MinTask}-{BackupController.MaxTask}]", RegexOptions.IgnoreCase);
            Console.WriteLine(uniqueJob);
            bool validInput = false;
            do
            {
                Console.Write(resourceManager.GetString("Index", cultureInfo));
                var userInput = Console.ReadLine();
                if (uniqueJob.IsMatch(userInput) || toJob.IsMatch(userInput) || andJob.IsMatch(userInput))
                {
                    validInput = true;
                    this.backupController.ExecuteTasks(userInput);
                }
                else
                {
                    Console.WriteLine($"{resourceManager.GetString("InvalidInput", cultureInfo)}, {resourceManager.GetString("PleaseEnterValidRange", cultureInfo)} (e.g., 1-3).");
                }
            } while (!validInput);


            /* var segments = userInput.Split(';');

            foreach (var segment in segments)
            {
                if (segment.Contains("-"))
                {
                    var parts = segment.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end) && start <= end)
                    {
                        for (int i = start; i <= end; i++)
                        {
                            //ExecuteSingleBackupJob(i - 1);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{resourceManager.GetString("InvalidRange", cultureInfo)} '{segment}', {resourceManager.GetString("PleaseEnterValidRange", cultureInfo)} (e.g., 1-3).");

                    }
                }
                else if (int.TryParse(segment, out int index))
                {
                    //ExecuteSingleBackupJob(index - 1);
                }
                else
                {
                    Console.WriteLine($"Invalid input in segment '{segment}', please enter a number or a range (e.g., 1 or 1-3).");
                }
            } */
        }
        //private void ExecuteSingleBackupJob(int jobIndex){}


        private void ExecuteAllBackupTasks()
        {
            backupController.ExecuteAllTasks();
            /* Console.WriteLine(resourceManager.GetString("AllJobsExecuted", cultureInfo)); */
        }

        private Boolean PathIsValid(string path)
        {
            if (Path.IsPathRooted(path)) // if format is valid
            {
                if (Directory.Exists(path)) // if directory exists 
                {
                    return true;
                }
            }
            return false;
        }

        public void DeleteBackupTask()
        {
            Boolean validInput = false;
            while (!validInput)
            {
                Console.WriteLine();
                Console.WriteLine(resourceManager.GetString("EnterDeleteBackup", cultureInfo));
                string userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "0":
                        validInput = true;
                        break;
                    default:
                        {
                            try
                            {
                                this.backupController.DeleteBackupTask(userInput);
                                validInput = true;

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(resourceManager.GetString(ex.Message, cultureInfo));
                                break;
                            }
                        }
                        break;
                }
            }
        }

        private string RequireValidPath(string messageLogPath)
        {
            Console.WriteLine($"{resourceManager.GetString(messageLogPath, cultureInfo)}");
            string folderPath = Console.ReadLine();
            Boolean pathIsValid = this.PathIsValid(folderPath);
            while (!pathIsValid)
            {
                Console.WriteLine($"{resourceManager.GetString("InvalidPath", cultureInfo)}");
                Console.WriteLine($"{resourceManager.GetString(messageLogPath, cultureInfo)}");
                folderPath = Console.ReadLine();
                pathIsValid = this.PathIsValid(folderPath);
            }
            return folderPath;
        }

        public string ChooseLogFileType()
        {
            while (true)
            {
                string fileTypesJSON = File.ReadAllText(BackupController.FileTypesPath);
                List<string> listFileTypes = JsonSerializer.Deserialize<List<string>>(fileTypesJSON);

                for (int i = 0; i < listFileTypes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {listFileTypes[i]}");
                }
                Console.WriteLine($"{resourceManager.GetString("EnterLogFileType", cultureInfo)}");

                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= listFileTypes.Count)
                {
                    string logFileType = listFileTypes[selectedIndex - 1];
                    return logFileType;
                }
                else
                {
                    Console.WriteLine($"{resourceManager.GetString("InvalidInput", cultureInfo)}");
                }
            }
        }



    }

}

