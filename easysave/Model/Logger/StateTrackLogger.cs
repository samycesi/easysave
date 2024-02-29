
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;


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
            foreach (var stateTrackData in _stateTrackDataList)
            {
                SubscribeToStateUpdatedEvent(stateTrackData); // Subscribe to the DataUpdated event of each StateTrackData
            }
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

        private void SubscribeToStateUpdatedEvent(StateTrackData data)
        {
            // Abonnez-vous à l'événement DataUpdated de chaque StateTrackData
            data.StateUpdated += HandleStateUpdated;
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
                            _stateTrackDataList.Add(backupTask.State); // Add the default data to the list
                        }
                    }
                    else
                    {
                        _stateTrackDataList = JsonSerializer.Deserialize<List<StateTrackData>>(json); // Deserialize the file content into _stateTrackDataList
                    }
                }
                else if (Path.GetExtension(filePath).ToLower() == ".xml")
                {
                    if (new FileInfo(filePath).Length == 0)
                    {
                        foreach (var backupTask in backupTasks.Values)
                        {
                            _stateTrackDataList.Add(backupTask.State); // Add the default data to the list
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
                    _stateTrackDataList.Add(backupTask.State);
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
            StateTrackData taskData = e.BackupTask.State;

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

        private void HandleStateUpdated(object sender, EventArgs e)
        {
            // Lorsque les données sont mises à jour, appelez la méthode SaveStateTrackDataToFile()
            SaveStateTrackDataToFile();
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
                    string json = JsonSerializer.Serialize(_stateTrackDataList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    break;
            }
        }

        /// <summary>
        ///    Converts the JSON file to an XML file
        /// </summary>
        public void ConvertJSONtoXML()
        {
            // From Json
            string json = File.ReadAllText(FilePath);
            // Deserialize the file content into stateTrackDataList
            List<StateTrackData> stateTrackDataList = JsonSerializer.Deserialize<List<StateTrackData>>(json);
            // Delete old file
            File.Delete(FilePath);
            // New File Path
            FilePath = Path.ChangeExtension(FilePath, ".xml");

            // Serialize the stateTrackDataList and save it to the file
            XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, stateTrackDataList);
            }
        }

        /// <summary>
        ///   Converts the XML file to a JSON file
        /// </summary>
        public void ConvertXMLtoJSON()
        {
            List<StateTrackData> myDataList;
            var serializer = new XmlSerializer(typeof(List<StateTrackData>));
            if (File.Exists(FilePath))
            {
                // From Xml
                using (var reader = new StreamReader(FilePath))
                {
                    myDataList = (List<StateTrackData>)serializer.Deserialize(reader);
                }
                // Delete old file
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".json");
                // TO JSON
                string convertedJSON = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.FilePath, convertedJSON);
            }
        }
    }
}