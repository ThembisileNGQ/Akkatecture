using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Events
{
    [EventVersion("AccountOpened", 1)]
    public class AccountOpenedEvent: AggregateEvent<Account,AccountId> 
    {
        public Money OpeningBalance { get; }
        
        public AccountOpenedEvent(Money openingBalance)
        {
            OpeningBalance = openingBalance;
        }
        
    }
}
