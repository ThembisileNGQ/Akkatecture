using System;
using Akka.Configuration;

namespace Akkatecture.Jobs
{
    public class JobSchedulerSettings
    {
        public string PersistenceId { get; }
        public string JournalPluginId { get; }
        public string SnapshotPluginId { get; }
        public TimeSpan TickInterval { get; }

        public JobSchedulerSettings(Config config)
        {
            
        }
    }
}