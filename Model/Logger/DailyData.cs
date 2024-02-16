using Newtonsoft.Json;

namespace easysave.Model.Logger
{
    public class DailyData
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SourceDirectory")]
        public string SourceDirectory { get; set; }

        [JsonProperty("DestinationDirectory")]
        public string DestinationDirectory { get; set; }

        [JsonProperty("FileSize")]
        public long FileSize { get; set; }


        [JsonProperty("FileTransferTime")]
        public long FileTransferTime { get; set; }

        [JsonProperty("Time")]
        public DateTime Time { get; set; }

        [JsonProperty("TotalEncryptionTime")]
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
    }
}