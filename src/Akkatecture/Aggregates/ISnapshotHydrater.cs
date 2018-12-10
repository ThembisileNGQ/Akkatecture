using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface ISnapshotHydrater<TAggregate,TIdentity>  : IEventApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        bool Hydrate(TAggregate aggregate, IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot);
    }
}