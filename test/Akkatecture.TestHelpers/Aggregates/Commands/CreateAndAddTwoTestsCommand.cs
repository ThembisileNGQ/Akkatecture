using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class CreateAndAddTwoTestsCommand: Command<TestAggregate, TestAggregateId>
    {
        public Test FirstTest { get; }
        public Test SecondTest { get; }
        
        public CreateAndAddTwoTestsCommand(
            TestAggregateId aggregateId,
            CommandId sourceId,
            Test firstTest,
            Test secondTest)
            : base(aggregateId, sourceId)
        {
            FirstTest = firstTest;
            SecondTest = secondTest;
        }
    }
}