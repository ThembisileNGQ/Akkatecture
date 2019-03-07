using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class AddFourTestsCommand: Command<TestAggregate, TestAggregateId>
    {
        public Test Test { get; }
        public AddFourTestsCommand(
            TestAggregateId aggregateId,
            Test test)
            : base(aggregateId)
        {
            Test = test;
        }
    }
}