using System;
using System.Linq.Expressions;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestAggregateSagaManager : AggregateSagaManager<TestSaga,TestSagaId,TestSagaLocator>
    {
        public TestAggregateSagaManager(Expression<Func<TestSaga>> sagaFactory)
            : base(sagaFactory)
        {
        }
    }
}