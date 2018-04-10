using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Events
{
    public class TestSagaStartedEvent : AggregateEvent<TestSaga,TestSagaId>
    {
        public TestAggregateId Sender { get; }
        public TestAggregateId Receiver { get; }
        public Test SentTest { get; }

        public TestSagaStartedEvent(TestAggregateId sender, TestAggregateId receiver, Test sentTest)
        {
            Sender = sender;
            Receiver = receiver;
            SentTest = sentTest;
        }

    }
}
