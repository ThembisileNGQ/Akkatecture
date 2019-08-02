using System.Collections.Immutable;

namespace Akkatecture.ScheduledJobs
{
    public class SchedulerState<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public static SchedulerState<TJob, TIdentity> New { get; } = new SchedulerState<TJob, TIdentity>(ImmutableDictionary<TIdentity, Schedule<TJob,TIdentity>>.Empty);

        public ImmutableDictionary<TIdentity, Schedule<TJob,TIdentity>> Entries { get; }
        public SchedulerState(ImmutableDictionary<TIdentity, Schedule<TJob,TIdentity>> entries)
        {
            Entries = entries;
        }
    }
}