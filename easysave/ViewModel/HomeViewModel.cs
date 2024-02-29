namespace easysave.ViewModel
{
    public class HomeViewModel
    {

        private MainViewModel MainViewModel;

        public RelayCommand CreateTaskViewCommand { get; }

        public HomeViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            CreateTaskViewCommand = mainViewModel.CreateSaveViewCommand;
        }

    }
}