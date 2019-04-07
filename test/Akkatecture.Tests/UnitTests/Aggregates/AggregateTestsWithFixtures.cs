using System;
using System.ComponentModel;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestFixtures.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Akkatecture.TestHelpers.Aggregates.Snapshots;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTestsWithFixtures : TestKit
    {
        private const string Category = "AggregatesWithFixtures";

        public AggregateTestsWithFixtures()
            : base(TestHelpers.Akka.Configuration.Config)
        {
            
        }
        
        [Fact]
        [Category(Category)]
        public void With_Test_Kit_InitialEvent_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var testId = TestId.New;
            
            fixture
                .For(aggregateId)
                .Given(new TestCreatedEvent(aggregateId), new TestAddedEvent(new Test(TestId.New)))
                .When(new AddTestCommand(aggregateId, new Test(testId)))
                .ThenExpect<TestAddedEvent>(x => x.Test.Id == testId);
        }
        
        [Fact]
        [Category(Category)]
        public void With_Test_Kit_InitialCommand_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var testId = TestId.New;
            
            fixture
                .For(aggregateId)
                .Given(new CreateTestCommand(aggregateId))
                .When(new AddTestCommand(aggregateId, new Test(testId)))
                .ThenExpect<TestAddedEvent>(x => x.Test.Id == testId);
        }
        
        [Fact]
        [Category(Category)]
        public void With_Test_Kit_InitialSnapshot_After_TestCreatedEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            
            fixture
                .For(aggregateId)
                .Given(new TestAggregateSnapshot(Enumerable.Range(0,10).Select(x => new TestAggregateSnapshot.TestModel(Guid.NewGuid())).ToList()), 10)
                .When(new PublishTestStateCommand(aggregateId))
                .ThenExpect<TestStateSignalEvent>(x => x.AggregateState.FromHydration);
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_InitialState_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId))
                .ThenExpect<TestCreatedEvent>(x => x.TestAggregateId == aggregateId);
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
                x => x.AggregateEvent.TestAggregateId.Equals(aggregateId));*/
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_EventContainerMetadata_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId))
                .ThenExpectDomainEvent<TestCreatedEvent>(
                    x => x.AggregateIdentity.Equals(aggregateId)
                         && x.IdentityType == typeof(TestAggregateId)
                         && x.AggregateType == typeof(TestAggregate)
                         && x.EventType == typeof(TestCreatedEvent)
                         && x.Metadata.EventName == "TestCreated"
                         && x.Metadata.AggregateId == aggregateId.Value
                         && x.Metadata.EventVersion == 1
                         && x.Metadata.AggregateSequenceNumber == 1);
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
                x => x.AggregateIdentity.Equals(aggregateId)
                    && x.IdentityType == typeof(TestAggregateId)
                    && x.AggregateType == typeof(TestAggregate)
                    && x.EventType == typeof(TestCreatedEvent)
                    && x.Metadata.EventName == "TestCreated"
                    && x.Metadata.AggregateId == aggregateId.Value
                    && x.Metadata.EventVersion == 1
                    && x.Metadata.AggregateSequenceNumber == 1);*/
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_InitialState_AfterAggregateCreation_TestStateSignalled()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId), new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 1
                         && x.AggregateEvent.Version == 1
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 0);
            
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId);
            var nextCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 1
                     && x.AggregateEvent.Version == 1
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 0);*/
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestCommand_AfterAggregateCreation_TestEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId), new AddTestCommand(aggregateId,test))
                .ThenExpect<TestAddedEvent>(x => x.Test.Equals(test));
            
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId);
            var testId = TestId.New;
            var test = new Test(testId);
            var nextCommand = new AddTestCommand(aggregateId,test);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test));*/
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestCommandTwice_AfterAggregateCreation_TestEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            var test2Id = TestId.New;
            var test2 = new Test(test2Id);
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId), new AddTestCommand(aggregateId,test),new AddTestCommand(aggregateId,test2))
                .ThenExpectDomainEvent<TestAddedEvent>(
                    x => x.AggregateEvent.Test.Equals(test)
                         && x.AggregateSequenceNumber == 2)
                .ThenExpectDomainEvent<TestAddedEvent>(
                    x => x.AggregateEvent.Test.Equals(test2)
                         && x.AggregateSequenceNumber == 3);
            
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");


            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId);
            var testId = TestId.New;
            var test = new Test(testId);
            var nextCommand = new AddTestCommand(aggregateId, test);
            var test2Id = TestId.New;
            var test2 = new Test(test2Id);
            var nextCommand2 = new AddTestCommand(aggregateId, test2);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);
            aggregateManager.Tell(nextCommand2);


            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test)
                     && x.AggregateSequenceNumber == 2);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test2)
                     && x.AggregateSequenceNumber == 3);*/
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            // this one needs aggregate manager
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var commands = Enumerable.Range(0, 5).Select(x => new AddTestCommand(aggregateId, new Test(TestId.New))).ToList();
            
            fixture
                .Using(() => new TestAggregateManager(), aggregateId)
                .Given(new TestCreatedEvent(aggregateId))
                .When(commands.ToArray())
                .AndWhen(new PoisonTestAggregateCommand(aggregateId))
                .AndWhen(new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 6
                         && x.AggregateEvent.Version == 6
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 5);
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            for (var i = 0; i < 5; i++)
            {
                var test = new Test(TestId.New);
                var testCommand = new AddTestCommand(aggregateId, test);
                aggregateManager.Tell(testCommand);
            }
            
            var poisonCommand = new PoisonTestAggregateCommand(aggregateId);
            aggregateManager.Tell(poisonCommand);

            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(reviveCommand);



            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 6
                     && x.AggregateEvent.Version == 6
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 5);*/

        }
        
        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestEventMultipleEmitSourcing_AfterManyMultiCommand_TestStateSignalled()
        {
            // this one needs aggregate manager START FROM HERE
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            
            fixture
                .Using(() => new TestAggregateManager(), aggregateId)
                .Given(new TestCreatedEvent(aggregateId))
                .When(new AddFourTestsCommand(aggregateId, new Test(TestId.New)))
                .AndWhen(new PoisonTestAggregateCommand(aggregateId))
                .AndWhen(new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 5
                         && x.AggregateEvent.Version == 5
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 4);
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            var test = new Test(TestId.New);
            var testCommand = new AddFourTestsCommand(aggregateId, test);
            aggregateManager.Tell(testCommand);
            
            var poisonCommand = new PoisonTestAggregateCommand(aggregateId);
            aggregateManager.Tell(poisonCommand);

            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(reviveCommand);


            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 5
                     && x.AggregateEvent.Version == 5
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 4);*/

        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestSnapShotting_AfterManyTests_TestStateSignalled()
        {
            // this one needs aggregate manager
            // this one needs aggregate manager
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var commands = Enumerable.Range(0, 10).Select(x => new AddTestCommand(aggregateId, new Test(TestId.New))).ToList();
            
            fixture
                .Using(() => new TestAggregateManager(), aggregateId)
                .Given(new TestCreatedEvent(aggregateId))
                .When(commands.ToArray())
                .AndWhen(new PoisonTestAggregateCommand(aggregateId))
                .AndWhen(new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 11
                         && x.AggregateEvent.Version == 11
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 10
                         && x.AggregateEvent.AggregateState.FromHydration);
            /*var probe = CreateTestActor("probe");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(command);

            for (var i = 0; i < 10; i++)
            {
                var test = new Test(TestId.New);
                var testCommand = new AddTestCommand(aggregateId, test);
                aggregateManager.Tell(testCommand);
            }
            

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 11
                     && x.AggregateEvent.Version == 11
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 10
                     && x.AggregateEvent.AggregateState.FromHydration);*/
        }
    }
}