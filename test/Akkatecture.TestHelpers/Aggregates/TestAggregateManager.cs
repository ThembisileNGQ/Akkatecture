using System.Runtime.CompilerServices;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates.Commands;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestAggregateManager : AggregateManager<TestAggregate, TestId, Command<TestAggregate, TestId>, TestState>
    {
        public TestAggregateManager()
            : base()
        {
            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));

            Receive<DeadLetter>(
                x => x.Message is Command<TestAggregate,TestId>,
                x =>
                {
                    ReDispatch(x.Message as Command<TestAggregate, TestId>);
                });
            
            /*Receive<DeadLetter>(deadLetter =>
            {
                switch (deadLetter.Message)
                {
                     case Command<TestAggregate, TestId> command:
                         ReDispatch(command);
                         break;
                         
                }
            });*/
            
        }
    }
}