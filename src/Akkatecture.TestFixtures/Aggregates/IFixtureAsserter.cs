using System;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.TestFixtures.Aggregates
{
    public interface IFixtureAsserter<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>;
    }
}