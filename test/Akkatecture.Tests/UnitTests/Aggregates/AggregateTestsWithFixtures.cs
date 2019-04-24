using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Akka.TestKit.Xunit2;
using Akkatecture.Commands;
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
            : base(TestHelpers.Akka.Configuration.Config, "aggregate-fixture-tests")
        {
            
        }
        
        //TODO API idea ; var fixture = this.FixtureFor<TestAggregate, TestAggregateId>
        
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
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;var commands = new List<ICommand<TestAggregate, TestAggregateId>>();
            commands.AddRange(Enumerable.Range(0, 5).Select(x => new AddTestCommand(aggregateId, new Test(TestId.New))));
            
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
        }
        
        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestEventMultipleEmitSourcing_AfterManyMultiCommand_TestStateSignalled()
        {
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
        }

        [Fact]
        [Category(Category)]
        public void With_Test_Kit_TestSnapShotting_AfterManyTests_TestStateSignalled()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var commands = new List<ICommand<TestAggregate, TestAggregateId>>();
            commands.AddRange(Enumerable.Range(0, 10).Select(x => new AddTestCommand(aggregateId, new Test(TestId.New))));
            
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
        }
    }
}