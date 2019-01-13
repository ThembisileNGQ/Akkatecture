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

using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Sagas;
using Akkatecture.TestHelpers.Aggregates.Sagas.Events;
using Xunit;

namespace Akkatecture.Tests.IntegrationTests.Aggregates.Sagas
{
    [Collection("AggregateSagaTests")]
    public class AggregateSagaTests : TestKit
    {
        private const string Category = "Sagas";
        public AggregateSagaTests()
            : base(TestHelpers.Akka.Configuration.Config)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSaga()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            Sys.ActorOf(Props.Create(() => new TestSagaManager(() => new TestSaga(aggregateManager))), "test-sagaaggregatemanager");
            
            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId, probe);
            aggregateManager.Tell(senderCreateAggregateCommand);

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId, probe);
            aggregateManager.Tell(receiverCreateAggregateCommand);

            var senderTestId = TestId.New;
            var senderTest = new Test(senderTestId);
            var nextAggregateCommand = new AddTestCommand(senderAggregateId, senderTest);
            aggregateManager.Tell(nextAggregateCommand);



            var sagaStartingCommand = new GiveTestCommand(senderAggregateId,receiverAggregateId,senderTest);
            aggregateManager.Tell(sagaStartingCommand);
            


            ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>>(
                x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                    && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                    && x.AggregateEvent.SentTest.Equals(senderTest));
        }
    }
}
