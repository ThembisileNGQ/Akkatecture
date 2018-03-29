using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Errors;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestId, TestState>
    {
        public int TestErrors { get; private set; }
        public TestAggregate(TestId aggregateId)
            : base(aggregateId)
        {
            TestErrors = 0;

            if (IsRecovering)
            {
                Logger.Info("Recovering");
            }

            Command<CreateTestCommand>(Execute);
            Command<PoisonTestAggregateCommand>(Execute);
            Command<PublishTestStateCommand>(Execute);
            Command<TestCommand>(Execute);
            Command<TestDomainErrorCommand>(Execute);

            Recover<TestCreatedEvent>(Recover);
            Recover<TestTestedEvent>(Recover);

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

        private bool Execute(TestCommand command)
        {
            if (!IsNew)
            {
                Emit(new TestTestedEvent(State.Test.TestsDone + 1));

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