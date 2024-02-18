using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json;

namespace Easysave.Model
{
    public class AppConfigData
    {

        [JsonProperty("stateTrackPath")]
        public string StateTrackPath{ get; set; }

        [JsonProperty("dailyPath")]
        public string DailyPath { get; set; }

        [JsonProperty("fileExtensionToEncrypt")]
        public string FileExtensionToEncrypt { get; set; }

        [JsonProperty("cryptosoftPath")]
        public string CryptosoftPath { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("businessSoftwarePath")]
        public string BusinessSoftwarePath { get; set; }

        [JsonProperty("logExtension")]
        public string LogExtension { get; set; }

        private string jsonFilePath;

        public string SavesPath { get; set; }

        public AppConfigData()
        {
            string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            jsonFilePath = Path.Combine(solutionDir, @"Config\AppConfig.json");
            SavesPath = Path.Combine(solutionDir, @"Config\TasksSave.json");
        }

        public void SaveToFile()
        {
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonString);
        }
    }
}