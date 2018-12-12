// The MIT License (MIT)
//
// Copyright (c) 2018 Lutando Ngqakaza
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
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates.Snapshot
{
    public abstract class SnapshotAggregateState<TAggregate, TIdentity> : SnapshotAggregateState<TAggregate, TIdentity,
        ISnapshotHydrater<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
    
    public abstract class SnapshotAggregateState<TAggregate, TIdentity, TSnapshotHydrater>  : AggregateState<TAggregate, TIdentity, IEventApplier<TAggregate,TIdentity>>,  ISnapshotHydrater<TAggregate, TIdentity>
        where TSnapshotHydrater : class, ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TSnapshotHydrater, ISnapshot>> HydrateMethods;

        static SnapshotAggregateState()
        {
            HydrateMethods = typeof(TSnapshotHydrater).GetAggregateSnapshotHydrateMethods<TAggregate, TIdentity, TSnapshotHydrater>();
        }

        protected SnapshotAggregateState()
        {
            var me = this as TSnapshotHydrater;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Snapshot applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TSnapshotHydrater).PrettyPrint()}'");
            }
        }

        public bool Hydrate(
            TAggregate aggregate,
            IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot)
        {
            var aggregateEventType = aggregateSnapshot.GetType();
            Action<TSnapshotHydrater, ISnapshot> hydrater;

            if (!HydrateMethods.TryGetValue(aggregateEventType, out hydrater))
            {
                return false;
            }

            hydrater((TSnapshotHydrater)(object)this, aggregateSnapshot);
            return true;
        }
    }
}