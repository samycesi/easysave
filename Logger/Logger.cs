using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Logger
{
    public abstract class Logger
    {
        public string LogFilePath { get; set; }

        public Logger(string filePath)
        {
            this.LogFilePath = filePath;
        }

        /// <summary>
        ///     This method writes the data in the log file
        ///     
        ///     ARGUMENTS A DEFINIR - PEUT ETRE PAS NECESSAIRE ?
        /// </summary>
        public abstract void WriteLog();

        /// <summary>
        ///     This method creates a file using the input as the name
        ///     using the attribute LogFilePath for the path
        /// </summary>
        public void CreateFile(string fileName)
        {

        }
    }
}
