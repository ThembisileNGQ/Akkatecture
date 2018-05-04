using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("MoneySent", 1)]
    public class MoneySentEvent: AggregateEvent<Account,AccountId> 
    {
        public Transaction Transaction { get; }
        
        public MoneySentEvent(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}