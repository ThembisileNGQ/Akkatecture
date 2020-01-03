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

using System.ComponentModel;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTests : TestKit
    {
        private const string Category = "Aggregates";

        public AggregateTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "aggregate-tests", testOutputHelper)
        {
            
        }
        
        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            aggregateManager.Tell(command);
            
            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
                x => x.AggregateEvent.TestAggregateId.Equals(aggregateId) &&
                     x.Metadata.ContainsKey("some-key"));
        }
        
        
        [Fact]
        [Category(Category)]
        public async Task SendingCommand_ToAggregateRoot_ShouldReplyWithProperMessage()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            var result = await aggregateManager.Ask<TestExecutionResult>(command);

            result.Result.IsSuccess.Should().BeTrue();
            result.SourceId.Should().Be(command.SourceId);
        }

        [Fact]
        [Category(Category)]
        public void EventContainerMetadata_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            aggregateManager.Tell(command);

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
                x => x.AggregateIdentity.Equals(aggregateId)
                    && x.IdentityType == typeof(TestAggregateId)
                    && x.AggregateType == typeof(TestAggregate)
                    && x.EventType == typeof(TestCreatedEvent)
                    && x.Metadata.EventName == "TestCreated"
                    && x.Metadata.AggregateId == aggregateId.Value
                    && x.Metadata.EventVersion == 1
                    && x.Metadata.SourceId.Value == commandId.Value
                    && x.Metadata.AggregateSequenceNumber == 1);
        }

        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestStateSignalled()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            var nextCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 1
                     && x.AggregateEvent.Version == 1
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 0);
        }

        [Fact]
        [Category(Category)]
        public void TestCommand_AfterAggregateCreation_TestEventEmitted()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            var testId = TestId.New;
            var test = new Test(testId);
            var nextCommandId = CommandId.New;
            var nextCommand = new AddTestCommand(aggregateId, nextCommandId, test);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test));
        }

        [Fact]
        [Category(Category)]
        public void TestCommandTwice_AfterAggregateCreation_TestEventEmitted()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");


            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            var testId = TestId.New;
            var test = new Test(testId);
            var nextCommandId = CommandId.New;
            var nextCommand = new AddTestCommand(aggregateId, nextCommandId, test);
            var test2Id = TestId.New;
            var test2 = new Test(test2Id);
            var nextCommandId2 = CommandId.New;
            var nextCommand2 = new AddTestCommand(aggregateId, nextCommandId2, test2);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);
            aggregateManager.Tell(nextCommand2);


            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test)
                     && x.AggregateSequenceNumber == 2);

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test2)
                     && x.AggregateSequenceNumber == 3);
        }

        [Fact]
        [Category(Category)]
        public void TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            
            var command = new CreateTestCommand(aggregateId, commandId);
            aggregateManager.Tell(command);

            for (var i = 0; i < 5; i++)
            {
                var test = new Test(TestId.New);
                var testCommandId = CommandId.New;
                var testCommand = new AddTestCommand(aggregateId, testCommandId, test);
                aggregateManager.Tell(testCommand);
            }
            
            var poisonCommand = new PoisonTestAggregateCommand(aggregateId);
            aggregateManager.Tell(poisonCommand);

            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(reviveCommand);


            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 6
                     && x.AggregateEvent.Version == 6
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 5);
        }
        
        [Fact]
        [Category(Category)]
        public void TestEventMultipleEmitSourcing_AfterManyMultiCreateCommand_EventsEmitted()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var firstTest = new Test(TestId.New);
            var secondTest = new Test(TestId.New);
            var command = new CreateAndAddTwoTestsCommand(aggregateId, commandId, firstTest, secondTest);
            
            aggregateManager.Tell(command);

            eventProbe.ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>();
            eventProbe.ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();
            eventProbe.ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();

        }
        
        [Fact]
        [Category(Category)]
        public void TestEventMultipleEmitSourcing_AfterManyMultiCommand_TestStateSignalled()
        {
            
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;

            
            var command = new CreateTestCommand(aggregateId, commandId);
            aggregateManager.Tell(command);

            var test = new Test(TestId.New);
            var testSourceId = CommandId.New;
            var testCommand = new AddFourTestsCommand(aggregateId, testSourceId, test);
            aggregateManager.Tell(testCommand);
            
            var poisonCommand = new PoisonTestAggregateCommand(aggregateId);
            aggregateManager.Tell(poisonCommand);

            var reviveCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(reviveCommand);

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();
            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();
            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();
            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>();

            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 5
                     && x.AggregateEvent.Version == 5
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 4);

        }

        [Fact]
        [Category(Category)]
        public void TestSnapShotting_AfterManyTests_TestStateSignalled()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;

            
            var command = new CreateTestCommand(aggregateId, commandId);
            aggregateManager.Tell(command);

            for (var i = 0; i < 10; i++)
            {
                var test = new Test(TestId.New);
                var testCommandId = CommandId.New;
                var testCommand = new AddTestCommand(aggregateId, testCommandId, test);
                aggregateManager.Tell(testCommand);
            }
            
            eventProbe
                .ExpectMsg<IDomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 11
                     && x.AggregateEvent.Version == 11
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 10
                     && x.AggregateEvent.AggregateState.FromHydration);
        }
        
        
        [Fact]
        [Category(Category)]
        public async Task InitialState_TestingSuccessCommand_SuccessResultReplied()
        {
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new TestSuccessExecutionResultCommand(aggregateId, commandId);
            
            await aggregateManager.Ask<SuccessExecutionResult>(command);
        }
        
        [Fact]
        [Category(Category)]
        public async Task InitialState_TestingFailedCommand_SuccessResultReplied()
        {
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new TestFailedExecutionResultCommand(aggregateId, commandId);

            await aggregateManager.Ask<FailedExecutionResult>(command);
        }
        [Fact]
        [Category(Category)]
        public void TestDistinctCommand_AfterTwoHandles_CommandFails()
        {
            // TODO https://dev.azure.com/lutando/Akkatecture/_workitems/edit/25
           /* 
            var probe = CreateTestProbe("event-probe");
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;
            var createCommand = new CreateTestCommand(aggregateId);
            aggregateManager.Tell(createCommand);

            var command = new TestDistinctCommand(aggregateId, 10);

            aggregateManager.Tell(command, probe);

            probe.ExpectNoMsg();

            aggregateManager.Tell(command, probe);

            probe.ExpectMsg<FailedExecutionResult>(TimeSpan.FromHours(1));
            */
        }
    }
}