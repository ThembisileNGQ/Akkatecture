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
using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.TestAsync;
using Akkatecture.TestHelpers.Aggregates.Sagas.TestAsync.Events;
using Xunit;
using Xunit.Abstractions;

namespace Akkatecture.Tests.IntegrationTests.Aggregates.Sagas
{
    [Collection("AggregateSagaTests")]
    public class AggregateSagaTests : TestKit
    {
        private const string Category = "Sagas";
        public AggregateSagaTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "aggregate-saga-tests", testOutputHelper)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSaga()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaTimeoutOccurred>));
            
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            Sys.ActorOf(Props.Create(() => new TestSagaManager(() => new TestSaga(aggregateManager))), "test-sagaaggregatemanager");
            
            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId, CommandId.New);
            aggregateManager.Tell(senderCreateAggregateCommand);

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId, CommandId.New);
            aggregateManager.Tell(receiverCreateAggregateCommand);

            var senderTestId = TestId.New;
            var senderTest = new Test(senderTestId);
            var nextAggregateCommand = new AddTestCommand(senderAggregateId, CommandId.New, senderTest);
            aggregateManager.Tell(nextAggregateCommand);

            var sagaStartingCommand = new GiveTestCommand(senderAggregateId, CommandId.New,receiverAggregateId,senderTest);
            aggregateManager.Tell(sagaStartingCommand);
            
            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>>(
                    x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                         && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                         && x.AggregateEvent.SentTest.Equals(senderTest),new TimeSpan(0,0,20));
            
            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>>(new TimeSpan(0,0,20));
            
            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>>(new TimeSpan(0,0,20));
                        
            eventProbe.ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaTimeoutOccurred>>(
                timeoutMsg => timeoutMsg.AggregateEvent.TimeoutMessage.Equals("First timeout test"),
                TimeSpan.FromSeconds(15));
            
            eventProbe.ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaTimeoutOccurred>>(
                timeoutMsg => timeoutMsg.AggregateEvent.TimeoutMessage.StartsWith("Second timeout test"),
                TimeSpan.FromSeconds(15));
        }
        
        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSagaAsync()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaStartedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaTransactionCompletedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            Sys.ActorOf(Props.Create(() => new TestAsyncSagaManager(() => new TestAsyncSaga(aggregateManager))), "test-sagaaggregatemanager");
            
            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId, CommandId.New);
            aggregateManager.Tell(senderCreateAggregateCommand);

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId, CommandId.New);
            aggregateManager.Tell(receiverCreateAggregateCommand);

            var senderTestId = TestId.New;
            var senderTest = new Test(senderTestId);
            var nextAggregateCommand = new AddTestCommand(senderAggregateId, CommandId.New, senderTest);
            aggregateManager.Tell(nextAggregateCommand);

            var sagaStartingCommand = new GiveTestCommand(senderAggregateId, CommandId.New,receiverAggregateId,senderTest);
            aggregateManager.Tell(sagaStartingCommand);

            eventProbe.
                ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaStartedEvent>>(
                    x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                         && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                         && x.AggregateEvent.SentTest.Equals(senderTest)
                         && x.Metadata.ContainsKey("some-key"));
            
            eventProbe.ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaTransactionCompletedEvent>>();
            
            eventProbe.ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaCompletedEvent>>();

        }
    }
}
