using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestState : AggregateState<TestAggregate, TestId, IEventApplier<TestAggregate, TestId>>,
        IApply<TestTestedEvent>
    {
        public void Apply(TestTestedEvent aggregateEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}