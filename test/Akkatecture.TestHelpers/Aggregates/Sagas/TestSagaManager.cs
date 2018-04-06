using System;
using System.Linq.Expressions;
using Akkatecture.Sagas;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaManager : SagaManager<TestSaga,TestSagaId,TestSagaLocator>
    {
        public TestSagaManager(Expression<Func<TestSaga>> sagaFactory)
            : base(sagaFactory)
        {
            
        }
    }
}