using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestState : AggregateState<TestAggregate, TestId, IEventApplier<TestAggregate, TestId>>,
        IApply<TestTestedEvent>,
        IApply<TestCreatedEvent>
    {
        public Test Test { get; set; }
        
        public void Apply(TestCreatedEvent aggregateEvent)
        {
            Test = new Test(aggregateEvent.TestId);
        }
        
        public void Apply(TestTestedEvent aggregateEvent)
        {
            Test.SetTestsDone(aggregateEvent.Tests);
        }
    }
}