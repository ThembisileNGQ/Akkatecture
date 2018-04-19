using System;
using System.Linq.Expressions;

namespace Akkatecture.Sagas.AggregateSaga
{
    public interface IAggregateSagaManager
    {
        
    }

    public interface IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : ISagaLocator<TIdentity>
    {
        Expression<Func<TAggregateSaga>> SagaFactory { get; }
    }
}