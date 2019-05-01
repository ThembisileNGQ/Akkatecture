using Akkatecture.Aggregates.Snapshot;
using Akkatecture.TestHelpers.Aggregates.Snapshots;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.TestAsync
{
    public class TestAsyncSagaSnapshot : IAggregateSnapshot<TestAsyncSaga, TestAsyncSagaId>
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public TestAggregateSnapshot.TestModel Test { get; set; }
    }
}