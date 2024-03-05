using System.IO;

namespace easysave.Model.Logger
{
    public class Logger
    {
        public string FilePath { get; set; }

        public string FolderPath { get; set; }

        public Logger(string folderPath, string fileName)
        {
            this.FolderPath = folderPath;
            string filePath = Path.Combine(folderPath, fileName);
            this.FilePath = filePath;
            if (!File.Exists(FilePath))
            {
                CreateFile();
            }
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
