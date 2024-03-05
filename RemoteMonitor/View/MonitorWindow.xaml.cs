using RemoteMonitor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RemoteMonitor.View
{
    /// <summary>
    /// Logique d'interaction pour MonitorWindow.xaml
    /// </summary>
    public partial class MonitorWindow : UserControl
    {
        public MonitorWindow()
        {
            InitializeComponent();

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MonitorViewModel monitorViewModel = (MonitorViewModel)DataContext;
            var selectedTasks = dataGrid.SelectedItems;

            int[] selectedTasksKey = new int[selectedTasks.Count];

            for (int i = 0; i < selectedTasks.Count; i++)
            {
                int index = dataGrid.Items.IndexOf(selectedTasks[i]);
                selectedTasksKey[i] = index;
            }

            monitorViewModel.Start(selectedTasksKey);

        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            MonitorViewModel monitorViewModel = (MonitorViewModel)DataContext;
            var selectedTasks = dataGrid.SelectedItems;

            int[] selectedTasksKey = new int[selectedTasks.Count];

            for (int i = 0; i < selectedTasks.Count; i++)
            {
                int index = dataGrid.Items.IndexOf(selectedTasks[i]);
                selectedTasksKey[i] = index;
            }

            monitorViewModel.Pause(selectedTasksKey);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MonitorViewModel monitorViewModel = (MonitorViewModel)DataContext;
            var selectedTasks = dataGrid.SelectedItems;

            int[] selectedTasksKey = new int[selectedTasks.Count];

            for (int i = 0; i < selectedTasks.Count; i++)
            {
                int index = dataGrid.Items.IndexOf(selectedTasks[i]);
                selectedTasksKey[i] = index;
            }

            monitorViewModel.Stop(selectedTasksKey);
        }
    }
}
