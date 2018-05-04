using System;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSagaLocator : ISagaLocator<MoneyTransferSagaId>
    {
        public const string LocatorIdPrefix = "moneytransfer";

        public MoneyTransferSagaId LocateSaga(IDomainEvent domainEvent)
        {
            switch (domainEvent.GetAggregateEvent())
            {
                case MoneySentEvent evt:
                    return new MoneyTransferSagaId($"{LocatorIdPrefix}-{evt.Transaction.Id}");
                case MoneyReceivedEvent evt:
                    return new MoneyTransferSagaId($"{LocatorIdPrefix}-{evt.Transaction.Id}");
                default:
                    throw new ArgumentNullException(nameof(domainEvent));
            }
        }
    }
}