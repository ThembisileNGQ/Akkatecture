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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Akka.Persistence.Journal;
using Akka.Util.Internal;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Events
{
    public abstract class AggregateEventUpcaster<TAggregate, TIdentity> : AggregateEventUpcaster<TAggregate, TIdentity,
        IEventUpcaster<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {

    }

    public abstract class AggregateEventUpcaster<TAggregate,TIdentity, TEventUpcaster> : IReadEventAdapter, IEventUpcaster<TAggregate, TIdentity>
        where TEventUpcaster : class, IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private static ConcurrentDictionary<Type, bool> DecisionCache = new ConcurrentDictionary<Type, bool>();
        public readonly IReadOnlyDictionary<Type, Func<TEventUpcaster, IAggregateEvent, IAggregateEvent>> UpcastFunctions;

        
        public AggregateEventUpcaster()
        {
            var upcastables = GetType().GetAggregateEventUpcastTypes();
            var dictionary = upcastables.ToDictionary(x => x, x=> true);
            DecisionCache = new ConcurrentDictionary<Type, bool>(dictionary);
            
            var me = this as TEventUpcaster;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Event applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TEventUpcaster).PrettyPrint()}'");
            }
            UpcastFunctions  = GetType().GetAggregateEventUpcastMethods<TAggregate, TIdentity, TEventUpcaster>();
          
        }
        
        private bool ShouldUpcast(object potentialUpcast)
        {
            var type = potentialUpcast.GetType();
            
            if (potentialUpcast is ICommittedEvent<TAggregate,TIdentity> comittedEvent)
            {
                var eventType = type.GenericTypeArguments[2];

                if (DecisionCache.ContainsKey(eventType))
                {
                    
                    return true;
                }
                else
                {
                    DecisionCache.AddOrSet(eventType, false);
                    return false;
                }
                
            }

            DecisionCache.AddOrSet(type, false);
            return false;
        }

        public IEventSequence FromJournal(object evt, string manifest)
        {
            if (ShouldUpcast(evt))
            {
                //dynamic dispach here to get AggregateEvent
                var comittedEvent = evt as dynamic;
                    
                var upcastedEvent = Upcast(comittedEvent.AggregateEvent);
                
                var genericType = typeof(CommittedEvent<,,>)
                    .MakeGenericType(typeof(TAggregate), typeof(TIdentity), upcastedEvent.GetType());
                
                var upcastedComittedEvent = Activator.CreateInstance(
                    genericType,
                    comittedEvent.AggregateIdentity,
                    upcastedEvent,
                    comittedEvent.Metadata,
                    comittedEvent.Timestamp,
                    comittedEvent.AggregateSequenceNumber);
                
                return EventSequence.Single(upcastedComittedEvent);
            }

            return EventSequence.Single(evt);
        }
        
        
        
        public IAggregateEvent<TAggregate, TIdentity> Upcast(
            IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var aggregateEventType = aggregateEvent.GetType();
            Func<TEventUpcaster,IAggregateEvent, IAggregateEvent> upcaster;

            if (!UpcastFunctions.TryGetValue(aggregateEventType, out upcaster))
            {
                throw new ArgumentException();
            }

            var evt = upcaster((TEventUpcaster)(object)this, aggregateEvent) as IAggregateEvent<TAggregate, TIdentity>;

            return evt;
        }
    }
}