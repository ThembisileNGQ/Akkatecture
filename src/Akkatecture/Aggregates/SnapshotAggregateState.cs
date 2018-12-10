using System;
using System.Collections.Generic;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public class SnapshotAggregateState<TAggregate, TIdentity, TSnapshotHydrater>  : AggregateState<TAggregate, TIdentity, IEventApplier<TAggregate,TIdentity>>,  ISnapshotHydrater<TAggregate, TIdentity>
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