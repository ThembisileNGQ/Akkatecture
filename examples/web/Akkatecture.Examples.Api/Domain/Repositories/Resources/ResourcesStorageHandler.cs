using System.Collections.Generic;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Sagas;
using Akkatecture.Examples.Api.Domain.Sagas.Events;
using Akkatecture.Subscribers;

namespace Akkatecture.Examples.Api.Domain.Repositories.Resources
{
    public class ResourcesStorageHandler : DomainEventSubscriber,
        ISubscribeTo<ResourceCreationSaga,ResourceCreationSagaId,ResourceCreationEndedEvent>
    {
        public List<ResourcesReadModel> Resources = new List<ResourcesReadModel>();

        public ResourcesStorageHandler()
        {
            Receive<GetResourcesQuery>(Handle);
        }
        
        public Task Handle(IDomainEvent<ResourceCreationSaga, ResourceCreationSagaId, ResourceCreationEndedEvent> domainEvent)
        {
            var readModel = new ResourcesReadModel(domainEvent.AggregateEvent.ResourceId.GetGuid(),domainEvent.AggregateEvent.Elapsed,domainEvent.AggregateEvent.EndedAt);
            
            Resources.Add(readModel);
            
            return Task.CompletedTask;
        }

        public bool Handle(GetResourcesQuery query)
        {
            Sender.Tell(Resources,Self);
            return true;
        }
    }
}