using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace easysave.Model.Logger
{
    public class StateTrackData : INotifyPropertyChanged
    {
        [JsonProperty(nameof(Name))]
        [XmlElement("Name")]
        public string Name { get; set; }

        [JsonProperty("DateTime")]
        [XmlElement("DateTime")]
        public DateTime DateAndTIme { get; set; }

        [JsonProperty(nameof(State))]
        [XmlElement("State")]
        public string State { get; set; }

        [JsonProperty(nameof(TotalFilesToCopy))]
        [XmlElement("TotalFilesToCopy")]
        public long TotalFilesToCopy { get; set; }

        [JsonProperty(nameof(TotalFilesSize))]
        [XmlElement("TotalFilesSize")]
        public long TotalFilesSize { get; set; }

        [JsonProperty(nameof(FilesLeftToDo))]
        [XmlElement("FilesLeftToDo")]
        public long FilesLeftToDo { get; set; }

        [JsonProperty(nameof(SizeFilesLeftToDo))]
        [XmlElement("SizeFilesLeftToDo")]
        public long SizeFilesLeftToDo { get; set; }

        [JsonProperty(nameof(SourceFileDirectory))]
        [XmlElement("SourceFileDirectory")]
        public string SourceFileDirectory { get; set; }

        [JsonProperty(nameof(TargetFilesDirectory))]
        [XmlElement("TargetFilesDirectory")]
        public string TargetFilesDirectory { get; set; }

        private int progress;

        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (progress != value)
                {
                    progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public event EventHandler StateUpdated;

        protected virtual void OnStateUpdated()
        {
            StateUpdated?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StateTrackData(string name)
        {
            this.Name = name;
            this.State = "INACTIVE";
            this.Progress = 0;
        }

        public void Init(string state, long totalFilesToCopy, long totalFilesSize, long filesLeftToDo, long fileSizeLeftToDo, string sourceFileDirectory, string targetFileDirectory)
        {
            this.State = state;
            this.TotalFilesToCopy = totalFilesToCopy;
            this.TotalFilesSize = totalFilesSize;
            this.FilesLeftToDo = filesLeftToDo;
            this.SizeFilesLeftToDo = fileSizeLeftToDo;
            this.SourceFileDirectory = sourceFileDirectory;
            this.TargetFilesDirectory = targetFileDirectory;
            this.Progress = 0;
            OnStateUpdated();
        }

        public void Update(string state, int progress, long filesLeftToDo, long fileSizeLeftToDo, string sourceFileDirectory, string targetFileDirectory)
        {
            this.State = state;
            this.Progress = progress;
            this.FilesLeftToDo = filesLeftToDo;
            this.SizeFilesLeftToDo = fileSizeLeftToDo;
            this.SourceFileDirectory = sourceFileDirectory;
            this.TargetFilesDirectory = targetFileDirectory;
            OnStateUpdated();
        }
    }
}
