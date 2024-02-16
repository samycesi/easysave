using Easysave.Model;
using System;
using System.Text.Json;
using System.Xml.Serialization;

namespace Easysave.Logger
{
    public class DailyLogger : Logger
    {
        public DailyLogger (string folderPath,string filename) : base(folderPath,filename)
        {
        }
        public DailyLogger(string filePath) : base(filePath)
        {
        }

        public void WriteDailyLogXML(BackupModel model, long fileSize, long fileTransferTime)
        {
            DailyData data = new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, DateTime.Now);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DailyData));
            if (File.Exists(FilePath))
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true))
                {
                    xmlSerializer.Serialize(sw, data);
                }
            }
            
        }

        public void WriteDailyLogJSON(BackupModel model,long fileSize,long fileTransferTime)
        {
            DailyData data = new DailyData(model.Name,model.SourceDirectory,model.DestinationDirectory,fileSize,fileTransferTime,DateTime.Now);
            if (File.Exists(FilePath))
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true))
                {
                    string JsonOutput = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                    sw.WriteLine($"{JsonOutput}");
                }
            }

        }


    }
}
