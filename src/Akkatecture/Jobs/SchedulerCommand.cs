using Akkatecture.ValueObjects;

namespace Akkatecture.Jobs
{
    public abstract class SchedulerMessage<TJob, TIdentity> : ValueObject
        where TJob : IJob
        where TIdentity : IJobId
    {
        
    }
    public abstract class SchedulerCommand<TJob, TIdentity> : SchedulerMessage<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TIdentity Id { get; }

        public SchedulerCommand(
            TIdentity id)
        {
            Id = id;
        }
    }
}