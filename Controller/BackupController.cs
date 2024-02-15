using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Easysave.Logger;
using Easysave.Model;
using Easysave.View;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Easysave.Controller
{
    public class BackupController
	{
        public static int MinTask = 1;
        public static int MaxTask = 5;

        public Dictionary<int, BackupModel> BackupTasks {  get; set; }
        public DailyLogger DailyLogger { get; set; }
        public StateTrackLogger StateTrackLogger{ get; set; }

        public BackupController(DailyLogger dailyLogger, StateTrackLogger stateTrackLogger) 
        {
            this.BackupTasks = new Dictionary<int, BackupModel>();
            this.DailyLogger = dailyLogger;
            this.StateTrackLogger = stateTrackLogger;
        }

        /// <summary>
        ///     This method adds a task in the dictionary of tasks
        /// </summary>
        /// <param name="task"></param>
        public void AddBackupTask(BackupModel task)
        {
            this.BackupTasks.Add(AvailableKey(), task);
        }

        /// <summary>
        ///     This method determines which key is available in the dictionary (from 1 to 5)
        ///     The view prevents the user from adding a sixth task
        /// </summary>
        private int AvailableKey()
        {
            for (int key = MinTask; key <= MaxTask; key++)
            {
                if (!this.BackupTasks.ContainsKey(key))
                {
                    return key;
                }
            }
            throw new InvalidOperationException("Plus de place disponible.");
        }

		/// <summary>
		///     This method executes tasks depending on the input command
		///     1-3 : tasks 1 to 3 
		///     1;3 : tasks 1 and 3
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
					ExecuteTaskByKey(i);
				}
			}
			else if (command.Contains(";"))
			{
				string[] tasks = command.Split(';');
				foreach (string task in tasks)
				{
					int key = int.Parse(task);
					ExecuteTaskByKey(key);
				}
			}
			else
			{
				int key = int.Parse(command);
				ExecuteTaskByKey(key);
			}
		}

        /// <summary>
        ///     This method deletes a task from the dictionary depending to the key corresponding to a ask
        /// </summary>
        /// <param name="taskToRemove"></param>
        public void DeleteBackupTask(int taskToRemove)
        {
            if (this.BackupTasks.ContainsKey(taskToRemove))
            {
                this.BackupTasks.Remove(taskToRemove);
            }
        }
		/// <summary>
		///     This method executes a task depending on the key input corresponding to a task
		/// </summary>
		/// <param name="key"></param>
		private void ExecuteTaskByKey(int key)
		{
			BackupModel task = BackupTasks[key];
			string sourceDirectory = task.SourceDirectory;
			string destinationDirectory = task.DestinationDirectory;
			BackupType backupType = task.Type;

			var sourceDir = new DirectoryInfo(sourceDirectory);
			var destDir = new DirectoryInfo(destinationDirectory);

			if (!sourceDir.Exists)
				throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");

			// Create destination directory if it doesn't exist
			if (!destDir.Exists)
			{
				Directory.CreateDirectory(destinationDirectory); // Create destination directory
			}

			// Measure the execution time
			var stopwatch = Stopwatch.StartNew();

			(long size, int fileCount) = CalculateTransferSize(task);

			// Total size of the backup
			long totalBackupSize = size;

			// Total number of files to backup
			int totalFileCount = fileCount;

			// Perform backup (full or differential)
			Stack<(DirectoryInfo, DirectoryInfo)> stack = new Stack<(DirectoryInfo, DirectoryInfo)>();
			stack.Push((sourceDir, destDir));
			
			// Remaining files and size to backup
			long filesLeftToDo = totalFileCount;
			long fileSizeLeftToDo = totalBackupSize;

			// Update the state of the task to active
			StateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, sourceDirectory, destinationDirectory, key);
			
			while (stack.Count > 0)
			{
				var (currentSource, currentDestination) = stack.Pop();

				// Get information about the current directory
				var dir = currentSource;

				// Check if the directory exists
				if (!dir.Exists)
					throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

				// Get subdirectories of the current directory
				DirectoryInfo[] dirs = dir.GetDirectories();

				// Create the destination directory if it doesn't exist (for differential backup)
				if (!currentDestination.Exists)
					Directory.CreateDirectory(currentDestination.FullName);

				// Copy files from the current directory to the destination directory
				foreach (FileInfo file in dir.GetFiles())
				{
					string targetFilePath = Path.Combine(currentDestination.FullName, file.Name);
					FileInfo destFile = new FileInfo(targetFilePath);

					// For full backup or if the file doesn't exist in the destination directory or if it's newer in the source directory,
					// perform a differential backup by copying the file from source to destination
					if (backupType == BackupType.Full || !destFile.Exists || file.LastWriteTime > destFile.LastWriteTime)
					{
						file.CopyTo(targetFilePath, true);
						filesLeftToDo--;
						fileSizeLeftToDo -= file.Length;
						StateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, Path.Combine(currentSource.FullName, file.Name), targetFilePath, key);
					}
				}

				// Add subdirectories to the stack for further processing
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(currentDestination.FullName, subDir.Name);
					stack.Push((subDir, new DirectoryInfo(newDestinationDir)));
				}
			}
			stopwatch.Stop();
			var duration = stopwatch.Elapsed;
			DailyLogger.WriteDailyLog(task, totalBackupSize, duration.Milliseconds);
		}

		/// <summary>
		///     This method changes the path to the log files
		/// </summary>
		/// <param name="path"></param>
		public void ChangeLogPath(string path)
		{

		}

		private (long size, int fileCount) CalculateTransferSize(BackupModel task)
		{
			long size = 0;
			int fileCount = 0;
			string sourceDirectory = task.SourceDirectory;
			string destinationDirectory = task.DestinationDirectory;
			BackupType backupType = task.Type;

			var sourceDir = new DirectoryInfo(sourceDirectory);
			var destDir = new DirectoryInfo(destinationDirectory);

			if (task.Type == BackupType.Full || !destDir.Exists)
			{
				Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
				stack.Push(sourceDir);

				while (stack.Count > 0)
				{
					var currentDir = stack.Pop();
					long dirSize = 0;
					foreach (FileInfo file in currentDir.GetFiles())
					{
						dirSize += file.Length;
						fileCount++;
					}
					size += dirSize;

					foreach (DirectoryInfo subDir in currentDir.GetDirectories())
					{
						stack.Push(subDir);
					}
				}
			}
			else if (task.Type == BackupType.Differential)
			{
				Stack<(DirectoryInfo, DirectoryInfo)> stack = new Stack<(DirectoryInfo, DirectoryInfo)>();
				stack.Push((sourceDir, destDir));

				while (stack.Count > 0)
				{
					var (currentSource, currentDestination) = stack.Pop();

					var dir = currentSource;

					if (!dir.Exists)
						throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

					DirectoryInfo[] dirs = dir.GetDirectories();

					foreach (FileInfo file in dir.GetFiles())
					{
						string targetFilePath = Path.Combine(currentDestination.FullName, file.Name);
						FileInfo destFile = new FileInfo(targetFilePath);

						if (!destFile.Exists || file.LastWriteTime > destFile.LastWriteTime)
						{
							size += file.Length;
							fileCount++;
						}
					}

					// Add subdirectories to the stack for further processing
					foreach (DirectoryInfo subDir in dirs)
					{
						string newDestinationDir = Path.Combine(currentDestination.FullName, subDir.Name);
						stack.Push((subDir, new DirectoryInfo(newDestinationDir)));
					}
				}
			}

			return (size, fileCount);
		}

        public void ExecuteAllTasks()
        {
            foreach (var task in BackupTasks)
			{
				ExecuteTaskByKey(task.Key);
			}
        }
    }
}