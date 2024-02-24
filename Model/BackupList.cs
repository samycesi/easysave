using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using easysave.Model.Logger;

namespace easysave.Model
{

    public class BackupList
    {

        public Dictionary<int, BackupModel> BackupTasks { get; set; }

        public event EventHandler<BackupEvent> BackupTaskAdded;
        public event EventHandler<BackupEvent> BackupTaskRemoved;

        public BackupList()
        {
            this.BackupTasks = LoadBackupTasks();
        }

        /// <summary>
        /// Add a new backup task to the list
        /// </summary>
        /// <param name="backup"></param>
        public void AddBackupTask(BackupModel backup)
        {
            int lastKey = this.BackupTasks.Keys.LastOrDefault();
            this.BackupTasks.Add(lastKey + 1, backup);
            SaveBackupTasks(); // Save the backup tasks to the save file
            BackupTaskAdded?.Invoke(this, new BackupEvent(backup)); // Notify that a backup task has been added
        }

        /// <summary>
        /// Delete a backup task from the list
        /// </summary>
        /// <param name="key"></param>
        public void DeleteBackupTask(int key)
        {
            BackupTaskRemoved?.Invoke(this, new BackupEvent(this.BackupTasks[key])); // Notify that a backup task has been removed
            this.BackupTasks.Remove(key);
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

            string extension = App.appConfigData.FileExtensionToEncrypt;

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
            stateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, sourceDirectory, destinationDirectory, task.Name);

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
                        stateTrackLogger.UpdateActive(totalFileCount, totalBackupSize, filesLeftToDo, fileSizeLeftToDo, Path.Combine(currentSource.FullName, file.Name), targetFilePath, task.Name);
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
            return (task, totalBackupSize, duration.Milliseconds, totalEncryptionTime); // Return the task, the total size of the backup, the duration of the backup and the total time of encryption for the DailyLogger
        }

        /// <summary>
        /// Execute all the backup tasks and update the state of the tasks
        /// </summary>
        /// <param name="stateTrackLogger"></param>
        public List<(BackupModel, long, long, long)> ExecuteAllTasks(StateTrackLogger stateTrackLogger)
        {
            List<(BackupModel, long, long, long)> results = new List<(BackupModel, long, long, long)>();
            foreach (var task in this.BackupTasks)
            {
                results.Add(ExecuteTaskByKey(task.Key, stateTrackLogger)); // Execute the task and add the result to the list
            }
            return results; // Return the task, the total size of the backup, the duration of the backup and the total time of encryption for each task executed for the DailyLogger
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

            // Get information about the source and destination directories
            var sourceDir = new DirectoryInfo(sourceDirectory);
            var destDir = new DirectoryInfo(destinationDirectory);
            
            if (task.Type == BackupType.Full || !destDir.Exists)
            {
                Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
                stack.Push(sourceDir); // Add the source directory to the stack

                // Calculate the size of the files to transfer and the number of files to transfer
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

                    // Add subdirectories to the stack for further processing
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

                // Calculate the size of the files to transfer and the number of files to transfer
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

                        // If the file doesn't exist in the destination directory or if it's newer in the source directory,
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

            return (size, fileCount); // Return the size of the files to transfer and the number of files to transfer
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
            startInfo.FileName = $"\"{App.appConfigData.CryptosoftPath}\""; // Path to the cryptosoft executable
            startInfo.Arguments = $"\"{sourceFile}\" \"{destinationFile}.chiffre\""; // Arguments for the cryptosoft executable
            startInfo.RedirectStandardOutput = true; // Redirect the standard output
            startInfo.UseShellExecute = false; // Don't use the shell execute to run the process (use the standard output)

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using (Process process = Process.Start(startInfo))
            {
                // Wait for the process to exit
                process.WaitForExit();
                stopWatch.Stop();

                // If the process exited with no errors, return the duration of the encryption
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

        /// <summary>
        /// Reorder the keys of the backup tasks
        /// </summary>
        public void ReorderKeys()
        {
            int newKey = 1;
            Dictionary<int, BackupModel> reordenedBackupTasks = new Dictionary<int, BackupModel>();

            // Create a new dictionary with the keys reordered
            foreach (var kvp in BackupTasks.OrderBy(kv => kv.Key))
            {
                reordenedBackupTasks.Add(newKey++, kvp.Value);
            }

            BackupTasks.Clear(); // Clear the backup tasks

            // Add the backup tasks with the reordered keys
            foreach (var kvp in reordenedBackupTasks)
            {
                BackupTasks.Add(kvp.Key, kvp.Value);
            }
            SaveBackupTasks(); // Save the backup tasks to the save file
        }

        /// <summary>
        /// Load the backup tasks from the save file
        /// </summary>
        /// <returns>
        /// A dictionary of backup tasks
        /// </returns>
        private Dictionary<int, BackupModel> LoadBackupTasks()
        {
            string savePath = App.appConfigData.SavesPath; // Path to the save file
            if (File.Exists(savePath))
            {
                string backupTasksJSON = File.ReadAllText(savePath); // Read the save file
                Dictionary<int, BackupModel> backupTasksFromJSON = JsonSerializer.Deserialize<Dictionary<int, BackupModel>>(backupTasksJSON); // Deserialize the JSON to a dictionary of backup tasks
                return backupTasksFromJSON;
            }
            return new Dictionary<int, BackupModel>(); // Return an empty dictionary if the save file doesn't exist
        }

        /// <summary>
        /// Save the backup tasks to the save file
        /// </summary>
        private void SaveBackupTasks()
        {
            string savePath = App.appConfigData.SavesPath; // Path to the save file
            string backupTasksToJSON = JsonSerializer.Serialize(this.BackupTasks, new JsonSerializerOptions { WriteIndented = true }); // Serialize the dictionary of backup tasks to JSON
            if (File.Exists(savePath))
            {
                File.WriteAllText(savePath, backupTasksToJSON); // Write the JSON to the save file
            } else
            {
                File.Create(savePath).Close(); // Create the save file if it doesn't exist
                File.WriteAllText(savePath, backupTasksToJSON); // Write the JSON to the save file
            }
        }
    }

}