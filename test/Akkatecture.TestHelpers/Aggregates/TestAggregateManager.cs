using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestAggregateManager : AggregateManager<TestAggregate, TestId, Command<TestAggregate, TestId>, TestState>
    {
        public TestAggregateManager()
        {
            
        }
    }
}