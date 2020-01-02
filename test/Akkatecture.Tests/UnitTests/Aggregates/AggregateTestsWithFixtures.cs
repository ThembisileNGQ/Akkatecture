// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Akka.TestKit.Xunit2;
using Akkatecture.Commands;
using Akkatecture.TestFixture.Aggregates;
using Akkatecture.TestFixture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Akkatecture.TestHelpers.Aggregates.Snapshots;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTestsWithFixtures : TestKit
    {
        private const string Category = "AggregatesWithFixtures";

        public AggregateTestsWithFixtures(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "aggregate-fixture-tests", testOutputHelper)
        {
            
        }
        
        [Fact]
        [Category(Category)]
        public void InitialEvent_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var testId = TestId.New;

            this.FixtureFor<TestAggregate, TestAggregateId>(aggregateId)
                .Given(new TestCreatedEvent(aggregateId), new TestAddedEvent(new Test(TestId.New)))
                .When(new AddTestCommand(aggregateId, commandId, new Test(testId)))
                .ThenExpect<TestAddedEvent>(x => x.Test.Id == testId)
                .ThenExpectReply<TestExecutionResult>(x => x.SourceId.Value == commandId.Value && x.Result.IsSuccess);


        }
        
        [Fact]
        [Category(Category)]
        public void InitialEvent_AfterAggregateCreationWithNoTestPredicate_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var testId = TestId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .Given(new TestCreatedEvent(aggregateId), new TestAddedEvent(new Test(TestId.New)))
                .When(new AddTestCommand(aggregateId, commandId, new Test(testId)))
                .ThenExpect<TestAddedEvent>();
        }
        
        [Fact]
        [Category(Category)]
        public void InitialEvent_AfterAggregateCreation_TestCreatedDomainEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var testId = TestId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .Given(new TestCreatedEvent(aggregateId), new TestAddedEvent(new Test(TestId.New)))
                .When(new AddTestCommand(aggregateId, commandId, new Test(testId)))
                .ThenExpectDomainEvent<TestAddedEvent>();
        }
        
        [Fact]
        [Category(Category)]
        public void InitialCommand_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var testId = TestId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .Given(new CreateTestCommand(aggregateId, commandId))
                .When(new AddTestCommand(aggregateId, CommandId.New, new Test(testId)))
                .ThenExpect<TestAddedEvent>(x => x.Test.Id == testId);
        }
        
        [Fact]
        [Category(Category)]
        public void NullInitialCommand_AfterAggregateCreation_ExceptionThrown()
        {
            var aggregateId = TestAggregateId.New;

            this.Invoking(test =>
                    this.FixtureFor<TestAggregate, TestAggregateId>(aggregateId)
                        .Given(commands: null))
                .Should().Throw<ArgumentNullException>(); 
        }
        
        [Fact]
        [Category(Category)]
        public void NullInInitialCommands_AfterAggregateCreation_ExceptionThrown()
        {
            var aggregateId = TestAggregateId.New;
            var nullCommand = (ICommand<TestAggregate, TestAggregateId>)null;
            var commands = new List<ICommand<TestAggregate, TestAggregateId>> {nullCommand};

            this.Invoking(test =>
                    this.FixtureFor<TestAggregate, TestAggregateId>(aggregateId)
                        .Given(commands.ToArray()))
                .Should().Throw<NullReferenceException>(); 
        }
        
        [Fact]
        [Category(Category)]
        public void InitialSnapshot_After_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .Given(new TestAggregateSnapshot(Enumerable.Range(0,10).Select(x => new TestAggregateSnapshot.TestModel(Guid.NewGuid())).ToList()), 10)
                .When(new PublishTestStateCommand(aggregateId))
                .ThenExpect<TestStateSignalEvent>(x => x.AggregateState.FromHydration);
        }

        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId, commandId))
                .ThenExpect<TestCreatedEvent>(x => x.TestAggregateId == aggregateId);
        }

        [Fact]
        [Category(Category)]
        public void EventContainerMetadata_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId, commandId))
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
        public void InitialState_AfterAggregateCreation_TestStateSignalled()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId, commandId), new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 1
                         && x.AggregateEvent.Version == 1
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 0);
        }

        [Fact]
        [Category(Category)]
        public void TestCommand_AfterAggregateCreation_TestEventEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var commandId2 = CommandId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            
            this.FixtureFor<TestAggregate,TestAggregateId>(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId, commandId), new AddTestCommand(aggregateId, commandId2 ,test))
                .ThenExpect<TestAddedEvent>(x => x.Test.Equals(test));
        }

        [Fact]
        [Category(Category)]
        public void TestCommandTwice_AfterAggregateCreation_TestEventEmitted()
        {
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(this);
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var commandId2 = CommandId.New;
            var commandId3 = CommandId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            var test2Id = TestId.New;
            var test2 = new Test(test2Id);
            
            fixture
                .For(aggregateId)
                .GivenNothing()
                .When(new CreateTestCommand(aggregateId, commandId), new AddTestCommand(aggregateId, commandId2,test),new AddTestCommand(aggregateId, commandId3,test2))
                .ThenExpectDomainEvent<TestAddedEvent>(
                    x => x.AggregateEvent.Test.Equals(test)
                         && x.AggregateSequenceNumber == 2)
                .ThenExpectDomainEvent<TestAddedEvent>(
                    x => x.AggregateEvent.Test.Equals(test2)
                         && x.AggregateSequenceNumber == 3);
        }

        [Fact]
        [Category(Category)]
        public void TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            var aggregateId = TestAggregateId.New;
            var commands = new List<ICommand<TestAggregate, TestAggregateId>>();
            commands.AddRange(Enumerable.Range(0, 5).Select(x => new AddTestCommand(aggregateId, CommandId.New, new Test(TestId.New))));
            
            this.FixtureFor<TestAggregateManager,TestAggregate, TestAggregateId>(() => new TestAggregateManager(), aggregateId)
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
        public void TestEventMultipleEmitSourcing_AfterManyMultiCreateCommand_EventsEmitted()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var firstTest = new Test(TestId.New);
            var secondTest = new Test(TestId.New);

            this.FixtureFor<TestAggregate, TestAggregateId>(aggregateId)
                .GivenNothing()
                .When(new CreateAndAddTwoTestsCommand(aggregateId, commandId, firstTest, secondTest))
                .ThenExpectDomainEvent<TestCreatedEvent>()
                .ThenExpect<TestAddedEvent>()
                .ThenExpect<TestAddedEvent>();
        }
        
        [Fact]
        [Category(Category)]
        public void TestEventMultipleEmitSourcing_AfterManyMultiCommand_TestStateSignalled()
        {
            var aggregateId = TestAggregateId.New;
            
            this.FixtureFor<TestAggregateManager,TestAggregate, TestAggregateId>(() => new TestAggregateManager(), aggregateId)
                .Given(new TestCreatedEvent(aggregateId))
                .When(new AddFourTestsCommand(aggregateId, CommandId.New, new Test(TestId.New)))
                .AndWhen(new PoisonTestAggregateCommand(aggregateId))
                .AndWhen(new PublishTestStateCommand(aggregateId))
                .ThenExpectDomainEvent<TestStateSignalEvent>(
                    x => x.AggregateEvent.LastSequenceNr == 5
                         && x.AggregateEvent.Version == 5
                         && x.AggregateEvent.AggregateState.TestCollection.Count == 4);
        }

        [Fact]
        [Category(Category)]
        public void TestSnapShotting_AfterManyTests_TestStateSignalled()
        {
            var aggregateId = TestAggregateId.New;
            var commands = new List<ICommand<TestAggregate, TestAggregateId>>();
            commands.AddRange(Enumerable.Range(0, 10).Select(x => new AddTestCommand(aggregateId, CommandId.New, new Test(TestId.New))));
            
            this.FixtureFor<TestAggregateManager,TestAggregate, TestAggregateId>(() => new TestAggregateManager(), aggregateId)
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