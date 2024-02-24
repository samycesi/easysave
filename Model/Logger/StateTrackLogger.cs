
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace easysave.Model.Logger
{
    public class StateTrackLogger : Logger
    {

        private List<StateTrackData> _stateTrackDataList;

        public StateTrackLogger(string folderPath, string filename, BackupList backupList) : base(folderPath, filename)
        {
            _stateTrackDataList = new List<StateTrackData>();
            SubscribeToBackupListEvents(backupList); // Subscribe to BackupList events
            Init(backupList.BackupTasks); // Initialize the state log with the current backup tasks
        }

        /// <summary>
        ///    Subscribes to the BackupList events
        /// </summary>
        /// <param name="backupList"></param>
        private void SubscribeToBackupListEvents(BackupList backupList)
        {
            backupList.BackupTaskAdded += HandleBackupTaskAdded; // Subscribe to the BackupTaskAdded event
            backupList.BackupTaskRemoved += HandleBackupTaskRemoved; // Subscribe to the BackupTaskRemoved event
        }

        /// <summary>
        ///     Initiates the log file
        /// </summary>
        private void Init(Dictionary<int, BackupModel> backupTasks)
        {
            // Read the state log file and deserialize it into _stateTrackDataList if it exists
            string filePath = FilePath;
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
            {
                if (Path.GetExtension(filePath).ToLower() == ".json")
                {
                    string json = File.ReadAllText(filePath);
                    Console.WriteLine(json);
                    if (json == "{}")
                    {
                        foreach (var backupTask in backupTasks.Values)
                        {
                            StateTrackData taskData = new StateTrackData(backupTask.Name, DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
                            _stateTrackDataList.Add(taskData); // Add the default data to the list
                        }
                    }
                    else
                    {
                        _stateTrackDataList = JsonConvert.DeserializeObject<List<StateTrackData>>(json); // Deserialize the file content into _stateTrackDataList
                    }
                }
                else if (Path.GetExtension(filePath).ToLower() == ".xml")
                {
                    if (new FileInfo(filePath).Length == 0)
                    {
                        foreach (var backupTask in backupTasks.Values)
                        {
                            StateTrackData taskData = new StateTrackData(backupTask.Name, DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
                            _stateTrackDataList.Add(taskData); // Add the default data to the list
                        }
                    }
                    else
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            _stateTrackDataList = (List<StateTrackData>)serializer.Deserialize(fileStream); // Deserialize the file content into _stateTrackDataList
                        }
                    }
                }
            }
            else
            {
                // Add the default data to the list
                foreach (var backupTask in backupTasks.Values)
                {
                    StateTrackData taskData = new StateTrackData(backupTask.Name, DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
                    _stateTrackDataList.Add(taskData);
                }
            }
        }

        /// <summary>
        ///    Handles the BackupTaskAdded event by adding a new task to the state log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBackupTaskAdded(object sender, BackupEvent e)
        {
            // Create a new StateTrackData object with the default values
            StateTrackData taskData = new StateTrackData(e.BackupTask.Name, DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");

            // Add the new StateTrackData object to the list
            _stateTrackDataList.Add(taskData);

            // Save the changes to the file
            SaveStateTrackDataToFile();
        }

        /// <summary>
        ///   Handles the BackupTaskRemoved event by removing a task from the state log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBackupTaskRemoved(object sender, BackupEvent e)
        {
            // Remove the StateTrackData object corresponding to the removed task from the list
            StateTrackData taskToRemove = _stateTrackDataList.FirstOrDefault(task => task.Name == e.BackupTask.Name);
            if (taskToRemove != null)
            {
                _stateTrackDataList.Remove(taskToRemove);

                // Save the changes to the file
                SaveStateTrackDataToFile();
            }
        }

        /// <summary>
        ///    Saves the state track data to the file
        /// </summary>
        private void SaveStateTrackDataToFile()
        {
            string filePath = FilePath;

            // Serialize the _stateTrackDataList and save it to the file
            string fileType = Path.GetExtension(filePath);
            switch (fileType)
            {
                case ".xml":
                    XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        serializer.Serialize(writer, _stateTrackDataList);
                    }
                    break;
                case ".json":
                    string json = JsonConvert.SerializeObject(_stateTrackDataList, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                    break;
            }
        }


        /// <summary>
        ///     Updates the real time log file of an active task
        /// </summary>
        /// <param name="totalFilesToCopy"></param>
        /// <param name="totalFilesSize"></param>
        /// <param name="filesLeftToDo"></param>
        /// <param name="fileSizeLeftToDo"></param>
        /// <param name="sourceFileDirectory"></param>
        /// <param name="targetFileDirectory"></param>
        /// <param name="name"></param>
        public void UpdateActive(long totalFilesToCopy, long totalFilesSize, long filesLeftToDo, long fileSizeLeftToDo, string sourceFileDirectory, string targetFileDirectory, string name)
        {
            // Find the corresponding StateTrackData object to the active task
            StateTrackData taskToUpdate = _stateTrackDataList.FirstOrDefault(task => task.Name == name);
            if (taskToUpdate != null)
            {
                // Update the properties of the StateTrackData object
                taskToUpdate.DateAndTIme = DateTime.Now;
                taskToUpdate.State = "ACTIVE";
                taskToUpdate.TotalFilesToCopy = totalFilesToCopy;
                taskToUpdate.TotalFilesSize = totalFilesSize;
                taskToUpdate.FilesLeftToDo = filesLeftToDo;
                taskToUpdate.SizeFilesLeftToDo = fileSizeLeftToDo;
                taskToUpdate.SourceFileDirectory = sourceFileDirectory;
                taskToUpdate.TargetFilesDirectory = targetFileDirectory;

                // Save the changes to the file
                SaveStateTrackDataToFile();
            }
        }

        public void UpdateInactive(string name)
        {
            // Find the corresponding StateTrackData object to the inactive task
            StateTrackData taskToUpdate = _stateTrackDataList.FirstOrDefault(task => task.Name == name);
            if (taskToUpdate != null)
            {
                // Mettez � jour les propri�t�s de l'objet StateTrackData
                taskToUpdate.DateAndTIme = DateTime.Now;
                taskToUpdate.State = "INACTIVE";
                taskToUpdate.TotalFilesToCopy = 0;
                taskToUpdate.TotalFilesSize = 0;
                taskToUpdate.FilesLeftToDo = 0;
                taskToUpdate.SizeFilesLeftToDo = 0;
                taskToUpdate.SourceFileDirectory = "";
                taskToUpdate.TargetFilesDirectory = "";

                // Save the changes to the file
                SaveStateTrackDataToFile();
            }
        }

        /// <summary>
        ///    Converts the JSON file to an XML file
        /// </summary>
        public void ConvertJSONtoXML()
        {
            string filePath = FilePath;
            string json = File.ReadAllText(filePath);
            List<StateTrackData> stateTrackDataList = JsonConvert.DeserializeObject<List<StateTrackData>>(json); // Deserialize the file content into stateTrackDataList

            // Serialize the stateTrackDataList and save it to the file
            XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, stateTrackDataList);
            }
        }

        /// <summary>
        ///   Converts the XML file to a JSON file
        /// </summary>
        public void ConvertXMLtoJSON()
        {
            string filePath = FilePath;
            XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<StateTrackData> stateTrackDataList = (List<StateTrackData>)serializer.Deserialize(reader);
                string json = JsonConvert.SerializeObject(stateTrackDataList, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
        }
    }
}