using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestFailedExecutionResultCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestFailedExecutionResultCommand(
            TestAggregateId aggregateId,
            CommandId sourceId)
            : base(aggregateId, sourceId)
        {
        }
    }
}