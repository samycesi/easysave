using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace easysave.ViewModel
{
    public class PriorityExtensionViewModel : INotifyPropertyChanged
    {
        private bool isPrioritySelected;
        private string extension;

        public bool IsPrioritySelected
        {
            get { return isPrioritySelected; }
            set
            {
                if (isPrioritySelected != value)
                {
                    isPrioritySelected = value;
                    OnPropertyChanged(nameof(isPrioritySelected));
                }
            }
        }

        public string Extension
        {
            get { return extension; }
            set
            {
                if (extension != value)
                {
                    extension = value;
                    OnPropertyChanged(nameof(Extension));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}