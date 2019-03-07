// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
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
using System.Reflection;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public static class DomainEventMapper
    {
        internal static object FromComittedEvent(object evt)
        {
            var type = typeof(ICommittedEvent<,,>);

            if (type.GetTypeInfo().IsAssignableFrom(evt.GetType()))
            {
                //dynamic dispach here to get AggregateEvent
                
                var comittedEvent = evt as dynamic;

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