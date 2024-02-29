using System.Windows.Controls;

namespace easysave.View
{

    public partial class SettingsWindow : UserControl
    {
        public SettingsWindow()
        {
            InitializeComponent();
            // Initialize ComboBoxItems with AppConfig data
            foreach (ComboBoxItem item in Extension.Items)
            {
                if (item.Tag as string == App.appConfigData.LogExtension)
                {
                    Extension.SelectedItem = item;
                    break;
                }
            }
        }
    }
}