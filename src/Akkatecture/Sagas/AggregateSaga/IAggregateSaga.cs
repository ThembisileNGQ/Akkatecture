using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Sagas.AggregateSaga
{
    public interface IAggregateSaga<out TIdentity, TLocator> : ISaga<TLocator>, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TLocator : ISagaLocator
    {
    }
}