using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class PoisonTestAggregateCommand : Command<TestAggregate, TestId>
    {
        public PoisonTestAggregateCommand(TestId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}