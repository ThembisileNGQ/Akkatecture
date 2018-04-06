using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestCreated", 1)]
    public class TestCreatedEvent : AggregateEvent<TestAggregate, TestAggregateId>
    {
        public TestAggregateId TestAggregateId { get; }

        public TestCreatedEvent(TestAggregateId testAggregateId)
        {
            TestAggregateId = testAggregateId;
        }
    }
}