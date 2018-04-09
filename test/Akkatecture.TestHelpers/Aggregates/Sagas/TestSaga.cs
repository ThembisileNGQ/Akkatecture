using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSaga : AggregateSaga<TestSaga,TestSagaId,TestSagaState>
    {
    }
}
