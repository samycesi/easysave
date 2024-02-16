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
        public string StateTrackPath { get; set; }
        public string DailyPath { get; set; }
        public string LogFileType { get; set; }
        public AppConfigData(string stateTrackPath, string dailyPath,string logFileType)
        {
            this.StateTrackPath = stateTrackPath;
            this.DailyPath = dailyPath;
            this.LogFileType = logFileType;
        }
    }
}