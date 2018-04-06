using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class CreateTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public CreateTestCommand(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}