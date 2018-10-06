using System;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;

namespace Akkatecture.Examples.Api.Domain.Sagas.Events
{
    public class ResourceCreationEndedEvent : AggregateEvent<ResourceCreationSaga, ResourceCreationSagaId>
    {
        public ResourceId ResourceId { get; }
        public int Progress { get; }
        public int Elapsed { get; }
        public DateTime EndedAt { get; }

        public ResourceCreationEndedEvent(
            ResourceId resourceId,
            int progress,
            int elapsed,
            DateTime endedAt)
        {
            ResourceId = resourceId;
            Progress = progress;
            Elapsed = elapsed;
            EndedAt = endedAt;
        }
    }
}