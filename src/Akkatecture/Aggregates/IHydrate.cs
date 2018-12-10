namespace Akkatecture.Aggregates
{
    public interface IHydrate<in TAggregateSnapshot>
        where TAggregateSnapshot : ISnapshot
    {
        void Apply(TAggregateSnapshot aggregateEvent);
    }
}