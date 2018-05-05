using Akkatecture.Aggregates;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer.Events
{
    public class MoneyTransferStartedEvent : AggregateEvent<MoneyTransferSaga, MoneyTransferSagaId>
    {
        public Transaction Transaction { get; }

        public MoneyTransferStartedEvent(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}