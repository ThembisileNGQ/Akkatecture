using System;
using Akka.Configuration;
using Akkatecture.Configuration;

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
            var schedulerConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            schedulerConfig = schedulerConfig.GetConfig("akkatecture.job-scheduler");
            PersistenceId = schedulerConfig.GetString("persistence-id");
            JournalPluginId = schedulerConfig.GetString("journal-plugin-id");
            SnapshotPluginId = schedulerConfig.GetString("snapshot-plugin-id");
            TickInterval = schedulerConfig.GetTimeSpan("tick-interval");
        }
    }
}