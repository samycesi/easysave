using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Easysave.Logger;
using Easysave.Model;
using Easysave.View;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Easysave.Controller
{
    public class BackupController
    {
        private const int MinTask = 1;
        private const int MaxTask = 5;
        public Dictionary<int, BackupModel> BackupTasks {  get; set; }
        public DailyLogger DailyLogger { get; set; }
        public StateTrackLogger StateTrackLogger{ get; set; }

        public BackupController(DailyLogger dailyLogger, StateTrackLogger stateTrackLogger) 
        {
            this.BackupTasks = new Dictionary<int, BackupModel>();
            this.DailyLogger = dailyLogger;
            this.StateTrackLogger = stateTrackLogger;
        }

        /// <summary>
        ///     This method adds a task in the dictionary of tasks
        /// </summary>
        /// <param name="task"></param>
        public void AddBackupTask(BackupModel task)
        {
            this.BackupTasks.Add(AvailableKey(), task);
        }

        /// <summary>
        ///     This method determines which key is available in the dictionary (from 1 to 5)
        ///     The view prevents the user from adding a sixth task
        /// </summary>
        private int AvailableKey()
        {
            for (int key = MinTask; key <= MaxTask; key++)
            {
                if (!this.BackupTasks.ContainsKey(key))
                {
                    return key;
                }
            }
            throw new InvalidOperationException("Plus de place disponible.");
        }

        /// <summary>
        ///     This method executes tasks depending on the input command
        ///     1-3 : tasks 1 to 3 
        ///     1;3 : tasks 1 and 3
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteTasks(string command)
        {
            
        }
        
        /// <summary>
        ///     This method executes a task depending on the key input corresponding to a task
        /// </summary>
        /// <param name="key"></param>
        private void ExecuteTaskByKey(int key) 
        {

        }

        /// <summary>
        ///     This method deletes a task from the dictionary depending to the key corresponding to a ask
        /// </summary>
        /// <param name="taskToRemove"></param>
        public void DeleteBackupTask(int taskToRemove)
        {
            if (this.BackupTasks.ContainsKey(taskToRemove))
            {
                this.BackupTasks.Remove(taskToRemove);
            }
        }

        /// <summary>
        ///     This method changes the path to the log files
        /// </summary>
        /// <param name="path"></param>
        public void ChangeLogPath(string path)
        {

        }

}
}
