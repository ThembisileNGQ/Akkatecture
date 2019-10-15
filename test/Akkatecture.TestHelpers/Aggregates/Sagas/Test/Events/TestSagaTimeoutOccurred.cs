using Akkatecture.Aggregates;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events
{
    public class TestSagaTimeoutOccurred : AggregateEvent<TestSaga, TestSagaId>
    {
        public string TimeoutMessage { get; }

        public TestSagaTimeoutOccurred(string timeoutMessage)
        {
            TimeoutMessage = timeoutMessage;
        }
    }
}