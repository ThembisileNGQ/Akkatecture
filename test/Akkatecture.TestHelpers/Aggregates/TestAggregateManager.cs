using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestAggregateManager : AggregateManager<TestAggregate, TestId, Command<TestAggregate, TestId>, TestState>
    {
        public TestAggregateManager()
        {
            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));

            Receive<DeadLetter>(
                x => x.Message is Command<TestAggregate,TestId>,
                x =>
                {
                    ReDispatch(x.Message as Command<TestAggregate, TestId>);
                });
            
        }
    }
}