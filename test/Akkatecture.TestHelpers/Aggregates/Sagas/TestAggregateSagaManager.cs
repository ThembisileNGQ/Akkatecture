using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestAggregateSagaManager : AggregateSagaManager<TestSaga,TestSagaId,TestSagaLocator>,
        ISagaIsStartedBy<TestAggregate,TestAggregateId,TestSentEvent>,
        ISagaHandles<TestAggregate, TestAggregateId, TestReceivedEvent>
    {
        public TestAggregateSagaManager(Expression<Func<TestSaga>> sagaFactory)
            : base(sagaFactory)
        {
            //ReceiveAsync<IDomainEvent<TestAggregate, TestAggregateId, TestSentEvent>>(Handle);
            //ReceiveAsync<IDomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent>>(Handle);
        }

        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestSentEvent> domainEvent)
        {
            var sagaId = SagaLocator.LocateSaga(domainEvent);
            var saga = FindOrSpawn(sagaId);
            saga.Tell(domainEvent,Sender);
            return Task.CompletedTask;
        }

        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent> domainEvent)
        {
            var sagaId = SagaLocator.LocateSaga(domainEvent);
            var saga = FindOrSpawn(sagaId);
            saga.Tell(domainEvent, Sender);
            return Task.CompletedTask;
        }
    }
}