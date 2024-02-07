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
			// Get the backup task corresponding to the key
			BackupModel task = BackupTasks[key];

			// Create a stack to manage directories to be processed
			var stack = new Stack<Tuple<string, string>>();

			// Push the initial directory onto the stack
			stack.Push(Tuple.Create(task.SourceDirectory, task.DestinationDirectory));

			// Continue processing directories until the stack is empty
			while (stack.Count > 0)
			{
				// Pop the top directory from the stack
				var (currentSource, currentDestination) = stack.Pop();

				// Get information about the current directory
				var dir = new DirectoryInfo(currentSource);

				// Check if the directory exists
				if (!dir.Exists)
					throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

				// Get subdirectories of the current directory
				DirectoryInfo[] dirs = dir.GetDirectories();

				// Create the destination directory
				Directory.CreateDirectory(currentDestination);

				// Copy files from the current directory to the destination directory
				foreach (FileInfo file in dir.GetFiles())
				{
					string targetFilePath = Path.Combine(currentDestination, file.Name);
					file.CopyTo(targetFilePath);
				}

				// Add subdirectories to the stack for further processing
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(currentDestination, subDir.Name);
					stack.Push(Tuple.Create(subDir.FullName, newDestinationDir));
				}
			}
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
