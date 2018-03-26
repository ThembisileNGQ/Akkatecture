using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTests : TestKit
    {
        private const string Category = "Aggregates";

        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestCreatedEvent>>(
                x => x.AggregateEvent.TestId.Equals(aggregateId));
        }

        [Fact]
        [Category(Category)]
        public void EventContainerMetadata_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestCreatedEvent>>(
                x => x.AggregateIdentity.Equals(aggregateId)
                    && x.IdentityType == typeof(TestId)
                    && x.AggregateType == typeof(TestAggregate)
                    && x.EventType == typeof(TestCreatedEvent)
                    //&& x.Metadata.EventName == "TestCreated"
                    && x.Metadata.AggregateId == aggregateId.Value
                    //&& x.Metadata.EventVersion == 1
                    && x.Metadata.AggregateSequenceNumber == 1);
        }

        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestStateSignalled()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            var nextCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 1
                     && x.AggregateEvent.Version == 1
                     && x.AggregateEvent.State.Test.Id.Equals(aggregateId)
                     && x.AggregateEvent.State.Test.TestsDone == 0);
        }


    }
}