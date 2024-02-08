using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Easysave.Logger;
using Easysave.Model;
using Easysave.View;


namespace Easysave.Controller
{
	public class BackupController
	{
		public Dictionary<int, BackupModel> BackupTasks { get; set; }
		public ConsoleView ConsoleView { get; set; }

		public DailyLogger DailyLogger { get; set; }
		public StateTrackLogger StateTrackLogger { get; set; }

		public BackupController(ConsoleView view, DailyLogger dailyLogger, StateTrackLogger stateTrackLogger)
		{
			this.BackupTasks = new Dictionary<int, BackupModel>();
			this.ConsoleView = view;
			this.DailyLogger = dailyLogger;
			this.StateTrackLogger = stateTrackLogger;
		}

		/// <summary>
		///     This method adds a task in the dictionary of tasks
		/// </summary>
		/// <param name="task"></param>
		public void AddBackupTask(BackupModel task)
		{


		}

		/// <summary>
		///     This method executes tasks depending on the input command
		///     1-3 : tasks 1 to 3 
		///     1;3 : tasks 1 and 3
		/// </summary>
		/// <param name="command"></param>
		public void ExecuteTasks(string command)
		{

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

			// Total size of the backup
			long totalBackupSize = 0;

			// Measure the execution time
			var stopwatch = Stopwatch.StartNew();

			// Perform backup (full or differential)
			Stack<(DirectoryInfo, DirectoryInfo)> stack = new Stack<(DirectoryInfo, DirectoryInfo)>();
			stack.Push((sourceDir, destDir));

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
				if (backupType == BackupType.Differential && !currentDestination.Exists)
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
						totalBackupSize += file.Length;
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
		}

		/// <summary>
		///     This method deletes a task from the dictionary depending to the key corresponding to a ask
		/// </summary>
		/// <param name="key"></param>
		public void DeleteBackupTask(int key)
		{

		}

		/// <summary>
		///     This method changes the path to the log files
		/// </summary>
		/// <param name="path"></param>
		public void ChangeLogPath(string path)
		{

		}

	}
}
