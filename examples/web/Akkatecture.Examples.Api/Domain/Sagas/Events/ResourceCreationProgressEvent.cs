using System;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;

namespace Akkatecture.Examples.Api.Domain.Sagas.Events
{
    public class ResourceCreationProgressEvent : AggregateEvent<ResourceCreationSaga, ResourceCreationSagaId>
    {
        public ResourceId ResourceId { get; }
        public int Progress { get; }
        public int Elapsed { get; }
        public DateTime UpdatedAt { get; }

        public ResourceCreationProgressEvent(
            ResourceId resourceId,
            int progress,
            int elapsed,
            DateTime updatedAt)
        {
            ResourceId = resourceId;
            Progress = progress;
            Elapsed = elapsed;
            UpdatedAt = updatedAt;
        }
    }
}