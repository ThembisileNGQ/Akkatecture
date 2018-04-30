using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSaga : AggregateSaga<TestSaga,TestSagaId,TestSagaState>,
        ISagaIsStartedBy<TestAggregate, TestAggregateId, TestSentEvent>,
        ISagaHandles<TestAggregate, TestAggregateId, TestReceivedEvent>
    {
        private IActorRef TestAggregateManager { get; }
        public TestSaga(IActorRef testAggregateManager)
        {
            TestAggregateManager = testAggregateManager;
            
            //Test Probe Command
            Command<EmitTestSagaState>(Handle);
        }

        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestSentEvent> domainEvent)
        {
            if (IsNew)
            {
                var command = new ReceiveTestCommand(domainEvent.AggregateEvent.RecipientAggregateId,
                    domainEvent.AggregateIdentity, domainEvent.AggregateEvent.Test);

                Emit(new TestSagaStartedEvent(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.RecipientAggregateId, domainEvent.AggregateEvent.Test));

                TestAggregateManager.Tell(command);

            }
            return Task.CompletedTask;
        }

        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent> domainEvent)
        {
            if (!IsNew)
            {
                Emit(new TestSagaTransactionCompletedEvent());
                Self.Tell(new EmitTestSagaState());

            }
            return Task.CompletedTask;
        }

        private bool Handle(EmitTestSagaState testCommmand)
        {
            Emit(new TestSagaCompletedEvent(State));
            return true;
        }

        private class EmitTestSagaState
        {
        }
    }
}
