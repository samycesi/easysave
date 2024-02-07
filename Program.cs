//using EasySave.Controller;
using Easysave.View;

class Program
{
    static void Main(string[] args)
    {
        string logDirectory = @"C:\Dev\Easysave_v1\Easysave_v1\Logs"; // Assurez-vous que ce répertoire existe ou ajustez selon votre configuration
                                                                      // BackupController controller = new BackupController(logDirectory);
        ConsoleView view = new ConsoleView();

        view.DisplayMenu(); // Démarrer l'interaction avec l'utilisateur
    }
}