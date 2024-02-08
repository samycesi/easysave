using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Model
{
    public class AppConfigData
    {
        [JsonProperty("StateTrackPath")]
        public string StateTrackPath { get; set; }

        [JsonProperty("DailyPath")]
        public string DailyPath { get; set; }

        public AppConfigData(string stateTrackPath, string dailyPath)
        {
            this.StateTrackPath = stateTrackPath;
            this.DailyPath = dailyPath;
        }
    }
}