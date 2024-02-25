using System;

namespace easysave.Model
{
    public class BackupEvent : EventArgs
    {
        // Propriétés pour transporter des données d'événement
        public BackupModel BackupTask { get; }
        public int Key { get; }

        public BackupEvent(BackupModel backupTask)
        {
            BackupTask = backupTask;
        }

        public BackupEvent(int key)
        {
            Key = key;
        }
    }
}
