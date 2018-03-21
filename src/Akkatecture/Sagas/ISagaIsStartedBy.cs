using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public interface ISagaIsStartedBy<TAggregate, in TIdentity, in TAggregateEvent> : ISagaHandles<TAggregate, TIdentity, TAggregateEvent>
        where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}