using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class AddTestCommand : Command<TestAggregate, TestAggregateId>
    {
        public Test Test { get; }
        public AddTestCommand(TestAggregateId aggregateId, Test test)
            : base(aggregateId)
        {
            Test = test;
        }
    }
}