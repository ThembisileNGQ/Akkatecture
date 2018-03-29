using System;
using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Akkatecture.TestHelpers.Akka;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTests : TestKit
    {
        private const string Category = "Aggregates";

        public AggregateTests()
            //: base(Configuration.Config)
        {
            
        }

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

        [Fact]
        [Category(Category)]
        public void TestCommand_AfterAggregateCreation_TestEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestTestedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            var nextCommand = new TestCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestTestedEvent>>(
                x => x.AggregateEvent.Tests == 1);
        }

        [Fact]
        [Category(Category)]
        public void TestCommandTwice_AfterAggregateCreation_TestEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestTestedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestId.New;
            var command = new CreateTestCommand(aggregateId);
            var nextCommand = new TestCommand(aggregateId);
            var nextCommand2 = new TestCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);
            aggregateManager.Tell(nextCommand2);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestTestedEvent>>(
                x => x.AggregateEvent.Tests == 1
                     && x.AggregateSequenceNumber == 2);

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestTestedEvent>>(
                x => x.AggregateEvent.Tests == 2
                     && x.AggregateSequenceNumber == 3);
        }

        [Fact]
        [Category(Category)]
        public void TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestId.New;

            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            for (var i = 0; i < 5; i++)
            {
                var testCommand = new TestCommand(aggregateId);
                aggregateManager.Tell(testCommand);
            }
            /*var poisonCommand = new PoisonTestAggregateCommand(aggregateId);
            aggregateManager.Tell(poisonCommand);*/

            aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager-two");
            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(reviveCommand);



            ExpectMsg<DomainEvent<TestAggregate, TestId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr > 4
                     && x.AggregateEvent.Version > 4
                     && x.AggregateEvent.State.Test.Id.Equals(aggregateId)
                     && x.AggregateEvent.State.Test.TestsDone > 4, TimeSpan.FromMinutes(2));

            //ExpectMsg<DomainEvent<TestAggregate, TestId, TestStateSignalEvent>>(TimeSpan.FromMinutes(2));
        }

        /*[Fact]
        [Category(Category)]
        public void TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            var probe = CreateTestActor("probeActor");

            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestId, TestStateSignalEvent>));
            var aggregateId = TestId.New;
            var aggregateRoot = Sys.ActorOf(Props.Create(() => new TestAggregate(aggregateId)), "test-aggregate");
            

            var command = new CreateTestCommand(aggregateId);
            aggregateRoot.Tell(command);

            for (var i = 0; i < 5; i++)
            {
                var testCommand = new TestCommand(aggregateId);
                aggregateRoot.Tell(testCommand);
            }


            aggregateRoot.Tell(PoisonPill.Instance);
            aggregateRoot = Sys.ActorOf(Props.Create(() => new TestAggregate(aggregateId)), "test-aggregate2");
            
            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateRoot.Tell(reviveCommand);



            /*ExpectMsg<DomainEvent<TestAggregate, TestId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr > 4
                     && x.AggregateEvent.Version > 4
                     && x.AggregateEvent.State.Test.Id.Equals(aggregateId)
                     && x.AggregateEvent.State.Test.TestsDone > 4, TimeSpan.FromMinutes(2));#1#

            ExpectMsg<DomainEvent<TestAggregate, TestId, TestStateSignalEvent>>(TimeSpan.FromMinutes(2));
            
        }*/


    }
}