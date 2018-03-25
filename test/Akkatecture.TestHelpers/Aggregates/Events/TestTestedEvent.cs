using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestTested", 1)]
    public class TestTestedEvent : AggregateEvent<TestAggregate, TestId>
    {
        public int Tests { get; }
        
        public TestTestedEvent(int tests)
        {
            Tests = tests;
        }
    }
}