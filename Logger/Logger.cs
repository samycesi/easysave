using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Logger
{
    public class Logger
    {
        public string FilePath { get; set; }

        public Logger(string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName);
            this.FilePath = filePath;
            Console.WriteLine("here");
            CreateFile();


        }

        /// <summary>
        ///     Creates a json file from the name 
        ///     using LogFilPath as the path
        /// </summary>
        public void CreateFile()
        {
            File.WriteAllText(this.FilePath, "{}");
        }


    }
}
