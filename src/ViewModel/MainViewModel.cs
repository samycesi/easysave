using Easysave.Model.Logger;
using Easysave.Model;

namespace Easysave.ViewModel
{

	public class MainViewModel
	{

		public BackupList BackupList { get; set; }

		public DailyLogger DailyLogger { get; set; }

		public StateTrackLogger StateTrackLogger { get; set; }

		public MainViewModel(DailyLogger dailylogger, StateTrackLogger stateTrackLogger)
		{
		}

		public void ChangeLogPath(string path)
		{
		}

		public void ExecuteTasks(string command)
		{
		}

		public void AddBackupTask(string name, string sourceDirectory, string destinationDirectory, BackupType type)
		{
		}

		public void DeleteBackupTask(int key)
		{
		}

		public void ExecuteAllTasks()
		{
		}

	}

}