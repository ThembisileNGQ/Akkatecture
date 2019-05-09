using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestSuccessExecutionResultCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestSuccessExecutionResultCommand(
            TestAggregateId aggregateId,
            CommandId sourceId)
            : base(aggregateId, sourceId)
        {
        }
    }
}