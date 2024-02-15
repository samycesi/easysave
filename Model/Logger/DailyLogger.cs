using Newtonsoft.Json;
using System.IO;

namespace easysave.Model.Logger
{
    public class DailyLogger : Logger
    {
        public DailyLogger(string folderPath, string filename) : base(folderPath, filename)
        {
        }

        public DailyLogger(string filePath) : base(filePath)
        {
        }

        public void WriteDailyLog(BackupModel model, long fileSize, long fileTransferTime)
        {
            DailyData data = new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, DateTime.Now);
            using (StreamWriter sw = new StreamWriter(this.FilePath, true))
            {
                string JsonOutput = JsonConvert.SerializeObject(data);
                sw.WriteLine($"{JsonOutput}");
            }
        }


    }
}
