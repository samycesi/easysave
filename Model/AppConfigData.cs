using Newtonsoft.Json;
using System.IO;
using System.Text.Json;

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

        private string jsonFilePath;
        public AppConfigData()
        {
            string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            jsonFilePath = Path.Combine(solutionDir, @"Config\AppConfig.json");
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                var configData = JsonConvert.DeserializeObject<AppConfigData>(jsonString);

                StateTrackPath = configData.StateTrackPath;
                DailyPath = configData.DailyPath;
                FileExtensionToEncrypt = configData.FileExtensionToEncrypt;
                CryptosoftPath = configData.CryptosoftPath;
                Language = configData.Language;
                BusinessSoftwarePath = configData.BusinessSoftwarePath;
            }
            else
            {
                throw new FileNotFoundException("Le fichier JSON spécifié est introuvable.", jsonFilePath);
            }
        }

        public void SaveToFile()
        {
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonString);
        }
    }
}