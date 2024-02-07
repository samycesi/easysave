using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Model
{
    public class BackupModel
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public BackupType Type { get; set; }

        public BackupModel(string name, string sourceDirectory, string destinationDirectory, BackupType type)
        {
            this.Name = name;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.Type = type;
        }      

        /// <summary>
        ///     This method calculates the size of the source directory
        /// </summary>
        /// <returns> size as string </returns>
        public string CalculateSize()
        {
            string sizeSource = "";

            /// TO DO 
            
            return sizeSource;
        }

        /// <summary>
        ///     This method calculates the number of files in the source directory
        /// </summary>
        /// <returns> number of files as string </returns>
        public string CalculateNbFiles() {
            string nbFiles = "";

            /// TO DO
            
            return nbFiles;
        }

        /// <summary>
        ///     This method calculates the duration of the backup task
        /// </summary>
        /// <returns> duration as string </returns>
        public string CalculateDuration()
        {
            string duration = "";

            /// TO DO

            return duration;
        }
    }
}
