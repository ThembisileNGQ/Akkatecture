using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestDomainErrorCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestDomainErrorCommand(TestAggregateId aggregateId)
            : base(aggregateId)
        {

        }
        
    }
}