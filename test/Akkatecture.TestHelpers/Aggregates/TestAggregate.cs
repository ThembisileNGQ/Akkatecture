using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Errors;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestAggregateId, TestState>
    {
        public int TestErrors { get; private set; }
        public TestAggregate(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            TestErrors = 0;


            //Aggregate Commands
            Command<CreateTestCommand>(Execute);
            Command<AddTestCommand>(Execute);
            Command<GiveTestCommand>(Execute);
            Command<ReceiveTestCommand>(Execute);

            //Aggregate Test Commands
            Command<PoisonTestAggregateCommand>(Execute);
            Command<PublishTestStateCommand>(Execute);         
            Command<TestDomainErrorCommand>(Execute);

            //Recovery
            Recover<TestAddedEvent>(Recover);
            Recover<TestReceivedEvent>(Recover);
            Recover<TestSentEvent>(Recover);
            Recover<TestCreatedEvent>(Recover);
        }

        private bool Execute(CreateTestCommand command)
        {
            if (IsNew)
            {
                Emit(new TestCreatedEvent(command.AggregateId));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(AddTestCommand command)
        {
            if (!IsNew)
            {

                Emit(new TestAddedEvent(command.Test));

            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }
            return true;
        }

        private bool Execute(GiveTestCommand command)
        {
            if (!IsNew)
            {
                if (State.TestCollection.ContainsKey(command.TestToGive.Id))
                {
                    Emit(new TestSentEvent(command.TestToGive,command.ReceiverAggregateId));
                }
                
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(ReceiveTestCommand command)
        {
            if (!IsNew)
            {
                Emit(new TestReceivedEvent(command.SenderAggregateId,command.TestToReceive));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(PoisonTestAggregateCommand command)
        {
            if (!IsNew)
            {
                Context.Stop(Self);
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }
        
        private bool Execute(PublishTestStateCommand command)
        {
            Signal(new TestStateSignalEvent(State,LastSequenceNr,Version));

            return true;
        }
       
        
        private bool Execute(TestDomainErrorCommand command)
        {
            TestErrors++;
            Throw(new TestedErrorEvent(TestErrors));

            return true;
        }
    }
}