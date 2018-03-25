using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestCreated", 1)]
    public class TestCreatedEvent : AggregateEvent<TestAggregate, TestId>
    {
        public TestId TestId { get; }

        public TestCreatedEvent(TestId testId)
        {
            TestId = testId;
        }
    }
}