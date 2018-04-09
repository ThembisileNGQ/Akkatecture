using Akkatecture.Aggregates;
using Akkatecture.Sagas;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaState : SagaState<TestSaga,TestSagaId,IEventApplier<TestSaga,TestSagaId>>
    {
        
    }
}