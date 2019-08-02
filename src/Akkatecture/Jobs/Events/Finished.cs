using System;
using Akkatecture.Jobs.Commands;

namespace Akkatecture.Jobs.Events
{
    public class Finished<TJob, TIdentity> : SchedulerEvent<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TIdentity Id { get; }
        public DateTime TriggerDate { get; }

        public Finished(
            TIdentity id,
            DateTime triggerDate)
        {
            Id = id;
            TriggerDate = triggerDate;

        }
    }
}