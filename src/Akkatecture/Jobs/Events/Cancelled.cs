using System;

namespace Akkatecture.Jobs.Events
{
    public class Cancelled<TJob, TIdentity> : SchedulerEvent<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TIdentity Id { get; }
        public DateTime TriggerDate { get; }

        public Cancelled(
            TIdentity id,
            DateTime triggerDate)
        {
            Id = id;
            TriggerDate = triggerDate;

        }
    }
}