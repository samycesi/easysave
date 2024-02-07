using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easysave.Logger
{
    public class DailyLogger : Logger
    {
        public DailyLogger (string filePath) : base(filePath)
        {

        }


        /// <summary>
        ///     Implements the method enabling to write data in the log file
        ///     
        ///     ARGUMENTS A DEFINIR - PEUT ETRE PAS NECESSAIRE ?
        /// </summary>
        public override void WriteLog()
        {

        }


    }
}
