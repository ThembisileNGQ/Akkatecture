using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.Specifications.Provided;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;
using Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer.Events;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSaga : AggregateSaga<MoneyTransferSaga, MoneyTransferSagaId, MoneyTransferSagaState>,
        ISagaIsStartedBy<Account, AccountId, MoneySentEvent>,
        ISagaHandles<Account, AccountId, MoneyReceivedEvent>
    {
        public IActorRef AccountAggregateManager { get; }
        public MoneyTransferSaga(IActorRef accountAggregateManager)
        {
            AccountAggregateManager = accountAggregateManager;
        }
        public Task Handle(IDomainEvent<Account, AccountId, MoneySentEvent> domainEvent)
        {
            var isNewSpec = new AggregateIsNewSpecification();
            if (isNewSpec.IsSatisfiedBy(this))
            {
                var command = new ReceiveMoneyCommand(
                    domainEvent.AggregateEvent.Transaction.Receiver,
                    domainEvent.AggregateEvent.Transaction);
            
                AccountAggregateManager.Tell(command);
                
                Emit(new MoneyTransferStartedEvent(domainEvent.AggregateEvent.Transaction));
            }
            
            return Task.CompletedTask;
        }

        public Task Handle(IDomainEvent<Account, AccountId, MoneyReceivedEvent> domainEvent)
        {
            var spec = new AggregateIsNewSpecification().Not();
            if (spec.IsSatisfiedBy(this))
            {
                Emit(new MoneyTransferCompletedEvent(domainEvent.AggregateEvent.Transaction));
            }
            
            return Task.CompletedTask;
        }
    }
}