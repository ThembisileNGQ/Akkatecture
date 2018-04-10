using System;
using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Sagas;
using Akkatecture.TestHelpers.Aggregates.Sagas.Events;
using Akkatecture.TestHelpers.Akka;
using Xunit;

namespace Akkatecture.Tests.IntegrationTests.Aggregates.Sagas
{
    [Collection("AggregateSagaTests")]
    public class AggregateSagaTests : TestKit
    {
        private const string Category = "Sagas";
        public AggregateSagaTests()
            : base(Configuration.Config)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSaga()
        {
            var probe = CreateTestActor("probeActor");
            Sys.EventStream.Subscribe(probe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>));
            var aggregateManager = Sys.ActorOf(Props.Create(() => new TestAggregateManager()), "test-aggregatemanager");
            var aggregateSagaManager = Sys.ActorOf(Props.Create(() => new TestSagaManager(() => new TestSaga(aggregateManager))), "test-sagaaggregatemanager");



            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId);
            aggregateManager.Tell(senderCreateAggregateCommand);

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId);
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
                    && x.AggregateEvent.SentTest.Equals(senderTest), TimeSpan.FromMinutes(1));

        }
    }
}
