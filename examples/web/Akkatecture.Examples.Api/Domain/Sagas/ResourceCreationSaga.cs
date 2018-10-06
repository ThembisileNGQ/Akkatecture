using System;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Events;
using Akkatecture.Examples.Api.Domain.Sagas.Events;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Examples.Api.Domain.Sagas
{
    public class ResourceCreationSaga : AggregateSaga<ResourceCreationSaga, ResourceCreationSagaId, ResourceCreationSagaState>,
        ISagaIsStartedBy<Resource, ResourceId, ResourceCreatedEvent>
    {
        public async Task Handle(IDomainEvent<Resource, ResourceId, ResourceCreatedEvent> domainEvent)
        {
            //simulates a long running process
            var resourceId = domainEvent.AggregateIdentity;
            var startedEvent = new ResourceCreationStartedEvent(resourceId, DateTime.UtcNow);
            var started = DateTimeOffset.UtcNow;
            Emit(startedEvent);
            
            var rng = new Random();
            var progress = 0;
            
            while (progress < 100)
            {
                var delay = rng.Next(1, 3);
                
                await Task.Delay(delay * 1000);
                progress += rng.Next(5, 15);
                var elapsed = DateTimeOffset.UtcNow - started;
                var progressEvent = new ResourceCreationProgressEvent(resourceId,progress,(int)elapsed.TotalSeconds,DateTime.UtcNow);
                Emit(progressEvent);
            }

            var elapsedTime = DateTimeOffset.UtcNow - started;
            var endedEvent = new ResourceCreationEndedEvent(resourceId,100, (int)elapsedTime.TotalSeconds,DateTime.UtcNow);
            
            Emit(endedEvent);
        }
    }
}