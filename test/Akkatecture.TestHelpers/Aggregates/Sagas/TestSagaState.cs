using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Sagas.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaState : SagaState<TestSaga, TestSagaId, IEventApplier<TestSaga, TestSagaId>>,
        IApply<TestSagaStartedEvent>,
        IApply<TestSagaTransactionCompletedEvent>,
        IApply<TestSagaCompletedEvent>
    {
        public TestAggregateId Sender { get; set; }
        public TestAggregateId Receiver { get; set; }
        public Test Test { get; set; }
        public void Apply(TestSagaStartedEvent aggregateEvent)
        {
            Sender = aggregateEvent.Sender;
            Receiver = aggregateEvent.Receiver;
            Test = aggregateEvent.SentTest;
            Start();
        }

        public void Apply(TestSagaTransactionCompletedEvent aggregateEvent)
        {
            Complete();
        }

        public void Apply(TestSagaCompletedEvent aggregateEvent)
        {
            //do nothing
        }
    }
}