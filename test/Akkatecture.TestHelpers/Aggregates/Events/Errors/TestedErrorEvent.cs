using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events.Errors
{
    [EventVersion("TestedError", 1)]
    public class TestedErrorEvent : AggregateEvent<TestAggregate, TestAggregateId>
    {
        public int TestErrors { get; }

        public TestedErrorEvent(int testErrors)
        {
            TestErrors = testErrors;
        }
    }
}