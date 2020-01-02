// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Akka.Persistence.Query;
using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public static class DomainEventMapper
    {
        internal static object FromCommittedEvent(object evt)
        {
            var eventType = evt.GetType();
            
            if (evt is ICommittedEvent && eventType.GenericTypeArguments.Length == 3)
            {
                //dynamic dispatch here to get AggregateEvent
                
                var committedEvent = evt as dynamic;

                var genericType = typeof(DomainEvent<,,>)
                    .MakeGenericType(eventType.GetGenericArguments()[0], eventType.GetGenericArguments()[1],eventType.GetGenericArguments()[2]);
                
                var domainEvent = Activator.CreateInstance(
                    genericType,
                    committedEvent.AggregateIdentity,
                    committedEvent.AggregateEvent,
                    committedEvent.Metadata,
                    committedEvent.Timestamp,
                    committedEvent.AggregateSequenceNumber);

                return domainEvent;
            }
            else
            {
                return evt;
            }
        }

        public static EventEnvelope FromEnvelope(EventEnvelope eventEnvelope)
        {
            var domainEvent = FromCommittedEvent(eventEnvelope.Event);
            
            var newEventEnvelope = new EventEnvelope(
                eventEnvelope.Offset,
                eventEnvelope.PersistenceId,
                eventEnvelope.SequenceNr,
                domainEvent);

            return newEventEnvelope;
        }
    }
    
}