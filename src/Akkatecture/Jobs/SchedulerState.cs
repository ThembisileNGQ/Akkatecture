using System.Collections.Immutable;
using Akkatecture.Jobs.Commands;

namespace Akkatecture.Jobs
{
    public class SchedulerState<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public static SchedulerState<TJob, TIdentity> New { get; } = new SchedulerState<TJob, TIdentity>(ImmutableDictionary<TIdentity, Schedule<TJob,TIdentity>>.Empty);

        public ImmutableDictionary<TIdentity, Schedule<TJob, TIdentity>> Entries { get; }
        public SchedulerState(
            ImmutableDictionary<TIdentity, Schedule<TJob,TIdentity>> entries)
        {
            Entries = entries;
        }
        
        public SchedulerState<TJob, TIdentity> AddEntry(Schedule<TJob,TIdentity> entry) => new SchedulerState<TJob, TIdentity>(Entries.SetItem(entry.Id, entry));
        public SchedulerState<TJob, TIdentity> RemoveEntry(TIdentity jobId) => new SchedulerState<TJob, TIdentity>(Entries.Remove(jobId));
    }
}