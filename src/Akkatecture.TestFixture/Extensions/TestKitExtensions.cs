using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.TestFixture.Aggregates;

namespace Akkatecture.TestFixture.Extensions
{
    public static class TestKitExtensions
    {
        public static IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregate, TIdentity>(
            this TestKitBase testKit, TIdentity aggregateId)
            where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(testKit).For(aggregateId);
        }
        
        public static IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregateManager, TAggregate, TIdentity>(
            this TestKitBase testKit, Expression<Func<TAggregateManager>> aggregateManagerFactory, TIdentity aggregateId)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(testKit).Using(aggregateManagerFactory,aggregateId);
        }
    }
}