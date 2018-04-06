using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class GiveTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestAggregateId ReceiverId { get; } 
        public Test TestToGive { get; }
        
        public GiveTestCommand(TestAggregateId aggregateId, TestAggregateId receiverId, Test testToGive)
            : base(aggregateId)
        {
            TestToGive = testToGive;
            ReceiverId = receiverId;
        }
        
    }
}