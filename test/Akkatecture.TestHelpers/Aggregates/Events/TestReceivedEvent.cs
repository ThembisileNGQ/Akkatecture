using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestReceived", 1)]
    public class TestReceivedEvent : AggregateEvent<TestAggregate, TestAggregateId>
    {
        public Test Test { get; }
        public TestAggregateId SenderAggregateId { get; }
        public TestReceivedEvent(TestAggregateId senderAggregateId, Test test)
        {
            Test = test;
            SenderAggregateId = senderAggregateId;
        }
    }
}