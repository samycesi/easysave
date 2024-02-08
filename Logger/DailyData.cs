using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Logger
{
    public class DailyData
    {
        [JsonProperty("Name")]
        private string Name { get; set; }

        [JsonProperty("SourceDirectory")]
        private string SourceDirectory { get; set; }

        [JsonProperty("DestinationDirectory")]
        private string DestinationDirectory { get; set; }

        [JsonProperty("FileSize")]
        private long FileSize { get; set; }


        [JsonProperty("FileTransferTime")]
        private long FileTransferTime { get; set; }

        [JsonProperty("Time")]
        private DateTime Time { get; set; }

        public DailyData(string name, string sourceDirectory, string destinationDirectory, long fileSize, long fileTransferTime, DateTime time)
        {
            this.Name = name;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.FileSize = fileSize;
            this.FileTransferTime = fileTransferTime;
            this.Time = time;
        }
    }
}