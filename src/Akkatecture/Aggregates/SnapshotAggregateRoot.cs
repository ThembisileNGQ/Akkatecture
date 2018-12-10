using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public abstract class SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, TSnapshot> : AggregateRoot<TAggregate, TIdentity, TAggregateState>, ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, TSnapshot>
        where TAggregateState : SnapshotAggregateState<TAggregate, TIdentity, ISnapshotHydrater<TAggregate,TIdentity>>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
        protected ISnapshotStrategy SnapshotStrategy { get; }
        public int? SnapshotVersion { get; private set; }

        protected SnapshotAggregateRoot(
            TIdentity id,
            ISnapshotStrategy snapshotStrategy)
            : base(id)
        {
            SnapshotStrategy = snapshotStrategy;
        }
        
    }
}