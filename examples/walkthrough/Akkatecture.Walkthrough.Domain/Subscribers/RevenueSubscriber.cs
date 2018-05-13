using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Commands;

namespace Akkatecture.Walkthrough.Domain.Subscribers
{
    public class RevenueSubscriber : DomainEventSubscriber,
         ISubscribeTo<Account,AccountId,FeesDeductedEvent>
    {
        public IActorRef RevenueRepository { get; }
        
        public RevenueSubscriber(IActorRef revenueRepository)
        {
            RevenueRepository = revenueRepository;
        }
        
        public Task Handle(IDomainEvent<Account, AccountId, FeesDeductedEvent> domainEvent)
        {
            var command = new AddRevenueCommand(domainEvent.AggregateEvent.Amount);
            RevenueRepository.Tell(command);
            
            return Task.CompletedTask;
        }
    }
}