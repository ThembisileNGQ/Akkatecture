using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface IEventApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        bool Apply(TAggregate aggregate, IAggregateEvent<TAggregate, TIdentity> aggregateEvent);
    }
}