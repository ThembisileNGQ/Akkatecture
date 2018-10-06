using System;
using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Events;
using Akkatecture.Sagas;

namespace Akkatecture.Examples.Api.Domain.Sagas
{
    public class ResourceCreationSagaLocator : ISagaLocator<ResourceCreationSagaId>
    {
        public const string LocatorIdPrefix = "resourcecreation";
        public ResourceCreationSagaId LocateSaga(IDomainEvent domainEvent)
        {
            switch (domainEvent.GetAggregateEvent())
            {
                case ResourceCreatedEvent evt:
                    return new ResourceCreationSagaId($"{LocatorIdPrefix}-{domainEvent.GetIdentity()}");
                default:
                    throw new ArgumentException(nameof(domainEvent));
            }
        }
    }
}