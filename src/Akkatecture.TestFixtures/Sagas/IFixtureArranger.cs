using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.TestFixtures.Aggregates;

namespace Akkatecture.TestFixtures.Sagas
{
    public interface IFixtureArranger<TAggregate, TIdentity, TSagaLocator>
        where TAggregate : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : ISagaLocator<TIdentity>
    {
        IFixtureArranger<TAggregate, TIdentity> For(TIdentity aggregateId);
        
        IFixtureExecutor<TAggregate, TIdentity> GivenNothing();
        
        IFixtureExecutor<TAggregate, TIdentity> Given<TAggregateEvent>(params TAggregateEvent[] aggregateEvents) 
            where TAggregateEvent : IAggregateEvent<TAggregate,TIdentity>;
        
        IFixtureExecutor<TAggregate, TIdentity> GivenCommands<TCommand>(params TCommand[] commands)
            where TCommand : ICommand<TAggregate,TIdentity>;
    }
}