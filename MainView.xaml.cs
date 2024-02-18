using easysave.View;
using easysave.ViewModel;
using Easysave.Model;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace easysave
{
    public partial class MainView : Window
    {

        private MainViewModel viewModel;

        ResourceManager ressourceManager;

        CultureInfo cultureInfo;

        public MainView()
        {
            InitializeComponent();
            ressourceManager = new ResourceManager("easysave.Properties.Resources", typeof(MainView).Assembly);
            cultureInfo = CultureInfo.CreateSpecificCulture(App.appConfigData.Language);
            SetLanguage(App.appConfigData.Language);
            viewModel = new MainViewModel();
            DataContext = viewModel;
        }

        /// <summary>
        /// Set the language of the application
        /// </summary>
        /// <param name="language"></param>
        private void SetLanguage(string language)
        {
            App.appConfigData.Language = language;
            App.appConfigData.SaveToFile(); // Save the language to the config file

            ResourceDictionary dict = new ResourceDictionary(); // Create a new resource dictionary
            // Load the resource dictionary according to the language
            switch (language)
            {
                case "fr":
                    dict.Source = new System.Uri(@"..\Languages\textResources_fr.xaml", System.UriKind.Relative);
                    break;
                case "en":
                    dict.Source = new System.Uri(@"..\Languages\textResources_en.xaml", System.UriKind.Relative);
                    break;
                default:
                    dict.Source = new System.Uri(@"..\Languages\textResources_en.xaml", System.UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict); // Add the resource dictionary to the application resources
        }

        /// <summary>
        /// Change the language to french
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frButton_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("fr");
        }

        /// <summary>
        /// Change the language to english
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enButton_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("en");
        }

    }
}