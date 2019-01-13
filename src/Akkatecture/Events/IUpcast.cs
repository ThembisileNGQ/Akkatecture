using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public interface IUpcast<in TFrom, out TTo>
        where TFrom : IAggregateEvent
        where TTo : IAggregateEvent
    {
        TTo Upcast(TFrom aggregateEvent);
    }
}