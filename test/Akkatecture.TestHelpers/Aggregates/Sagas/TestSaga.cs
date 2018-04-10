using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSaga : AggregateSaga<TestSaga,TestSagaId,TestSagaState>
    {
        private IActorRef TestAggregateManager { get; }
        public TestSaga(IActorRef testAggregateManager)
        {
            TestAggregateManager = testAggregateManager;

            //Saga "Commands"
            Command<DomainEvent<TestAggregate, TestAggregateId, TestSentEvent>>(Handle);
            Command<DomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent>>(Handle);
            Command<EmitTestSagaState>(Handle);

            //Recovery
            Recover<TestSagaCompletedEvent>(Recover);
            Recover<TestSagaTransactionCompletedEvent>(Recover);
            Recover<TestSagaCompletedEvent>(Recover);
        }

        private bool Handle(DomainEvent<TestAggregate, TestAggregateId, TestSentEvent> domainEvent)
        {
            if (IsNew)
            {
                var command = new ReceiveTestCommand(domainEvent.AggregateEvent.RecipientAggregateId,
                    domainEvent.AggregateIdentity, domainEvent.AggregateEvent.Test);

                Emit(new TestSagaStartedEvent(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.RecipientAggregateId, domainEvent.AggregateEvent.Test));

                TestAggregateManager.Tell(command);

                return true;
            }
            return false;
        }

        private bool Handle(DomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent> domainEvent)
        {
            if (!IsNew)
            {
                Emit(new TestSagaTransactionCompletedEvent());
                Self.Tell(new EmitTestSagaState());
                return true;
            }
            return false;
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
