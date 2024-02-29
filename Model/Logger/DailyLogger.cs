using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace easysave.Model.Logger
{
    public class DailyLogger : Logger
    {
        public DailyLogger(string folderPath, string filename) : base(folderPath, filename)
        {
            var fileInfo = new FileInfo(FilePath);
            
            // Initialization of file with empty list
            switch (Path.GetExtension(FilePath))
            {
                case ".xml":
                    // Check if file needs to be initialized (created empty in xml)
                    if (fileInfo.Length == 0)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<DailyData>));
                        using (TextWriter writer = new StreamWriter(FilePath))
                        {
                            serializer.Serialize(writer, new List<DailyData>());
                        }
                    }
                    break;
                case ".json":
                    // Check if file needs to be initialized (created with {} in json)
                    if(fileInfo.Length == 0 || File.ReadAllText(FilePath) == "{}")
                    {
                        List<DailyData> initList = new List<DailyData>();
                        string jsonContent = JsonSerializer.Serialize(initList, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(FilePath, jsonContent);
                    }
                    break;

            }

            

        }

        /// <summary>
        /// Write the daily log in the file (XML)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileTransferTime"></param>
        /// <param name="totalEncryptionTime"></param>
        public void WriteDailyLogXML(BackupModel model, long fileSize, long fileTransferTime, long totalEncryptionTime)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DailyData>));
            List<DailyData> myDataList;
            // Read the existing XML file into a list
            using (TextReader reader = new StreamReader(FilePath))
            {
                myDataList = (List<DailyData>)serializer.Deserialize(reader);
            }
            Console.WriteLine(myDataList.Count);
            myDataList.Add(new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, totalEncryptionTime, DateTime.Now));
            using (TextWriter writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, myDataList);
            }
        }

        /// <summary>
        /// Write the daily log in the file (JSON)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileTransferTime"></param>
        /// <param name="totalEncryptionTime"></param>
        public void WriteDailyLogJSON(BackupModel model, long fileSize, long fileTransferTime, long totalEncryptionTime)
        {
            string data = File.ReadAllText(this.FilePath);
            List<DailyData> myDataList = JsonSerializer.Deserialize<List<DailyData>>(data);
            myDataList.Add(new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, totalEncryptionTime, DateTime.Now));
            string updatedData = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(this.FilePath, updatedData);
        }

        /// <summary>
        /// Convert the XML file to a JSON file
        /// </summary>
        public void ConvertXMLtoJSON()
        {
            List<DailyData> myDataList;
            var serializer = new XmlSerializer(typeof(List<DailyData>));
            if (File.Exists(FilePath))
            {
                // From xml
                using (var reader = new StreamReader(FilePath))
                {
                    myDataList = (List<DailyData>)serializer.Deserialize(reader);
                }
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".json");
                // To json
                string convertedJSON = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.FilePath, convertedJSON);
            }
        }

        /// <summary>
        /// Convert the JSON file to an XML file
        /// </summary>
        public void ConvertJSONtoXML()
        {
            List<DailyData> myDataList;
            var serializer = new XmlSerializer(typeof(List<DailyData>));
            if (File.Exists(FilePath))
            {
                // From json
                myDataList = JsonSerializer.Deserialize<List<DailyData>>(File.ReadAllText(this.FilePath));
                // Delete Old file
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".xml");
                // To xml
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, myDataList);
                    File.WriteAllText(this.FilePath, writer.ToString());
                }
            }
        }


    }
}
