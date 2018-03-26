using Akkatecture.Aggregates;

namespace Akkatecture.TestHelpers.Aggregates.Events.Signals
{
    public class TestStateSignalEvent : AggregateEvent<TestAggregate, TestId>
    {
        public TestState State { get; }
        public long LastSequenceNr { get; }
        public long Version { get; }

        public TestStateSignalEvent(TestState state, long lastSequenceNr, long version)
        {
            State = state;
            LastSequenceNr = lastSequenceNr;
            Version = version;
        }
    }
}