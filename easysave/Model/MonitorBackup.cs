namespace easysave.Model
{
    public class MonitorBackup
    {
        public string Name { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; }

        public MonitorBackup(string name, int progress, string status)
        {
            Name = name;
            Progress = progress;
            Status = status;
        }
    }
}
