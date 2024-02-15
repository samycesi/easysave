using System.IO;

namespace easysave.Model.Logger
{
    public class Logger
    {
        public string FilePath { get; set; }

        public Logger(string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName);
            this.FilePath = filePath;
            CreateFile();
        }

        public Logger(string filepath)
        {
            this.FilePath = filepath;
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