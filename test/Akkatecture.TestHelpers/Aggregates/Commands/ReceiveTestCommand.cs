using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class ReceiveTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestAggregateId SenderAggregateId { get; }
        public Test TestToReceive { get; }

        public ReceiveTestCommand(TestAggregateId aggregateId, TestAggregateId senderAggregateId, Test testToReceive)
            : base(aggregateId)
        {
            SenderAggregateId = senderAggregateId;
            TestToReceive = testToReceive;
        }
    }
}
