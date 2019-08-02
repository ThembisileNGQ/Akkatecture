using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Jobs
{
    
    public interface ISchedulerEvent : IVersionedType
    {
    }

    public interface ISchedulerEvent<TJob, TIdentity> : ISchedulerEvent
        where TJob : IJob
        where TIdentity : IJobId
    {
    }
}