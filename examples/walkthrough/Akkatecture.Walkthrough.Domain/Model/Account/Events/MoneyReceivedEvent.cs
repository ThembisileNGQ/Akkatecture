using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("MoneyReceived", 1)]
    public class MoneyReceivedEvent : AggregateEvent<Account,AccountId> 
    {
        public Transaction Transaction { get; }

        public MoneyReceivedEvent(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}