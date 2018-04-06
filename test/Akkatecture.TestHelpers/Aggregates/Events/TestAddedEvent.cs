using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestTested", 1)]
    public class TestAddedEvent : AggregateEvent<TestAggregate, TestAggregateId>
    {
        public Test Test { get; }
        
        public TestAddedEvent(Test test)
        {
            Test = test;
        }
    }
}