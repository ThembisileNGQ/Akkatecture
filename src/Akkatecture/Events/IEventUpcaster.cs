using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Events
{
    public interface IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IAggregateEvent<TAggregate,TIdentity> Upcast(IAggregateEvent<TAggregate, TIdentity> aggregateEvent);
    }
}