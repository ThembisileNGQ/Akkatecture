using Akkatecture.Jobs.Commands;

namespace Akkatecture.Jobs.Events
{
    public class Scheduled<TJob, TIdentity> : SchedulerEvent<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public Schedule<TJob, TIdentity> Entry { get; }
        
        public Scheduled(
            Schedule<TJob, TIdentity> entry)
        {
            Entry = entry;
        }

    }
}