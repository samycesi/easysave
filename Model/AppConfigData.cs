using Newtonsoft.Json;

namespace Easysave.Model
{
    public class AppConfigData
    {
        [JsonProperty("StateTrackPath")]
        public string StateTrackPath { get; set; }

        [JsonProperty("DailyPath")]
        public string DailyPath { get; set; }

        [JsonProperty("fileExtensionToEncrypt")]
        public string FileExtensionToEncrypt { get; set; }

        [JsonProperty("cryptosoftPath")]
        public string CryptosoftPath { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("businessSoftwarePath")]
        public string BusinessSoftwarePath { get; set; }

        public AppConfigData(string stateTrackPath, string dailyPath)
        {
            this.StateTrackPath = stateTrackPath;
            this.DailyPath = dailyPath;
        }
    }
}