using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events
{
    [EventVersion("TestTested", 1)]
    public class TestTestedEvent : AggregateEvent<TestAggregate, TestId>
    {
        public Test Test { get; }

        public TestTestedEvent(Test test)
        {
            Test = test;
        }
    }
}