using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Events;
using Akkatecture.Examples.Api.Domain.Sagas.Events;
using Akkatecture.Sagas;

namespace Akkatecture.Examples.Api.Domain.Sagas
{
    public class ResourceCreationSagaState : SagaState<ResourceCreationSaga, ResourceCreationSagaId, IEventApplier<ResourceCreationSaga, ResourceCreationSagaId>>,
        IApply<ResourceCreationStartedEvent>,
        IApply<ResourceCreationProgressEvent>,
        IApply<ResourceCreationEndedEvent>
    {
        public int Progress { get; private set; }
        
        public void Apply(ResourceCreationStartedEvent aggregateEvent)
        {
            Progress = 0;
            Start();
        }

        public void Apply(ResourceCreationProgressEvent aggregateEvent)
        {
            Progress = aggregateEvent.Progress;
        }

        public void Apply(ResourceCreationEndedEvent aggregateEvent)
        {
            Progress = 100;
            Complete();
        }
    }
}