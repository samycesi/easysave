using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Easysave.Model
{
    public class AppConfigData
    {
        [JsonProperty("StateTrackPath")]
        [XmlElement("StateTrackPath")]
        public string StateTrackPath { get; set; }
        [JsonProperty("DailyPath")]
        [XmlElement("DailyPath")]
        public string DailyPath { get; set; }
        [JsonProperty("LogFileType")]
        [XmlElement("LogFileType")]
        public string LogFileType { get; set; }
        public AppConfigData(string stateTrackPath, string dailyPath,string logFileType)
        {
            this.StateTrackPath = stateTrackPath;
            this.DailyPath = dailyPath;
            this.LogFileType = logFileType;
        }
    }
}