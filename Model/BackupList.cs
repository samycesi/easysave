using System.Diagnostics;
using System.IO;
using easysave.Model.Logger;

namespace easysave.Model
{

    public class BackupList
    {

        public Dictionary<int, BackupModel> BackupTasks { get; set; }

        public BackupList()
        {
            this.BackupTasks = new Dictionary<int, BackupModel>();
        }

        /// <summary>
        /// Add a new backup task to the list
        /// </summary>
        /// <param name="backup"></param>
        public void AddBackupTask(BackupModel backup)
        {
            int lastKey = this.BackupTasks.Keys.LastOrDefault();
            this.BackupTasks.Add(lastKey + 1, backup);
        }

        /// <summary>
        /// Delete a backup task from the list
        /// </summary>
        /// <param name="key"></param>
        public void DeleteBackupTask(int key)
        {
            this.BackupTasks.Remove(key);
            ReorderKeys();
        }

        /// <summary>
        /// Execute a backup task by its key and update the state of the task
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stateTrackLogger"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public (BackupModel, long, long, long) ExecuteTaskByKey(int key, StateTrackLogger stateTrackLogger)
        {
            BackupModel task = BackupTasks[key];
            string sourceDirectory = task.SourceDirectory;
            string destinationDirectory = task.DestinationDirectory;
            BackupType backupType = task.Type;

            string extension = ".txt"; // Extension to encrypt (change for the one in the configuration file)

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

            // Total time of encryption for the backup
            long totalEncryptionTime = 0;

            // Perform backup (full or differential)
            Stack<(DirectoryInfo, DirectoryInfo)> stack = new Stack<(DirectoryInfo, DirectoryInfo)>();
            stack.Push((sourceDir, destDir));

            // Remaining files and size to backup
            long filesLeftToDo = totalFileCount;
            long fileSizeLeftToDo = totalBackupSize;

            // Update the state of the task to active
            stateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, sourceDirectory, destinationDirectory, key);

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
                        long encryptionDuration;
                        if (Path.GetExtension(file.Name) == extension)
                        {
                            // Encrypt the file
                            encryptionDuration = EncryptFile(file.FullName, targetFilePath);
                        }
                        else
                        {
                            // Copy the file
                            file.CopyTo(targetFilePath, true);
                            encryptionDuration = 0;
                        }
                        totalEncryptionTime += encryptionDuration;
                        filesLeftToDo--;
                        fileSizeLeftToDo -= file.Length;
                        stateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, Path.Combine(currentSource.FullName, file.Name), targetFilePath, key);
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
            return (task, totalBackupSize, duration.Milliseconds, totalEncryptionTime);
        }

        /// <summary>
        /// Execute all backup tasks
        /// </summary>
        public List<(BackupModel, long, long, long)> ExecuteAllTasks(StateTrackLogger stateTrackLogger)
        {
            List<(BackupModel, long, long, long)> results = new List<(BackupModel, long, long, long)>();
            foreach (var task in this.BackupTasks)
            {
                results.Add(ExecuteTaskByKey(task.Key, stateTrackLogger));
            }
            return results;
        }

        /// <summary>
        /// Calculate the size of the files to transfer and the number of files to transfer
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private (long size, int filecount) CalculateTransferSize(BackupModel task)
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

        /// <summary>
        /// Transfer files from source to destination directory and encrypt them
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private long EncryptFile(string sourceFile, string destinationFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\bilel\source\repos\cryptosoft\cryptosoft\bin\Release\net8.0\publish\cryptosoft.exe"; // Chemin de l'exécutable de chiffrement
            startInfo.Arguments = $"\"{sourceFile}\" \"{destinationFile}.chiffre\""; // Fichier source et destination
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using (Process process = Process.Start(startInfo))
            {
                // Attendre que le processus de chiffrement se termine
                process.WaitForExit();
                stopWatch.Stop();

                // Vérifier le code de retour du processus
                if (process.ExitCode > 0)
                {
                    return stopWatch.ElapsedMilliseconds;
                }
                else
                {
                    return -1;
                }
            }
        }

        private void ReorderKeys()
        {
            int newKey = 1;
            Dictionary<int, BackupModel> reordenedBackupTasks = new Dictionary<int, BackupModel>();

            foreach (var kvp in BackupTasks.OrderBy(kv => kv.Key))
            {
                reordenedBackupTasks.Add(newKey++, kvp.Value);
            }

            BackupTasks.Clear();

            foreach (var kvp in reordenedBackupTasks)
            {
                BackupTasks.Add(kvp.Key, kvp.Value);
            }
        }
    }

}