using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface IMessageApplier<TAggregate, TIdentity> : IEventApplier<TAggregate, TIdentity>, ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
}