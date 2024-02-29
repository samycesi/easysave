using easysave.Model;
using System;
using System.ComponentModel;

namespace easysave.ViewModel
{
    public class BackupTaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                Console.WriteLine("IsSelected: " + value);
            }
        }

        public int Key { get; }
        public BackupModel Backup { get; }

        public BackupTaskViewModel(int key, BackupModel backup)
        {
            Key = key;
            Backup = backup;
        }
    }
}
