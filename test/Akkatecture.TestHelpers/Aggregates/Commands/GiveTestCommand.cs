using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class GiveTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestAggregateId ReceiverAggregateId { get; } 
        public Test TestToGive { get; }
        
        public GiveTestCommand(TestAggregateId aggregateId, TestAggregateId receiverAggregateId, Test testToGive)
            : base(aggregateId)
        {
            TestToGive = testToGive;
            ReceiverAggregateId = receiverAggregateId;
        }
        
    }
}