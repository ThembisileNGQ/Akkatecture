using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Subscribers
{
    public abstract class DomainEventSubscriber : ReceiveActor
    {
        protected DomainEventSubscriber()
        {
            
            var subscriptionTypes =
                GetType()
                        .GetDomainEventSubscriberSubscriptionTypes(); 
            
            foreach (var type in subscriptionTypes)
            {
                Context.System.EventStream.Subscribe(Self, type);
            }

        }
    }
}