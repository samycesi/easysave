using easysave.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
