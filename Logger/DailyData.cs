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
    public class DailyData
    {
        [JsonProperty("Name")]
        [XmlElement("Name")]
        public string Name { get; set; }
        [JsonProperty("SourceDirectory")]
        [XmlElement("SourceDirectory")]
        public string SourceDirectory { get; set; }
        [JsonProperty("DestinationDirectory")]
        [XmlElement("DestinationDirectory")]
        public string DestinationDirectory { get; set; }
        [JsonProperty("FileSize")]
        [XmlElement("FileSize")]
        public long FileSize { get; set; }
        [JsonProperty("FileTransferTime")]
        [XmlElement("FileTransferTime")]
        public long FileTransferTime { get; set; }
        [JsonProperty("Time")]
        [XmlElement("Time")]
        public DateTime Time { get; set; }

        public DailyData(string name, string sourceDirectory, string destinationDirectory, long fileSize, long fileTransferTime, DateTime time)
        {
            this.Name = name;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.FileSize = fileSize;
            this.FileTransferTime = fileTransferTime;
            this.Time = time;
        }
        public DailyData()
        {
        }
    }
}