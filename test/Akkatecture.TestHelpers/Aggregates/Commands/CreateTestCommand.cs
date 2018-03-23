using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class CreateTestCommand : Command<TestAggregate, TestId>
    {
        public CreateTestCommand(TestId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}