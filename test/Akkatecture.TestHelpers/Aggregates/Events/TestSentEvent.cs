using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestSent", 1)]
    public class TestSentEvent : AggregateEvent<TestAggregate, TestAggregateId>
    {
        public Test Test { get; }
        public TestAggregateId RecipientAggregateId { get; }
        public TestSentEvent(Test test, TestAggregateId recipientAggregateId)
        {
            Test = test;
            RecipientAggregateId = recipientAggregateId;
        }
    }
}
