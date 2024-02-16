using Easysave.Controller;
using Easysave.Logger;
using Easysave.Model;
using Easysave.View;
using System.Text.Json;
using System.Configuration;


class Program
{
    static void Main(string[] args)
    {
        string jsonContent = File.ReadAllText(BackupController.ConfigFilePath);
        AppConfigData appConfig = JsonSerializer.Deserialize<AppConfigData>(jsonContent);
        ConsoleView view = new ConsoleView(appConfig.StateTrackPath, appConfig.DailyPath, appConfig.LogFileType);
        
        view.DisplayMenu();
    }
}