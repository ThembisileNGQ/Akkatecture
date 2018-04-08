using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Subscribers
{
    public abstract class DomainEventSubscriber<TAggregate, TIdentity, TEvent> : ReceiveActor
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        protected DomainEventSubscriber()
        {
            
            var subscriptionTypes =
                GetType()
                .GetDomainEventSubscriberSubscriptionTypes<TAggregate, TIdentity>(); 
            
            foreach (var type in subscriptionTypes)
            {
                Context.System.EventStream.Subscribe(Self, type);
            }

        }
    }
}