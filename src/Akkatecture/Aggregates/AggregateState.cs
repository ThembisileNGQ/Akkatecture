// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
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
using System.Collections.Generic;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateState<TAggregate, TIdentity> : AggregateState<TAggregate, TIdentity, IMessageApplier<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
    
    public abstract class AggregateState<TAggregate, TIdentity, TMessageApplier> : IMessageApplier<TAggregate, TIdentity>
        where TMessageApplier : class, IMessageApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TMessageApplier, IAggregateEvent>> ApplyMethods;
        private static readonly IReadOnlyDictionary<Type, Action<TMessageApplier, IAggregateSnapshot>> HydrateMethods;

        static AggregateState()
        {
            ApplyMethods = typeof(TMessageApplier).GetAggregateEventApplyMethods<TAggregate, TIdentity, TMessageApplier>();
            HydrateMethods = typeof(TMessageApplier).GetAggregateSnapshotHydrateMethods<TAggregate, TIdentity, TMessageApplier>();
        }

        protected AggregateState()
        {
            var me = this as TMessageApplier;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Event applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TMessageApplier).PrettyPrint()}'");
            }
        }

        public bool Apply(
            TAggregate aggregate,
            IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var aggregateEventType = aggregateEvent.GetType();
            Action<TMessageApplier, IAggregateEvent> applier;

            if (!ApplyMethods.TryGetValue(aggregateEventType, out applier))
            {
                return false;
            }

            applier((TMessageApplier)(object)this, aggregateEvent);
            return true;
        }
        
        public bool Hydrate(
            TAggregate aggregate,
            IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot)
        {
            var aggregateEventType = aggregateSnapshot.GetType();
            Action<TMessageApplier, IAggregateSnapshot> hydrater;

            if (!HydrateMethods.TryGetValue(aggregateEventType, out hydrater))
            {
                return false;
            }

            hydrater((TMessageApplier)(object)this, aggregateSnapshot);
            return true;
        }
    }
}
