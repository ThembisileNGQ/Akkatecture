// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
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
using System.ComponentModel;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Aggregates
{
    [Collection("AggregateTests")]
    public class AggregateTests : TestKit
    {
        private const string Category = "Aggregates";

        public AggregateTests()
            : base(TestHelpers.Akka.Configuration.Config)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void InitialState_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            
            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
                x => x.AggregateEvent.TestAggregateId.Equals(aggregateId));
        }

        [Fact]
        [Category(Category)]
        public void EventContainerMetadata_AfterAggregateCreation_TestCreatedEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
            aggregateManager.Tell(command);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(
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
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
            var nextCommand = new PublishTestStateCommand(aggregateId);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>>(
                x => x.AggregateEvent.LastSequenceNr == 1
                     && x.AggregateEvent.Version == 1
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 0);
        }

        [Fact]
        [Category(Category)]
        public void TestCommand_AfterAggregateCreation_TestEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");

            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
            var testId = TestId.New;
            var test = new Test(testId);
            var nextCommand = new AddTestCommand(aggregateId,test);
            aggregateManager.Tell(command);
            aggregateManager.Tell(nextCommand);

            ExpectMsg<DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>>(
                x => x.AggregateEvent.Test.Equals(test));
        }

        [Fact]
        [Category(Category)]
        public void TestCommandTwice_AfterAggregateCreation_TestEventEmitted()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");


            var aggregateId = TestAggregateId.New;
            var command = new CreateTestCommand(aggregateId, probe);
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
                     && x.AggregateSequenceNumber == 3);
        }

        [Fact]
        [Category(Category)]
        public void TestEventSourcing_AfterManyTests_TestStateSignalled()
        {
            
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId, probe);
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
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 5);

        }
        
        [Fact]
        [Category(Category)]
        public void TestEventMultipleEmitSourcing_AfterManyMultiCommand_TestStateSignalled()
        {
            
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId, probe);
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
                     && x.AggregateEvent.AggregateState.TestCollection.Count == 4);

        }

        [Fact]
        [Category(Category)]
        public void TestSnapShotting_AfterManyTests_TestStateSignalled()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestAggregate, TestAggregateId, TestStateSignalEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateId = TestAggregateId.New;

            
            var command = new CreateTestCommand(aggregateId, probe);
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
                     && x.AggregateEvent.AggregateState.FromHydration);
        }
    }
}