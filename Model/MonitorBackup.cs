namespace easysave.Model
{
    public class MonitorBackup
    {
        public string Name { get; set; }
        public int Progress { get; set; }

        public MonitorBackup(string name, int progress)
        {
            Name = name;
            Progress = progress;
        }
    }
}
