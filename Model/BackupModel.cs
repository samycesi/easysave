using easysave.Model.Logger;

namespace easysave.Model
{
    public class BackupModel
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public BackupType Type { get; set; }
        public StateTrackData State { get; set; }

        public BackupModel(string name, string sourceDirectory, string destinationDirectory, BackupType type)
        {
            this.Name = name;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.Type = type;
            this.State = new StateTrackData(name);
        }

        override
        public string ToString()
        {
            return "Name: " + this.Name + " Source: " + this.SourceDirectory + " Destination: " + this.DestinationDirectory + " Type: " + this.Type;
        }
    }
}
