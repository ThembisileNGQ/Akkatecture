using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface ISnapshotAggregateRoot : IAggregateRoot
    {
        int? SnapshotVersion { get; }
    }

    public interface ISnapshotAggregateRoot<out TIdentity, TSnapshot> : ISnapshotAggregateRoot, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
    }
}