using System;
using System.Globalization;
using System.Resources;
using Easysave.Controller;
using Easysave.Model;


namespace Easysave.View
{
    public class ConsoleView
    {
        private readonly BackupController backupController;
        private ResourceManager resourceManager;
        private CultureInfo cultureInfo;

        
        public ConsoleView(BackupController controller)
        {
            backupController = controller;
            resourceManager = new ResourceManager("EasySave.View.Messages", typeof(ConsoleView).Assembly);
            cultureInfo = new CultureInfo("en");
        }
        

        public ConsoleView()
        {
            resourceManager = new ResourceManager("EasySave.View.Messages", typeof(ConsoleView).Assembly);
            cultureInfo = new CultureInfo("en");
        }



        private void ChangeLanguage()
        {

        }

        public void DisplayMenu()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                Console.WriteLine($"\n--------- {resourceManager.GetString("MenuTitle", cultureInfo)} ---------");
                Console.WriteLine($"1. {resourceManager.GetString("AddBackupJob", cultureInfo)}");
                Console.WriteLine($"2. {resourceManager.GetString("ListBackupJobs", cultureInfo)}");
                Console.WriteLine($"2. {resourceManager.GetString("ExecuteBackupJob", cultureInfo)}");
                Console.WriteLine($"3. {resourceManager.GetString("ExecuteAllBackupJobs", cultureInfo)}");
                Console.WriteLine($"4. {resourceManager.GetString("Exit", cultureInfo)}");
                Console.WriteLine($"5. {resourceManager.GetString("ChangeLanguage", cultureInfo)}");

                Console.Write($"{resourceManager.GetString("SelectOption", cultureInfo)}: ");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        //AddBackupJob();
                        break;
                    case "2":
                        //ExecuteBackupJob();
                        break;
                    case "3":
                        //backupController.ExecuteAllBackupJobs();
                        Console.WriteLine(resourceManager.GetString("AllJobsExecuted", cultureInfo));
                        break;
                    case "4":
                        keepRunning = false;
                        break;
                    case "5":
                        ChangeLanguage();
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption", cultureInfo));
                        break;
                }

            }
        }

    }
}
