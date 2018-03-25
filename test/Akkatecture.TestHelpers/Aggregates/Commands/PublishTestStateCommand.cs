using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class PublishTestStateCommand : Command<TestAggregate, TestId>
    {
        public PublishTestStateCommand(TestId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}