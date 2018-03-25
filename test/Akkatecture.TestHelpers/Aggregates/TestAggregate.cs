using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestId, TestState>
    {
        public TestAggregate(TestId aggregateId)
            : base(aggregateId)
        {
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
                
            }
            else
            {
                //signal domain error
            }
            return true;
        }
        
        private bool Execute(PoisonTestAggregateCommand command)
        {
            if (!IsNew)
            {
                
            }
            else
            {
                //signal domain error
            }
            return true;
        }
        
        private bool Execute(PublishTestStateCommand command)
        {
            if (!IsNew)
            {
                
            }
            else
            {
                //signal domain error
            }
            return true;
        }
        
        private bool Execute(TestCommand command)
        {
            if (!IsNew)
            {
                
            }
            else
            {
                //signal domain error
            }
            return true;
        }
        
        private bool Execute(TestDomainErrorCommand command)
        {
            if (!IsNew)
            {
                
            }
            else
            {
                //signal domain error
            }
            return true;
        }
    }
}