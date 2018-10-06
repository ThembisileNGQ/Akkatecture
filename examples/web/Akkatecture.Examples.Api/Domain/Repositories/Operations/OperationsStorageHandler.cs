using System.Collections.Generic;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Sagas;
using Akkatecture.Examples.Api.Domain.Sagas.Events;
using Akkatecture.Subscribers;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public class OperationsStorageHandler : DomainEventSubscriber,
        ISubscribeTo<ResourceCreationSaga,ResourceCreationSagaId,ResourceCreationStartedEvent>,
        ISubscribeTo<ResourceCreationSaga,ResourceCreationSagaId,ResourceCreationProgressEvent>,
        ISubscribeTo<ResourceCreationSaga,ResourceCreationSagaId,ResourceCreationEndedEvent>
    {
        public List<OperationsReadModel> Operations = new List<OperationsReadModel>();

        public OperationsStorageHandler()
        {
            Receive<GetOperationsQuery>(Handle);
        }
        
        public Task Handle(IDomainEvent<ResourceCreationSaga, ResourceCreationSagaId, ResourceCreationStartedEvent> domainEvent)
        {
            var operation = new OperationsReadModel(domainEvent.AggregateEvent.ResourceId.GetGuid(),0,0);
            
            Operations.Add(operation);
            return Task.CompletedTask;
        }

        public Task Handle(IDomainEvent<ResourceCreationSaga, ResourceCreationSagaId, ResourceCreationProgressEvent> domainEvent)
        {
            var operation = new OperationsReadModel(domainEvent.AggregateEvent.ResourceId.GetGuid(),domainEvent.AggregateEvent.Progress,domainEvent.AggregateEvent.Elapsed);

            Operations.RemoveAll(x => x.Id == domainEvent.AggregateEvent.ResourceId.GetGuid());
            Operations.Add(operation);
            return Task.CompletedTask;
        }

        public Task Handle(IDomainEvent<ResourceCreationSaga, ResourceCreationSagaId, ResourceCreationEndedEvent> domainEvent)
        {
            var operation = new OperationsReadModel(domainEvent.AggregateEvent.ResourceId.GetGuid(),domainEvent.AggregateEvent.Progress,domainEvent.AggregateEvent.Elapsed);
            
            Operations.RemoveAll(x => x.Id == domainEvent.AggregateEvent.ResourceId.GetGuid());
            Operations.Add(operation);
            return Task.CompletedTask;
        }

        public bool Handle(GetOperationsQuery query)
        {
            Sender.Tell(Operations,Self);
            return true;
        }
    }
}