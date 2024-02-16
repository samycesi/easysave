using easysave.ViewModel;
using Easysave.Model;
using System.Globalization;
using System.Resources;

namespace easysave
{
    public partial class MainView
    {

        private MainViewModel viewModel;

        ResourceManager ressourceManager;

        CultureInfo cultureInfo;

        // à implémenter
        public MainView()
        {
            AppConfigData appConfigData = new AppConfigData();
        }

    }
}