using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Model.Logger
{
    public class StateTrackData
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("DateTime")]
        public DateTime DateAndTIme { get; set; }
        [JsonProperty("State")]
        public string State { get; set; }
        [JsonProperty("TotalFilesToCopy")]
        public long TotalFilesToCopy { get; set; }
        [JsonProperty("TotalFilesSize")]
        public long TotalFilesSize { get; set; }
        [JsonProperty("FilesLeftToDo")]
        public long FilesLeftToDo { get; set; }
        [JsonProperty("SizeFilesLeftToDo")]
        public long SizeFilesLeftToDo { get; set; }
        [JsonProperty("SourceFileDirectory")]
        public string SourceFileDirectory { get; set; }
        [JsonProperty("TargetFilesDirectory")]
        public string TargetFilesDirectory { get; set; }

        public StateTrackData(string name, DateTime dateTime, string state, long totalFilesToCopy, long totalFilesSize, long filesLeftToDo, long fileSizeLeftToDo, string sourceFileDirectory, string targetFileDirectory)
        {
            this.Name = name;
            this.DateAndTIme = dateTime;
            this.State = state;
            this.TotalFilesToCopy = totalFilesToCopy;
            this.TotalFilesSize = totalFilesSize;
            this.FilesLeftToDo = filesLeftToDo;
            this.SizeFilesLeftToDo = fileSizeLeftToDo;
            this.SourceFileDirectory = sourceFileDirectory;
            this.TargetFilesDirectory = targetFileDirectory;
        }
    }
}
