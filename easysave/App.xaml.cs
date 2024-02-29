using easysave.Model;
using System.Windows;

namespace easysave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppConfigData appConfigData; // Déclaration de AppConfigData comme une variable statique

        public App()
        {
            Config config = new Config();
            appConfigData = config.LoadFromFile();
        }
    }

}
