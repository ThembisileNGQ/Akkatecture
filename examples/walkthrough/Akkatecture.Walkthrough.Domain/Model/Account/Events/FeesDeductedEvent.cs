using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("FeesDeducted", 1)]
    public class FeesDeductedEvent : AggregateEvent<Account, AccountId> 
    {
        public Money Amount { get;  }
        
        public FeesDeductedEvent(Money amount)
        {
            Amount = amount;
        }
    }
}
