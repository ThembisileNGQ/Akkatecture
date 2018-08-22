using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public class UncommittedEvent
    {
        public IAggregateEvent AggregateEvent { get; }
        public IMetadata Metadata { get; }

        public UncommittedEvent(IAggregateEvent aggregateEvent, IMetadata metadata)
        {
            AggregateEvent = aggregateEvent;
            Metadata = metadata;
        }
    }
}