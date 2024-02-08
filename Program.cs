using Easysave.Controller;
using Easysave.Logger;
using Easysave.Model;
using Easysave.View;
using Newtonsoft.Json;
using System.Configuration;


class Program
{
    static void Main(string[] args)
    {
        string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
        string jsonFilePath = Path.Combine(solutionDir, "AppConfig.json");

  
        string jsonContent = File.ReadAllText(jsonFilePath);

        AppConfigData appConfig = JsonConvert.DeserializeObject<AppConfigData>(jsonContent);
        String stateTrackPath = appConfig.StateTrackPath;
        String dailyPath = appConfig.DailyPath;
    
        ConsoleView view = new ConsoleView(stateTrackPath, dailyPath);
        
        view.DisplayMenu(); // Démarrer l'interaction avec l'utilisateur
    }
}