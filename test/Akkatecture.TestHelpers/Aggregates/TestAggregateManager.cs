using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestAggregateManager : AggregateManager<TestAggregate, TestAggregateId, Command<TestAggregate, TestAggregateId>, TestState>
    {
        
    }
}