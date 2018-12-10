using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public abstract class SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, TSnapshot> : AggregateRoot<TAggregate, TIdentity, TAggregateState>,
        ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TAggregate : SnapshotAggregateRoot<TAggregate, TIdentity,  TAggregateState, TSnapshot>
        where TAggregateState : AggregateState<TAggregate,TIdentity, IEventApplier<TAggregate,TIdentity>>
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