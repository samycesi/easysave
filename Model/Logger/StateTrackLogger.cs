using Easysave.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;


namespace Easysave.Model.Logger
{
    public class StateTrackLogger : Logger
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
            string json = JsonConvert.SerializeObject(myDataList, Formatting.Indented);
            Console.WriteLine(this.FilePath);
            File.WriteAllText(this.FilePath, json);
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
            Update(currentTask, key);
        }

        /// <summary>
        ///     Updates the real time log file of an inactive task
        /// </summary>
        /// <param name="key"></param>
        public void UpdateInactive(int key)
        {
            StateTrackData currentTask = new StateTrackData("Save" + key.ToString(), DateTime.Now, "INACTIVE", 0, 0, 0, 0, "", "");
            Update(currentTask, key);
        }

        /// <summary>
        ///     Deserializes the real time log file and writes updated state
        /// </summary>
        /// <param name="currentTask"></param>
        private void Update(StateTrackData currentTask, int key)
        {
            string json = File.ReadAllText(this.FilePath);
            List<StateTrackData> myDataList = JsonConvert.DeserializeObject<List<StateTrackData>>(json);
            myDataList[key] = currentTask;
            string updatedJson = JsonConvert.SerializeObject(myDataList, Formatting.Indented);
            File.WriteAllText(this.FilePath, updatedJson);
        }
    }
}