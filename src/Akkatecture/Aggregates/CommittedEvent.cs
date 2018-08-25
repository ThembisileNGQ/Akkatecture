namespace Akkatecture.Aggregates
{
    public class CommittedEvent<TAggregateEvent> : ICommittedEvent<TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        public TAggregateEvent AggregateEvent { get; }
        public IMetadata Metadata { get; }

        public CommittedEvent(
            TAggregateEvent aggregateEvent,
            IMetadata metadata)
        {
            AggregateEvent = aggregateEvent;
            Metadata = metadata;
        }
    }
}