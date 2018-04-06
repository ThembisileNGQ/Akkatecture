using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class PublishTestStateCommand : Command<TestAggregate, TestAggregateId>
    {
        public PublishTestStateCommand(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            
        }
    }
}