using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface ISnapshotApplier<TAggregate,TIdentity>  : IEventApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        bool Hydrate(TAggregate aggregate, IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot);
    }
}