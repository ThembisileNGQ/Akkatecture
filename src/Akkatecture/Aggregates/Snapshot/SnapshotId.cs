using Akkatecture.Core;

namespace Akkatecture.Aggregates.Snapshot
{
    public class SnapshotId : Identity<SnapshotId>, ISnapshotId
    {
        public SnapshotId(string value) 
            : base(value)
        {
        }
    }
}