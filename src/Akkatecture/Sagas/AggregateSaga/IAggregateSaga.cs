using Akkatecture.Aggregates;

namespace Akkatecture.Sagas.AggregateSaga
{
    public interface IAggregateSaga<TIdentity> : ISaga<TIdentity,ISagaState<TIdentity>>, IAggregateRoot<TIdentity>
        where TIdentity : ISagaId
    {
    }
}