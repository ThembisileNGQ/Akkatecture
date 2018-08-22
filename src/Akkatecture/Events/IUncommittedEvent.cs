using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public interface IUncommittedEvent
    {
        IAggregateEvent AggregateEvent { get; }
        IMetadata Metadata { get; }
    }
}