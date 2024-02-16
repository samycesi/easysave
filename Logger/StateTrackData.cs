using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Easysave.Logger
{
    public class StateTrackData
    {
        [JsonProperty("Name")]
        [XmlElement("Name")]
        public string Name { get; set; }
        [JsonProperty("DateTime")]
        [XmlElement("DateTime")]
        public DateTime DateAndTIme { get; set; }
        [JsonProperty("State")]
        [XmlElement("State")]
        public string State { get; set; }
        [JsonProperty("TotalFilesToCopy")]
        [XmlElement("TotalFilesToCopy")]
        public long TotalFilesToCopy { get; set; }
        [JsonProperty("TotalFilesSize")]
        [XmlElement("TotalFilesSize")]
        public long TotalFilesSize { get; set; }
        [JsonProperty("FilesLeftToDo")]
        [XmlElement("FilesLeftToDo")]
        public long FilesLeftToDo { get; set; }
        [JsonProperty("SizeFilesLeftToDo")]
        [XmlElement("SizeFilesLeftToDo")]
        public long SizeFilesLeftToDo { get; set; }
        [JsonProperty("SourceFileDirectory")]
        [XmlElement("SourceFileDirectory")]
        public string SourceFileDirectory { get; set; }
        [JsonProperty("TargetFilesDirectory")]
        [XmlElement("TargetFilesDirectory")]
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
        public StateTrackData()
        {
        }
    }
}
