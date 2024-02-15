namespace Easysave.Model
{

	public class BackupList
	{

		public Dictionary<int, BackupModel> BackupTasks { get; set; }

		public BackupList()
		{
			this.BackupTasks = new Dictionary<int, BackupModel>();
		}

		public void AddBackupTask(BackupModel backup)
		{
		}

		public void DeleteBackupTask(int key)
		{
		}

		public void ExecuteTaskByKey(int key)
		{
		}

		public void ExecuteAllTasks()
		{
		}

		private (long size, int filecount) CalculateTransferSize(BackupModel task)
		{
			return (0, 0);
		}

		private void EncryptFile(string sourceFile, string destinationFile)
		{
		}

	}
	
}