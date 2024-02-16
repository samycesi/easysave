using Easysave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Logger
{
    public class LoggerModel
    {
        public string FilePath { get; set; }

        public LoggerModel(string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName);
            this.FilePath = filePath;
            CreateFile();
        }

        public LoggerModel(string filepath)
        {
            this.FilePath = filepath;
        }

        /// <summary>
        ///     Creates a json file from the name 
        ///     using LogFilPath as the path
        /// </summary>
        public void CreateFile()
        {
            switch (Path.GetExtension(FilePath))
            {
                case ".xml":
                    File.WriteAllText(this.FilePath, "");
                    break;
                case ".json":
                    File.WriteAllText(this.FilePath, "{}");
                    break;
            }
        }


    }
}