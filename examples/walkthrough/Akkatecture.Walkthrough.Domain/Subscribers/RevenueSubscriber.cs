using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Subscribers
{
    public class RevenueSubscriber : DomainEventSubscriber,
         ISubscribeTo<Account,AccountId,FeesDeductedEvent>
    {
        public Money Revenue { get; private set; } = new Money(0m);
        
        public Task Handle(IDomainEvent<Account, AccountId, FeesDeductedEvent> domainEvent)
        {
            Revenue += domainEvent.AggregateEvent.Amount;
            
            return Task.CompletedTask;
        }
    }
}