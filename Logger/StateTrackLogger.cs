using Easysave.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.Json;
using System.Xml;
using System.IO.Pipes;


namespace Easysave.Logger
{
    public class StateTrackLogger : LoggerModel
    {
        public StateTrackLogger(string folderPath,string filename) : base(folderPath,filename)
        {
            this.Init();
        }

        public StateTrackLogger(string folderPath) : base(folderPath)
        {
            this.Init();
        }

        /// <summary>
        ///     Initiates the log file
        /// </summary>
        public void Init()
        {
            List<StateTrackData> myDataList = new List<StateTrackData>();
            for (int i = 1; i <= 5; i++)
            {
                StateTrackData task = new StateTrackData("Save" + i.ToString(), DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
                myDataList.Add(task);
            }


            string fileType = Path.GetExtension(FilePath);
            switch (fileType)
            {
                case ".xml":
                    XmlSerializer serializer = new XmlSerializer(typeof(List<StateTrackData>));
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, myDataList);
                        File.WriteAllText(this.FilePath, writer.ToString());
                    }
                    break;
                case ".json":
                    string json = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(this.FilePath, json);
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
        /// <param name="key"></param>
        public void UpdateActive(long totalFilesToCopy, long totalFilesSize, long filesLeftToDo, long fileSizeLeftToDo, string sourceFileDirectory, string targetFileDirectory, int key)
        {
            StateTrackData currentTask = new StateTrackData("Save" + key.ToString(), DateTime.Now, "ACTIVE",
                                                            totalFilesToCopy, totalFilesSize, filesLeftToDo, fileSizeLeftToDo,
                                                             sourceFileDirectory, targetFileDirectory);
            switch (Path.GetExtension(FilePath))
            {
                case ".xml":
                    UpdateXML(currentTask, key);
                    break;
                case ".json":
                    UpdateJSON(currentTask, key);
                    break;
            }
           
        }

        /// <summary>
        ///     Updates the real time log file of an inactive task
        /// </summary>
        /// <param name="key"></param>
        public void UpdateInactive(int key)
        {
            StateTrackData currentTask = new StateTrackData("Save" + key.ToString(), DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
            switch (Path.GetExtension(FilePath))
            {
                case ".xml":
                    UpdateXML(currentTask, key);
                    break;
                case ".json":
                    UpdateJSON(currentTask, key);
                    break;
            }
        }

        /// <summary>
        ///     Deserializes the real time log file and writes updated state
        /// </summary>
        /// <param name="currentTask"></param>
        private void UpdateJSON(StateTrackData currentTask, int key)
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(this.FilePath);
                List<StateTrackData> myDataList = JsonSerializer.Deserialize<List<StateTrackData>>(json);
                myDataList[key] = currentTask;
                string updatedJson = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.FilePath, updatedJson);
            }
            
        }

        /// <summary>
        ///     Deserializes the real time log file and writes updated state
        /// </summary>
        /// <param name="currentTask"></param>
        private void UpdateXML(StateTrackData currentTask, int key)
        {
            List<StateTrackData> myDataList;
            var serializer = new XmlSerializer(typeof(List<StateTrackData>));
            if (File.Exists(FilePath))
            {
                using (var reader = new StreamReader(FilePath))
                {
                    myDataList = (List<StateTrackData>)serializer.Deserialize(reader);
                    myDataList[key] = currentTask;
                }
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, myDataList);
                    File.WriteAllText(this.FilePath, writer.ToString());
                }
            }
        }

        public void ConvertXMLtoJSON()
        {
            List<StateTrackData> myDataList;
            var serializer = new XmlSerializer(typeof(List<StateTrackData>));
            if (File.Exists(FilePath))
            {
                // FROM XML 
                using (var reader = new StreamReader(FilePath))
                {
                    myDataList = (List<StateTrackData>)serializer.Deserialize(reader);
                }
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".json");
                // TO JSON
                string convertedJSON = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.FilePath, convertedJSON);
            }
        }

        public void ConvertJSONtoXML()
        {
            List<StateTrackData> myDataList;
            var serializer = new XmlSerializer(typeof(List<StateTrackData>));
            if (File.Exists(FilePath))
            {
                // FROM JSON
                myDataList = JsonSerializer.Deserialize<List<StateTrackData>>(File.ReadAllText(this.FilePath));
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".xml");
                // TO XML
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, myDataList);
                    File.WriteAllText(this.FilePath, writer.ToString());
                }
            }
        }





    }
}