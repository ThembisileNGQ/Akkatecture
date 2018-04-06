using Akkatecture.Sagas;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaId : SagaId<TestSagaId>
    {
        public TestSagaId(string value) : base(value)
        {
        }
    }
}