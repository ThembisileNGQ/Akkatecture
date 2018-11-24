using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public interface IUpcast<out TNewerAggregateEvent, in TOlderAggregateEvent>
        where TOlderAggregateEvent : IAggregateEvent
        where TNewerAggregateEvent : IAggregateEvent
    {
        TNewerAggregateEvent Upcast(TOlderAggregateEvent aggregateEvent);
    }
}