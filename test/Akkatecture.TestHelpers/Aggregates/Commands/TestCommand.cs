using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestCommand : Command<TestAggregate, TestId>
    {
        public TestCommand(TestId aggregateId)
            : base(aggregateId)
        {

        }
    }
}