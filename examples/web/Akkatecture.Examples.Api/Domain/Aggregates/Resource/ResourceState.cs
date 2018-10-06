using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Events;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    public class ResourceState : AggregateState<Resource, ResourceId>,
        IApply<ResourceCreatedEvent>
    {
        public void Apply(ResourceCreatedEvent aggregateEvent)
        {
            //nothing to be done
        }
    }
}