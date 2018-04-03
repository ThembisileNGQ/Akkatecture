using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Akka;
using Akkatecture.TestHelpers.Subscribers;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Subscribers
{
    [Collection("SubsriberTests")]
    public class SubscriberTests : TestKit
    {
        private const string Category = "Subscribers";

        public SubscriberTests()
            :base(Configuration.Config)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void Test()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(Handled<TestCreatedEvent>));
            var aggregateSubscriber = Sys.ActorOf(Props.Create(() => new TestAggregateSubscriber()), "test-subscriber");        
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            ExpectMsg<Handled<TestCreatedEvent>>(x =>
                x.AggregateEvent.TestId == command.AggregateId);


        }
    }
}