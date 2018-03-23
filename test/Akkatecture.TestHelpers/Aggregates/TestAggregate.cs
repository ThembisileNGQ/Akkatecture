using Akkatecture.Aggregates;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestId, TestState>
    {
        public TestAggregate(TestId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}