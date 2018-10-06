using System;
using System.Linq.Expressions;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Examples.Api.Domain.Sagas
{
    public class ResourceCreationSagaManager: AggregateSagaManager<ResourceCreationSaga,ResourceCreationSagaId,ResourceCreationSagaLocator>
    {
        public ResourceCreationSagaManager(Expression<Func<ResourceCreationSaga>> sagaFactory) 
            : base(sagaFactory)
        {
        }
    }
}