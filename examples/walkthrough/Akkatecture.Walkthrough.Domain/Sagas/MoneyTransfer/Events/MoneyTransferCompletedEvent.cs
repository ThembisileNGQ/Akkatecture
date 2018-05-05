using Akkatecture.Aggregates;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer.Events
{
    public class MoneyTransferCompletedEvent : AggregateEvent<MoneyTransferSaga, MoneyTransferSagaId>
    {
        public Transaction Transaction { get; }

        public MoneyTransferCompletedEvent(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}