namespace Akkatecture.Aggregates
{
    public interface ICommittedEvent<out TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        TAggregateEvent AggregateEvent { get; }
        IMetadata Metadata { get; }
    }
}