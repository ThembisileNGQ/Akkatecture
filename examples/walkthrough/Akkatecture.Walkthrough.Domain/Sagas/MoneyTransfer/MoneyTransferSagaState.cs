using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;
using Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer.Events;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSagaState : SagaState<MoneyTransferSaga,MoneyTransferSagaId,IEventApplier<MoneyTransferSaga, MoneyTransferSagaId>>,
        IApply<MoneyTransferStartedEvent>,
        IApply<MoneyTransferCompletedEvent>
    {
        public Transaction Transaction { get; private set; }
        public void Apply(MoneyTransferStartedEvent aggregateEvent)
        {
            Transaction = aggregateEvent.Transaction;
            Start();
        }

        public void Apply(MoneyTransferCompletedEvent aggregateEvent)
        {
           Complete();
        }
    }
}
