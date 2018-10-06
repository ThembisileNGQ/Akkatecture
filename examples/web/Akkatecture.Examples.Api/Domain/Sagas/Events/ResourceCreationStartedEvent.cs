using System;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;

namespace Akkatecture.Examples.Api.Domain.Sagas.Events
{
    public class ResourceCreationStartedEvent : AggregateEvent<ResourceCreationSaga, ResourceCreationSagaId>
    {
        public ResourceId ResourceId { get; }
        public DateTime StartedAt { get; }
        
        public ResourceCreationStartedEvent(
            ResourceId resourceId,
            DateTime startedAt)
        {
            ResourceId = resourceId;
            StartedAt = startedAt;
        }
    }
}