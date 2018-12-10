namespace Akkatecture.Aggregates
{
    public interface ISnapshotStrategy
    {
        bool ShouldCreateSnapshot(ISnapshotAggregateRoot snapshotAggregateRoot);
    }
}