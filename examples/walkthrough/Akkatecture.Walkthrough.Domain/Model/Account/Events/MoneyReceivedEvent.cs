using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("MoneyReceived", 1)]
    public class MoneyReceivedEvent : AggregateEvent<Account,AccountId> 
    {
        public Money Amount { get;  }
        public AccountId SenderId { get; }
        
        public MoneyReceivedEvent(AccountId senderId, Money amount)
        {
            Amount = amount;
            SenderId = senderId;
        } 
    }
}