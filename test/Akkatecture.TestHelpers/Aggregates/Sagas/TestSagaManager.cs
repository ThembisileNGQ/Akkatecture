using System;
using System.Linq.Expressions;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaManager : AggregateSagaManager<TestSaga,TestSagaId,TestSagaLocator>
    {
        public TestSagaManager(Expression<Func<TestSaga>> sagaFactory)
            : base(sagaFactory)
        {
        }
    }
}