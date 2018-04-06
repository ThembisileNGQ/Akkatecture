using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class PoisonTestAggregateCommand : Command<TestAggregate, TestAggregateId>
    {
        public PoisonTestAggregateCommand(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}