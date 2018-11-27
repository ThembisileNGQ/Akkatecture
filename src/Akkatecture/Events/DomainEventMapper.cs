using System;
using System.Reflection;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public static class DomainEventMapper
    {
        public static object FromComittedEvent(object evt)
        {
            var type = typeof(ICommittedEvent<,,>);

            if (type.GetTypeInfo().IsAssignableFrom(evt.GetType()))
            {
                //dynamic dispach here to get AggregateEvent
                
                var comittedEvent = evt as dynamic;
                var typeInfo = evt.GetType().GetTypeInfo();
                
                var genericType = typeof(DomainEvent<,,>)
                    .MakeGenericType(type.GetGenericArguments()[0], type.GetGenericArguments()[1],type.GetGenericArguments()[2]);
                
                var domainEvent = Activator.CreateInstance(
                    genericType,
                    comittedEvent.AggregateIdentity,
                    comittedEvent.AggregateEvent,
                    comittedEvent.Metadata,
                    comittedEvent.Timestamp,
                    comittedEvent.AggregateSequenceNumber);

                return domainEvent;
            }
            else
            {
                return evt;
            }
        }
    }
    
    public class DomainEventReadAdapter : IReadEventAdapter
    {
        public IEventSequence FromJournal(object evt, string manifest)
        {
            var newEvent = DomainEventMapper.FromComittedEvent(evt);
            
            return new SingleEventSequence(newEvent);
        }
    }
}