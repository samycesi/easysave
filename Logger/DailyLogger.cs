using Easysave.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Easysave.Logger
{
    public class DailyLogger : LoggerModel
    {
        public DailyLogger (string folderPath,string filename) : base(folderPath,filename)
        {
            // initialization of file with empty list
            switch (Path.GetExtension(FilePath))
            {
                case ".xml":
                    XmlSerializer serializer = new XmlSerializer(typeof(List<DailyData>));
                    using (TextWriter writer = new StreamWriter(FilePath))
                    {
                        serializer.Serialize(writer, new List<DailyData>());
                    }
                    break;
                case ".json":
                    List<DailyData> initList = new List<DailyData>();
                    string jsonContent = JsonSerializer.Serialize(initList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(FilePath, jsonContent);
                    break;
            }

        }
        public DailyLogger(string filePath) : base(filePath)
        {

        }

        public void WriteDailyLogXML(BackupModel model, long fileSize, long fileTransferTime)
        {
            /*            DailyData data = new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, DateTime.Now);
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DailyData));
                        if (File.Exists(FilePath))
                        {
                            using (StreamWriter sw = new StreamWriter(FilePath, true))
                            {
                                xmlSerializer.Serialize(sw, data);
                            }
                        }*/

            XmlSerializer serializer = new XmlSerializer(typeof(List<DailyData>));
            List<DailyData> myDataList;
            // Read the existing XML file into a list
            using (TextReader reader = new StreamReader(FilePath))
            {
                myDataList = (List<DailyData>)serializer.Deserialize(reader);
            }
            Console.WriteLine(myDataList.Count);
            myDataList.Add(new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, DateTime.Now));
            using (TextWriter writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, myDataList);
            }
        }

        public void WriteDailyLogJSON(BackupModel model,long fileSize,long fileTransferTime)
        {
            /*            DailyData data = new DailyData(model.Name,model.SourceDirectory,model.DestinationDirectory,fileSize,fileTransferTime,DateTime.Now);
                        if (File.Exists(FilePath))
                        {
                            using (StreamWriter sw = new StreamWriter(FilePath, true))
                            {
                                string JsonOutput = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                                sw.WriteLine($"{JsonOutput}");
                            }
                        }*/

            string data = File.ReadAllText(this.FilePath);
            List<DailyData> myDataList = JsonSerializer.Deserialize<List<DailyData>>(data);
            myDataList.Add(new DailyData(model.Name, model.SourceDirectory, model.DestinationDirectory, fileSize, fileTransferTime, DateTime.Now));
            string updatedData = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(this.FilePath, updatedData);
        }

        public void ConvertXMLtoJSON()
        {
            /*            // FROM JSON
                        string content;
                        DailyData data;
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DailyData));
                        if (File.Exists(FilePath))
                        {
                            using (StreamWriter sw = new StreamWriter(FilePath, true))
                            {
                                xmlSerializer.Serialize(sw, data);
                            }
                        }

                        File.Delete(FilePath);
                        // New File Path
                        FilePath = Path.ChangeExtension(FilePath, ".json");
                        // TO XML*/
            List<DailyData> myDataList;
            var serializer = new XmlSerializer(typeof(List<DailyData>));
            if (File.Exists(FilePath))
            {
                // FROM XML 

                using (var reader = new StreamReader(FilePath))
                {
                    myDataList = (List<DailyData>)serializer.Deserialize(reader);
                }
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".json");
                // TO JSON
                string convertedJSON = JsonSerializer.Serialize(myDataList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.FilePath, convertedJSON);
            }

        }

        public void ConvertJSONtoXML()
        {
            /*            // FROM XML 
                        string xmlContent = File.ReadAllText(FilePath);

                        File.Delete(FilePath);
                        // New File Path
                        FilePath = Path.ChangeExtension(FilePath, ".json");
                        // To JSON
                        string json = ConvertXmlToJson(xmlContent);
                        File.WriteAllText(this.FilePath, json);*/


            List<DailyData> myDataList;
            var serializer = new XmlSerializer(typeof(List<DailyData>));
            if (File.Exists(FilePath))
            {
                // FROM JSON
                myDataList = JsonSerializer.Deserialize<List<DailyData>>(File.ReadAllText(this.FilePath));
                File.Delete(FilePath);
                // New File Path
                FilePath = Path.ChangeExtension(FilePath, ".xml");
                // TO XML
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, myDataList);
                    File.WriteAllText(this.FilePath, writer.ToString());
                }
            }
        }

    }
}
