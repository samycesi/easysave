using easysave.Model;
using easysave.Model.Logger;

namespace easysave.ViewModel
{

    public class MainViewModel
    {

        public BackupList BackupList { get; set; }

        public DailyLogger DailyLogger { get; set; }

        public StateTrackLogger StateTrackLogger { get; set; }

        public MainViewModel(DailyLogger dailyLogger, StateTrackLogger stateTrackLogger)
        {
            this.BackupList = new BackupList();
            this.DailyLogger = dailyLogger;
            this.StateTrackLogger = stateTrackLogger;
        }

        /// <summary>
        /// Change the path of the log file
        /// </summary>
        /// <param name="path"></param>
        public void ChangeLogPath(string path)
        {

        }

        /// <summary>
        /// Execute the tasks depending on the command given by the user (can be a single task, a range of tasks or a list of tasks)
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteTasks(string command)
        {
            if (command.Contains("-"))
            {
                string[] tasks = command.Split('-');
                int start = int.Parse(tasks[0]);
                int end = int.Parse(tasks[1]);

                for (int i = start; i <= end; i++)
                {
                    (BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) = BackupList.ExecuteTaskByKey(i, StateTrackLogger);
                    DailyLogger.WriteDailyLog(task, fileSize, fileTransferTime, totalEncryptionTime);
                }
            }
            else if (command.Contains(";"))
            {
                string[] tasks = command.Split(';');
                foreach (string taskKey in tasks)
                {
                    int key = int.Parse(taskKey);
                    (BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) = BackupList.ExecuteTaskByKey(key, StateTrackLogger);
                    DailyLogger.WriteDailyLog(task, fileSize, fileTransferTime, totalEncryptionTime);
                }
            }
            else
            {
                int key = int.Parse(command);
                (BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) = BackupList.ExecuteTaskByKey(key, StateTrackLogger);
                DailyLogger.WriteDailyLog(task, fileSize, fileTransferTime, totalEncryptionTime);
            }
        }

        /// <summary>
        /// Add a new backup task to the list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="type"></param>
        public void AddBackupTask(string name, string sourceDirectory, string destinationDirectory, BackupType type)
        {
            BackupModel newTask = new BackupModel(name, sourceDirectory, destinationDirectory, type);
            BackupList.AddBackupTask(newTask);
        }

        /// <summary>
        /// Delete a backup task from the list
        /// </summary>
        /// <param name="key"></param>
        public void DeleteBackupTask(int key)
        {
            BackupList.DeleteBackupTask(key);
        }

        public void ExecuteAllTasks()
        {
            List<(BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime)> results = BackupList.ExecuteAllTasks(StateTrackLogger);
            foreach ((BackupModel task, long fileSize, long fileTransferTime, long totalEncryptionTime) in results)
            {
                DailyLogger.WriteDailyLog(task, fileSize, fileTransferTime, totalEncryptionTime);
            }
        }

    }

}