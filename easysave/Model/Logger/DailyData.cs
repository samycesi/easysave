using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace easysave.Model.Logger
{
    public class DailyData
    {
        [JsonProperty(nameof(Name))]
        [XmlElement("Name")]
        public string Name { get; set; }

        [JsonProperty(nameof(SourceDirectory))]
        [XmlElement("SourceDirectory")]
        public string SourceDirectory { get; set; }

        [JsonProperty(nameof(DestinationDirectory))]
        [XmlElement("DestinationDirectory")]
        public string DestinationDirectory { get; set; }

        [JsonProperty(nameof(FileSize))]
        [XmlElement("FileSize")]
        public long FileSize { get; set; }

        [JsonProperty(nameof(FileTransferTime))]
        [XmlElement("FileTransferTime")]
        public long FileTransferTime { get; set; }

        [JsonProperty(nameof(Time))]
        [XmlElement("Time")]
        public DateTime Time { get; set; }

        [JsonProperty(nameof(TotalEncryptionTime))]
        [XmlElement("TotalEncryptionTime")]
        public long TotalEncryptionTime { get; set; }

        public DailyData(string name, string sourceDirectory, string destinationDirectory, long fileSize, long fileTransferTime, long totalEncryptionTime, DateTime time)
        {
            this.Name = name;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.FileSize = fileSize;
            this.FileTransferTime = fileTransferTime;
            this.Time = time;
            this.TotalEncryptionTime = totalEncryptionTime;
        }

        public DailyData()
        {
        }
    }
}