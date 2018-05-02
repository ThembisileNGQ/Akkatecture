using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("MoneySent", 1)]
    public class MoneySentEvent: AggregateEvent<Account,AccountId> 
    {
        public Money Amount { get; }
        public AccountId ReceiverId { get;  }
        
        public MoneySentEvent(AccountId receiverId, Money amount)
        {
            Amount = amount;
            ReceiverId = receiverId;
        }
    }
}