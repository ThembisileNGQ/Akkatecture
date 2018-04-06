using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class ReceiveTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public TestAggregateId ReceiverId { get; }
        public Test TestToReceive { get; }

        public ReceiveTestCommand(TestAggregateId aggregateId, TestAggregateId receiverId, Test testToReceive)
            : base(aggregateId)
        {
            ReceiverId = receiverId;
            TestToReceive = testToReceive;
        }
    }
}
