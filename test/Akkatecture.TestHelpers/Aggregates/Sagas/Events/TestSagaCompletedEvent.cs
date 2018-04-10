using Akkatecture.Aggregates;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Events
{
    public class TestSagaCompletedEvent : AggregateEvent<TestSaga, TestSagaId>
    {
        public TestSagaState State { get; }

        public TestSagaCompletedEvent(TestSagaState state)
        {
            State = state;
        }
    }
}